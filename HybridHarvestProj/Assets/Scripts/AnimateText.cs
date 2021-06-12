using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class AnimateText : MonoBehaviour
{
    [SerializeField] Text input;
    string currentText;
    int frame, frequency;
    string[] splited;

    // Start is called before the first frame update
    void Start()
    {
        currentText = input.text;
        splited = currentText.Split();
        input.text = "";
        frequency = 20;
    }

    // Update is called once per frame
    void Update()
    {
        if (frame++ % frequency == 0 && frame / frequency < splited.Length)
            input.text += $"{splited[frame / frequency]} ";
    }

    void OnDisable()
    {
        frame = 0;
        input.text = "";
    }
}
