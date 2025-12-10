using System;
using UnityEngine;

public class WorkComponent : MonoBehaviour
{
    
    public bool isWorkDone { get; private set; }
    private void Awake()
    {
        onWork += () => isWorkDone = true;
    }

    [SerializeField] private float workRange = 5f;
    public event Action onWork;
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
