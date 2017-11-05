using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;

//[InitializeOnLoad]
public class CustomGizmoSwitch {

    private static bool _allowGizmoInScene = false;
    private static bool _allowGizmoInPlayMode = false;
    private static bool isInitialized = false;

    private const string key_sceneGizmoBoolKey = "sceneGizmoBool";
    private const string key_playGizmoBoolKey = "playGizmoBool";
    private const string MENU_NAME = "Portbliss/Custom Gizmo Switch";

    public static bool CanDrawOnScene
    {
        get
        {
            if (!isInitialized)
                Initialize();
            return _allowGizmoInScene;
        }
    }

    public static bool CanDrawOnPlayMode
    {
        get
        {
            if (!isInitialized)
                Initialize();
            return _allowGizmoInPlayMode;
        }
    }

    public static bool IsInPlayMode
    {
        get
        {
            return (Application.isEditor && Application.isPlaying);
        }
    }

    private static void Initialize()
    {
        _allowGizmoInScene = EditorPrefs.GetBool(key_sceneGizmoBoolKey,true);
        _allowGizmoInPlayMode = EditorPrefs.GetBool(key_playGizmoBoolKey, true);
        isInitialized = true;

    }


    private static bool enabled_;
    /// Called on load thanks to the InitializeOnLoad attribute
    static CustomGizmoSwitch() {
        CustomGizmoSwitch.enabled_ = EditorPrefs.GetBool(CustomGizmoSwitch.MENU_NAME, false);

        /// Delaying until first editor tick so that the menu
        /// will be populated before setting check state, and
        /// re-apply correct action
        EditorApplication.delayCall += () => {
            Initialize();
            PerformAction(CustomGizmoSwitch.enabled_);
        };
    }

    //[MenuItem(CustomGizmoSwitch.MENU_NAME)]
    private static void ToggleAction() {

        Debug.Log("old version");
        /// Toggling action
        PerformAction( !CustomGizmoSwitch.enabled_);
    }

    public static void PerformAction(bool enabled) {

        /// Set checkmark on menu item
        Menu.SetChecked(CustomGizmoSwitch.MENU_NAME, enabled);
        /// Saving editor state
        EditorPrefs.SetBool(CustomGizmoSwitch.MENU_NAME, enabled);

        CustomGizmoSwitch.enabled_ = enabled;

        /// Perform your logic here...
        _allowGizmoInScene = enabled;
        EditorPrefs.SetBool(key_sceneGizmoBoolKey, enabled);
    }
}
