using System;
using UnityEditor;
using UnityEngine;

public abstract class WorkTask : MonoBehaviour
{
    [CustomEditor(typeof(WorkTask), true)]
    public class TaskEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            WorkTask workTask = (WorkTask)target;

            if (GUILayout.Button("Work"))
            {
                workTask.Work();
            }
        }
    }
    
    public event Action onComplete;
    public float progress { get; private set; } = 0f;
    [field:SerializeField] public float progressIncrement { get; protected set; } = 0.1f;

    public virtual void Work()
    {
        print("working");
        progress += progressIncrement;
        if (progress >= 1f)
        {
            OnComplete();
            onComplete?.Invoke();
        }
    }
    
    protected abstract void OnComplete();
}