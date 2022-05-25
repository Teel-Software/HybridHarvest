using UnityEngine;

/// <summary>
/// Вызывает Update у неактивных объектов.
/// </summary>
public class UpdateInactiveObjects : MonoBehaviour
{
    [SerializeField] private GameObject[] objects;

    private void Update()
    {
        foreach (var obj in objects)
        {
            if (!obj.gameObject.activeSelf)
                obj.GetComponent<IUpdateable>()?.Update();
        }
    }
}
