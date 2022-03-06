using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public static class TaskTools
{
    private static readonly string[] StatCategories =
        { "GrowedSeeds", "CrossedSeeds", "SelledSeeds", "PurchasedSeeds" };

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

    public void CreateNewTask()
    {
        var newTask = Instantiate(taskPrefab, transform.parent);
        newTask.GetComponent<Task>().FillParameters(TaskTools.GetStatCategory(), TaskTools.GetKey(),
            TaskTools.GetAmountToComplete(), TaskTools.GetCharacter());
        UpdateTaskView(newTask);
        transform.SetAsLastSibling();
    }

    private static void UpdateTaskView(GameObject taskObj)
    {
        var taskComp = taskObj.GetComponent<Task>();
        var taskName = taskComp.Details.StatCategory switch
        {
            "GrowedSeeds" => "вырастить",
            "CrossedSeeds" => "получить при скрещивании",
            "SelledSeeds" => "продать",
            "PurchasedSeeds" => "купить",
            _ => "приготовить"
        };
        var itemName = taskComp.Details.Key switch
        {
            "Tomato" => "помидоров",
            "Cucumber" => "огурцов",
            "Potato" => "картофелин",
            "Pea" => "стручков гороха",
            _ => "учпочмаков"
        };

        taskComp.description.text = $"Нужно {taskName} {taskComp.Details.AmountToComplete} {itemName}.";
        taskComp.characterSpritePlace.sprite =
            Resources.Load<Sprite>($"Characters\\{taskComp.Details.FromCharacter}");
        taskComp.CheckForCompletion();
        if (taskComp.IsCompleted)
            taskComp.description.transform.parent.parent.GetComponent<Text>().color =
                new Color(100 / 255f, 1f, 100 / 255f);
    }

    private void RenderCurrentTasks()
    {
        var reader = QSReader.Create("Tasks");
        var allKeys = reader.GetAllKeys();
        var CGD = gameObject.GetComponent<ClearGameData>() ?? gameObject.AddComponent<ClearGameData>();
        CGD.ClearChildren(gameObject);

        foreach (var key in allKeys)
        {
            var details = reader.Read<TaskDetails>(key);
            if (details.AmountToComplete <= 0) continue;

            var newTask = Instantiate(taskPrefab, transform);
            var taskComp = newTask.GetComponent<Task>();
            taskComp.Details = details;
            UpdateTaskView(newTask);

            if (taskComp.IsCompleted)
                newTask.transform.SetAsFirstSibling();
        }

        Instantiate(taskAddPrefab, transform);
    }

    private void OnEnable()
    {
        if (RenderTasksHere)
            RenderCurrentTasks();
    }
}
