using UnityEngine;

public class ClearGameData : MonoBehaviour
{
    [SerializeField] GameObject RewatchButton; // кнопка просмотра вступления

    // методы начинают работу только после выхода с текущей сцены

    /// <summary>
    /// Очищает инвентарь
    /// </summary>
    public void ClearInventory()
    {
        PlayerPrefs.SetInt("mony", 0);
        PlayerPrefs.SetInt("repa", 0);
        PlayerPrefs.SetInt("amo", 0);
    }

    /// <summary>
    /// Удаляет флаг первой инициализации приложения
    /// </summary>
    public void UndoGameInitialization()
    {
        PlayerPrefs.DeleteKey("GameInitialised");
    }

    /// <summary>
    /// Выключает кнопку просмотра вступления
    /// </summary>
    public void DisableRewatchButton()
    {
        if (!PlayerPrefs.HasKey("GameInitialised") && RewatchButton != null)
            RewatchButton.SetActive(false);
    }

    /// <summary>
    /// Завершает работу приложения
    /// </summary>
    public void QuitApplication()
    {
        Application.Quit();
    }
}
