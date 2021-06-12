using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class AnimateText : MonoBehaviour
{
    [SerializeField] Text input;
    string currentText;
    int frame, frequency;

    // Start is called before the first frame update
    void Start()
    {
        currentText = input.text;
        input.text = "";
        frequency = 15;
    }

    // Update is called once per frame
    void Update()
    {
        if (frame++ % frequency == 0 && frame / frequency < currentText.Length)
            input.text += $"{currentText[frame / frequency]}";
    }

    void OnDisable()
    {
        frame = 0;
        input.text = "";
    }
}
