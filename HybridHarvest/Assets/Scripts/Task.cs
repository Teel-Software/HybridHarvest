using System.Linq;
using CI.QuickSave;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Хранит информацию о подзадании.
/// </summary>
public class SubTask
{
    public string TaskCategory { get; }
    public string Key { get; }
    public int AmountToComplete { get; }
    public int ProgressAmount { get; set; }
    public int RemainingAmount => AmountToComplete - ProgressAmount;
    public bool IsCompleted => ProgressAmount >= AmountToComplete;

    /// <summary>
    /// Создаёт подзадание.
    /// </summary>
    /// <param name="taskCategory">Категория задания.</param>
    /// <param name="key">Ключевой предмет задания.</param>
    /// <param name="amountToComplete">Количество предметов, требуемое для завершения задания.</param>
    public SubTask(string taskCategory, string key, int amountToComplete)
    {
        TaskCategory = taskCategory;
        Key = key;
        AmountToComplete = amountToComplete;
    }
}

/// <summary>
/// Хранит всю информацию о конкретном задании.
/// </summary>
public class TaskDetails
{
    public int ID { get; set; }
    public string FromCharacter { get; }
    public string Tag { get; }
    public SubTask[] SubTasks { get; }
    public Award[] Awards { get; }

    /// <summary>
    /// Создаёт информацию о конкретном задании.
    /// </summary>
    /// <param name="fromCharacter">Название спрайта персонажа, от которого задание.</param>
    /// <param name="awards">Награды за выполнение.</param>
    /// <param name="tag">Специальный тег для задания (если требуется).</param>
    /// <param name="subTasks">Подзадания.</param>
    public TaskDetails(string fromCharacter, Award[] awards, string tag = "Default", params SubTask[] subTasks)
    {
        FromCharacter = fromCharacter;
        Awards = awards;
        SubTasks = subTasks;
        Tag = tag;
    }
}

public class Task : MonoBehaviour
{
    [SerializeField] public Text Title;
    [SerializeField] public Image CharacterSpritePlace;
    [SerializeField] private Transform placeForSubTasks;
    [SerializeField] private TMP_Text SubTaskPrefab;
    [SerializeField] private Button getRewardBtn;

    public TaskDetails Details { get; set; }

    /// <summary>
    /// Добавляет новое задание.
    /// </summary>
    /// <param name="fromCharacter">Персонаж, давший задание.</param>
    /// <param name="awards">Награды за выполнение.</param>
    /// <param name="taskTag">Специальный тег для задания (если требуется).</param>
    /// <param name="subTasks">Подзадания.</param>
    public void Add(string fromCharacter, Award[] awards, string taskTag = "Default", params SubTask[] subTasks)
    {
        Details = new TaskDetails(fromCharacter, awards, taskTag, subTasks);
        Details.ID = Details.GetHashCode();
        Save();
    }

    /// <summary>
    /// Обновляет внешний вид карточки задачи.
    /// </summary>
    public void UpdateView()
    {
        if (Details?.SubTasks == null || Details.SubTasks.Length < 1)
        {
            Destroy(gameObject);
            return;
        }

        foreach (var subTask in Details.SubTasks)
        {
            var stObj = Instantiate(SubTaskPrefab, placeForSubTasks, false);
            stObj.text = $"<sprite=0> {subTask.ProgressAmount}/{subTask.AmountToComplete}";
            stObj.spriteAsset = (TMP_SpriteAsset)Resources.Load($"TMP_Assets\\{subTask.Key}");
        }

        CharacterSpritePlace.sprite =
            Resources.Load<Sprite>($"Characters\\{Details.FromCharacter}");

        CheckForCompletion();
    }

    /// <summary>
    /// Запускает конечный диалог и выдаёт награды.
    /// </summary>
    public void ApplyAwards()
    {
        var scenario = GameObject.FindGameObjectWithTag("TutorialHandler").GetComponent<Scenario>();
        scenario.FirstCharacterSprite = CharacterSpritePlace.sprite;

        switch (Details.Tag)
        {
            case "FirstTask":
                scenario.CreateFirstTaskDialog(Details.Awards);
                break;
            default:
            {
                if (Details.Awards != null)
                    scenario.CreateTaskEndDialog(TaskTools.GetPhrase(), Details.Awards);
                else
                    scenario.CreateTaskEndDialog(TaskTools.GetPhrase(),
                        new Award(AwardType.Money, amount: SumAmountToComplete() * 10),
                        new Award(AwardType.Reputation, amount: SumAmountToComplete() * 15)
                    );
                break;
            }
        }

        Delete();
        Destroy(gameObject);
    }

    /// <summary>
    /// Суммирует количество квестовых предметов для выполения.
    /// </summary>
    /// <returns>Общее количество квестовых предметов для выполения.</returns>
    public int SumAmountToComplete() =>
        Details.SubTasks.Sum(st => st.AmountToComplete);

    /// <summary>
    /// Суммирует оставшееся количество квестовых предметов.
    /// </summary>
    /// <returns>Общее количество оставшихся квестовых предметов.</returns>
    public int SumRemainingAmount() =>
        Details.SubTasks.Sum(st => st.RemainingAmount);

    /// <summary>
    /// Сохраняет задание.
    /// </summary>
    private void Save()
    {
        var writer = QuickSaveWriter.Create("Tasks");
        writer.Write(Details.ID.ToString(), Details);
        writer.Commit();
    }

    /// <summary>
    /// Удаляет задание.
    /// </summary>
    private void Delete()
    {
        var writer = QuickSaveWriter.Create("Tasks");
        writer.Delete(Details.ID.ToString());
        writer.Commit();
    }

    /// <summary>
    /// Обновляет внешний вид карточки задачи при завершении.
    /// </summary>
    private void CheckForCompletion()
    {
        if (Details.SubTasks.Any(subTask => !subTask.IsCompleted)) return;

        getRewardBtn.interactable = true;
        Title.color = new Color(100 / 255f, 1f, 100 / 255f);
    }
}
