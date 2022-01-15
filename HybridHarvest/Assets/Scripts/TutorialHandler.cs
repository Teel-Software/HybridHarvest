using CI.QuickSave;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialHandler : MonoBehaviour
{
    void Start()
    {
        GetComponent<Scenario>()?.Tutorial_Beginning();
    }

    public void SkipTutorial()
    {
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
        scenario?.Tutorial_SideMenu();
    }
}
