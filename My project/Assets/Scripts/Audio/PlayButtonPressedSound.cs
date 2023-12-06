using UnityEngine;

public class PlayButtonPressedSound : MonoBehaviour
{
    [SerializeField] private AudioClip clip;

    public void PlaySound(){
        SoundManager.Instance.PlayButtonSound(clip);
    }
}
