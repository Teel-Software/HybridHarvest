using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HarvestProcessor : MonoBehaviour
{
    [SerializeField] GameObject VegItem;
    [SerializeField] GameObject Inventory;
    [SerializeField] RectTransform Place;
    private List<Seed> seeds = new List<Seed>();
    private List<GameObject> items = new List<GameObject>();

    public void Show(Seed ParentSeed)
    {
        for (var i = 0; i < ParentSeed.Amount; i++)
        {
            var newSeed = MutateSeed(ParentSeed);
            seeds.Add(newSeed);
            var item = Instantiate(VegItem, Place);
            items.Add(item);
            var button = item.transform.Find("Button");
            var label = item.transform.Find("Text");
            var img = item.transform.Find("Image");
            button.GetComponentInChildren<Text>().text = "А не сохранить ли?";
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                Inventory.GetComponent<Inventory>().AddItem(newSeed);
                seeds.Remove(newSeed);
                items.Remove(item);
                Destroy(item);
            });
            label.GetComponent<Text>().text = newSeed.NameInRussian +"/"+ newSeed.Name + "/" + newSeed.NameInLatin + "\nВкус: " + newSeed.Taste.ToString() + "\nГабитус: " + newSeed.Gabitus.ToString() + "\nВремя роста: " + newSeed.GrowTime.ToString();
            img.GetComponent<Image>().sprite = newSeed.PlantSprite;
        }
    }

    private Seed MutateSeed(Seed oldSeed)
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
        }
        return newSeed;
    }

    private void Sell(Seed seed)
    {
        var inventory = Inventory.GetComponent<Inventory>();
        inventory.ChangeMoney(seed.Price);
        inventory.ChangeReputation(seed.Gabitus);
    }

    public void SellAll()
    {
        for(var i=0; i < seeds.Count; i++)
        {
            Sell(seeds[i]);
            Destroy(items[i]);
        }
        seeds.RemoveAll(x => x);
        items.RemoveAll(x=>x);
        gameObject.SetActive(false);
    }
}
