using UnityEngine;
using TMPro;

public class ShoppingPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Txt_moneyAmount;
    private PlayerStatistics ps;
    private TowerManager[] tm;

    void Start()
    {
        ps = BattleSceneManager.Instance._playerStatistics;
        tm = BattleSceneManager.Instance.towerStatistics._towerManagers;
    }
    void Update()
    {
        Txt_moneyAmount.text = ps.money() + " $";
    }
    public void upgradeMovementSpeed()
    {
        if (BattleSceneManager.Instance.buyableUpgrades.MovementSpeedCost <= ps.money())
        {
            ps.changeMovementSpeed(0.5f);
            ps.decreaseMoney(BattleSceneManager.Instance.buyableUpgrades.MovementSpeedCost);
            BattleSceneManager.Instance.buyableUpgrades.MovementSpeedLevel++;
            BattleSceneManager.Instance.buyableUpgrades.MovementSpeedCost = changeItemCost(3, 1, BattleSceneManager.Instance.buyableUpgrades.MovementSpeedLevel);
        }

    }

    public void upgradeAttackDamage()
    {
        if (BattleSceneManager.Instance.buyableUpgrades.AttackDamageCost <= ps.money())
        {
            ps.changeMovementSpeed(0.5f);
            ps.decreaseMoney(BattleSceneManager.Instance.buyableUpgrades.AttackDamageCost);
            BattleSceneManager.Instance.buyableUpgrades.AttackDamageLevel++;
            BattleSceneManager.Instance.buyableUpgrades.AttackDamageCost = changeItemCost(3, 1, BattleSceneManager.Instance.buyableUpgrades.AttackDamageLevel);
        }
        ps.changeDamage(2);
    }

    public void upgradeCooldownReduction()
    {
        if (BattleSceneManager.Instance.buyableUpgrades.CooldownReductionCost <= ps.money())
        {
            ps.changeShootingCooldown(-0.1f);
            ps.decreaseMoney(BattleSceneManager.Instance.buyableUpgrades.CooldownReductionCost);
            BattleSceneManager.Instance.buyableUpgrades.CooldownReductionLevel++;
            BattleSceneManager.Instance.buyableUpgrades.CooldownReductionCost = changeItemCost(3, 1, BattleSceneManager.Instance.buyableUpgrades.CooldownReductionLevel);
        }

    }

    public void buildTowers()
    {
        if (BattleSceneManager.Instance.towerStatistics.currentAmountOfTowers < BattleSceneManager.Instance.towerStatistics.maxAmountOfTowers && BattleSceneManager.Instance.buyableUpgrades.BuildTowerCost <= ps.money())
        {
            tm[BattleSceneManager.Instance.towerStatistics.currentAmountOfTowers].LevelUp();
            BattleSceneManager.Instance.towerStatistics.currentAmountOfTowers++;
            ps.decreaseMoney(BattleSceneManager.Instance.buyableUpgrades.BuildTowerCost);
            BattleSceneManager.Instance.buyableUpgrades.BuildTowerCost = changeItemCost(3, 1, BattleSceneManager.Instance.towerStatistics.currentAmountOfTowers++);
        }

    }

    public void UpgradeTowers()
    {

        if (BattleSceneManager.Instance.buyableUpgrades.UpgradeTowerCost <= ps.money())
        {
            foreach (TowerManager tower in tm)
            {
                tower.LevelUp();
            }
            ps.decreaseMoney(BattleSceneManager.Instance.buyableUpgrades.UpgradeTowerCost);
            BattleSceneManager.Instance.buyableUpgrades.UpgradeTowerLevel++;
            BattleSceneManager.Instance.buyableUpgrades.UpgradeTowerCost = changeItemCost(75, 1, BattleSceneManager.Instance.buyableUpgrades.UpgradeTowerLevel);
        }

    }
    public void endShopping()
    {
        BattleSceneManager.Instance.StartGameAfterShopping();
    }

    private bool canAfford()
    {
        return false;
    }

    public int changeItemCost(int sqrtbase, int addition, int upgradeLevel)
    {
        return (int)(sqrtbase * (Mathf.Sqrt(upgradeLevel) + 1));
    }
}