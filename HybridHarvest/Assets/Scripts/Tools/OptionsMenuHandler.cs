using System;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuHandler : MonoBehaviour
{
    [SerializeField] public Text Timer;
    
    public Action СancelAction { get; set; }
    public Action SpeedUpAction { get; set; }

    /// <summary>
    /// Выполняет СancelAction.
    /// </summary>
    public void Cancel()
    {
        СancelAction.Invoke();
    }

    /// <summary>
    /// Выполняет SpeedUpAction.
    /// </summary>
    public void SpeedUp()
    {
        SpeedUpAction.Invoke();
    }
}
