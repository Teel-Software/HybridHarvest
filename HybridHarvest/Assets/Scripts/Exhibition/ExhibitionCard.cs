﻿using Exhibition;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExhibitionCard : MonoBehaviour
{
    public Image portrait;
    public TextMeshProUGUI opponentName;
    public Button showSeedsBtn;
    public void SetOpponent(Opponent opponent)
    {
        portrait.sprite = opponent.Portrait();
        opponentName.text = opponent.Name;
    }
}