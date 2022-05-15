using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreateMiniGame : MonoBehaviour
{
    [SerializeField] int ElementsCount;
    [SerializeField] GameObject GamingPlace;
    [SerializeField] Sprite CardSprite;
    [SerializeField] GameObject Blocker;
    [SerializeField] Button CrossingPerformer;
    [SerializeField] InventoryDrawer InventoryFrame;
    [SerializeField] GameObject NameGenerator;

    public Button ResultPlace;
    private Seed currentSeed;
    private Button currentPot;

    /// <summary>
    /// Restarts mini game
    /// </summary>
    public void RestartGame()
    {
        var panel = transform.Find("Panel").gameObject;
        var textSample = panel.transform.Find("TextSample").gameObject;
        Blocker.SetActive(false);
        ClearGameData.ClearChildren(GamingPlace);

        for (var i = 0; i < ElementsCount; i++)
        {
            var card = CreateCard(i);

            currentSeed = ResultPlace.GetComponent<LabGrowth>().growingSeed;
            currentPot = ResultPlace.GetComponent<LabGrowth>().Pot;

            var GC = CrossingPerformer.GetComponent<GeneCrossing>();
            var reader = QSReader.Create("MiniGameStats");
            var chances = reader.Read<List<int>>("SelectionChances" + currentPot.name);
            var oppositeStats = reader.Read<List<int>>("OppositeSeedStats" + currentPot.name);

            var cardText = Instantiate(textSample, card.transform);
            cardText.GetComponent<Text>().text =
                $"Вкус: {GC.GetNewValueByPossibility(currentSeed.Taste, chances[0], oppositeStats[0])}\n" +
                $"Габитус: {GC.GetNewValueByPossibility(currentSeed.Gabitus, chances[1], oppositeStats[1])}\n" +
                $"Время роста: {GC.GetNewValueByPossibility(currentSeed.GrowTime, chances[2], oppositeStats[2])}";
        }
    }

    public void RestartQuantumGame()
    {
        var panel = transform.Find("Panel").gameObject;
        var textSample = panel.transform.Find("TextSample").gameObject;
        Blocker.SetActive(false);
        ClearGameData.ClearChildren(GamingPlace);

        for (var i = 0; i < ElementsCount; i++)
        {
            var card = CreateCard(i);

            currentSeed = ResultPlace.GetComponent<QuantumGrowth>().growingSeed;
            currentPot = ResultPlace.GetComponent<QuantumGrowth>().Pot;

            var cardText = Instantiate(textSample, card.transform);
            cardText.GetComponent<Text>().text =
                $"Вкус: {currentSeed.Taste}\n" +
                $"Габитус: {currentSeed.Gabitus}\n" +
                $"Время роста: {currentSeed.GrowTime}";
        }
    }

    private GameObject CreateCard(int i)
    {
        var card = new GameObject($"Card {i}", typeof(Button));

        card.AddComponent<Image>().sprite = CardSprite;
        card.GetComponent<Image>().type = Image.Type.Sliced;
        card.GetComponent<Image>().pixelsPerUnitMultiplier = 0.5f;
        card.GetComponent<Image>().color = new Color(1, 0.8f, 0.5f);

        card.GetComponent<Button>().onClick.AddListener(OnButtonClicked);
        card.GetComponent<Button>().targetGraphic = card.GetComponent<Image>();

        var scaleFactor = 1 / 47.34849f;
        card.transform.localScale = new Vector2(scaleFactor, scaleFactor);
        card.transform.SetParent(GamingPlace.transform);

        return card;
    }

    public void UpdateRussianName(string name)
    {
        currentSeed.NameInRussian = name;
        Blocker.SetActive(true);
    }

    /// <summary>
    /// Adds chosen seed in inventory
    /// </summary>
    public void AddGrownSeed()
    {
        InventoryFrame.UpdateActions();
        InventoryFrame.targetInventory.AddItem(currentSeed);

        Statistics.UpdateCrossedSeeds(currentSeed.Name);
    }

    /// <summary>
    /// Called when user clicks on button
    /// </summary>
    private void OnButtonClicked()
    {
        var button = EventSystem.current.currentSelectedGameObject;
        if (button == null) return;

        foreach (var buttonText in GamingPlace.transform.GetComponentsInChildren<Text>())
            buttonText.enabled = true;
        button.GetComponent<Image>().color = new Color(0.5f, 1, 0.5f);

        var seedStats = button.GetComponentInChildren<Text>().text.Split('\n')
            .Select(stat => stat
                .Replace(" ", "")
                .Split(':')
                .Last())
            .ToArray();

        currentSeed.Taste = int.Parse(seedStats[0]);
        currentSeed.Gabitus = int.Parse(seedStats[1]);
        currentSeed.GrowTime = int.Parse(seedStats[2]);
        //currentSeed.UpdateRating();

        if (SceneManager.GetActiveScene().buildIndex == 4)
        {
            NameGenerator.SetActive(true);
            NameGenerator.GetComponent<QuantumNameCreator>().DefaultFill();
        }
        else
            Blocker.SetActive(true);
    }

    private void OnEnable()
    {
        var scenario = GameObject.FindGameObjectWithTag("TutorialHandler")?.GetComponent<Scenario>();
        if (scenario == null) return;

        // тутор для мини-игры
        if (QSReader.Create("TutorialState").Exists("Tutorial__Played"))
            scenario.Tutorial_MiniGame();
    }
}
