using UnityEngine;
using UnityEngine.UI;

public class ItemIconDrawer : MonoBehaviour
{
    public Button Button;
    [SerializeField] private Image packetIcon;
    [SerializeField] private Image plantIcon;

    public void SetSeed(Seed seed)
    { 
        //seed.UpdateRating();
        plantIcon.sprite = seed.PlantSprite;
        packetIcon.sprite = seed.PacketSprite;
    }

    public void SetPlus()
    {
        plantIcon.sprite = Resources.Load<Sprite>("Transparent");
        packetIcon.sprite = Resources.Load<Sprite>("seedsplus");
    }
}