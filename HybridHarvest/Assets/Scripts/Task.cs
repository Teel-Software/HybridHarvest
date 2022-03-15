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
    [SerializeField] public Image characterSpritePlace;
    [SerializeField] public Text description;
    [SerializeField] private GameObject getRewardBtn;
    [SerializeField] public Text progressLabel;

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

    public void Save()
    {
        var writer = QuickSaveWriter.Create("Tasks");
        writer.Write(Details.ID.ToString(), Details);
        writer.Commit();
    }

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
            _ => "учпочмаков"
        };

        description.text = $"Нужно {taskName} {Details.AmountToComplete} {itemName}.";
        progressLabel.text =
            $"Прогресс: {Math.Min(Details.ProgressAmount, Details.AmountToComplete)}" +
            $"/{Details.AmountToComplete}";
        characterSpritePlace.sprite =
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
        progressLabel.text =
            $"{Math.Min(Details.ProgressAmount, Details.AmountToComplete)}" +
            $"/{Details.AmountToComplete}";
        characterSpritePlace.sprite =
            Resources.Load<Sprite>($"Characters\\{Details.FromCharacter}");
    }

    /// <summary>
    /// Обновляет внешний вид карточки задачи при завершении
    /// </summary>
    public void CheckForCompletion()
    {
        if (!Details.IsCompleted) return;

        getRewardBtn.GetComponent<Button>().interactable = true;
        description.transform.parent.parent.GetComponent<Text>().color =
            new Color(100 / 255f, 1f, 100 / 255f);
    }

    /// <summary>
    /// Запускает конечный диалог и выдаёт награды
    /// </summary>
    public void ApplyAwards()
    {
        var placeForTasks = transform.parent;
        placeForTasks.GetComponent<TaskController>().taskCount--;
        placeForTasks.gameObject.GetComponent<Scenario>().FirstCharacterSprite = characterSpritePlace.sprite;
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

    public void AddItemAndUpdate()
    {
        AddQuestItem.Invoke();
        Details.ProgressAmount++;
        UpdatePreview();
        Save();
    }
}
