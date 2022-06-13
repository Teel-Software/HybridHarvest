using System;
using UnityEngine;
using UnityEngine.UI;

public class AnimateText : MonoBehaviour
{
    [SerializeField] private float frameTimeSeconds;
    [SerializeField] private GameObject TextBlockerPrefab;

    private string origText;
    private GameObject activeBlocker;
    private Text currentTextComp;

    private int textIndex;
    private bool renderNeeded;
    private DateTime lastCheckedTime;

    /// <summary>
    /// Перезапускает анимацию.
    /// </summary>
    public void RestartAnimation()
    {
        currentTextComp = GetComponent<Text>();
        origText = currentTextComp.text;
        currentTextComp.text = "";
        textIndex = 0;
        renderNeeded = true;
        lastCheckedTime = DateTime.Now;

        if (activeBlocker != null)
            Destroy(activeBlocker);

        var canvas = GameObject.FindGameObjectWithTag("Canvas");
        activeBlocker = Instantiate(TextBlockerPrefab, canvas.transform, false);
        activeBlocker.GetComponent<Button>().onClick.AddListener(SkipText);
        activeBlocker.name = $"AnimTextBlocker: {origText.Substring(0, Math.Min(7, origText.Length))}";
    }

    /// <summary>
    /// Выводит текст без анимации.
    /// </summary>
    private void SkipText()
    {
        currentTextComp.text = origText;
        renderNeeded = false;
        Destroy(activeBlocker);
    }

    /// <summary>
    /// Рисует следующий символ текущей строки.
    /// </summary>
    private void Update()
    {
        if (!renderNeeded) return;

        if (textIndex > origText.Length - 1)
        {
            Destroy(activeBlocker);
            renderNeeded = false;
        }

        var timePassed = DateTime.Now - lastCheckedTime;
        if (timePassed.TotalSeconds < frameTimeSeconds) return;

        try
        {
            currentTextComp.text += origText[textIndex++];
            lastCheckedTime = DateTime.Now;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ошибка анимации текста: {origText}. textIndex {textIndex - 1}");
            Console.WriteLine(e.Message);
        }
    }

    private void OnEnable()
    {
        RestartAnimation();
    }

    private void OnDisable()
    {
        if (activeBlocker != null)
            Destroy(activeBlocker);
    }
}
