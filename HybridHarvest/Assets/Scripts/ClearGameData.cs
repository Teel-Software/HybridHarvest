using System;
using UnityEngine;

public class ClearGameData : MonoBehaviour
{
    [SerializeField] GameObject RewatchButton; // ������ ��������� ����������

    // ������ �������� ������ ������ ����� ������ � ������� �����

    /// <summary>
    /// ������� ���������
    /// </summary>
    public void ClearInventory()
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

    /// <summary>
    /// ������� ���� ������ ������������� ����������
    /// </summary>
    public void UndoGameInitialization()
    {
        PlayerPrefs.DeleteKey("GameInitialised");
    }

    /// <summary>
    /// ��������� ������ ��������� ����������
    /// </summary>
    public void DisableRewatchButton()
    {
        if (!PlayerPrefs.HasKey("GameInitialised") && RewatchButton != null)
            RewatchButton.SetActive(false);
    }

    /// <summary>
    /// ��������� ������ ����������
    /// </summary>
    public void QuitApplication()
    {
        Application.Quit();
    }
}
