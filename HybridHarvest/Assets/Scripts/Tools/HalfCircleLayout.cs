using UnityEngine;

public class HalfCircleLayout : MonoBehaviour
{
    // Радиус полукруга
    [SerializeField] public float Radius = 80f;
    // Минимальное расстояние до ближайшего края
    [SerializeField] public float Padding = 30f;
    
    /// <summary>
    /// Организует дочерние объекты в виде верхнего полукруга.
    /// </summary>
    private void Start()
    {
        if (transform.childCount < 2) return;

        var step = (Radius - Padding) * 2 / (transform.childCount - 1);
        var nextX = -Radius + Padding;

        foreach (RectTransform child in transform)
        {
            child.anchoredPosition = new Vector2(nextX, Mathf.Sqrt(Radius * Radius - nextX * nextX));
            nextX += step;
        }
    }

    // void Update()
    // {
    //     Start();
    // }
}
