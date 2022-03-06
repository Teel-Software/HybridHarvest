using System.Collections.Generic;
using CI.QuickSave;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TaskDetails
{
    public string StatCategory { get; }
    public string Key { get; }
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
    public TaskDetails Details { get; set; }
    public bool IsCompleted = false;

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

    public void CheckForCompletion()
    {
        var seedsInfo = new Dictionary<string, int>();
        var reader = QSReader.Create("Statistics");
        var currentAmount = 0;

        if (reader.Exists(Details.StatCategory))
            seedsInfo = reader.Read<Dictionary<string, int>>(Details.StatCategory);

        if (seedsInfo.ContainsKey(Details.Key))
            currentAmount = seedsInfo[Details.Key];

        if (currentAmount - Details.StartAmount < Details.AmountToComplete) return;

        getRewardBtn.GetComponent<Button>().interactable = true;
        IsCompleted = true;
    }

    public void ApplyAwards()
    {
        var parent = transform.parent;
        parent.gameObject.GetComponent<Scenario>().FirstCharacterSprite = characterSpritePlace.sprite;
        parent.GetComponent<Scenario>()
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
