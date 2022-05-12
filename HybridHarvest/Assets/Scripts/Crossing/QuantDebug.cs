using CI.QuickSave;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuantDebug : MonoBehaviour
{
    public void ResetQuantumTime()
    {
        var writer = QuickSaveWriter.Create("Quantum");
        var date = new DateTime(2002, 7, 7);
        writer.Write("CooldownEnd", date);
        writer.Commit();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
