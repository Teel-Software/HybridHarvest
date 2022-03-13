using System;
using CI.QuickSave;
using Tools;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public static class TaskTools
{
    private static readonly string[] StatCategories =
        { "GrowedSeeds", "CrossedSeeds", "SelledSeeds", "PurchasedSeeds" };

    private static readonly string[] Keys = { "Tomato", "Cucumber", "Potato", "Pea" };
    private static readonly string[] Characters = { "OldMan", "OldLady", "Salesman" };

    private static readonly string[] EndPhrases =
    {
        "Благодарю тебя, путник!",
        "Огромное спасибо!",
        "Задача выполнена добротно, молодец!",
        "Спасибо, молодой человек!",
    };

    private static T GetRandomElement<T>(T[] items) =>
        items[new Random().Next(0, items.Length)];

    public static string GetStatCategory() =>
        GetRandomElement(StatCategories);

    public static string GetKey() =>
        GetRandomElement(Keys);

    public static int GetAmountToComplete() =>
        new Random().Next(5, 11);

    public static string GetCharacter() =>
        GetRandomElement(Characters);

    public static string GetPhrase() =>
        GetRandomElement(EndPhrases);
}

public class TaskController : MonoBehaviour
{
    [SerializeField] private GameObject taskPrefab;
    [SerializeField] private bool RenderTasksHere;
    [SerializeField] private GameObject taskAddPrefab;
    [SerializeField] private Text timeLabel;
    public int taskCount { get; set; }
    private bool taskAddBtnIsRendered { get; set; }
    private DateTime cooldownEnd;

    /// <summary>
    /// Создаёт новую задачу (вызывается из кнопки добавления задач)
    /// </summary>
    public void CreateNewTask()
    {
        var placeForTasks = transform.parent;
        var newTask = Instantiate(taskPrefab, placeForTasks);
        newTask.GetComponent<Task>().FillParameters(TaskTools.GetStatCategory(), TaskTools.GetKey(),
            TaskTools.GetAmountToComplete(), TaskTools.GetCharacter());
        UpdateTaskView(newTask);

        var trueTaskController = placeForTasks.GetComponent<TaskController>();
        trueTaskController.cooldownEnd = DateTime.Now.AddSeconds(10); // время кулдауна
        trueTaskController.SaveCooldownTime();
        trueTaskController.taskAddBtnIsRendered = false;
        trueTaskController.taskCount++;

        Destroy(gameObject);
    }

    /// <summary>
    /// Загружает сохранённое время окончания кулдауна
    /// </summary>
    private void LoadCooldownTime()
    {
        var reader = QSReader.Create("Tasks");
        cooldownEnd = reader.Exists("CooldownEnd")
            ? reader.Read<DateTime>("CooldownEnd")
            : DateTime.Now;
    }

    /// <summary>
    /// Сохраняет время окончания кулдауна
    /// </summary>
    private void SaveCooldownTime()
    {
        var writer = QuickSaveWriter.Create("Tasks");
        writer.Write("CooldownEnd", cooldownEnd);
        writer.Commit();
    }

    /// <summary>
    /// Выводит информацию о задании в соответствии с его параметрами
    /// </summary>
    /// <param name="taskObj">Карточка для отрисовки задания</param>
    private static void UpdateTaskView(GameObject taskObj)
    {
        var taskComp = taskObj.GetComponent<Task>();
        var taskName = taskComp.Details.StatCategory switch
        {
            "GrowedSeeds" => "вырастить",
            "CrossedSeeds" => "получить при скрещивании",
            "SelledSeeds" => "продать",
            "PurchasedSeeds" => "купить",
            _ => "приготовить"
        };
        var itemName = taskComp.Details.Key switch
        {
            "Tomato" => "помидоров",
            "Cucumber" => "огурцов",
            "Potato" => "картофелин",
            "Pea" => "стручков гороха",
            _ => "учпочмаков"
        };

        taskComp.description.text = $"Нужно {taskName} {taskComp.Details.AmountToComplete} {itemName}.";
        taskComp.progressLabel.text =
            $"Прогресс: {Math.Min(taskComp.Details.ProgressAmount, taskComp.Details.AmountToComplete)}" +
            $"/{taskComp.Details.AmountToComplete}";
        taskComp.characterSpritePlace.sprite =
            Resources.Load<Sprite>($"Characters\\{taskComp.Details.FromCharacter}");
        
        taskComp.CheckForCompletion();
        if (taskComp.IsCompleted)
            taskComp.description.transform.parent.parent.GetComponent<Text>().color =
                new Color(100 / 255f, 1f, 100 / 255f);
    }

    /// <summary>
    /// Отрисовывыет уже существующие задачи
    /// </summary>
    private void RenderCurrentTasks()
    {
        var reader = QSReader.Create("Tasks");
        var allKeys = reader.GetAllKeys();
        var CGD = gameObject.GetComponent<ClearGameData>() ?? gameObject.AddComponent<ClearGameData>();
        CGD.ClearChildren(gameObject);

        foreach (var key in allKeys)
        {
            var success = reader.TryRead<TaskDetails>(key, out var details);
            if (!success || details.AmountToComplete <= 0) continue;

            var newTask = Instantiate(taskPrefab, transform);
            var taskComp = newTask.GetComponent<Task>();
            taskComp.Details = details;
            UpdateTaskView(newTask);
            taskCount++;

            if (taskComp.IsCompleted)
                newTask.transform.SetAsFirstSibling();
        }
    }

    private void OnEnable()
    {
        if (!RenderTasksHere) return;

        taskAddBtnIsRendered = false;
        RenderCurrentTasks();
        LoadCooldownTime();
    }

    private void Update()
    {
        if (!RenderTasksHere) return;

        var secondsRemaining = (cooldownEnd - DateTime.Now).TotalSeconds;
        timeLabel.text = secondsRemaining >= 0
            ? $"До нового задания осталось {TimeFormatter.Format((int) secondsRemaining)}"
            : "Доступно новое задание!";

        if (taskAddBtnIsRendered || !(secondsRemaining < 0)) return;

        Instantiate(taskAddPrefab, transform);
        taskAddBtnIsRendered = true;
    }
}
