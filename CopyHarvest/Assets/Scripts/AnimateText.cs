using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

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
    int frame, frequency;

    // Start is called before the first frame update
    void Start()
    {
        currentText = TextPanel.text;
        TextPanel.text = "";
        frequency = 10;
    }

    // Update is called once per frame
    void Update()
    {
        if (frame % frequency == 0 && frame / frequency < currentText.Length)
            TextPanel.text += $"{currentText[frame / frequency]}";
        if (frame != int.MaxValue)
            frame++;
    }

    public void SkipText()
    {
        if (frame / frequency < currentText.Length)
        {
            TextPanel.text = currentText;
            frame = int.MaxValue;
        }
        else
        {
            CurrentSlide.SetActive(false);
            if (NextSlide != null)
                NextSlide.SetActive(true);
            else
            {
                Title_LastSlide.SetActive(true);
                StartLabel_LastSlide.SetActive(true);
                StartButton_LastSlide.SetActive(true);
                Options_LastSlide.SetActive(true);
            }
        }
    }

    void OnDisable()
    {
        frame = 0;
        TextPanel.text = "";
    }
}
