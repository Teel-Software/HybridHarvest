using TMPro;
using UnityEngine;

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
        var tmPro = Instantiate(notificationPrefab, canvas.transform, false)
            .GetComponent<TextMeshProUGUI>();
        tmPro.text = notificationText;
        if (!useDefaultColor)
            tmPro.color = color;
    }
}
