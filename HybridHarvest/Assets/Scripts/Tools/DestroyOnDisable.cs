using UnityEngine;

public class DestroyOnDisable : MonoBehaviour
{
    /// <summary>
    /// Уничтожает объект при отключении
    /// </summary>
    public void OnDisable()
    {
        Destroy(gameObject);
    }
}
