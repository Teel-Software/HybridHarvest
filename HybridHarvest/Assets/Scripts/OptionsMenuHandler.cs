using System;
using UnityEngine;

public class OptionsMenuHandler : MonoBehaviour
{
    public Action СancelAction { get; set; }

    /// <summary>
    /// Выполняет СancelAction.
    /// </summary>
    public void Cancel()
    {
        СancelAction.Invoke();
    }
}
