using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ButtonManager : MonoBehaviour
{

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void LeaderBoard()
    {
        string filePath = Application.persistentDataPath + "/TwoDimensionalTrouble.txt";
        string[] data = File.ReadAllLines(filePath);
        Dictionary<string, int> scores = new Dictionary<string, int>();
        foreach (string line in data)
        {
            string[] parts = line.Split(":");
            scores.Add(parts[0].Trim(), int.Parse(parts[1]));
        }
    }

}
