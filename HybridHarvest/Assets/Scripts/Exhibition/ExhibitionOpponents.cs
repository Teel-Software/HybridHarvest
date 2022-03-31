using System;
using UnityEngine;
using Random = System.Random;

public class ExhibitionOpponents : MonoBehaviour
{
    [SerializeField] private Sprite[] portraits;
    [SerializeField] private GameObject[] cards;
        
    private void OnEnable()
    {            
        var rand = new Random(DateTime.Now.Millisecond);
        var count = rand.Next(0, 3);
        foreach (var card in cards)
        {
            var port = rand.Next(0, 2);
            card.SetActive(true);
            card.GetComponent<ExhibitionCard>().portrait.sprite = portraits[port];
        }
        for (var i = 1; i <= count; i++)
            cards[cards.Length - i].gameObject.SetActive(false);
    }
}