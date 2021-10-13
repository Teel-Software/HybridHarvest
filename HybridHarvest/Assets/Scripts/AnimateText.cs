using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class AnimateText : MonoBehaviour
{
    [SerializeField] Text TextPanel;
    [SerializeField] GameObject CurrentSlide;
    [SerializeField] GameObject NextSlide;
    [SerializeField] GameObject Title_LastSlide;
    [SerializeField] GameObject StartLabel_LastSlide;
    [SerializeField] GameObject StartButton_LastSlide;
    [SerializeField] GameObject Options_LastSlide;

    string currentText;
    int index, freqMilliseconds;
    DateTime lastFrameTime;

    // Start is called before the first frame update
    void Start()
    {
        currentText = TextPanel.text;
        TextPanel.text = "";
        index = 0;
        lastFrameTime = DateTime.MinValue;
        freqMilliseconds = 25; // врем€ между показом символов в миллисекундах
    }

    // Update is called once per frame
    void Update()
    {
        if ((DateTime.Now - lastFrameTime).TotalMilliseconds > freqMilliseconds)
        {
            lastFrameTime = DateTime.Now;

            if (index < currentText.Length)
                TextPanel.text += $"{currentText[index++]}";
        }
    }

    public void SkipText()
    {
        if (index < currentText.Length)
        {
            TextPanel.text = currentText;
            index = int.MaxValue;
        }
        else
        {
            if (NextSlide != null)
                NextSlide.SetActive(true);
            else if (PlayerPrefs.HasKey("GameInitialised"))
            {
                Title_LastSlide.SetActive(true);
                StartLabel_LastSlide.SetActive(true);
                StartButton_LastSlide.SetActive(true);
                Options_LastSlide.SetActive(true);
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
        lastFrameTime = DateTime.MinValue;
    }
}
