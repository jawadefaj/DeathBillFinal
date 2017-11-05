using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class CinematicsManager : MonoBehaviour
{

    public static bool vanish = false;
    public GameObject scene1;
    public GameObject scene2;
    public GameObject scene3;
    public GameObject scene4;
    public GameObject scene5;
    public GameObject scene6;
    public GameObject scene7;
    public GameObject scene8;

	private GameObject ob;

	private CineCollection[] cineCollection;
	public static int cineIndex =0;

    [Range(0,1)]
    public float volume = 1;
    public System.Collections.Generic.List<AudioClip> soundList = new System.Collections.Generic.List<AudioClip>();
	
    void Start()
    {
		//init cine collection
		cineCollection = new CineCollection[6];

		cineCollection[0] = new CineCollection(new GameObject[]{scene1,scene2});
		cineCollection[1] = new CineCollection(new GameObject[]{scene3,scene4});
		cineCollection[2] = new CineCollection(new GameObject[]{scene5});
		cineCollection[3] = new CineCollection(new GameObject[]{scene6});
		cineCollection[4] = new CineCollection(new GameObject[]{scene7});
		cineCollection[5] = new CineCollection(new GameObject[]{scene8});

        if (UserSettings.SoundOn)
        {
            AudioSource asource = this.GetComponent<AudioSource>();
            if (asource == null)
                asource = this.gameObject.AddComponent<AudioSource>();

            if (cineIndex < soundList.Count)
            {
                asource.clip = soundList[cineIndex];
                asource.loop = true;
                asource.playOnAwake = false;
                asource.volume = volume;
                asource.Play();
            }
            else
            {
                Debug.Log("No sound added for this cine!");
            }
        }

        DOTween.Init();
		vanish = false;

		//start first scene
		ob = Instantiate(cineCollection[cineIndex].GetCineObj(), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
    }

    void Update()
    {
        if (vanish)
        {
			//Destroy(ob,.5f);

            vanish = false;

			if(cineCollection[cineIndex].HasNext())
			{
				StartCoroutine(PlayNext());
			}
			else
			{
				WeaponLoader.ClearWeapon(true);
			}

        }
    }

	private IEnumerator PlayNext()
	{
		yield return new WaitForSeconds(0.5f);
		Destroy(ob);
		yield return null;
		ob = Instantiate(cineCollection[cineIndex].GetCineObj(), new Vector3(0, 0, 0), Quaternion.identity) as GameObject; 
	}

	public void OnSkipClicked()
	{
		//vanish = true;
		Debug.Log("hh");
		WeaponLoader.ClearWeapon(true);
	}



	public struct CineCollection
	{
		public GameObject[] cineCollection;
		public int index;

		public CineCollection(GameObject[] c)
		{
			cineCollection = c;
			index = 0;
		}

		public GameObject GetCineObj()
		{
			if(index>(cineCollection.Length-1))
			{
				Debug.LogError("Invalid call");
				return null;
			}

			index++;
			return cineCollection[index-1];
		}

		public bool HasNext()
		{
			if(index>(cineCollection.Length-1)) return false;
			else return true;
		}
	}
}
