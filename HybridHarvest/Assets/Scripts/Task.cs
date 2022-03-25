using System;
using CI.QuickSave;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Хранит всю информацию о конкретном задании
/// </summary>
public class TaskDetails
{
    public string TaskCategory { get; }
    public string Key { get; }
    public string FromCharacter { get; }
    public int AmountToComplete { get; }
    public bool IsCompleted => ProgressAmount >= AmountToComplete;
    public int ID { get; set; }
    public int ProgressAmount { get; set; }

    public TaskDetails(string taskCategory, string key, int amountToComplete, string fromCharacter)
    {
        TaskCategory = taskCategory;
        Key = key;
        AmountToComplete = amountToComplete;
        FromCharacter = fromCharacter;
    }
}

public class Task : MonoBehaviour
{
    [SerializeField] public Image CharacterSpritePlace;
    [SerializeField] public Text Description;
    [SerializeField] private GameObject getRewardBtn;
    [SerializeField] public Text ProgressLabel;

    public TaskDetails Details { get; private set; }
    public Action AddQuestItem { get; set; }

    /// <summary>
    /// Заполняет параметрами пустое задание
    /// </summary>
    /// <param name="statCategory">Категория задания</param>
    /// <param name="key">Ключевой предмет задания</param>
    /// <param name="amountToComplete">Количество предметов, требуемое для завершения задания</param>
    /// <param name="fromCharacter">Персонаж, давший задание</param>
    public void Create(string statCategory, string key, int amountToComplete, string fromCharacter)
    {
        Details = new TaskDetails(statCategory, key, amountToComplete, fromCharacter);
        Details.ID = Details.GetHashCode();

        Save();
    }

    /// <summary>
    /// Загружает задание из памяти
    /// </summary>
    /// <param name="details">Детали задачи</param>
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
            "Carrot" => "штук моркови",
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

        var writer = QuickSaveWriter.Create("Tasks");
        writer.Delete(Details.ID.ToString());
        writer.Commit();

        Destroy(gameObject);
    }

    /// <summary>
    /// Обновляет прогресс задания
    /// </summary>
    public void AddItemAndUpdate()
    {
        AddQuestItem.Invoke();
        UpdatePreview();
        Save();
    }

    /// <summary>
    /// Сохраняет задание
    /// </summary>
    private void Save()
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

        getRewardBtn.GetComponent<Button>().interactable = true;
        Description.transform.parent.parent.GetComponent<Text>().color =
            new Color(100 / 255f, 1f, 100 / 255f);
    }
}
