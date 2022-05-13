using System;
using CI.QuickSave;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Хранит всю информацию о конкретном задании
/// </summary>
public class TaskDetails
{
    public int ID { get; set; }
    public string TaskCategory { get; }
    public string Key { get; }
    public string FromCharacter { get; }
    public int AmountToComplete { get; }
    public int ProgressAmount { get; set; }
    public string Tag { get; }

    public bool IsCompleted => ProgressAmount >= AmountToComplete;

    public TaskDetails(string taskCategory, string key, int amountToComplete, string fromCharacter, string tag = "Default")
    {
        TaskCategory = taskCategory;
        Key = key;
        AmountToComplete = amountToComplete;
        FromCharacter = fromCharacter;
        Tag = tag;
    }
}

public class Task : MonoBehaviour
{
    [SerializeField] public Image CharacterSpritePlace;
    [SerializeField] public Text Description;
    [SerializeField] private Button getRewardBtn;
    [SerializeField] public Text ProgressLabel;
    [SerializeField] public Text FutureProgressLabel;
    [SerializeField] private Button sendToQuestBtn;

    public int AmountToAdd { get; set; }
    public Action AddQuestItems { get; set; }
    public TaskDetails Details { get; private set; }

    /// <summary>
    /// Заполняет параметрами пустое задание.
    /// </summary>
    /// <param name="statCategory">Категория задания.</param>
    /// <param name="key">Ключевой предмет задания.</param>
    /// <param name="amountToComplete">Количество предметов, требуемое для завершения задания.</param>
    /// <param name="fromCharacter">Персонаж, давший задание.</param>
    /// <param name="taskTag">Специальный тег для задания (если требуется).</param>
    public void Create(string statCategory, string key, int amountToComplete, string fromCharacter, string taskTag = "Default")
    {
        Details = new TaskDetails(statCategory, key, amountToComplete, fromCharacter, taskTag);
        Details.ID = Details.GetHashCode();

        Save();
    }

    /// <summary>
    /// Загружает задание из памяти.
    /// </summary>
    /// <param name="details">Детали задачи.</param>
    public void Load(TaskDetails details)
    {
        Details = details;
    }

    /// <summary>
    /// Обновляет внешний вид карточки задачи
    /// </summary>
    public void UpdateView()
    {
        var taskName = Details.TaskCategory switch
        {
            "Grow" => "вырастить",
            "Cross" => "получить при скрещивании",
            "Sell" => "продать",
            "Buy" => "купить",
            _ => "приготовить"
        };
        var itemName = Details.Key switch
        {
            "Tomato" => "помидоров",
            "Cucumber" => "огурцов",
            "Potato" => "картофелин",
            "Pea" => "стручков гороха",
            "Carrot" => "морковок",
            _ => "учпочмаков"
        };

        Description.text = $"Нужно {taskName} {Details.AmountToComplete} {itemName}.";
        ProgressLabel.text =
            $"Прогресс: {Math.Min(Details.ProgressAmount, Details.AmountToComplete)}" +
            $"/{Details.AmountToComplete}";
        CharacterSpritePlace.sprite =
            Resources.Load<Sprite>($"Characters\\{Details.FromCharacter}");

        CheckForCompletion();

        if (taskName == "приготовить"
            || itemName == "учпочмаков")
            Destroy(gameObject);
    }

    /// <summary>
    /// Обновляет внешний вид предпросмотра задачи
    /// </summary>
    public void UpdatePreview()
    {
        ProgressLabel.text =
            $"{Math.Min(Details.ProgressAmount, Details.AmountToComplete)}" +
            $"/{Details.AmountToComplete}";
        CharacterSpritePlace.sprite =
            Resources.Load<Sprite>($"Characters\\{Details.FromCharacter}");

        var futureAmount = Details.ProgressAmount + AmountToAdd;
        FutureProgressLabel.text =
            $"{futureAmount}" +
            $"/{Details.AmountToComplete}";
        FutureProgressLabel.color = futureAmount > Details.AmountToComplete
            ? new Color(1f, 0.5f, 0.5f)
            : futureAmount == Details.ProgressAmount 
                ? Color.white
                : new Color(0.5f, 1f, 0.5f);
        sendToQuestBtn.interactable = futureAmount <= Details.AmountToComplete && futureAmount != Details.ProgressAmount;
    }

    /// <summary>
    /// Запускает конечный диалог и выдаёт награды
    /// </summary>
    public void ApplyAwards()
    {
        var placeForTasks = transform.parent;
        placeForTasks.gameObject.GetComponent<Scenario>().FirstCharacterSprite = CharacterSpritePlace.sprite;
        placeForTasks.GetComponent<Scenario>()
            .CreateTaskEndDialog(TaskTools.GetPhrase(),
                new Award(AwardType.Money, money: Details.AmountToComplete * 10),
                new Award(AwardType.Reputation, reputation: Details.AmountToComplete * 15)
            );

        if (Details.Tag.Contains("FirstTask"))
        {
            var tutorWriter = QuickSaveWriter.Create("TutorialState");
            tutorWriter.Write(Details.Tag + "Completed", true);
            tutorWriter.Commit();

            if (QSReader.Create("TutorialState").ExistsAll("FirstTaskCucumberCompleted", "FirstTaskTomatoCompleted"))
            {
                var scenario = GameObject.FindGameObjectWithTag("TutorialHandler")?.GetComponent<Scenario>();
                if (scenario == null) return;
                
                // тутор для выполнения первого задания
                if (QSReader.Create("TutorialState").Exists("Tutorial_GetFirstQuest_Played"))
                    scenario.Tutorial_FirstQuestCompleted();
            }
        }

        var writer = QuickSaveWriter.Create("Tasks");
        writer.Delete(Details.ID.ToString());
        writer.Commit();

        Destroy(gameObject);
    }

    /// <summary>
    /// Обновляет прогресс задания
    /// </summary>
    public void AddItemsAndUpdate()
    {
        AddQuestItems.Invoke();
        UpdatePreview();
    }

    /// <summary>
    /// Сохраняет задание
    /// </summary>
    public void Save()
    {
        var writer = QuickSaveWriter.Create("Tasks");
        writer.Write(Details.ID.ToString(), Details);
        writer.Commit();
    }

    /// <summary>
    /// Обновляет внешний вид карточки задачи при завершении
    /// </summary>
    private void CheckForCompletion()
    {
        if (!Details.IsCompleted) return;

        getRewardBtn.interactable = true;
        Description.transform.parent.parent.GetComponent<Text>().color =
            new Color(100 / 255f, 1f, 100 / 255f);
    }
}
