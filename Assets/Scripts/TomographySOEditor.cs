using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TomographySO))]
public class TomographySOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var myScript = (TomographySO)target;
        if (GUILayout.Button("Add Tomography"))
        {
            myScript.PopulateDictionary();
        }
    }
}
