using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HarvestProcessor : MonoBehaviour
{
    [SerializeField] GameObject VegItem;
    [SerializeField] GameObject Inventory;
    [SerializeField] RectTransform Place;
    private List<Seed> seeds;
    private List<GameObject> seedPlaces = new List<GameObject>();
    private Button Patch;

    public void ShowHarvestMenu(List<Seed> ParentSeed, Button patch)
    {
        Patch = patch;
        seeds = ParentSeed;
        for (var i = 0; i < seeds.Count; i++)
        {
            // var newSeed = MutateSeed(ParentSeed);
            // seeds.Add(newSeed);
            var seed = seeds[i];
            //Debug.Log(i);
            
            var item = Instantiate(VegItem, Place);
            seedPlaces.Add(item);
            
            var button = item.transform.Find("Button");
            button.GetComponentInChildren<Text>().text = "Может сохранить?";
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                Inventory.GetComponent<Inventory>().AddItem(seed);
                seeds.Remove(seed);
                seedPlaces.Remove(item);
                Destroy(item);
                if (seedPlaces.Count == 0) ClearSpace();
            });
            
            var label = item.transform.Find("Text");
            label.GetComponent<Text>().text =
                $"{seeds[i].NameInRussian} (англ. {seeds[i].Name}, лат. {seeds[i].NameInLatin})\n" +
                $"Вкус: {seeds[i].Taste}\n" +
                $"Габитус: {seeds[i].Gabitus}\n" +
                $"Время роста: {seeds[i].GrowTime}";
            
            var img = item.transform.Find("Image");
            img.GetComponent<Image>().sprite = seeds[i].PlantSprite;
        }
    }

    //private Seed MutateSeed(Seed oldSeed)
    //{
    //    var procentage = UnityEngine.Random.value;
    //    var newSeed = ScriptableObject.CreateInstance<Seed>();
    //    newSeed.SetValues(oldSeed.ToString());
    //    var plusAmount = UnityEngine.Random.value;
    //    if (procentage < 0.5 && newSeed.Gabitus <= 100)
    //    {
    //        newSeed.Gabitus += (int)(plusAmount * 5 + 1);
    //        newSeed.Price += (int)(plusAmount * 5 + 1);
    //    }
    //    else if (newSeed.Taste <= 100)
    //    {
    //        newSeed.Taste += (int)(plusAmount * 5 + 1);
    //    }
    //    return newSeed;
    //}

    private void Sell(Seed seed)
    {
        var inventory = Inventory.GetComponent<Inventory>();
        inventory.ChangeMoney(seed.Price);
        inventory.ChangeReputation(seed.Gabitus);
        inventory.SaveAllData();
    }

    public void SellAll()
    {
        for(var i=0; i < seeds.Count; i++)
        {
            Sell(seeds[i]);
            Destroy(seedPlaces[i]);
        }
        seeds.RemoveAll(x => x);
        seedPlaces.RemoveAll(x=>x);
        Patch.GetComponent<PatchGrowth>().ClearPatch();
        gameObject.SetActive(false);
        Save();
    }

    public void ClearSpace()
    {
        if(seeds.Count==0)
            Patch.GetComponent<PatchGrowth>().ClearPatch();
        for (var i = 0; i < seeds.Count; i++)
        {
            Destroy(seedPlaces[i]);
        }
        seedPlaces.RemoveAll(x => x);
        //Patch.GetComponent<PatchGrowth>().ClearPatch();
        gameObject.SetActive(false);
        Save();
    }

    private void Save()
    {
        if (seeds.Count == 0) return;
        PlayerPrefs.SetInt(Patch.name + "seedsCount", seeds.Count);
        for(var i=0; i< seeds.Count;i++)
            PlayerPrefs.SetString(Patch.name + "seedElement"+i.ToString(), seeds[i].ToString());
    }
}
