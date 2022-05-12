using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Exhibition
{
    public class TabControls : MonoBehaviour
    {
        // Каждая кнопка должна соответствовать панели
        // 0-й элемент в панелях - пустой, т.к. начальная панель всегда должна быть активна
        [SerializeField] 
        private Button[] tabButtons;
        [SerializeField] 
        private GameObject[] tabPanels;
        public void Start()
        {
            foreach (var panel in tabPanels.Skip(1))
            {
                panel.SetActive(false);
            }

            void disableClickedButton(int index)
            {
                for (var i = 0; i < tabButtons.Length; i++)
                    tabButtons[i].interactable = i != index;
            }
            void closeTabs() 
            {
                for (var i = 1; i < tabPanels.Length; i++)
                    tabPanels[i].SetActive(false);
            }
        
            tabButtons[0].onClick.AddListener(() =>
            {
                disableClickedButton(0);
                closeTabs();
            });
        
            for (var i = 1; i < tabButtons.Length; i++)
            {
                var index = i;
                tabButtons[index].onClick.AddListener(() =>
                {
                    disableClickedButton(index);
                    closeTabs();
                    tabPanels[index].SetActive(true);
                });
            }
        }
    
        void Update()
        {
            //tabButtons[currentTab].interactable = false;
        }
    }
}
