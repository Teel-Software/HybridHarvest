﻿using UnityEngine;
using UnityEngine.UI;

public class NotificationCenter : MonoBehaviour
{
    [SerializeField] GameObject notificationPrefab; // префаб объекта уведомления
    [SerializeField] Color color; // цвет текста уведомления
    [SerializeField] bool useDefaultColor; // установите значение true, чтобы использовать цвет префаба

    /// <summary>
    /// Выводит на экран уведомление.
    /// </summary>
    public void Show(string notificationText)
    {
        var prevNotification = GameObject.FindGameObjectWithTag("Notification");
        if (prevNotification != null)
            Destroy(prevNotification);
        if (notificationText == "")
            notificationText = "Над этим мы ещё работаем!";

        var canvas = GameObject.FindGameObjectWithTag("Canvas");
        var textComp = Instantiate(notificationPrefab, canvas.transform, false).GetComponent<Text>();
        textComp.text = notificationText;
        if (!useDefaultColor)
            textComp.color = color;
    }
}
