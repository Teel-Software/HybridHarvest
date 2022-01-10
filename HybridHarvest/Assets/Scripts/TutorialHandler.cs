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
}
