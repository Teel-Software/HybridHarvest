using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
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
    private Coroutine activeCoroutine;

    /// <summary>
    /// Перезапускает анимацию
    /// </summary>
    public void RestartAnimation()
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            if (activeBlocker != null)
                Destroy(activeBlocker);
        }

        currentTextComp = GetComponent<Text>();
        origText = currentTextComp.text;
        currentTextComp.text = "";
        textIndex = 0;
        renderNeeded = true;

        var canvas = GameObject.FindGameObjectWithTag("Canvas");
        activeBlocker = Instantiate(TextBlockerPrefab, canvas.transform, false);
        activeBlocker.GetComponent<Button>().onClick.AddListener(OnBlockerClicked);

        activeCoroutine = StartCoroutine(RenderNextSymbol());
    }

    /// <summary>
    /// Рисует следующий символ текущей строки
    /// </summary>
    private IEnumerator RenderNextSymbol()
    {
        while (renderNeeded)
        {
            if (textIndex > origText.Length - 1)
            {
                Destroy(activeBlocker);
                break;
            }
            currentTextComp.text += origText[textIndex++];
            yield return new WaitForSeconds(frameTimeSeconds);
        }
    }

    // Выводит текст без анимации
    private void SkipText()
    {
        currentTextComp.text = origText;
        renderNeeded = false;
    }

    /// <summary>
    /// Вызывается при нажатии на блокер
    /// </summary>
    private void OnBlockerClicked()
    {
        var blocker = EventSystem.current.currentSelectedGameObject;
        if (blocker == null) return;

        Destroy(blocker);
        SkipText();
    }

    void OnEnable()
    {
        RestartAnimation();
    }

    private void OnDisable()
    {
        if (activeBlocker != null)
            Destroy(activeBlocker);
    }
}
