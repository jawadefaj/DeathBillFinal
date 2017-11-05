using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PlayerInputController))]
public class PlayerInputControllerEditor : Editor {

    PlayerInputController pic;
    bool gotErrors = false;


    void OnEnable()
    {
        pic = (PlayerInputController)target;
    }


	public override void OnInspectorGUI()
    {
        gotErrors = false;

        EditorGUILayout.Space();

        ShowHudRect();

        EditorGUILayout.Space();

        ShowPlayers();

        EditorGUILayout.Space();

        ShowCurrentPlayer();

        EditorGUILayout.Space();

        if (gotErrors || pic.players.Length<1)
        {
            EditorGUILayout.HelpBox("You have got some reference errors. Fix this now", MessageType.Error);
        }

        if(GUILayout.Button("Fix Issues",GUILayout.Height(30)))
        {
            FixHudRect();
            FixPlayers();
            FixCurrentPlayer();
        }

    }

    void ShowPlayers()
    {
        EditorGUILayout.LabelField("Registered Players", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("Box");

        if (pic.players == null)
        {
            EditorGUILayout.LabelField("No player is registered!!!");
            EditorGUILayout.EndVertical();
            gotErrors = true;
            return;
        }

        if (pic.players.Length < 1)
        {
            EditorGUILayout.LabelField("No player is registered!!!");
            EditorGUILayout.EndVertical();
            gotErrors = true;
            return;
        }

        for (int i = 0; i < pic.players.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();
            string tag = (i + 1).ToString();
            tag = string.Concat(tag + ". ");
            if (pic.players[i] == null)
            {
                tag = string.Concat(tag,"Reference is Missing!!!");
                gotErrors = true;   
            }
            else
            {
                tag = string.Concat(tag, pic.players[i].name);
            }
            EditorGUILayout.LabelField(tag);

            //removing players
            if (GUILayout.Button("X", GUILayout.Width(30)))
            {
                RemovePlayer(i);
                return;
            }

            EditorGUILayout.EndHorizontal();
        } 
        EditorGUILayout.EndVertical();
    }

    void ShowHudRect()
    {


        EditorGUILayout.BeginVertical("Box");
        if (pic.hudRect != null)
        {
            EditorGUILayout.LabelField("Hud Rectangle : "+ pic.hudRect.name);
        }
        else
        {
            EditorGUILayout.LabelField("Hud Rectangle Reference is Missing!!!");
            gotErrors = true;
        }
        EditorGUILayout.EndVertical();
    }

    void ShowCurrentPlayer()
    {
        EditorGUILayout.BeginVertical("Box");

        if (pic.current_player == null)
        {
            gotErrors = true;
        }

        int index = System.Array.IndexOf(pic.players, pic.current_player);
        if (index < 0)
        {
            pic.current_player = null;
            gotErrors = true;
        }

        pic.current_player = (ThirdPersonController)EditorGUILayout.ObjectField("Current Player",pic.current_player, typeof(ThirdPersonController), true);
        EditorGUILayout.EndVertical();
    }

    void FixCurrentPlayer()
    {
        if(pic.current_player ==null)
            pic.current_player = pic.players[0];

        int index = System.Array.IndexOf(pic.players, pic.current_player);
        if (index < 0)
            pic.current_player = pic.players[0];
    }

    void FixPlayers()
    {
        pic.players = new ThirdPersonController[0];
        ThirdPersonController[] tpcs = GameObject.FindObjectsOfType<ThirdPersonController>();

        if (tpcs == null)
        {
            EditorUtility.DisplayDialog("Player Fixer","No player object found on the scene!","Ok");
            return;
        }

        if (tpcs.Length < 1)
        {
            EditorUtility.DisplayDialog("Player Fixer","No player object found on the scene!","Ok");
            return;
        }

        pic.players = new ThirdPersonController[tpcs.Length];

        for (int i = 0; i < tpcs.Length; i++)
            pic.players[i] = tpcs[i];
    }

    void RemovePlayer(int atIndex)
    {
        pic.players[atIndex] = null;

        for (int i = atIndex; i < pic.players.Length-1; i++)
        {
            pic.players[i] = pic.players[i + 1];
        }

        ThirdPersonController[] newList = new ThirdPersonController[pic.players.Length-1];

        for (int i = 0; i < newList.Length; i++)
        {
            newList[i] = pic.players[i];
        }

        pic.players = newList;
    }

    void FixHudRect()
    {
        if (pic.hudRect == null)
        {
            RectTransform hudRect;
            HUDManager hudManager = GameObject.FindObjectOfType<HUDManager>();
            hudRect = hudManager.shootGroup.noDragGroup.rect;
            pic.hudRect = hudRect;
        }

        //pic.hudRect = (RectTransform)EditorGUILayout.ObjectField("Hud Rectangle",pic.hudRect, typeof(RectTransform), true);
    }
}
