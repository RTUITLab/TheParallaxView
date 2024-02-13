using UnityEditor;
using UnityEditor.UI;
 
[CustomEditor(typeof(ObButton),true)]
[CanEditMultipleObjects]
public class ObButtonEditor : ButtonEditor
{
    private SerializedProperty buttonOff;
	private SerializedProperty buttonOn;
	private SerializedProperty board;
 
    protected override void OnEnable()
    {
        base.OnEnable();
        buttonOff = serializedObject.FindProperty("buttonOff");
		buttonOn = serializedObject.FindProperty("buttonOn");
		board = serializedObject.FindProperty("board");
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        serializedObject.Update();
        EditorGUILayout.PropertyField(buttonOff);
		EditorGUILayout.PropertyField(buttonOn);
		EditorGUILayout.PropertyField(board);
        serializedObject.ApplyModifiedProperties();
    }
}