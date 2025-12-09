using UnityEditor;
using UnityEngine;
using Components;

[CustomEditor(typeof(WaterSystem))]
public class WaterSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WaterSystem waterSystem = (WaterSystem)target;
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Water Bodies (Read Only)", EditorStyles.boldLabel);
        
        GUI.enabled = false;
        
        if (waterSystem.WaterBodies != null)
        {
            EditorGUILayout.LabelField("Count:", waterSystem.WaterBodies.Count.ToString());
            
            int index = 0;
            foreach (WaterComponent water in waterSystem.WaterBodies)
            {
                EditorGUILayout.ObjectField($"Element {index}", water, typeof(WaterComponent), true);
                index++;
            }
        }
        else
        {
            EditorGUILayout.LabelField("Count: 0");
        }
        
        GUI.enabled = true;
        
        if (Application.isPlaying)
        {
            Repaint();
        }
    }
}

