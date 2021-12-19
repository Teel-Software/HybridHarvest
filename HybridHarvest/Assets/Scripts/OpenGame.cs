using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpenGame : MonoBehaviour
{
    [SerializeField] GameObject RewatchButton;

    /// <summary>
    /// Меняет сцену
    /// </summary>
    /// <param номер следующей сцены ="sceneNum"></param>
    public void ChangeScene(int sceneNum)
    {
        if (!PlayerPrefs.HasKey("GameInitialised") && RewatchButton != null)
        {
            ClearPlayerStats();
            InitializeBeginning();
            PlayerPrefs.Save();
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
    /// Приводит статистику игрока к дефолтным значениям
    /// </summary>
    private void ClearPlayerStats()
    {
        PlayerPrefs.SetInt("money", 100);
        PlayerPrefs.SetInt("reputation", 0);
        PlayerPrefs.SetInt("reputationLimit", 500);
        PlayerPrefs.SetInt("reputationLevel", 1);
        PlayerPrefs.SetInt("amount", 0);
        PlayerPrefs.SetInt("energyMax", 10);
        PlayerPrefs.SetInt("energy", 0);
        PlayerPrefs.SetFloat("energytimebuffer", 0);
        PlayerPrefs.SetString("energytime", DateTime.Now.ToString());
    }

    /// <summary>
    /// Запускает вступительные слайды
    /// </summary>
    private void InitializeBeginning()
    {
        RewatchButton.GetComponent<Button>().onClick.Invoke();
    }
}
