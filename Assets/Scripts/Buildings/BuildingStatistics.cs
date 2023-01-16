using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuildingStatistics : MonoBehaviour
{
    [SerializeField] private int _level;
    [SerializeField] private int[] _cost;

    private void upgrade(int level = 0, bool overwrite = false)
    {
        _level = overwrite ? level : _level + 1;
    }

    public int currentLevelCost()
    {
        return _cost[_level - 1];
    }

    public bool makeUpgrade()
    {
        return BattleSceneManager.Instance._playerStatistics.money() >= currentLevelCost() ? true : false;
    }

}
