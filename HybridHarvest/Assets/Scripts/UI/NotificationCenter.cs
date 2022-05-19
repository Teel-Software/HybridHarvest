using UnityEngine;
using UnityEngine.UI;

public class NotificationCenter : MonoBehaviour
{
    [SerializeField] private GameObject notificationPrefab; // префаб объекта уведомления
    [SerializeField] private Color color; // цвет текста уведомления
    [SerializeField] private bool useDefaultColor = true; // установите значение true, чтобы использовать цвет префаба

    /// <summary>
    /// Выводит на экран уведомление.
    /// </summary>
    public void Show(string notificationText)
    {
        if (notificationPrefab == null)
        {
            Debug.Log("Префаб уведомления не указан!");
            return;
        }
        
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
