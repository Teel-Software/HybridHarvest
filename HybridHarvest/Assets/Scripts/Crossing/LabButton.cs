using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LabButton : MonoBehaviour
{
    [SerializeField] public Button PlaceForResult;
    [SerializeField] public InventoryDrawer InventoryFrame;
    [SerializeField] Button SelectButton;
    [SerializeField] Button SecondButton;
    [SerializeField] Sprite defaultSprite;
    [SerializeField] Button CrossingPerformer;
    public Seed NowSelected;

    public void Clicked()
    {
        InventoryFrame.GetComponent<InventoryDrawer>().GrowPlace = SelectButton;
        InventoryFrame.GetComponent<InventoryDrawer>().SetPurpose(PurposeOfDrawing.AddToLab);
        InventoryFrame.gameObject.SetActive(true);
        if (SceneManager.GetActiveScene().buildIndex != 4
            && SecondButton.GetComponent<LabButton>().NowSelected != null)
            InventoryFrame.Redraw(filter_RussianName: SecondButton.GetComponent<LabButton>().NowSelected.NameInRussian);

        var scenario = GameObject.FindGameObjectWithTag("TutorialHandler")?.GetComponent<Scenario>();
        if (scenario == null) return;

        // тутор для выбора семечка 2
        if (QSReader.Create("TutorialState").Exists("Tutorial_HybridPanelSecond_Played"))
            scenario.Tutorial_ChooseItemToCrossSecond();

        // тутор для выбора семечка 1
        if (QSReader.Create("TutorialState").Exists("Tutorial_HybridPanel_Played"))
            scenario.Tutorial_ChooseItemToCrossFirst();
    }

    public void ChosenSeed(Seed seed)
    {
        NowSelected = seed;
        SelectButton.GetComponent<Image>().sprite = seed.PlantSprite;
        var seedInfo = seed.NameInRussian +
            $"\nВкус: {seed.Taste}" +
            $"\nГабитус: {seed.Gabitus}" +
            $"\nВремя роста: {seed.GrowTime}" +
            $"\nКол-во: {seed.MinAmount}-{seed.MaxAmount}" +
            $"\nШанс мутации: {seed.MutationChance}";
        SelectButton.GetComponentInChildren<Text>().text = seedInfo;
        if (SecondButton == null) return;
        var seed1 = SecondButton.GetComponent<LabButton>().NowSelected;
        if (seed1 == null || NowSelected == null) return;
        DrawResult(seed1);
    }

    public void ChosenSeed(Seed seed, int[] chance)
    {
        NowSelected = seed;
        SelectButton.GetComponent<Image>().sprite = seed.PlantSprite;
        var seedInfo = seed.NameInRussian +
            $"\nВкус: {seed.Taste} {chance[0]}%" +
            $"\nГабитус: {seed.Gabitus} {chance[1]}%" +
            $"\nВремя роста:\n {seed.GrowTime} {chance[2]}%" +
            $"\nКол-во: {seed.MinAmount}-{seed.MaxAmount}  {chance[1]}%" +
            $"\nШанс мутации: \n{seed.MutationChance} {chance[1]}%";
        SelectButton.GetComponentInChildren<Text>().text = seedInfo;
        if (SecondButton == null) return;
        var seed1 = SecondButton.GetComponent<LabButton>().NowSelected;
        if (seed1 == null || NowSelected == null) return;
        DrawResult(seed1);
    }

    private void DrawResult(Seed seed1)
    {
        var newseed = CrossingPerformer.GetComponent<GeneCrossing>().MixTwoParents(seed1, NowSelected);
        if (SceneManager.GetActiveScene().buildIndex == 4)
        {
            //newseed.NameInRussian = "???";
            //newseed.Name = seed1.Name + "-" + NowSelected.Name;
            newseed = CrossingPerformer.GetComponent<GeneCrossing>().GetQuantumSeed(seed1, NowSelected);
        }
        var chance = CrossingPerformer.GetComponent<GeneCrossing>().Chances;
        PlaceForResult.GetComponent<LabButton>().ChosenSeed(newseed, chance);
        PlaceForResult.gameObject.SetActive(true);
    }

    public void ClearButton()
    {
        gameObject.GetComponent<LabButton>().NowSelected = null;
        gameObject.GetComponent<Image>().sprite = defaultSprite;
        SelectButton.GetComponentInChildren<Text>().text = "";
    }
}
