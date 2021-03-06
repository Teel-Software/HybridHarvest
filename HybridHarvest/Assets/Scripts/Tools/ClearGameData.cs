using System;
using CI.QuickSave;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearGameData : MonoBehaviour
{
    [SerializeField] private Inventory Inventory;
    [SerializeField] private InventoryDrawer InventoryFrame;
    [SerializeField] private GameObject[] RewatchButtons; // кнопки просмотра начальных роликов
    [SerializeField] private ConfirmationPanelLogic confirmationPanelPrefab;

    /// <summary>
    /// Удаляет все дочерние объекты
    /// </summary>
    public static void ClearChildren(GameObject obj)
    {
        var allChildren = new HashSet<GameObject>();

        foreach (Transform child in obj.transform)
            allChildren.Add(child.gameObject);

        foreach (var child in allChildren)
            Destroy(child);
    }

    public void ClearAll()
    {
        // var quickSavePath = Path.Combine(QuickSaveGlobalSettings.StorageLocation, "QuickSave");
        var savePath = QuickSaveGlobalSettings.StorageLocation;
        var childDirectories = Directory.GetDirectories(savePath);
        var childFiles = Directory.GetFiles(savePath);

        foreach (var dir in childDirectories)
        {
            try
            {
                Directory.Delete(dir, true);
            }
            catch (Exception e)
            {
                Debug.Log($"Ошибка при удалении директории {dir}!");
                Debug.Log(e.Message);
            }
        }

        foreach (var file in childFiles)
        {
            try
            {
                File.Delete(file);
            }
            catch (Exception e)
            {
                Debug.Log($"Ошибка при удалении файла {file}!");
                Debug.Log(e.Message);
            }
        }

        ResetPlayerPrefs();
        ClearExhibition();

        if (Inventory != null)
            Inventory.Awake();
        if (InventoryFrame != null)
            InventoryFrame.Redraw();
    }

    /// <summary>
    /// Перезапускает игру.
    /// </summary>
    public void RestartGame()
    {
        var canvas = GameObject.FindGameObjectWithTag("Canvas");
        var confPanel = Instantiate(confirmationPanelPrefab, canvas.transform, false);
        confPanel.SetQuestion("Перезапустить игру?",
            "Все игровые данные будут утеряны.");
        confPanel.SetYesAction(() =>
        {
            ClearAll();
            SceneManager.LoadScene(0);
        });
    }

    /// <summary>
    /// Выключает кнопки просмотра начальных роликов
    /// </summary>
    public void DisableRewatchButtons()
    {
        if (QSReader.Create("GameState").Exists("GameInitialised") || RewatchButtons == null) return;

        foreach (var btn in RewatchButtons)
            btn.SetActive(false);
    }

    /// <summary>
    /// Перезапускает туториал
    /// </summary>
    public void WatchTutorial()
    {
        ClearAll();
        ChangeStartSceneOrDisable();

        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Выходит с начальной сцены, либо деактивирует текущий объект (использовать на последнем слайде)
    /// </summary>
    public void ChangeStartSceneOrDisable()
    {
        const string key = "GameInitialised";

        if (!QSReader.Create("GameState").Exists(key))
        {
            var writer = QuickSaveWriter.Create("GameState");
            writer.Write(key, true);
            writer.Commit();

            SceneManager.LoadScene(1);
        }
        else gameObject.SetActive(false);
    }

    /// <summary>
    /// Закрывает игру
    /// </summary>
    public void CloseGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Очищает данные выставки
    /// </summary>
    private static void ClearExhibition()
    {
        GameObject.Find("Exhibition")?.SetActive(false);
        var writer = QuickSaveWriter.Create("ExhibitionData");
        writer.Write("ExhSeed", "no");
        writer.Commit();
    }

    /// <summary>
    /// Очищает PlayerPrefs
    /// </summary>
    private static void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
