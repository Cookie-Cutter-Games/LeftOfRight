
using UnityEngine;


public class GameManager : MonoBehaviour
{
    // public Array currentSceneManagers;
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("Game Manager is null");
            }
            return _instance;
        }
    }


    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError("Many instances of Game Manager");
        }
        _instance = this;
    }

}