using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearGameData : MonoBehaviour
{
    [SerializeField] GameObject RewatchButton;

    public void ClearInventory()
    {
        PlayerPrefs.SetInt("mony", 0);
        PlayerPrefs.SetInt("repa", 0);
        PlayerPrefs.SetInt("amo", 0);
    }

    public void UndoGameInitialization()
    {
        PlayerPrefs.DeleteKey("GameInitialised");
    }

    public void DisableRewatchButton()
    {
        if (!PlayerPrefs.HasKey("GameInitialised") && RewatchButton != null)
            RewatchButton.SetActive(false);
    }

    public void QuitApplication() 
    {
        Application.Quit();
    }
}
