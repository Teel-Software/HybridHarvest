using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class AnimateText : MonoBehaviour
{
    [SerializeField] Text TextPanel;
    [SerializeField] GameObject CurrentSlide;
    [SerializeField] GameObject NextSlide;

    string currentText;
    int frame, frequency;

    // Start is called before the first frame update
    void Start()
    {
        currentText = TextPanel.text;
        TextPanel.text = "";
        frequency = 15;
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
        }
    }

    void OnDisable()
    {
        frame = 0;
        TextPanel.text = "";
    }
}
