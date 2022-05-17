using System;
using System.Linq;
using CI.QuickSave;
using UnityEngine;
using UnityEngine.UI;

public class TaskPreview : MonoBehaviour
{
    [SerializeField] public Image CharacterSpritePlace;
    [SerializeField] public Text ProgressLabel;
    [SerializeField] public Text FutureProgressLabel;
    [SerializeField] private Button sendToQuestBtn;

    public SubTask CurrentSubTask { get; set; }
    public int AmountToAdd { get; set; }
    public Action AddQuestItems { get; set; }
    public TaskDetails Details { get; set; }

    /// <summary>
    /// Обновляет внешний вид предпросмотра задачи
    /// </summary>
    public void UpdatePreview()
    {
        if (CurrentSubTask == null)
        {
            Destroy(gameObject);
            return;
        }

        ProgressLabel.text = $"{CurrentSubTask.ProgressAmount}/{CurrentSubTask.AmountToComplete}";
        CharacterSpritePlace.sprite =
            Resources.Load<Sprite>($"Characters\\{Details.FromCharacter}");

        var futureAmount = CurrentSubTask.ProgressAmount + AmountToAdd;
        FutureProgressLabel.text =
            $"{futureAmount}" +
            $"/{CurrentSubTask.AmountToComplete}";
        FutureProgressLabel.color = futureAmount > CurrentSubTask.AmountToComplete
            ? new Color(1f, 0.5f, 0.5f)
            : futureAmount == CurrentSubTask.ProgressAmount
                ? Color.white
                : new Color(0.5f, 1f, 0.5f);
        sendToQuestBtn.interactable =
            futureAmount <= CurrentSubTask.AmountToComplete && futureAmount != CurrentSubTask.ProgressAmount;
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
    /// Суммирует оставшееся количество квестовых предметов.
    /// </summary>
    /// <returns>Общее количество оставшихся квестовых предметов.</returns>
    public int SumRemainingAmount() =>
        Details.SubTasks.Sum(st => st.RemainingAmount);

    /// <summary>
    /// Сохраняет задание.
    /// </summary>
    public void Save()
    {
        var writer = QuickSaveWriter.Create("Tasks");
        writer.Write(Details.ID.ToString(), Details);
        writer.Commit();
    }
}
