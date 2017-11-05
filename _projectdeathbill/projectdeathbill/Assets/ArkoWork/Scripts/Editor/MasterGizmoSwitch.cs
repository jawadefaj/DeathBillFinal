using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.Collections;


[InitializeOnLoad]
public class MasterGizmoSwitch {

    private static bool masterGizmoEnabled_;
    private static bool runModeGizmoEnabled_;
    private const string MASTER_GIZMO_MENU_NAME = "Portbliss/Gizmos/Master Gizmo Switch";
    private const string RUN_MODE_GIZMO = "Portbliss/Gizmos/Allow Gizmo in Run Mode";

    /// Called on load thanks to the InitializeOnLoad attribute
    static MasterGizmoSwitch() {
        MasterGizmoSwitch.masterGizmoEnabled_ = EditorPrefs.GetBool(MasterGizmoSwitch.MASTER_GIZMO_MENU_NAME, false);
        MasterGizmoSwitch.runModeGizmoEnabled_ = EditorPrefs.GetBool(MasterGizmoSwitch.RUN_MODE_GIZMO, true);

        /// Delaying until first editor tick so that the menu
        /// will be populated before setting check state, and
        /// re-apply correct action
        EditorApplication.delayCall += () => {
            ToggleGizmos(MasterGizmoSwitch.masterGizmoEnabled_);
        };

        EditorApplication.playmodeStateChanged += OnPlayModeStateChanged;
    }

    [MenuItem(MasterGizmoSwitch.MASTER_GIZMO_MENU_NAME)]
    private static void ToggleAction() {
        /// Toggling action
        ToggleGizmos( !MasterGizmoSwitch.masterGizmoEnabled_);
    }

    [MenuItem(MasterGizmoSwitch.RUN_MODE_GIZMO)]
    private static void ToggleRunModeGizmoOption() {
        /// Toggling action
        ToggleRunModeGizmoOption(!MasterGizmoSwitch.runModeGizmoEnabled_);
    }

    public static void ToggleRunModeGizmoOption(bool isOn)
    {
        /// Set checkmark on menu item
        Menu.SetChecked(MasterGizmoSwitch.RUN_MODE_GIZMO, isOn);
        /// Saving editor state
        EditorPrefs.SetBool(MasterGizmoSwitch.RUN_MODE_GIZMO, isOn);

        MasterGizmoSwitch.runModeGizmoEnabled_ = isOn;
    }

    public static void ToggleGizmos(bool gizmosOn)
    {
        //Debug.LogError("Gizmo is turned "+ gizmosOn.ToString());
        /// Set checkmark on menu item
        Menu.SetChecked(MasterGizmoSwitch.MASTER_GIZMO_MENU_NAME, gizmosOn);
        /// Saving editor state
        EditorPrefs.SetBool(MasterGizmoSwitch.MASTER_GIZMO_MENU_NAME, gizmosOn);

        MasterGizmoSwitch.masterGizmoEnabled_ = gizmosOn;

        int val = gizmosOn ? 1 : 0;
        Assembly asm = Assembly.GetAssembly(typeof(Editor));
        Type type = asm.GetType("UnityEditor.AnnotationUtility");
        if (type != null)
        {
            MethodInfo getAnnotations = type.GetMethod("GetAnnotations", BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo setGizmoEnabled = type.GetMethod("SetGizmoEnabled", BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo setIconEnabled = type.GetMethod("SetIconEnabled", BindingFlags.Static | BindingFlags.NonPublic);
            var annotations = getAnnotations.Invoke(null, null);
            foreach (object annotation in (IEnumerable)annotations)
            {
                Type annotationType = annotation.GetType();
                FieldInfo classIdField = annotationType.GetField("classID", BindingFlags.Public | BindingFlags.Instance);
                FieldInfo scriptClassField = annotationType.GetField("scriptClass", BindingFlags.Public | BindingFlags.Instance);
                if (classIdField != null && scriptClassField != null)
                {
                    int classId = (int)classIdField.GetValue(annotation);
                    string scriptClass = (string)scriptClassField.GetValue(annotation);
                    setGizmoEnabled.Invoke(null, new object[] { classId, scriptClass, val });
                    setIconEnabled.Invoke(null, new object[] { classId, scriptClass, val });
                }
            }
        }
    }

    private static bool wasGizmoOn;
    private static void OnPlayModeStateChanged()
    {
        if (EditorApplication.isPlaying)
        {
            if (!runModeGizmoEnabled_)
            {
                if (masterGizmoEnabled_)
                {
                    wasGizmoOn = true;
                    ToggleGizmos(false);
                }
            }
        }
        else
        {
            if (!runModeGizmoEnabled_)
            {
                if (wasGizmoOn)
                {
                    ToggleGizmos(wasGizmoOn);
                    wasGizmoOn = false;
                }
            }
        }
    }
}
