
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using TMPro;
public class ReturnMainMenu : MonoBehaviour
{
    public TextMeshProUGUI playerNameObject;
    public void LoadMainMenu()
    {
        saveHighScore(BattleSceneManager.Instance.enemyStatistics.globallyKilledEnemies);
        SceneManager.LoadScene("MainMenu");
    }

    private void saveHighScore(int points)
    {
        if (playerNameObject.text.Length == 0)
            return;
        string filePath = Application.persistentDataPath + "/TwoDimensionalTrouble.txt";
        string[] data;
        Dictionary<string, int> scores = new Dictionary<string, int>(); ;
        if (File.Exists(filePath))
        {
            data = File.ReadAllLines(filePath);
            foreach (string line in data)
            {
                string[] parts = line.Split(":");
                if (int.Parse(parts[1]) <= BattleSceneManager.Instance.enemyStatistics.globallyKilledEnemies)
                    scores.Add(playerNameObject.text, BattleSceneManager.Instance.enemyStatistics.globallyKilledEnemies);
                if (scores.Count >= 10)
                    break;
                scores.Add(parts[0].Trim(), int.Parse(parts[1]));
            }
        }
        List<string> lines = scores.Select(kvp => $"{kvp.Key}: {kvp.Value}").ToList();
        foreach (string item in lines)
        {
            Debug.Log(item);
        }
        File.WriteAllLines(filePath, lines);

    }
}
