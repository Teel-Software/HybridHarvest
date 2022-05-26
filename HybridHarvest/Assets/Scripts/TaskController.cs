using System;
using System.Collections.Generic;
using System.Linq;
using CI.QuickSave;
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

    private static T GetRandomElement<T>(params T[] items) =>
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
    [SerializeField] private GameObject EmptyListText;
    [SerializeField] private GameObject taskAddPrefab;
    [SerializeField] private Text timeLabel;
    [SerializeField] private GameObject previewPrefab;

    private const int CooldownTimeSeconds = 300; // время кулдауна
    private GameObject QuestsPreviewPanel { get; set; }
    private bool taskAddBtnIsRendered { get; set; }
    private DateTime cooldownEnd;

    // /// <summary>
    // /// Создаёт новую задачу (вызывается из кнопки добавления задач).
    // /// </summary>
    // public void CreateNewTask()
    // {
    //     var placeForTasks = transform.parent;
    //     var newTask = Instantiate(taskPrefab, placeForTasks).GetComponent<Task>();
    //     newTask.Create(TaskTools.GetStatCategory(), TaskTools.GetKey(),
    //         TaskTools.GetAmountToComplete(), TaskTools.GetCharacter());
    //     newTask.UpdateView();
    //
    //     var trueTaskController = placeForTasks.GetComponent<TaskController>();
    //     trueTaskController.cooldownEnd = DateTime.Now.AddSeconds(CooldownTimeSeconds);
    //     trueTaskController.SaveCooldownTime();
    //     trueTaskController.taskAddBtnIsRendered = false;
    //
    //     Destroy(gameObject);
    // }

    /// <summary>
    /// Открывает просмотр доступных заданий.
    /// </summary>
    /// <param name="seedName">Английское название плода для задания.</param>
    /// <param name="itemsCount">Количество плодов, которые могут быть добавлены в задачу.</param>
    public GameObject RenderQuestsPreview(string seedName, int itemsCount)
    {
        if (QuestsPreviewPanel == null)
            QuestsPreviewPanel = GameObject.Find("QuestsPreview");

        var placeForRender = QuestsPreviewPanel
            .GetComponentInChildren<VerticalLayoutGroup>()
            .gameObject;
        RenderCurrentTasks(true, placeForRender, seedName, itemsCount);

        return placeForRender;
    }

    /// <summary>
    /// Удаляет задание.
    /// </summary>
    private static void DeleteTask(string ID)
    {
        var writer = QuickSaveWriter.Create("Tasks");
        writer.Delete(ID);
        writer.Commit();
    }

    /// <summary>
    /// Сортирует задачи по возрастанию оставшегося количества квестовых предметов.
    /// </summary>
    /// <param name="renderedTasks">Задачи.</param>
    private static void SortTasks(List<GameObject> renderedTasks)
    {
        renderedTasks.Sort((taskObj1, taskObj2) =>
            {
                var t1 = taskObj1.GetComponent<Task>();
                var t2 = taskObj2.GetComponent<Task>();
                return t1.SumRemainingAmount() - t2.SumRemainingAmount();
            }
        );
    }

    /// <summary>
    /// Сортирует задачи по возрастанию оставшегося количества квестовых предметов.
    /// </summary>
    /// <param name="renderedTasks">Задачи.</param>
    private static void SortTaskPreviews(List<GameObject> renderedTasks)
    {
        renderedTasks.Sort((taskObj1, taskObj2) =>
            {
                var t1 = taskObj1.GetComponent<TaskPreview>();
                var t2 = taskObj2.GetComponent<TaskPreview>();
                return t1.SumRemainingAmount() - t2.SumRemainingAmount();
            }
        );
    }

    /// <summary>
    /// Создаёт первое задание.
    /// </summary>
    private void CreateFirstTask()
    {
        var newTask = Instantiate(taskPrefab, transform).GetComponent<Task>();
        newTask.Add("OldLady", new[]
            {
                new Award(AwardType.Money, amount: 200),
                new Award(AwardType.Reputation, amount: 300),
            },
            "FirstTask",
            new SubTask("Grow", "Cucumber", 5),
            new SubTask("Grow", "Tomato", 5));
        newTask.UpdateView();
    }

    /// <summary>
    /// Сохраняет время окончания кулдауна.
    /// </summary>
    private void SaveCooldownTime()
    {
        var writer = QuickSaveWriter.Create("Tasks");
        writer.Write("CooldownEnd", cooldownEnd);
        writer.Commit();
    }

    /// <summary>
    /// Загружает сохранённое время окончания кулдауна.
    /// </summary>
    private void LoadCooldownTime()
    {
        var reader = QSReader.Create("Tasks");
        cooldownEnd = reader.Exists("CooldownEnd")
            ? reader.Read<DateTime>("CooldownEnd")
            : DateTime.Now;
    }

    /// <summary>
    /// Отрисовывыет существующие задачи.
    /// </summary>
    /// <param name="isPreview">Указать true, если необходимо отрисовать только предпросмотр задач.</param>
    /// <param name="placeForRender">Место для отрисовки предпросмотра.</param>
    /// <param name="seedName">Название семечка, к которому относится предпросмотр.</param>
    /// <param name="itemsCount">Количество плодов, которые могут быть добавлены в задачу.</param>
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
            if (!success || details.SubTasks == null)
            {
                DeleteTask(key);
                continue;
            }

            if (!isPreview)
            {
                var newTask = Instantiate(taskPrefab, transform);
                var taskComp = newTask.GetComponent<Task>();

                renderedTasks.Add(newTask);
                taskComp.Details = details;
                taskComp.UpdateView();
            }
            else
            {
                var neededSubTasks = details.SubTasks
                    .Where(st => st.Key == seedName
                                 && !st.IsCompleted
                                 && st.TaskCategory == "Grow");

                foreach (var subTask in neededSubTasks)
                {
                    var newTask = Instantiate(previewPrefab, placeForRender.transform);
                    var taskComp = newTask.GetComponent<TaskPreview>();

                    renderedTasks.Add(newTask);
                    taskComp.Details = details;
                    taskComp.CurrentSubTask = subTask;
                    taskComp.AmountToAdd = itemsCount;
                    taskComp.UpdatePreview();
                }
            }
        }

        if (!isPreview)
            SortTasks(renderedTasks);
        else SortTaskPreviews(renderedTasks);

        for (var i = 0; i < renderedTasks.Count; i++)
            renderedTasks[i].transform.SetSiblingIndex(i);

        if (isPreview)
        {
            QuestsPreviewPanel
                .transform
                .Find("EmptyListText")
                .gameObject
                .SetActive(renderedTasks.Count == 0);
        }
    }

    /// <summary>
    /// Отрисовывает сохранённые задачи.
    /// </summary>
    private void OnEnable()
    {
        if (!renderTasksHere) return;

        // taskAddBtnIsRendered = false;
        // LoadCooldownTime();

        var scenario = GameObject.FindGameObjectWithTag("TutorialHandler")?.GetComponent<Scenario>();
        if (scenario == null) return;

        // тутор для выдачи первого квеста
        if (QSReader.Create("StoryState").Exists("Story_SideMenuToQuests_Played")
            && !QSReader.Create("StoryState").Exists("Story_GetFirstQuest_Played"))
        {
            CreateFirstTask();
            scenario.GetFirstQuest();
        }

        RenderCurrentTasks();
    }

    /// <summary>
    /// Обновляет время, оставшееся до нового задания
    /// </summary>
    private void Update()
    {
        if (!renderTasksHere) return;

        EmptyListText.SetActive(transform.childCount == 0);

        // var secondsRemaining = (cooldownEnd - DateTime.Now).TotalSeconds;
        // timeLabel.text = secondsRemaining >= 0
        //     ? $"До нового задания осталось {TimeFormatter.Format((int) secondsRemaining)}"
        //     : "Доступно новое задание!";
        //
        // if (taskAddBtnIsRendered || !(secondsRemaining < 0)) return;
        //
        // Instantiate(taskAddPrefab, transform);
        // taskAddBtnIsRendered = true;
    }
}
