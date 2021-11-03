using UnityEngine;
using UnityEngine.UI;

public class FixLayoutPadding : MonoBehaviour
{
    /// <summary>
    /// Gets data from current object and sets padding of the layout
    /// </summary>
    void Start()
    {
        var currentLayout = transform.GetComponent<GridLayoutGroup>();
        var objectWidth = GetComponent<RectTransform>().rect.width;
        var actualCellSize = currentLayout.cellSize.x + currentLayout.spacing.x;
        var rowElementCount = Mathf.Floor((objectWidth - currentLayout.cellSize.x) / actualCellSize) + 1;

        currentLayout.padding.left = (int)Mathf.Floor((objectWidth
            - (currentLayout.cellSize.x + actualCellSize * (rowElementCount - 1))) / 2);
        currentLayout.childAlignment = TextAnchor.UpperLeft;
    }
}
