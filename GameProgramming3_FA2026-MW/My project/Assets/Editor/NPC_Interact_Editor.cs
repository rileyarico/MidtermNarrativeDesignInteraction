using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NPC_Interact))]
public class NewEmptyCSharpScript : Editor
{
    public override void OnInspectorGUI()
    {
        //"draws" the default inspector of our NPC Interact Class
        //or whatever class this editor is affecting
        //DrawDefaultInspector();
        //EditorGUILayout.HelpBox("Reminder: Load NpcCam prefab before setting variables", MessageType.Info);

        //we are loading a serializedProperty temp var called typeProp
        //and we are loading that from the 'type' var from our NPC_Interact class
        SerializedProperty typeProp = serializedObject.FindProperty("type");
        EditorGUILayout.PropertyField(typeProp); //this is drawing just the type prop. We are going to draw more based on the enum val

        iType currentType = (iType)typeProp.enumValueIndex;

        if(currentType == iType.npc)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("textData"));
            EditorGUILayout.HelpBox("Don't forget to write the string you want to display upon canInteract", MessageType.Info);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("camData"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dialogData"));
        }

        //load the class we want to modify/change
        //so that we can call the methods from that class in our editor layout
        NPC_Interact npcI = (NPC_Interact)target;
        //if statements in editor scripts "generate" GUILayout elements instead of checking to see that they exist
        //the logic below does not query a condition, but instead makes the condition true by default
        if(GUILayout.Button("Set Interact Text Object"))
        {
            //use the npcI variable called above to call SetTextVars in the editor/inspector
            npcI.SetTextVars();
        }
        else if(GUILayout.Button("Load NPC Cam Prefab"))
        {
            npcI.LoadNpcCamPrefab();
        }
        else if(GUILayout.Button("Set NPC Cam Variable"))
        {
            npcI.SetNpcCam();
        }
        else if (GUILayout.Button("Set YarnSpinner Dialogue Runner"))
        {
            npcI.SetDialogue();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
