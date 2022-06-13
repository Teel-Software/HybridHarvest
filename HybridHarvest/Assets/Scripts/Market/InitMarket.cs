using UnityEngine;

public class InitMarket : MonoBehaviour
{
    [SerializeField] public GameObject MarketWindowPrefab;

    /// <summary>
    /// Спавнит префаб биржи.
    /// </summary>
    public void OpenMarket()
    {
        var canvas = GameObject.FindGameObjectWithTag("Canvas");
        Instantiate(MarketWindowPrefab, canvas.transform, false);
    }
}
