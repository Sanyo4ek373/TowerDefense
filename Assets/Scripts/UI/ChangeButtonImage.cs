using UnityEngine;
using UnityEngine.UI;
using System;

public class ChangeButtonImage : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Sprite[] images;

    [SerializeField] private bool _isMusic;
    [SerializeField] private bool _isEffects;

    private void Awake() {
        ChangeImage();
    }

    public void ChangeImage() {
        if (_isMusic) button.image.sprite = images[Convert.ToInt32(!SoundManager.Instance.Music)];
        if (_isEffects) button.image.sprite = images[Convert.ToInt32(!SoundManager.Instance.Effects)];
    }
}
