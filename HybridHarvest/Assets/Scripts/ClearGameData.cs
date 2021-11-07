using System;
using UnityEngine;

public class ClearGameData : MonoBehaviour
{
    [SerializeField] GameObject RewatchButton; // кнопка просмотра вступления

    // чтобы изменения сохранились необходимо перезайти на сцену

    public void ClearAll()
    {
        ClearPlayerStats();
        UndoGameInitialization();
    }

    /// <summary>
    /// Выключает кнопку просмотра вступления
    /// </summary>
    public void DisableRewatchButton()
    {
        if (!PlayerPrefs.HasKey("GameInitialised") && RewatchButton != null)
            RewatchButton.SetActive(false);
    }

    private void ClearPlayerStats()
    {
        PlayerPrefs.SetInt("money", 0);
        PlayerPrefs.SetInt("reputation", 0);
        PlayerPrefs.SetInt("reputationLimit", 500);
        PlayerPrefs.SetInt("reputationLevel", 1);
        PlayerPrefs.SetInt("amount", 0);
        PlayerPrefs.SetInt("energy", 0);
        PlayerPrefs.SetFloat("energytimebuffer", 0);
        PlayerPrefs.SetString("energytime", DateTime.Now.ToString());
    }

    private void UndoGameInitialization()
    {
        PlayerPrefs.DeleteKey("GameInitialised");
    }
}
