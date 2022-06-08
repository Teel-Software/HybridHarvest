using CI.QuickSave;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialHandler : MonoBehaviour
{
    [SerializeField] private GameObject ContinueTutorialPrefab;

    public void SkipTutorial()
    {
        var writer = QuickSaveWriter.Create("TutorialState");
        writer.Write("TutorialSkipped", true)
            .Write("NowPlaying", "");
        ;
        writer.Commit();

        // перезагружает сцену, чтобы неактивные кнопки обновились
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SideMenuTutorial()
    {
        var scenario = GetComponent<Scenario>();
        if (scenario == null) return;

        // тутор для захода на склад
        if (QSReader.Create("TutorialState").Exists("Tutorial__Played"))
            scenario.Tutorial_SideMenuToInventory();

        // тутор для захода в квесты
        else if (QSReader.Create("StoryState").Exists("Story_GetFirstQuest_Played"))
            scenario.SideMenuToQuests();

        // тутор для боковой панели
        else if (QSReader.Create("TutorialState").Exists("Tutorial_BeginningChoice_Played"))
            scenario.Tutorial_SideMenuToShop();
    }

    public void ShopTutorial()
    {
        var scenario = GetComponent<Scenario>();
        if (scenario == null) return;

        // тутор для открытия предметов на уровне 2
        if (QSReader.Create("LevelState").Exists("LevelUp2"))
            scenario.ShopLevel2();

        // тутор для покупки огурца
        else if (QSReader.Create("TutorialState").Exists("Tutorial_SideMenuToShop_Played"))
            scenario.Tutorial_Shop();
    }

    private void Start()
    {
        GetComponent<Scenario>()?.Tutorial_Beginning();

        var reader = QSReader.Create("TutorialState");
        var nowPlaying = reader.Exists("NowPlaying")
            ? reader.Read<string>("NowPlaying")
            : "";

        if (ContinueTutorialPrefab != null
            && nowPlaying != ""
            && !nowPlaying.Contains("BeginningChoice")
            && SceneManager.GetActiveScene().buildIndex == 1)
            Instantiate(ContinueTutorialPrefab, GameObject.FindGameObjectWithTag("Canvas").transform, false);
    }
}
