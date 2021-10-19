using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class AnimateText : MonoBehaviour
{
    [SerializeField] Text TextPanel;
    [SerializeField] GameObject CurrentSlide;
    [SerializeField] GameObject NextSlide;

    // ����� ������������ ��������������� ��������, ����������� ��� ������ ��������
    // ����������� ��������� ����������� ������ ��� ���������� ������
    [SerializeField] GameObject Title_LastSlide;
    [SerializeField] GameObject StartLabel_LastSlide;
    [SerializeField] GameObject StartButton_LastSlide;
    [SerializeField] GameObject Options_LastSlide;
    [SerializeField] GameObject Background_LastSlide;

    string currentText;
    int index, freqMilliseconds;
    DateTime lastSlideTime;

    // Start is called before the first frame update
    void Start()
    {
        currentText = TextPanel.text;
        TextPanel.text = "";
        index = 0; // ��������� �� ������� ������
        lastSlideTime = DateTime.MinValue; // �����, � ������� �������� ������� �����
        freqMilliseconds = 25; // ����� ����� ������� �������� � �������������
    }

    // Update is called once per frame
    void Update()
    {
        // ������ freqMilliseconds ����� �����������, �������� ��������� ������
        if ((DateTime.Now - lastSlideTime).TotalMilliseconds > freqMilliseconds)
        {
            lastSlideTime = DateTime.Now;

            if (index < currentText.Length)
                TextPanel.text += $"{currentText[index++]}";
        }
    }

    /// <summary>
    /// ����� ������� ������� �����, ��� ��������
    /// </summary>
    public void SkipText()
    {
        if (index < currentText.Length)
        {
            TextPanel.text = currentText;
            index = int.MaxValue;
        }
        else
        {
            // �������� �� ��������� �����
            if (NextSlide != null)
                NextSlide.SetActive(true);
            else if (PlayerPrefs.HasKey("GameInitialised"))
            {
                Title_LastSlide.SetActive(true);
                StartLabel_LastSlide.SetActive(true);
                StartButton_LastSlide.SetActive(true);
                Options_LastSlide.SetActive(true);
                Background_LastSlide.SetActive(true);
            }

            if (NextSlide == null && !PlayerPrefs.HasKey("GameInitialised"))
            {
                PlayerPrefs.SetInt("GameInitialised", 1);
                SceneManager.LoadScene(1);
            }
            else CurrentSlide.SetActive(false);
        }
    }

    void OnDisable()
    {
        index = 0;
        TextPanel.text = "";
        lastSlideTime = DateTime.MinValue;
    }
}
