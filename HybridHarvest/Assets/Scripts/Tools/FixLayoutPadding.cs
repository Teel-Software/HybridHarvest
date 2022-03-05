using UnityEngine;
using UnityEngine.UI;

public class FixLayoutPadding : MonoBehaviour
{
    [SerializeField] float minPaddingLeft;

    /// <summary>
    /// Gets data from current object and sets padding of the layout
    /// </summary>
    private void OnEnable()
    {
        var currentLayout = transform.GetComponent<GridLayoutGroup>();
        var rectWidth = GetComponent<RectTransform>().rect.width - minPaddingLeft * 2;
        var actualCellWidth = currentLayout.cellSize.x + currentLayout.spacing.x;
        var rowElementCount = Mathf.Floor((rectWidth - currentLayout.cellSize.x) / actualCellWidth) + 1;

        currentLayout.padding.left =
            (int) (Mathf.Floor((rectWidth - (currentLayout.cellSize.x + actualCellWidth * (rowElementCount - 1))) / 2) +
                   minPaddingLeft);
        currentLayout.childAlignment = TextAnchor.UpperLeft;
    }
}
