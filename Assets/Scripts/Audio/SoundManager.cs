using UnityEngine;

public class SoundManager : MonoBehaviour{
    public static SoundManager Instance;

    [SerializeField] private AudioSource _music;
    [SerializeField] private AudioSource _effects;

    public bool Music => _music.mute;
    public bool Effects => _effects.mute;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
    
    public void ToggleEffects() {
        _effects.mute = !_effects.mute;
    }

    public void ToggleMusic() {
        _music.mute = !_music.mute;
    }

    public void PlayButtonSound(AudioClip clip) {
        _effects.PlayOneShot(clip);
    }
}