using System;
using UnityEngine;
using UnityEngine.UI;

public class TaskController : MonoBehaviour
{
    [SerializeField] private GameObject taskPrefab;
    [SerializeField] private bool RenderTasksHere;
    [SerializeField] private GameObject taskAddPrefab;

    public void CreateNewTask()
    {
        var newTask = Instantiate(taskPrefab, transform.parent);
        newTask.GetComponent<Task>().FillParameters("GrowedSeeds", "Cucumber", 5, "OldLady");
        UpdateTaskView(newTask);
        transform.SetAsLastSibling();
    }

    private static void UpdateTaskView(GameObject taskObj)
    {
        var taskComp = taskObj.GetComponent<Task>();
        var taskName = taskComp.Details.StatCategory switch
        {
            "GrowedSeeds" => "вырастить",
            _ => "приготовить"
        };
        var itemName = taskComp.Details.Key switch
        {
            "Tomato" => "помидоров",
            "Cucumber" => "огурцов",
            _ => "учпочмаков"
        };

        taskComp.description.text = $"Нужно {taskName} {taskComp.Details.AmountToComplete} {itemName}.";
        taskComp.characterSpritePlace.sprite =
            Resources.Load<Sprite>($"Characters\\{taskComp.Details.FromCharacter}");
        taskComp.CheckForCompletion();
        if (taskComp.IsComplete)
            taskComp.description.transform.parent.parent.GetComponent<Text>().color =
                new Color(100 / 255f, 1f, 100 / 255f);
    }

    private void RenderCurrentTasks()
    {
        var reader = QSReader.Create("Tasks");
        var allKeys = reader.GetAllKeys();
        var CGD = gameObject.GetComponent<ClearGameData>() ?? gameObject.AddComponent<ClearGameData>();
        CGD.ClearChildren(gameObject);

        foreach (var key in allKeys)
        {
            var details = reader.Read<TaskDetails>(key);
            if (details.AmountToComplete <= 0) continue;

            var newTask = Instantiate(taskPrefab, transform);
            newTask.GetComponent<Task>().Details = details;
            UpdateTaskView(newTask);
        }

        Instantiate(taskAddPrefab, transform);
    }

    private void OnEnable()
    {
        if (RenderTasksHere)
            RenderCurrentTasks();
    }
}
