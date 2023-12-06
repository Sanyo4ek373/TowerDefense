using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITopBarView : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _hpLabel;
    [SerializeField] private Slider _healthBar;

    [SerializeField] private TextMeshProUGUI _moneyLabel;
    [SerializeField] private TextMeshProUGUI _moneyBalanceLabel;

    [SerializeField] private TextMeshProUGUI _foodLabel;
    [SerializeField] private TextMeshProUGUI _foodBalanceLabel;


    private void Awake() {
        MainBuilding.Instance.OnTakeDamage.AddListener(ChangeHealth);
    }

    private void OnDestroy() {
        MainBuilding.Instance.OnTakeDamage.RemoveListener(ChangeHealth);
    }

    private void ChangeHealth(int health) {
        _hpLabel.text = $"{health}";
        _healthBar.value = health;
    }

    public void ChangeMoney(int money) {
        _moneyLabel.text = $"{money}";
    }

    public void ChangeMoneyBalance(int moneyBalance) {
        _moneyBalanceLabel.text = $"{moneyBalance}";
    }

    public void ChangeFood(int food) {
        _foodLabel.text = $"{food}";
    }

    public void ChangeFoodBalance(int foodBalance) {
        _foodBalanceLabel.text = $"{foodBalance}";
    }
}
