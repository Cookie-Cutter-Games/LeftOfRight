using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

[System.Serializable]
public class StringGameObjectPair
{
    public string key;
    public GameObject value;
}

[System.Serializable]
public class TowerStatistics
{
    [SerializeField] public int maxAmountOfTowers;
    [SerializeField] public int currentAmountOfTowers = 0;
    [SerializeField] public TowerManager[] _towerManagers;
}

[System.Serializable]
public class EnemyAndWaveStatistics
{
    public int waveNumber;
    public int enemyNumberNeedsToKill;
    public int locallyKilledEnemies;
    public int globallyKilledEnemies;
    [SerializeField] public EnemySpawner _enemySpawner;
}

[System.Serializable]
public class BuyableUpgrades
{
    public int MovementSpeedLevel = 0;
    public int AttackDamageLevel = 0;
    public int CooldownReductionLevel = 0;
    public int BuildTowerLevel = 0;
    public int UpgradeTowerLevel = 0;
    public int MovementSpeedCost;
    public int AttackDamageCost;
    public int CooldownReductionCost;
    public int BuildTowerCost;
    public int UpgradeTowerCost;
}

public interface IclassInformation
{
    public MonoBehaviour me();
    public Type meType();
}

public class BattleSceneManager : MonoBehaviour
{
    [SerializeField] public PlayerStatistics _playerStatistics;
    [SerializeField] public GameObject Joystick;
    [SerializeField] public EnemyAndWaveStatistics enemyStatistics;
    [SerializeField] public TowerStatistics towerStatistics;
    [SerializeField] public BuyableUpgrades buyableUpgrades;
    [SerializeField] private Dictionary<string, GameObject> dictionary = new Dictionary<string, GameObject> { };
    [SerializeField] private List<StringGameObjectPair> serializedDictionary;

    private static BattleSceneManager _instance;
    public static BattleSceneManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("Battle Scene Manager is null");
            }
            return _instance;
        }
    }


    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError("Many instances of Battle Scene Manager");
        }
        _instance = this;
        OnAfterDeserialize();
        OnBeforeSerialize();
    }

    void Update()
    {
        if (enemyStatistics.locallyKilledEnemies >= enemyStatistics.enemyNumberNeedsToKill)
        {
            enemyStatistics._enemySpawner.stopSpawning = true;
            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
            {
                ShoppingTime();
            }
        }
    }

    void Start()
    {
        GameObject.Find("Capitol").GetComponent<CapitolManager>().capitolDeathEvent += GameOver;
        EntryDialogue();
    }

    private void OnDrawGizmos()
    {
        foreach (KeyValuePair<string, GameObject> entry in dictionary)
        {
            // Wyświetl klucz i wartość w Inspektorze
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, entry.Value.transform.position);
            Gizmos.DrawWireSphere(entry.Value.transform.position, 0.5f);
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(entry.Value.transform.position, 0.51f);
            Handles.Label(entry.Value.transform.position, entry.Key);
        }
    }
    // Funkcja wywoływana przez Unity podczas deserializacji skryptu
    private void OnAfterDeserialize()
    {
        // Konwertuj listę na słownik
        dictionary = new Dictionary<string, GameObject>();
        foreach (StringGameObjectPair pair in serializedDictionary)
        {
            dictionary.Add(pair.key, pair.value);
        }
    }
    private void OnBeforeSerialize()
    {
        // Konwertuj słownik na listę
        serializedDictionary = new List<StringGameObjectPair>();
        foreach (KeyValuePair<string, GameObject> entry in dictionary)
        {
            serializedDictionary.Add(new StringGameObjectPair
            {
                key = entry.Key,
                value = entry.Value
            });
        }
    }

    public void EntryDialogue()
    {

    }

    public void GameOver(object c, EventArgs e)
    {
        changeUI(true, "gameover");
        findAndKillObjects("Enemy");
    }

    public void ShoppingTime()
    {
        changeUI(true, "shopping");
    }
    public void StartGameAfterShopping()
    {
        changeUI(false, "shopping");
    }


    public void Pause()
    {
        changeUI(true, "pause");
    }

    public void Unpause()
    {
        changeUI(false, "pause");
    }

    private void findAndKillObjects(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject obj in objects)
        {
            Destroy(obj);
        }
    }
    private void findAndFreezeObject(string tag, bool state)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject obj in objects)
        {
            obj.GetComponent<IStateManager>().FreezeObject(state);
        }
    }

    public void changeUI(bool goingToPause, string value)
    {
        switch (value)
        {
            case "shopping":
                dictionary["BtnPause"].SetActive(!goingToPause);
                dictionary["BtnFire"].SetActive(!goingToPause);
                Joystick.SetActive(!goingToPause);
                dictionary["ShoppingPanel"].SetActive(goingToPause);
                if (!goingToPause)
                {
                    enemyStatistics.waveNumber++;
                    enemyStatistics.locallyKilledEnemies = 0;
                    enemyStatistics.enemyNumberNeedsToKill *= enemyStatistics.waveNumber;
                    enemyStatistics._enemySpawner.changeSpawnCooldown(-0.2f);
                    enemyStatistics._enemySpawner.stopSpawning = false;
                }
                break;

            case "pause":
                enemyStatistics._enemySpawner.stopSpawning = goingToPause;
                foreach (TowerManager tower in towerStatistics._towerManagers)
                {
                    tower.stopAttacking = goingToPause;
                }
                Joystick.SetActive(!goingToPause);
                findAndFreezeObject("Bullet", goingToPause);
                findAndFreezeObject("Enemy", goingToPause);
                dictionary["BtnResume"].SetActive(goingToPause);
                dictionary["BtnQuit"].SetActive(goingToPause);
                dictionary["BtnFire"].SetActive(!goingToPause);
                dictionary["BtnPause"].SetActive(!goingToPause);
                break;
            case "gameover":
                enemyStatistics._enemySpawner.stopSpawning = goingToPause;
                foreach (TowerManager tower in towerStatistics._towerManagers)
                {
                    tower.stopAttacking = goingToPause;
                }
                enemyStatistics._enemySpawner.GetComponent<EnemySpawner>().stopSpawning = goingToPause;
                Joystick.SetActive(!goingToPause);
                findAndFreezeObject("Bullet", goingToPause);
                findAndFreezeObject("Enemy", goingToPause);
                dictionary["BtnFire"].SetActive(!goingToPause);
                dictionary["BtnPause"].SetActive(!goingToPause);
                dictionary["PanelGameOver"].SetActive(goingToPause);
                break;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}


public enum GameState
{
    Paused,
    RoundGoing,
    Ended,
    Shopping,
}