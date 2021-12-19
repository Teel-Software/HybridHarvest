using UnityEngine;
using UnityEngine.UI;

public class TextBestFit : MonoBehaviour
{
    [SerializeField] int minSize;
    [SerializeField] int maxSize;

    Text currTextComp;
    RectTransform currRT;

    /// <summary>
    /// Находит текущий компонент "Text"
    /// </summary>
    void Start()
    {
        currTextComp = gameObject.GetComponent<Text>();
        currRT = gameObject.GetComponent<RectTransform>();

        // default
        if (minSize == 0)
            minSize = maxSize >= 10 ? 10 : 1;
        if (maxSize == 0)
            maxSize = minSize <= 25 ? 25 : 100;
    }

    /// <summary>
    /// Изменяет размер шрифта согласно заданным параметрам
    /// </summary>
    void Update()
    {
        var tHeight = currTextComp.preferredHeight;
        var rtHeight = currRT.rect.height;
        var fSize = currTextComp.fontSize;

        // уменьшает шрифт
        if (tHeight > rtHeight || fSize > maxSize)
        {
            var newFontSize = fSize - 1;
            while (tHeight > rtHeight || newFontSize >= maxSize)
            {
                if (newFontSize >= minSize)
                    currTextComp.fontSize = newFontSize--;
                else break;
                tHeight = currTextComp.preferredHeight;
            }
        }
        // увеличивает шрифт
        else if (tHeight < rtHeight || fSize < minSize)
        {
            var newFontSize = fSize + 1;
            while (tHeight <= rtHeight)
            {
                if (newFontSize <= maxSize)
                    currTextComp.fontSize = newFontSize++;
                else break;
                tHeight = currTextComp.preferredHeight;
            }

            if (tHeight > rtHeight)
                currTextComp.fontSize--;
        }
    }
}
