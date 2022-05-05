using System.Collections.Generic;
using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Exhibition
{
    public class OpponentSeedPanel : MonoBehaviour
    {
        public GameObject seedListing;
        public VerticalLayoutGroup scrollContent;

        public void SetSeeds(List<Seed> seeds)
        {
            foreach (var seed in seeds)
            {
                var listingObj = Instantiate(seedListing, scrollContent.transform);
                
                var text = listingObj.GetComponentInChildren<TextMeshProUGUI>();
                text.text = SeedStatFormatter.FormatSmall(seed);

                var listing = listingObj.GetComponentInChildren<OpponentSeedListing>();
                listing.plantImage.sprite = seed.PlantSprite;
            }
        }

        public void OnDisable()
        {
            Destroy(gameObject);
        }
    }
}