using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class AdjustGridSize : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector2 baseCellSize;
    private Vector2 baseCellSpacing;
    private GridLayoutGroup layoutGroup;

    private Vector2 baseSize = new Vector2( 1800, 900);
    void Start()
    {
        layoutGroup = GetComponent<GridLayoutGroup>();
        baseCellSize = layoutGroup.cellSize;
        baseCellSpacing = layoutGroup.spacing;
        Update();
    }

    // Update is called once per frame
    void Update()
    {
        var screenSize = new Vector2(Screen.width, Screen.height);
        var ratio = screenSize.x / screenSize.y;
        var modifier = 10;
        var offset = (int) math.round(screenSize.x) / modifier;
        var offsetSize = new Vector2(screenSize.x * (1 - (float)2 / modifier), screenSize.y * (1 - (float)2 / modifier));
        var columns = 6;
        var rows = 3;
        //layoutGroup.cellSize = new Vector2(offsetSize.y / (columns * 2 - 1), offsetSize.y / (columns * 2 - 1));
        layoutGroup.spacing = new Vector2(layoutGroup.cellSize.x / (columns - 1), layoutGroup.cellSize.y / (rows - 1));
    }
}
