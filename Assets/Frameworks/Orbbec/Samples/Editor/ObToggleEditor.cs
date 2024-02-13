using UnityEditor;
using UnityEditor.UI;
 
[CustomEditor(typeof(ObToggle),true)]
[CanEditMultipleObjects]
public class ObToggleEditor : ToggleEditor
{
    private SerializedProperty toggleOff;
	private SerializedProperty toggleOn;
    private SerializedProperty checkmark;
    private SerializedProperty hightLight;
 
    protected override void OnEnable()
    {
        base.OnEnable();
        toggleOff = serializedObject.FindProperty("toggleOff");
		toggleOn = serializedObject.FindProperty("toggleOn");
        checkmark = serializedObject.FindProperty("checkmark");
        hightLight = serializedObject.FindProperty("hightLight");
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        serializedObject.Update();
        EditorGUILayout.PropertyField(toggleOff);
		EditorGUILayout.PropertyField(toggleOn);
        EditorGUILayout.PropertyField(checkmark);
        EditorGUILayout.PropertyField(hightLight);
        serializedObject.ApplyModifiedProperties();
    }
}