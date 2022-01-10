using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpenGame : MonoBehaviour
{
    [SerializeField] GameObject RewatchBeginningBtn;
    [SerializeField] ClearGameData ClearGameData;

    /// <summary>
    /// Меняет сцену
    /// </summary>
    /// <param name="sceneNum">Номер следующей сцены</param>
    public void ChangeScene(int sceneNum)
    {
        // проверка на первый заход в игру
        if (!QSReader.Create("GameState").Exists("GameInitialised")
            && RewatchBeginningBtn != null)
        {
            ClearGameData.ClearAll();
            InitializeBeginning();
        }
        else
        {
            if (sceneNum == 0)
            {
                GameObject obj = GameObject.Find("player");
                Destroy(obj);
            }
            SceneManager.LoadScene(sceneNum);
        }
    }

    /// <summary>
    /// Запускает вступительные слайды
    /// </summary>
    private void InitializeBeginning()
    {
        RewatchBeginningBtn.GetComponent<Button>().onClick.Invoke();
    }
}
