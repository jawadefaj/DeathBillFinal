using UnityEngine;
using System.Collections;
using SimpleJSON;

public class ExternalOptionChecker:MonoBehaviour {

    public static ExternalOptionChecker instace
    {
        get
        {
            if (_instance == null)
                Initialize();

            return _instance;
        }
    }

    private static ExternalOptionChecker _instance;

    private OptionData options;
    private static string url = "http://portbliss.org/deathbill/external_options.txt";
    private static string webData = "";

    public bool IsGPGDataSaverOK
    {
        get
        {
            return options.isGPGDataSaverOk;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(this);
        //Debug.Log(OptionData.GetDefaultJSON());
    }

    public static void StartOptionChecking(System.Action<bool> callback)
    {
        instace.StartCoroutine(instace.CheckData(callback));
    }

    public IEnumerator CheckData(System.Action<bool> callback)
    {
        yield return StartCoroutine(DownloadData());
        callback(options.isGPGDataSaverOk);
    }

   

    static void Initialize()
    {
        if (_instance == null)
        {
            GameObject epc = new GameObject("External Option Checker");
            _instance = epc.AddComponent<ExternalOptionChecker>();
        }
    }

    IEnumerator DownloadData()
    {
        //read data from web url
        WWW newsChecker = new WWW(url);
        do
        {
            yield return null;
        }while(newsChecker.isDone == false);

        if (!string.IsNullOrEmpty(newsChecker.error))
        {
            Debug.Log("error occured " + newsChecker.error);
            webData = "";
        }
        else
        {
            webData = newsChecker.text;
        }

        if (string.IsNullOrEmpty(webData))
        {
            options = new OptionData();
        }
        else
        {
            options = JsonUtility.FromJson<OptionData>(webData);
        }

    }
}

public class OptionData
{
    public bool isGPGDataSaverOk = false;


    public static string GetDefaultJSON()
    {
        return JsonUtility.ToJson(new OptionData());
    }

    public override string ToString ()
    {
        return JsonUtility.ToJson(this);
    }
}
