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

    public Button ResultPlace;
    private Seed currentSeed;

    /// <summary>
    /// Restarts mini game
    /// </summary>
    public void RestartGame()
    {
        var textSample = transform.Find("TextSample").gameObject;
        Blocker.SetActive(false);
        GamingPlace.transform.DetachChildren();

        for (var i = 0; i < ElementsCount; i++)
        {
            var card = new GameObject($"Card {i}", typeof(Button));

            card.AddComponent<Image>().sprite = CardSprite;
            card.GetComponent<Image>().type = Image.Type.Sliced;
            card.GetComponent<Image>().pixelsPerUnitMultiplier = 0.5f;
            card.GetComponent<Image>().color = new Color(1, 0.8f, 0.5f);

            card.GetComponent<Button>().onClick.AddListener(OnButtonClicked);
            card.GetComponent<Button>().targetGraphic = card.GetComponent<Image>();

            if (SceneManager.GetActiveScene().buildIndex == 4)
                currentSeed = ResultPlace.GetComponent<QuantumGrowth>().growingSeed;
            else
                currentSeed = ResultPlace.GetComponent<LabGrowth>().growingSeed;

            var GC
                = CrossingPerformer.GetComponent<GeneCrossing>();
            var cardText = Instantiate(textSample, card.transform, true);
            cardText.GetComponent<Text>().text =
                $"Вкус: {GC.GetNewValueByPossibility(currentSeed.Taste, GC.Chances[0], GC.OppositeSeedStats[0])}\n" +
                $"Габитус: {GC.GetNewValueByPossibility(currentSeed.Gabitus, GC.Chances[1], GC.OppositeSeedStats[1])}\n" +
                $"Время роста: {GC.GetNewValueByPossibility(currentSeed.GrowTime, GC.Chances[2], GC.OppositeSeedStats[2])}";

            var scaleFactor = 1 / 47.34849f;
            card.transform.localScale = new Vector2(scaleFactor, scaleFactor);
            card.transform.SetParent(GamingPlace.transform);
        }
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
        currentSeed.Price = currentSeed.Taste;

        Blocker.SetActive(true);
    }
}
