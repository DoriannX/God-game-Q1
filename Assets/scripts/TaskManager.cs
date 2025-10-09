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

    public WorkTask CreateRandomTask(Vector2 position)
    {
        Vector3Int cell = TilemapManager.instance.tilemap.WorldToCell(position);
        if(completedTasks.Contains(cell))
            return null;
        int taskType = Random.Range(0, taskPrefabs.Count);
        WorkTask task = Instantiate(taskPrefabs[taskType], TilemapManager.instance.GetCellCenterWorld(cell), Quaternion.identity);
        task.onComplete += () =>
        {
            availableTasks.Remove(task);
            completedTasks.Add(cell);
        };
        availableTasks.Add(task);
        return task;
    }
    
    public WorkTask GetNearestTask(Vector2 position, float range)
    {
        WorkTask task = null;
        float minDistance = range;
        foreach (var availableTask in availableTasks)
        {
            if (availableTask == null || availableTask.progress >= 1) continue;
            float distance = Vector2.Distance(position, availableTask.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                task = availableTask;
            }
        }
        return task;
    }
}
