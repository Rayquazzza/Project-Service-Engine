using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayOneShotAudioClip(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioController: Attempted to play a null AudioClip.");
            return;
        }

        audioSource.PlayOneShot(clip);
    }

    public void PlayAudioClip(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioController: Attempted to play a null AudioClip.");
            return;
        }

        audioSource.clip = clip;

        audioSource.Play();
    }

}
