using UnityEngine;
using UnityEngine.UI;

public class MarketButton : MonoBehaviour
{
    public GameObject ConfirmationPanel;
    public Inventory targetInventory;

    /// <summary>
    /// Создаёт подтверждающую панель в магазине
    /// </summary>
    /// <param name="seedName">Имя семечка</param>
    public void PrepareConfirmation(string seedName)
    {
        var panelObj = Instantiate(ConfirmationPanel, GameObject.Find("Shop").transform);
        var panel = panelObj.transform.Find("Panel");
        var text = panel.transform.Find("QuestionText").GetComponent<Text>();
        var yes = panel.transform.Find("YesButton").GetComponent<Button>();
        var script = panel.transform.Find("YesButton").GetComponent<ConfirmationPanelLogic>();

        script.targetInventory = targetInventory;
        text.text = "Купить";
        script.HasPrice = true;
        yes.onClick.AddListener(script.AddOneMore);

        script.DefineItem(seedName);

        panelObj.SetActive(true);
    }

    public void CSVTest()
    {
        var stats = CSVReader.ParseSeedStats("Peas");
        Debug.Log(string.Join(" ", stats.Gabitus));
        Debug.Log(string.Join(" ", stats.Taste));
        Debug.Log(string.Join(" ", stats.MinAmount));
        Debug.Log(string.Join(" ", stats.MaxAmount));
        Debug.Log(string.Join(" ", stats.MutationChance));
        Debug.Log(string.Join(" ", stats.GrowTime));
    }
}
