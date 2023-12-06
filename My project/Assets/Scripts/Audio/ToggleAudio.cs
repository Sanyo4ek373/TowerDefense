using UnityEngine;

public class ToggleAudio : MonoBehaviour
{
    [SerializeField] private bool toggleMusic;
    [SerializeField] private bool toggleEffects;

    public void Toggle(){
        if(toggleMusic) SoundManager.Instance.ToggleMusic();
        if(toggleEffects) SoundManager.Instance.ToggleEffects();
    }
}
