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
        
        if (waterSystem.waterSources != null)
        {
            EditorGUILayout.LabelField("Count:", waterSystem.waterSources.Count.ToString());
            
            int index = 0;
            foreach (WaterSource water in waterSystem.waterSources)
            {
                EditorGUILayout.ObjectField($"Element {index}", water, typeof(WaterSource), true);
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

