using System;
using UnityEngine;

public class WorkComponent : MonoBehaviour
{
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
