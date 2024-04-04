using TMPro;
using UnityEngine;

public class BuildingsManager : MonoBehaviour {
    [SerializeField] private GameObject[] _buildings;
    [SerializeField] private ResourcesManager _resources;
    [SerializeField] private TextMeshProUGUI _buildingCostLabel;

    [SerializeField] private string _buildingType;

    private int _buildingNumber = 0;
    private int _buildingCost = 50;

    public void Build() {
        if (_buildingNumber == _buildings.Length) return;
        if (_resources.CheckMoney() < _buildingCost) return;

        _resources.SpendMoney(_buildingCost);
        _resources.ChangeBuildingsAmount(_buildingType);

        _buildings[_buildingNumber].SetActive(true);

        _buildingNumber += 1;
        _buildingCost += 50;

        ShowBuildCost();
    }

    private void ShowBuildCost(){
        _buildingCostLabel.text = $"{_buildingCost}";
    }
}