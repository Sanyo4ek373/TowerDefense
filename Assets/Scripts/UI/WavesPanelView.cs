using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WavesPanelView : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _wavesLabel;
    [SerializeField] private Slider _wavesBar;
    [SerializeField] private EnemySpawner _enemySpawner;

    private void Awake() {
       _enemySpawner.OnWaveChange += ChangeWaveLabel;
    }

    private void OnDestroy() {
        _enemySpawner.OnWaveChange -= ChangeWaveLabel;
    }

    private void ChangeWaveLabel(int waveLevel, int waveNumber) {
        _wavesLabel.text = $"{waveLevel}";
        _wavesBar.value = waveNumber;
    }
}
