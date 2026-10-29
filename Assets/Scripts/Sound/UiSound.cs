using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Audio;

public class UiSound : MonoBehaviour
{
    [SerializeField] private AudioClip _OnHoverHeadedButtonSound;
    [SerializeField] private AudioClip _OnClickedButtonSound;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioMixer AudioMixer;

    public void HoverHeadedSound()
    {
        _audioSource.PlayOneShot(_OnHoverHeadedButtonSound);
    }

    public void ClickedSound()
    {
        _audioSource.PlayOneShot(_OnClickedButtonSound);
    }

    private void Start()
    {
        //_audioSource.outputAudioMixerGroup.audioMixer;
        Debug.Log(AudioMixer.GetEntityId());
    }
}
