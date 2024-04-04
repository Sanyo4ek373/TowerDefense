using UnityEngine;

public class PlayButtonPressedSound : MonoBehaviour
{
    [SerializeField] private AudioClip _clip;

    public void PlaySound(){
        SoundManager.Instance.PlayButtonSound(_clip);
    }
}
