using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ExhibitionTabSwitcher : MonoBehaviour
{
    // Каждая кнопка должна соответствовать панели
    // 0-й элемент в панелях - пустой, т.к. начальная панель всегда должна быть активна
    [SerializeField] private Button[] tabButtons;
    [SerializeField] private GameObject[] tabPanels;
    void Start()
    {
        void disableClickedButton(int index)
        {
            for (var i = 0; i < tabButtons.Length; i++)
                tabButtons[i].interactable = i != index;
        }
        void closeNonZeroTabs() 
        {
            for (var i = 1; i < tabPanels.Length; i++)
                tabPanels[i].SetActive(false);
        }
        
        tabButtons[0].interactable = false;
        tabButtons[0].onClick.AddListener(() =>
        {
            disableClickedButton(0);
            closeNonZeroTabs();
        });
        
        for (var i = 1; i < tabButtons.Length; i++)
        {
            var index = i;
            tabButtons[i].onClick.AddListener(() =>
            {
                disableClickedButton(index);
                closeNonZeroTabs();
                tabPanels[index].SetActive(true);
            });
        }
    }
    
    void Update()
    {
        
    }
}
