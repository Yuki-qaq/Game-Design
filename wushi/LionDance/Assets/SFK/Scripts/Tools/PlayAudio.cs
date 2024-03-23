using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;

public class PlayAudio : MonoBehaviour
{
    public enum SoundPosition
    {
        OnSource,
        AtPosition,
        OnTransform
    }

    public AudioSource audioSource;
    
    public bool randomizeClip;
    
    [HideIf(nameof(randomizeClip))]
    public AudioClip clip;

    [ShowIf(nameof(randomizeClip))]
    public AudioClip[] randomAudioClips = new AudioClip[1];

    [Range(0, 1)] public float volume = 1;

    [Space]
    public bool playOnStart = false;

    [Space]
    [Tooltip("Where in the game space this audio should play: Attached to this object, At the specified position, or at the position of this transform")]
    public SoundPosition sourcePosition = SoundPosition.OnTransform;

    [ShowIf(nameof(sourcePosition), SoundPosition.AtPosition)]
    public Vector3 position = Vector3.zero;

    [ShowIf(nameof(sourcePosition), SoundPosition.OnTransform)]
    public Transform sourceTransform;

    public bool playMultiple = false;
    [ShowIf(nameof(playMultiple))]
    public float playInterval = 1;

    AudioClip lastClip;
    float timer;

    private void Reset()
    {
        position = transform.position;
        sourceTransform = transform;
    }

    private void Start()
    {
        timer = 0;

        if (playOnStart)
            Play();
    }

    void Update()
    {
        if (!playMultiple)
            return;

        timer += Time.deltaTime;
        if (timer > playInterval)
        {
            Play();
            timer = 0;
        }
    }

    public AudioClip GetClip()
    {
        if (!randomizeClip)
        {
            lastClip = clip;
            return clip;
        }
        else
        {
            int attempts = 3;
            AudioClip newClip = randomAudioClips[Random.Range(0, randomAudioClips.Length)];

            while (newClip == lastClip && attempts > 0)
            {
                newClip = randomAudioClips[Random.Range(0, randomAudioClips.Length)];
                attempts--;
            }
            
            lastClip = newClip;
            return newClip;
        }
    }

    public void Play()
    {
        switch (sourcePosition)
        {
            case SoundPosition.OnSource:
                audioSource.PlayOneShot(clip, volume);
                break;
            case SoundPosition.AtPosition:
                AudioSource.PlayClipAtPoint(clip, position);
                break;
            case SoundPosition.OnTransform:
                AudioSource.PlayClipAtPoint(clip, transform.position);
                break;
            default:
                break;
        }
    }
}
