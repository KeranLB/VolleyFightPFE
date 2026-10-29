using UnityEngine;

public class TestAudioEvent : MonoBehaviour
{
    private AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Event AudioTest(AudioClip audio)
    {
        audioSource.PlayOneShot(audioSource.clip);
        return null;
    }
}
