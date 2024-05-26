using TMPro;
using UnityEngine;

public class UpgradesManager : MonoBehaviour {
    [SerializeField] private DefenseTower[] _defenseTowers;
    [SerializeField] private ResourcesManager _resources;
    [SerializeField] private TextMeshProUGUI[] _upgradesCostLabels;

    private int _upgradesCost;
    private int _upgradesCostIncrease;
    private int _defenseTowerUpgrades = 0;
    private int _defenseTowerMaxUpgrades = 6;
    private float _reloadingTime = 0.2f;

    public void UpgradeTowerAttackSpeed() {
        if (_resources.CheckMoney() < _upgradesCost) return;
        if (_defenseTowerUpgrades == _defenseTowerMaxUpgrades) return;

        foreach (DefenseTower tower in _defenseTowers) {
            tower.ChangeReloadingTime(_reloadingTime);
        }
        
        DoUpgrade();
        _defenseTowerUpgrades += 1;
    }

    public void UpgradeMoneyIncome() {
        if (_resources.CheckMoney() < _upgradesCost) return;

        _resources.ChangeMoneyIncome(1);
        DoUpgrade();
    }

    public void UpgradeFoodIncome() {
        if (_resources.CheckMoney() < _upgradesCost) return;

        _resources.ChangeFoodIncome(1);
        DoUpgrade();
    }

    private void Awake() {
        var _resources = Resources.LoadAll<GeneratedSO_20240526_140935>("");
        _upgradesCost = _resources[0].UpgradeCost._serializableValue;
        _upgradesCostIncrease = _resources[0].UpgradeCostIncrease._serializableValue;
    }
    private void DoUpgrade() {
        _resources.SpendMoney(_upgradesCost);
        _upgradesCost += _upgradesCostIncrease;

        ShowUpgradeCost();
    }

    private void ShowUpgradeCost() {
        foreach (TextMeshProUGUI label in _upgradesCostLabels) {
            label.text = $"{_upgradesCost}";
        }
    }
}