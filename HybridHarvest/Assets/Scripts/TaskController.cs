using System;
using System.Collections.Generic;
using CI.QuickSave;
using Tools;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public static class TaskTools
{
    private static readonly Random rnd = new Random();

    private static readonly string[] StatCategories =
    {
        "Grow",
        // "Cross",
        // "Sell",
        // "Buy"
    };

    private static readonly string[] Keys = { "Tomato", "Cucumber", "Potato", "Pea", "Carrot" };
    private static readonly string[] Characters = { "OldMan", "OldLady", "Salesman" };

    private static readonly string[] EndPhrases =
    {
        "Благодарю тебя, путник!",
        "Огромное спасибо!",
        "Задача выполнена добротно, молодец!",
        "Спасибо, молодой человек!",
    };

    private static T GetRandomElement<T>(T[] items) =>
        items[rnd.Next(0, items.Length)];

    public static string GetStatCategory() =>
        GetRandomElement(StatCategories);

    public static string GetKey() =>
        GetRandomElement(Keys);

    public static int GetAmountToComplete() =>
        rnd.Next(5, 11);

    public static string GetCharacter() =>
        GetRandomElement(Characters);

    public static string GetPhrase() =>
        GetRandomElement(EndPhrases);
}

public class TaskController : MonoBehaviour
{
    [SerializeField] private GameObject taskPrefab;
    [SerializeField] private bool renderTasksHere;
    [SerializeField] private GameObject taskAddPrefab;
    [SerializeField] private Text timeLabel;
    [SerializeField] private GameObject previewPrefab;

    private const int CooldownTimeSeconds = 3; // время кулдауна
    public GameObject QuestsPreviewPanel { get; private set; }
    private bool taskAddBtnIsRendered { get; set; }
    private DateTime cooldownEnd;

    /// <summary>
    /// Создаёт новую задачу (вызывается из кнопки добавления задач)
    /// </summary>
    public void CreateNewTask()
    {
        var placeForTasks = transform.parent;
        var newTask = Instantiate(taskPrefab, placeForTasks).GetComponent<Task>();
        newTask.Create(TaskTools.GetStatCategory(), TaskTools.GetKey(),
            TaskTools.GetAmountToComplete(), TaskTools.GetCharacter());
        newTask.UpdateView();

        var trueTaskController = placeForTasks.GetComponent<TaskController>();
        trueTaskController.cooldownEnd = DateTime.Now.AddSeconds(CooldownTimeSeconds);
        trueTaskController.SaveCooldownTime();
        trueTaskController.taskAddBtnIsRendered = false;

        Destroy(gameObject);
    }

    /// <summary>
    /// Открывает просмотр доступных заданий
    /// </summary>
    /// <param name="seedName">Английское название плода для задания</param>
    /// <param name="itemsCount">Количество плодов, которые могут быть добавлены в задачу</param>
    public GameObject OpenQuestsPreview(string seedName, int itemsCount)
    {
        if (QuestsPreviewPanel == null)
        {
            QuestsPreviewPanel = gameObject;
            QuestsPreviewPanel = QuestsPreviewPanel
                .transform
                .Find("QuestsPreview")
                .gameObject;
        }

        QuestsPreviewPanel.SetActive(true);
        var placeForRender = QuestsPreviewPanel
            .GetComponentInChildren<GridLayoutGroup>()
            .gameObject;
        RenderCurrentTasks(true, placeForRender, seedName, itemsCount);

        return placeForRender;
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
    /// Отрисовывыет существующие задачи
    /// </summary>
    /// <param name="isPreview">Указать true, если необходимо отрисовать только предпросмотр задач</param>
    /// <param name="placeForRender">Место для отрисовки предпросмотра</param>
    /// <param name="seedName">Название семечка, к которому относится предпросмотр</param>
    /// <param name="itemsCount">Количество плодов, которые могут быть добавлены в задачу</param>
    private void RenderCurrentTasks(
        bool isPreview = false,
        GameObject placeForRender = null,
        string seedName = null,
        int itemsCount = 0)
    {
        var reader = QSReader.Create("Tasks");
        var renderedTasks = new List<GameObject>();
        var allKeys = reader.GetAllKeys();

        ClearGameData.ClearChildren(isPreview ? placeForRender : gameObject);

        foreach (var key in allKeys)
        {
            var success = reader.TryRead<TaskDetails>(key, out var details);
            if (!success
                || details.AmountToComplete <= 0
                || details.TaskCategory == null
                || isPreview && (details.TaskCategory != "Grow"
                                 || details.Key != seedName
                                 // || details.ProgressAmount + itemsCount > details.AmountToComplete
                                 || details.IsCompleted))
                continue;

            var newTask = Instantiate(isPreview ? previewPrefab : taskPrefab,
                isPreview ? placeForRender.transform : transform);
            renderedTasks.Add(newTask);

            var taskComp = newTask.GetComponent<Task>();
            taskComp.Load(details);
            taskComp.AmountToAdd = itemsCount;
            if (isPreview)
                taskComp.UpdatePreview();
            else taskComp.UpdateView();
        }

        renderedTasks.Sort((taskObj1, taskObj2) =>
            {
                var taskDetails1 = taskObj1.GetComponent<Task>().Details;
                var taskDetails2 = taskObj2.GetComponent<Task>().Details;
                return (taskDetails1.AmountToComplete - taskDetails1.ProgressAmount) -
                       (taskDetails2.AmountToComplete - taskDetails2.ProgressAmount);
            }
        );

        for (var i = 0; i < renderedTasks.Count; i++)
            renderedTasks[i].transform.SetSiblingIndex(i);

        if (!isPreview || renderedTasks.Count != 0) return;
        
        QuestsPreviewPanel.SetActive(false);
        GetComponent<HarvestProcessor>().previewsShouldBeOpen = false;
        GetComponent<NotificationCenter>().Show("Подходящих заданий нет.");
    }

    /// <summary>
    /// Отрисовывает сохранённые задачи
    /// </summary>
    private void OnEnable()
    {
        if (!renderTasksHere) return;

        taskAddBtnIsRendered = false;
        RenderCurrentTasks();
        LoadCooldownTime();
    }

    /// <summary>
    /// Обновляет время, оставшееся до нового задания
    /// </summary>
    private void Update()
    {
        if (!renderTasksHere) return;

        var secondsRemaining = (cooldownEnd - DateTime.Now).TotalSeconds;
        timeLabel.text = secondsRemaining >= 0
            ? $"До нового задания осталось {TimeFormatter.Format((int) secondsRemaining)}"
            : "Доступно новое задание!";

        if (taskAddBtnIsRendered || !(secondsRemaining < 0)) return;

        Instantiate(taskAddPrefab, transform);
        taskAddBtnIsRendered = true;
    }
}
