using UnityEngine;

public class MakeExitButtonsVisible : MonoBehaviour
{
    public GameObject ExitButton;

    // Start is called before the first frame update
    void Start()
    {
        ExitButton.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        ExitButton.SetActive(gameObject.activeSelf);
    }

    void OnDisable()
    {
        ExitButton.SetActive(false);
    }
}
