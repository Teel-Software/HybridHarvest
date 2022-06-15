using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreateMiniGame : MonoBehaviour
{
    [SerializeField] int ElementsCount;
    [SerializeField] GameObject WholeMinigame;
    [SerializeField] GameObject GamingPlace;
    [SerializeField] Sprite CardSprite;
    [SerializeField] GameObject Blocker;
    [SerializeField] Button CrossingPerformer;
    [SerializeField] InventoryDrawer InventoryFrame;
    [SerializeField] GameObject NameGenerator;
    [SerializeField] GameObject InfoPanelPrefab;

    public Button ResultPlace;
    private Seed currentSeed;
    private Button currentPot;
    private GameObject infoPanel;

    enum QuantumBoost
    {
        Fail,
        Double,
        Quadriple,
        None
    }

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

            card.GetComponent<Button>().onClick.AddListener(OnButtonClicked);
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
        var boosts = GetBoostsForButtons();

        for (var i = 0; i < ElementsCount; i++)
        {
            var card = CreateCard(i);

            card.GetComponent<Button>().onClick.AddListener(QuantumOnButtonClicked);
            currentSeed = ResultPlace.GetComponent<QuantumGrowth>().growingSeed;
            currentPot = ResultPlace.GetComponent<QuantumGrowth>().Pot;

            var cardText = Instantiate(textSample, card.transform);
            cardText.GetComponent<Text>().text = ((int)boosts[i]).ToString();
            switch (boosts[i])
            {
                case QuantumBoost.Fail:
                    cardText.GetComponent<Text>().color = new Color(0.75f, 0f, 0f);
                    //cardText.GetComponent<Text>().text = QuantumBoost.Fail.ToString();
                    break;
                case QuantumBoost.Double:
                    cardText.GetComponent<Text>().color = new Color(0f, 0.9f, 0f);
                    //cardText.GetComponent<Text>().text = QuantumBoost.Double.ToString();
                    break;
                case QuantumBoost.Quadriple:
                    cardText.GetComponent<Text>().color = new Color(0f, 0.75f, 0f);
                    //cardText.GetComponent<Text>().text = QuantumBoost.Qadriple.ToString();
                    break;
                case QuantumBoost.None:
                    //cardText.GetComponent<Text>().text = QuantumBoost.None.ToString();
                    break;
            }
        }
    }

    private QuantumBoost[] GetBoostsForButtons()
    {
        if (ElementsCount != 9) Debug.LogError("Возможно вам стоит пересмотреть шансы бонусов");

        var boosts = new[] { QuantumBoost.Quadriple, QuantumBoost.Double, QuantumBoost.Double,
                          QuantumBoost.Fail, QuantumBoost.None, QuantumBoost.None,
                          QuantumBoost.None, QuantumBoost.None, QuantumBoost.None};
        //var boosts = new[] { QuantumBoost.Double, QuantumBoost.Double, QuantumBoost.Double,
        //                  QuantumBoost.Double, QuantumBoost.Double, QuantumBoost.Double,
        //                  QuantumBoost.Double, QuantumBoost.Double, QuantumBoost.Double};

        for (int firstInd = ElementsCount - 1; firstInd >= 1; firstInd--)
        {
            var secondInd = (int)((UnityEngine.Random.value * 100) % (firstInd + 1));
            var tmp = boosts[secondInd];
            boosts[secondInd] = boosts[firstInd];
            boosts[firstInd] = tmp;
        }
        return boosts;
    }

    private GameObject CreateCard(int i)
    {
        var card = new GameObject($"Card {i}", typeof(Button));

        card.AddComponent<Image>().sprite = CardSprite;
        card.GetComponent<Image>().type = Image.Type.Sliced;
        card.GetComponent<Image>().pixelsPerUnitMultiplier = 0.5f;
        card.GetComponent<Image>().color = new Color(1, 0.8f, 0.5f);

        card.GetComponent<Button>().targetGraphic = card.GetComponent<Image>();

        var scaleFactor = 1 / 47.34849f;
        card.transform.localScale = new Vector2(scaleFactor, scaleFactor);
        card.transform.SetParent(GamingPlace.transform);

        return card;
    }

    /// <summary>
    /// завершение и закрытие
    /// </summary>
    public void UpdateRussianName(string name)
    {
        Destroy(infoPanel);
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

        Blocker.SetActive(true);
    }

    /// <summary>
    /// нажатие на кнопку, но в кванте
    /// </summary>
    private void QuantumOnButtonClicked()
    {
        var button = EventSystem.current.currentSelectedGameObject;
        if (button == null) return;

        foreach (var buttonText in GamingPlace.transform.GetComponentsInChildren<Text>())
            buttonText.enabled = true;
        button.GetComponent<Image>().color = new Color(0.5f, 1, 0.5f);

        var booster = (QuantumBoost)int.Parse(button.GetComponentInChildren<Text>().text);

        switch (booster)
        {
            case QuantumBoost.Fail:
                InventoryFrame.GetComponent<InventoryDrawer>().SuccessfulAddition.Invoke();
                Blocker.SetActive(true);
                return;
            case QuantumBoost.Double:
                ChangeSeedDouble();
                Blocker.GetComponent<Button>().onClick.AddListener(AddGrownSeed);
                break;
            case QuantumBoost.Quadriple:
                ChangeSeedQuadriple();
                Blocker.GetComponent<Button>().onClick.AddListener(AddGrownSeed);
                break;
            case QuantumBoost.None:
                Blocker.GetComponent<Button>().onClick.AddListener(AddGrownSeed);
                break;
        }

        //ShowInfoPanel();
        NameGenerator.SetActive(true);
        NameGenerator.GetComponent<QuantumNameCreator>().DefaultFill();

    }

    private void ChangeSeedDouble()
    {
        var seedData = CSVReader.GetSeedStats(currentSeed.Name);
        var statsData = new[]{
            Tuple.Create(currentSeed.Gabitus, seedData.Gabitus.Keys.ToArray()),
            Tuple.Create(currentSeed.Taste, seedData.Taste.Keys.ToArray())};

        if (UnityEngine.Random.value > 0.5)
        {
            var startInd = Array.IndexOf(statsData[0].Item2, statsData[0].Item1);
            if (startInd * 2 > statsData[0].Item2.Length)
                currentSeed.Gabitus = statsData[0].Item2.Last();
            else
                currentSeed.Gabitus = startInd != 0 ?
                statsData[0].Item2[startInd * 2] :
                statsData[0].Item2[1];
        }
        else
        {
            var startInd = Array.IndexOf(statsData[1].Item2, statsData[1].Item1);
            if (startInd * 2 > statsData[1].Item2.Length)
                currentSeed.Taste = statsData[1].Item2.Last();
            else
                currentSeed.Taste = startInd != 0 ?
                statsData[1].Item2[startInd * 2] :
                statsData[1].Item2[1];
        }
    }

    private void ChangeSeedQuadriple()
    {
        var seedData = CSVReader.GetSeedStats(currentSeed.Name);
        var statsData = new[]{
            Tuple.Create(currentSeed.Gabitus, seedData.Gabitus.Keys.ToArray()),
            Tuple.Create(currentSeed.Taste, seedData.Taste.Keys.ToArray())};

        var startInd = Array.IndexOf(statsData[0].Item2, statsData[0].Item1);
        if (startInd * 2 > statsData[0].Item2.Length)
            currentSeed.Gabitus = statsData[0].Item2.Last();
        else
            currentSeed.Gabitus = startInd != 0 ?
                statsData[0].Item2[startInd * 2] :
                statsData[0].Item2[1];

        startInd = Array.IndexOf(statsData[1].Item2, statsData[1].Item1);
        if (startInd * 2 > statsData[1].Item2.Length)
            currentSeed.Taste = statsData[1].Item2.Last();
        else
            currentSeed.Taste = startInd != 0 ?
                statsData[1].Item2[startInd * 2] :
                statsData[1].Item2[1];
    }

    private void OnEnable()
    {
        Blocker.GetComponent<Button>().onClick.RemoveAllListeners();

        var scenario = GameObject.FindGameObjectWithTag("TutorialHandler")?.GetComponent<Scenario>();
        if (scenario == null) return;

        // тутор для мини-игры
        if (QSReader.Create("TutorialState").Exists("Tutorial__Played"))
            scenario.Tutorial_MiniGame();
    }

    private void ShowInfoPanel()
    {
        infoPanel = Instantiate(InfoPanelPrefab, GameObject.Find("MiniGamePanel").transform);
        var statPanelDrawer = infoPanel.GetComponentInChildren<StatPanelDrawer>();
        statPanelDrawer.DisplayQuantumStats(currentSeed);

        var yesButton = statPanelDrawer.ProceedButton.GetComponent<Button>();

        yesButton.onClick.AddListener(() =>
        {
            NameGenerator.SetActive(true);
            NameGenerator.GetComponent<QuantumNameCreator>().DefaultFill();
        });

    }
}

