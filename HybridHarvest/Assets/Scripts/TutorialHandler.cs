using CI.QuickSave;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialHandler : MonoBehaviour
{
    public static void ClearGameAfterTutorial()
    {
        var inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        inventory.Elements = inventory.Elements
            .Where(seed => !seed.NameInRussian.Contains("Обучаю"))
            .ToList();
        inventory.Save();
        ShopLogic.UnlockSeeds("Cucumber", "Tomato", "Potato", "Pea", "Carrot", "Debug");
    }

    public void SkipTutorial()
    {
        ClearGameAfterTutorial();

        var writer = QuickSaveWriter.Create("TutorialState");
        writer.Write("TutorialSkipped", true);
        writer.Commit();

        // перезагружает сцену, чтобы неактивные кнопки обновились
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SideMenuTutorial()
    {
        var scenario = GetComponent<Scenario>();

        // тутор для захода на склад
        if (QSReader.Create("TutorialState").Exists("Tutorial_ShopExit_Played"))
            scenario?.Tutorial_SideMenuInventory();

        // тутор для боковой панели
        else if (QSReader.Create("TutorialState").Exists("Tutorial_BeginningChoice_Played"))
            scenario?.Tutorial_SideMenu();
    }

    private void Start()
    {
        GetComponent<Scenario>()?.Tutorial_Beginning();
    }
}
