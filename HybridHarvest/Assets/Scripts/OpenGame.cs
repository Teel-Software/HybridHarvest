using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class OpenGame : MonoBehaviour
{
    [SerializeField] GameObject RewatchBeginningBtn;
    [SerializeField] ClearGameData ClearGameData;
    [SerializeField] Text debugtext;

    /// <summary>
    /// Меняет сцену
    /// </summary>
    /// <param name="sceneNum">Номер следующей сцены</param>
    public void ChangeScene(int sceneNum)
    {
        try
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
        catch(Exception ex)
        {
            debugtext.GetComponent<Text>().text = ex.Message;
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
