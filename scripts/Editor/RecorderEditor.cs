using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

[CustomEditor(typeof(Recorder))]
[CanEditMultipleObjects]

public class RecorderEditor : Editor {

	SerializedProperty action_prop;
	SerializedProperty animationController_prop;
    SerializedProperty animationSpeed_prop;

    SerializedProperty frameRate_prop;
    SerializedProperty selectedFrame_prop;
    SerializedProperty currentFrame_prop;

    void OnEnable(){
		action_prop = serializedObject.FindProperty ("action");
		animationController_prop = serializedObject.FindProperty ("animController");
        animationSpeed_prop = serializedObject.FindProperty("animationSpeed");
        frameRate_prop = serializedObject.FindProperty("frameRate");
        selectedFrame_prop = serializedObject.FindProperty("selectedFrame");
        currentFrame_prop = serializedObject.FindProperty("currentFrame");
    }

    public override void OnInspectorGUI() { 

		// Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
		serializedObject.Update ();

        EditorGUILayout.PropertyField(animationController_prop);

        if (animationController_prop.objectReferenceValue != null)
        {
            EditorGUILayout.PropertyField(action_prop);
            if (action_prop.enumNames[action_prop.enumValueIndex] == "USE_ANIMATION")
            {
                EditorGUILayout.Separator();
                EditorGUILayout.Slider(animationSpeed_prop, -10f, 10f, "Animation Speed: ");

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(frameRate_prop, new GUIContent("Animation FPS:"));
                EditorGUILayout.PropertyField(selectedFrame_prop, new GUIContent("Selected Frame:"));
                EditorGUILayout.EndHorizontal();

                if(GUILayout.Button("Move to selected frame"))
                {
                    ((Recorder)target).MoveToSelectedFrame();
                }
                EditorGUILayout.Separator();

                int numberOfFrames = Mathf.FloorToInt(frameRate_prop.intValue * ((AnimatorController)animationController_prop.objectReferenceValue).animationClips[0].length);
                float value = (float) currentFrame_prop.intValue / numberOfFrames;
                ProgressBar(value, "Current frame: " + currentFrame_prop.intValue + "/" + numberOfFrames);
            }
        }
        else
        {

            EditorGUILayout.Separator();
            if (GUILayout.Button("Add new controller"))
            {
                if (!System.IO.Directory.Exists("Assets/Recorder/"))
                    System.IO.Directory.CreateDirectory("Assets/Recorder/");
                
                AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath("Assets/Recorder/RecorderAnimatorController.controller");
                controller.AddParameter("speed", AnimatorControllerParameterType.Float);
                AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;
                AnimatorState state = rootStateMachine.AddState("RecordedAnimation");
                AnimationClip clip = new AnimationClip();
                AssetDatabase.CreateAsset(clip, "Assets/Recorder/RecordedAnimation.anim");
                AssetDatabase.SaveAssets();

                state.motion = clip;
                state.speedParameterActive = true;

                animationController_prop.objectReferenceValue = controller;
            }
            EditorGUILayout.Separator();
        }

        // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
        serializedObject.ApplyModifiedProperties ();
	}

    void ProgressBar(float value, string label)
    {
        // Get a rect for the progress bar using the same margins as a textfield:
        Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
        EditorGUI.ProgressBar(rect, value, label);
        EditorGUILayout.Space();
    }
}
