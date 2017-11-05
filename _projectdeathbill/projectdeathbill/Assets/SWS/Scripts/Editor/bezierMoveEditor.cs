﻿/*  This file is part of the "Simple Waypoint System" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them directly or indirectly
 *  from Rebound Games. You shall not license, sublicense, sell, resell, transfer, assign,
 *  distribute or otherwise make available to any third party the Service or the Content. 
 */

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace SWS
{
    /// <summary>
    /// Custom inspector for bezierMove.
    /// <summary>
    [CustomEditor(typeof(bezierMove))]
    public class bezierMoveEditor : moveEditor
    {
        //returns BezierPathManager component for later use
        public new BezierPathManager GetPathTransform()
        {
            return m_Object.FindProperty("pathContainer").objectReferenceValue as BezierPathManager;
        }


        //override inspector gui of moveEditor
        public override void OnInspectorGUI()
        {
            //this pulls the relative variables from unity runtime and stores them in the object
            m_Object.Update();
            //DrawDefaultInspector();

            //draw custom variable properties by using serialized properties
            EditorGUILayout.PropertyField(m_Object.FindProperty("pathContainer"));
            EditorGUILayout.PropertyField(m_Object.FindProperty("onStart"));
            EditorGUILayout.PropertyField(m_Object.FindProperty("moveToPath"));
            EditorGUILayout.PropertyField(m_Object.FindProperty("reverse"));

            EditorGUILayout.PropertyField(m_Object.FindProperty("startPoint"));
            EditorGUILayout.PropertyField(m_Object.FindProperty("sizeToAdd"));
            EditorGUILayout.PropertyField(m_Object.FindProperty("speed"));

            SerializedProperty timeValue = m_Object.FindProperty("timeValue");
            EditorGUILayout.PropertyField(timeValue);
            if (timeValue.enumValueIndex == 0)
            {
                SerializedProperty easeType = m_Object.FindProperty("easeType");
                EditorGUILayout.PropertyField(easeType);
                if (easeType.enumValueIndex == 31)
                    EditorGUILayout.PropertyField(m_Object.FindProperty("animEaseType"));
            }

            SerializedProperty loopType = m_Object.FindProperty("loopType");
            EditorGUILayout.PropertyField(loopType);
            EditorGUILayout.PropertyField(m_Object.FindProperty("pathType"));
            SerializedProperty orientToPath = m_Object.FindProperty("pathMode");
            EditorGUILayout.PropertyField(orientToPath);
            if (orientToPath.enumValueIndex != 0)
            {
                EditorGUILayout.PropertyField(m_Object.FindProperty("lookAhead"));
                EditorGUILayout.PropertyField(m_Object.FindProperty("lockRotation"));
            }

            EditorGUILayout.PropertyField(m_Object.FindProperty("lockPosition"));

            //get Path Manager component
            var path = GetPathTransform();
            EditorGUILayout.Space();
            GUILayout.Label("Settings:", EditorStyles.boldLabel);

            //check whether a Bezier Path Manager component is set, if not display a label
            if (path == null)
            {
                GUILayout.Label("No path set.");
                m_List.arraySize = 0;
            }
            else
            {
                //draw message options
                EventSettings();
            }

            //we push our modified variables back to our serialized object
            m_Object.ApplyModifiedProperties();
        }


        //override message settings of moveEditor
        public override void EventSettings()
        {
            //path is set and boolean for displaying events settings is true
            if (showEventSetup)
            {
                //draw button for hiding events
                if (GUILayout.Button("Hide Events"))
                    showEventSetup = false;

                EditorGUILayout.BeginHorizontal();

                //clear events values
                if (GUILayout.Button("Clear All"))
                {
                    //display custom dialog and wait for user input to clear all events
                    if (EditorUtility.DisplayDialog("Are you sure?",
                        "Do you really want to reset all events and their values?",
                        "Continue",
                        "Cancel"))
                    {
                        m_List.arraySize = 0;
                    }
                }

                EditorGUILayout.EndHorizontal();

                //begin a scrolling view inside GUI, pass in Vector2 scroll position 
                scrollPosEvents = EditorGUILayout.BeginScrollView(scrollPosEvents, GUILayout.Height(190));

                //loop through list
                for (int i = 0; i < m_List.arraySize; i++)
                {
                    //draw label with waypoint index, display total count of events for this waypoint
                    EditorGUILayout.HelpBox(i + ". Waypoint", MessageType.None);		
					EditorGUILayout.PropertyField (m_List.GetArrayElementAtIndex(i));
                }
                //ends the scrollview defined above
                EditorGUILayout.EndScrollView();

                //here we check for the last GUI pass, where the Inspector gets drawn
                //the first pass is responsible for the GUI layout of all fields,
                //and if we resize the list in the first pass it throws an error in the second pass
                //this is because the first and the second pass must have the same values on redraw
                if (Event.current.type == EventType.Repaint)
                {
                    //get total list size and set it to the waypoint size,
                    //so each waypoint has one event
                    m_List.arraySize = GetPathTransform().bPoints.Count;
                }
            }
            else
            {
                //draw button to toggle events
                if (GUILayout.Button("Show Events"))
                    showEventSetup = true;
            }
        }
        

        //if this path is selected, display small info boxes above all waypoint positions
        void OnSceneGUI()
        {
            //get Bezier Path Manager component
            var path = GetPathTransform();

            //do not execute further code if we have no path defined
            if (path == null) return;

            //begin GUI block
            Handles.BeginGUI();
            //loop through waypoint array
            for (int i = 0; i < path.bPoints.Count; i++)
            {
                //translate waypoint vector3 position in world space into a position on the screen
                var guiPoint = HandleUtility.WorldToGUIPoint(path.bPoints[i].wp.position);
                //create rectangle with that positions and do some offset
                var rect = new Rect(guiPoint.x - 50.0f, guiPoint.y - 40, 100, 20);
                //draw box at rect position with current waypoint name
                GUI.Box(rect, "Waypoint: " + i);
            }
            Handles.EndGUI(); //end GUI block
        }
    }
}