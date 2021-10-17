using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HarvestProcessor : MonoBehaviour
{
    [SerializeField] GameObject VegItem;
    [SerializeField] RectTransform Place;
    public Seed ParentSeed;
    public RectTransform InventoryFrame;
    // Start is called before the first frame update
    void Start()
    {
        for (var i = 0; i < ParentSeed.Amount; i++)
        {
            var newSeed = MutateSeed(ParentSeed);
            var item = Instantiate(VegItem, Place);
            //item.transform.position = new Vector3(0, 0, 0);
            //place.transform.SetPositionAndRotation(new Vector3(), new Quaternion());
            //item.transform.position = new Vector3(0, /*i * item.transform.localPosition.y - 2 * item.transform.localPosition.y*/2*i - 6, 0);
            //Debug.Log(i * place.transform.localPosition.y);
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
