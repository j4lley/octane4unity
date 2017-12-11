using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(Recorder))]
public class RecorderEditor : Editor {

	SerializedProperty action_prop;
	SerializedProperty animationController_prop;
	SerializedProperty selectedFrame_prop;

	void OnEnable(){
		action_prop = serializedObject.FindProperty ("action");
		animationController_prop = serializedObject.FindProperty ("animationController");
		selectedFrame_prop = serializedObject.FindProperty ("selectedFrame"); 
	}

	void OnInspectorGUI(){
		// Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
		serializedObject.Update ();



		// Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
		serializedObject.ApplyModifiedProperties ();
	}
}
