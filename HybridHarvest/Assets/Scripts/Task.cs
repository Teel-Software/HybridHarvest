using System.Collections.Generic;
using CI.QuickSave;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Хранит всю информацию о конкретном задании
/// </summary>
public class TaskDetails
{
    public string StatCategory { get; }
    public string Key { get; }

    public int ProgressAmount
    {
        get
        {
            var seedsInfo = new Dictionary<string, int>();
            var reader = QSReader.Create("Statistics");
            var currentAmount = 0;

            if (reader.Exists(StatCategory))
                seedsInfo = reader.Read<Dictionary<string, int>>(StatCategory);

            if (seedsInfo.ContainsKey(Key))
                currentAmount = seedsInfo[Key];

            return currentAmount - StartAmount;
        }
    }

    public int AmountToComplete { get; }
    public string FromCharacter { get; }
    public int StartAmount { get; set; }
    public int ID { get; set; }

    public TaskDetails(string statCategory, string key, int amountToComplete, string fromCharacter)
    {
        StatCategory = statCategory;
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

    public TaskDetails Details { get; set; }
    public bool IsCompleted { get; private set; }

    /// <summary>
    /// Заполняет параметрами пустое задание
    /// </summary>
    /// <param name="statCategory">Категория задания</param>
    /// <param name="key">Ключевой предмет задания</param>
    /// <param name="amountToComplete">Количество предметов, требуемое для завершения задания</param>
    /// <param name="fromCharacter">Персонаж, давший задание</param>
    public void FillParameters(string statCategory, string key, int amountToComplete, string fromCharacter)
    {
        Details = new TaskDetails(statCategory, key, amountToComplete, fromCharacter);
        Details.ID = Details.GetHashCode();
        var seedsInfo = new Dictionary<string, int>();
        var reader = QSReader.Create("Statistics");

        if (reader.Exists(statCategory))
            seedsInfo = reader.Read<Dictionary<string, int>>(Details.StatCategory);
        if (seedsInfo.ContainsKey(key))
            Details.StartAmount = seedsInfo[key];

        var writer = QuickSaveWriter.Create("Tasks");
        writer.Write(Details.ID.ToString(), Details);
        writer.Commit();
    }

    /// <summary>
    /// Проверяет задачу на завершение
    /// </summary>
    public void CheckForCompletion()
    {
        if (Details.ProgressAmount < Details.AmountToComplete) return;

        getRewardBtn.GetComponent<Button>().interactable = true;
        IsCompleted = true;
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
}
