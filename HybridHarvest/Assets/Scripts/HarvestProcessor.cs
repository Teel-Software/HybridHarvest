using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HarvestProcessor : MonoBehaviour
{
    [SerializeField] GameObject VegItem;
    public Seed ParentSeed;
    public RectTransform InventoryFrame;
    // Start is called before the first frame update
    void Start()
    {
        for (var i = 0; i < ParentSeed.Amount; i++)
        {
            var newSeed = MutateSeed(ParentSeed);
            Instantiate(VegItem, gameObject.transform);
            //InventoryFrame.GetComponent<Drawinventory>().targetInventory.AddItem(newSeed);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Seed MutateSeed(Seed oldSeed)
    {
        var procentage = UnityEngine.Random.value;
        var newSeed = ScriptableObject.CreateInstance<Seed>();
        newSeed.SetValues(oldSeed.ToString());
        var plusAmount = UnityEngine.Random.value;
        if (procentage < 0.5 && newSeed.Gabitus <= 100)
        {
            newSeed.Gabitus += (int)(plusAmount * 5 + 1);
            newSeed.Price += (int)(plusAmount * 5 + 1);
        }
        else if (newSeed.Taste <= 100)
        {
            newSeed.Taste += (int)(plusAmount * 5 + 1);
            //newSeed.Price += (int)(plusAmount * 5 + 1);
        }
        return newSeed;
    }
}
