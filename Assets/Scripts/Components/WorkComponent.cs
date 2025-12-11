using System;
using UnityEngine;

public class WorkComponent : MonoBehaviour
{
    public event Action onWork;
    public bool isWorkDone { get; private set; }
    [SerializeField] private float workRange = 5f;
    private void Awake()
    {
        onWork += () => isWorkDone = true;
    }

    public void Work()
    {
        GetTask()?.Work();
        onWork?.Invoke();
    }
    
    public WorkTask GetTask()
    {
        return TaskManager.instance.GetNearestTask(transform.position, workRange) ?? TaskManager.instance.CreateRandomTask(transform.position);
    }
}
