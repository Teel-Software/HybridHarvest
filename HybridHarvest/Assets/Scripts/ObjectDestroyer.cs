using System.Collections;
using UnityEngine;

public class ObjectDestroyer : MonoBehaviour
{
    [SerializeField] float lifeTimeSeconds;

    void Start()
    {
        StartCoroutine(DestroyObj(lifeTimeSeconds));
    }

    /// <summary>
    /// Уничтожает объект, на котором висит этот скрипт через указанное время
    /// </summary>
    IEnumerator DestroyObj(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
