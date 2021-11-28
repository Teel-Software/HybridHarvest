using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class AnimateSlideText : MonoBehaviour
{
    [SerializeField] Text TextPanel;
    [SerializeField] GameObject CurrentSlide;
    [SerializeField] GameObject NextSlide;

    // здесь активируются соответствующие элементы, выключенные при начале анимации
    // нижестоящие параметры указываются только для последнего слайда
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
        index = 0; // указатель на текущий символ
        lastSlideTime = DateTime.MinValue; // время, в которое появился прошлый слайд
        freqMilliseconds = 25; // время между показом символов в миллисекундах
    }

    // Update is called once per frame
    void Update()
    {
        // каждые freqMilliseconds текст обновляется, добавляя очередной символ
        if ((DateTime.Now - lastSlideTime).TotalMilliseconds > freqMilliseconds)
        {
            lastSlideTime = DateTime.Now;

            if (index < currentText.Length)
                TextPanel.text += $"{currentText[index++]}";
        }
    }

    /// <summary>
    /// Сразу выводит готовый текст, без задержки
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
            // проверка на последний слайд
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
