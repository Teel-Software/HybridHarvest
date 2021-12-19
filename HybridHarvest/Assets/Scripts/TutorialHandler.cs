using CI.QuickSave;
using UnityEngine;

public class TutorialHandler : MonoBehaviour
{
    [SerializeField] bool MainHandler;

    void Start()
    {
        if (MainHandler)
            GetComponent<Scenario>().Tutorial_Beginning();
    }

    public void SkipTutorial()
    {
        var writer = QuickSaveWriter.Create("TutorialState");
        writer.Write("TutorialSkipped", true);
        writer.Commit();
    }
}
