using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResourcesManager : MonoBehaviour {
    public Action OnResourceLack;

    [SerializeField] private UITopBarView _uiPanel;

    private const string TOWER = "Tower";
    private const string FARM = "Farm";
    private const string MINE = "Mine";

    private Dictionary<string, int> _buildingsAmount;

    private bool _isChanged = false;
    private float _updateTime = 3f;

    private int _money, _moneyBalance;
    private int _food, _foodBalance;
    private int _moneyIncome = 4;
    private int _foodIncome = 2;

    public int CheckMoney() {
        return _money;
    }

    public void SpendMoney(int money) {
        _money -= money;
    }

    public void ChangeBuildingsAmount(string buildingType) {
        _buildingsAmount[buildingType] += 1;
    }

    public void ChangeMoneyIncome(int income) {
        _moneyIncome += income;
    }

    public void ChangeFoodIncome(int income) {
        _foodIncome += income;
    }

    private void Awake() {
        var _resources = Resources.LoadAll<LevelResources>("");
        _money = _resources[0].Money;
        _food = _resources[0].Food;

        _buildingsAmount = new Dictionary<string, int> {
            { FARM, 1 }, { MINE, 1}, { TOWER, 1}
        };
    }

    private void Update() {
        _uiPanel.ChangeMoney(_money);
        _uiPanel.ChangeMoneyBalance(_moneyBalance);

        _uiPanel.ChangeFood(_food);
        _uiPanel.ChangeFoodBalance(_foodBalance);

        if (_money < 0 || _food < 0) OnResourceLack();

        if (_isChanged) return;
        StartCoroutine(ChangeResources(_updateTime));

    }

    private IEnumerator ChangeResources(float waitTime) {
        _isChanged = true;

        int moneyExpenses = 2 * _buildingsAmount[TOWER];
        int moneyProfit = _moneyIncome * _buildingsAmount[MINE];

        int foodExpenses = 3 * _buildingsAmount[TOWER];
        int foodProfit = _foodIncome * _buildingsAmount[FARM];

        _moneyBalance = moneyProfit - moneyExpenses;
        _money += _moneyBalance;

        _foodBalance = foodProfit - foodExpenses;
        _food += _foodBalance;

        yield return new WaitForSeconds(waitTime);

        _isChanged = false;
    }
}