using System;
using CI.QuickSave;
using Tools;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public static class TaskTools
{
    private static readonly string[] StatCategories =
    {
        "Grow",
        // "Cross",
        // "Sell",
        // "Buy"
    };

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
    [SerializeField] private GameObject previewPrefab;

    public int taskCount { get; set; }
    [NonSerialized] public GameObject questsPreviewPanel;
    private bool taskAddBtnIsRendered { get; set; }
    private DateTime cooldownEnd;
    private GameObject placeForPreview;
    private string lastSeedName;

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
        trueTaskController.cooldownEnd = DateTime.Now.AddSeconds(10); // время кулдауна
        trueTaskController.SaveCooldownTime();
        trueTaskController.taskAddBtnIsRendered = false;
        trueTaskController.taskCount++;

        Destroy(gameObject);
    }

    /// <summary>
    /// Открывает просмотр доступных заданий
    /// </summary>
    /// <param name="openBtnText">Текст на кнопке открытия превью</param>
    /// <param name="seed">Плод для задания</param>
    public GameObject OpenQuestsPreview(Text openBtnText, Seed seed)
    {
        if (questsPreviewPanel == null)
        {
            questsPreviewPanel = gameObject;
            questsPreviewPanel = questsPreviewPanel
                .transform
                .Find("QuestsPreview")
                .gameObject;
        }

        var placeForRender = questsPreviewPanel.GetComponentInChildren<GridLayoutGroup>().gameObject;

        questsPreviewPanel.SetActive(openBtnText.text != "<" || !questsPreviewPanel.activeSelf);

        var sendBtns = GameObject.FindGameObjectsWithTag("SendToQuestBtn");
        foreach (var obj in sendBtns)
            obj.GetComponentInChildren<Text>().text = ">";

        if (questsPreviewPanel.activeSelf)
            openBtnText.text = "<";

        RenderTaskPreviews(placeForRender, seed.Name);

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
    /// Отрисовывыет уже существующие задачи
    /// </summary>
    private void RenderCurrentTasks()
    {
        var reader = QSReader.Create("Tasks");
        var allKeys = reader.GetAllKeys();
        ClearChildren(gameObject);

        foreach (var key in allKeys)
        {
            var success = reader.TryRead<TaskDetails>(key, out var details);
            if (!success
                || details.AmountToComplete <= 0
                || details.TaskCategory == null)
                continue;

            var newTask = Instantiate(taskPrefab, transform);
            var taskComp = newTask.GetComponent<Task>();
            taskComp.Load(details);
            taskComp.UpdateView();
            taskCount++;

            if (!taskComp.Details.IsCompleted) continue;

            newTask.transform.SetAsFirstSibling();
        }
    }

    public void RenderTaskPreviews(GameObject placeForRender = null, string seedName = null)
    {
        if (placeForRender != null)
            placeForPreview = placeForRender;

        if (seedName != null)
            lastSeedName = seedName;

        var reader = QSReader.Create("Tasks");
        var allKeys = reader.GetAllKeys();
        ClearChildren(placeForPreview);

        foreach (var key in allKeys)
        {
            var success = reader.TryRead<TaskDetails>(key, out var details);
            if (!success
                || details.AmountToComplete <= 0
                || details.TaskCategory == null
                || details.TaskCategory != "Grow"
                || details.Key != lastSeedName)
                continue;

            var newPreview = Instantiate(previewPrefab, placeForPreview.transform);
            var taskComp = newPreview.GetComponent<Task>();
            taskComp.Load(details);
            taskComp.UpdatePreview();

            if (!taskComp.Details.IsCompleted) continue;

            newPreview.GetComponentInChildren<Button>().interactable = false;
            newPreview.transform.SetAsLastSibling();
        }
    }

    private void ClearChildren(GameObject obj)
    {
        var CGD = obj.GetComponent<ClearGameData>()
                  ?? obj.AddComponent<ClearGameData>();
        CGD.ClearChildren(obj);
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
