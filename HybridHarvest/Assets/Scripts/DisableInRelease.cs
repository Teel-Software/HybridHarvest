using UnityEngine;

public class DisableInRelease : MonoBehaviour
{
    public void Awake() {
#if !DEBUG
		gameObject.SetActive(false);
#endif
	}
}
