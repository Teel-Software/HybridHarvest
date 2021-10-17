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
        PlayerPrefs.SetInt("mony", 0);
        PlayerPrefs.SetInt("repa", 0);
        PlayerPrefs.SetInt("amo", 0);
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
