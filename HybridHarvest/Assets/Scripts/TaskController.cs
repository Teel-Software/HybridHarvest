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
    [SerializeField] private bool renderTasksHere;
    [SerializeField] private GameObject taskAddPrefab;
    [SerializeField] private Text timeLabel;
    [SerializeField] private GameObject previewPrefab;

    public int TaskCount { get; set; }
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
        trueTaskController.cooldownEnd = DateTime.Now.AddSeconds(10); // время кулдауна
        trueTaskController.SaveCooldownTime();
        trueTaskController.taskAddBtnIsRendered = false;
        trueTaskController.TaskCount++;

        Destroy(gameObject);
    }

    /// <summary>
    /// Открывает просмотр доступных заданий
    /// </summary>
    /// <param name="openBtnText">Текст на кнопке открытия превью</param>
    /// <param name="seed">Плод для задания</param>
    public GameObject OpenQuestsPreview(Text openBtnText, Seed seed)
    {
        if (QuestsPreviewPanel == null)
        {
            QuestsPreviewPanel = gameObject;
            QuestsPreviewPanel = QuestsPreviewPanel
                .transform
                .Find("QuestsPreview")
                .gameObject;
        }

        QuestsPreviewPanel.SetActive(openBtnText.text == ">" || !QuestsPreviewPanel.activeSelf);

        var sendBtns = GameObject.FindGameObjectsWithTag("SendToQuestBtn");
        foreach (var obj in sendBtns)
            obj.GetComponentInChildren<Text>().text = ">";
        if (QuestsPreviewPanel.activeSelf)
            openBtnText.text = "<";

        var placeForRender = QuestsPreviewPanel
            .GetComponentInChildren<GridLayoutGroup>()
            .gameObject;
        RenderCurrentTasks(true, placeForRender, seed.Name);

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
    private void RenderCurrentTasks(bool isPreview = false, GameObject placeForRender = null, string seedName = null)
    {
        var reader = QSReader.Create("Tasks");
        var allKeys = reader.GetAllKeys();
        ClearChildren(isPreview ? placeForRender : gameObject);

        foreach (var key in allKeys)
        {
            var success = reader.TryRead<TaskDetails>(key, out var details);
            if (!success
                || details.AmountToComplete <= 0
                || details.TaskCategory == null
                || isPreview && (details.TaskCategory != "Grow"
                                 || details.Key != seedName
                                 || details.IsCompleted))
                continue;

            var newTask = Instantiate(isPreview ? previewPrefab : taskPrefab,
                isPreview ? placeForRender.transform : transform);
            var taskComp = newTask.GetComponent<Task>();
            taskComp.Load(details);
            if (isPreview)
                taskComp.UpdatePreview();
            else taskComp.UpdateView();
            TaskCount++;

            if (!taskComp.Details.IsCompleted) continue;

            newTask.transform.SetAsFirstSibling();
        }
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

    /// <summary>
    /// Удаляет все дочерние объекты
    /// </summary>
    /// <param name="obj">Родительский объект</param>
    private static void ClearChildren(GameObject obj)
    {
        var CGD = obj.GetComponent<ClearGameData>()
                  ?? obj.AddComponent<ClearGameData>();
        ClearGameData.ClearChildren(obj);
    }
}
