using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static GameObject Instance;
    void Awake() 
    {
        if (Instance == null)
            Instance = gameObject;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
}
