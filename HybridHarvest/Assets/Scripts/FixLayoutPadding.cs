using UnityEngine;
using UnityEngine.UI;

public class FixLayoutPadding : MonoBehaviour
{
    /// <summary>
    /// Gets data from current object and sets padding of the layout
    /// </summary>
    void Start()
    {
        var minPaddingLeft = 30f;
        var currentLayout = transform.GetComponent<GridLayoutGroup>();
        var rectWidth = GetComponent<RectTransform>().rect.width - minPaddingLeft * 2;
        var actualCellSize = currentLayout.cellSize.x + currentLayout.spacing.x;
        var rowElementCount = Mathf.Floor((rectWidth - currentLayout.cellSize.x) / actualCellSize) + 1;

        currentLayout.padding.left = (int)Mathf.Floor((rectWidth
            - (currentLayout.cellSize.x + actualCellSize * (rowElementCount - 1))) / 2);
        currentLayout.childAlignment = TextAnchor.UpperLeft;
    }
}
