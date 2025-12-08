using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }
    [SerializeField] private List<WorkTask> taskPrefabs;
    private List<WorkTask> availableTasks = new();
    private HashSet<Vector3Int> completedTasks = new();

    public WorkTask CreateRandomTask(Vector3 position)
    {
        Vector3Int cell = TilemapManager.instance.WorldToHexAxial(position);
        if(completedTasks.Contains(cell))
            return null;
        int taskType = Random.Range(0, taskPrefabs.Count);
        WorkTask task = Instantiate(taskPrefabs[taskType], TilemapManager.instance.HexAxialToWorld(cell), Quaternion.identity);
        task.onComplete += () =>
        {
            availableTasks.Remove(task);
            completedTasks.Add(cell);
        };
        availableTasks.Add(task);
        return task;
    }
    
    public WorkTask GetNearestTask(Vector3 position, float range)
    {
        WorkTask task = null;
        float minDistance = range;
        foreach (var availableTask in availableTasks)
        {
            if (availableTask == null || availableTask.progress >= 1) continue;
            float distance = Vector3.Distance(position, availableTask.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                task = availableTask;
            }
        }

        if (task != null)
        {
            Debug.Log("nearest task: " + task.transform.position);
        }
        return task;
    }
    
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
