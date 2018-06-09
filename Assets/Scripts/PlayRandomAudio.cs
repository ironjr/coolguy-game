using UnityEngine;

public class PlayRandomAudio : MonoBehaviour
{
    public AudioClip[] AudioClipList;

    private AudioSource _audioSource;
    private float _defaultVolume;

    void Awake()
    {
        _audioSource = gameObject.GetComponent<AudioSource>();
        _defaultVolume = _audioSource.volume;
    }

    void Start()
    {
        Play();
	}

    public void Play()
    {
        if (_audioSource.isActiveAndEnabled && AudioClipList != null)
        {
            int index = Random.Range(0, AudioClipList.Length);
            _audioSource.clip = AudioClipList[index];
            _audioSource.Play();
        }
    }

    public void SetVolume()
    {
        _audioSource.volume = _defaultVolume;
    }

    public void SetVolume(bool absolute, float volume)
    {
        float newVolume = volume;
        if (!absolute)
        {
            newVolume *= _audioSource.volume;
        }

        if (newVolume <= 1.0f && newVolume >= 0.0f)
        {
            _audioSource.volume = newVolume;
        }
        else if (newVolume < 0.0f)
        {
            _audioSource.volume = 0.0f;
        }
        else
        {
            _audioSource.volume = 1.0f;
        }
    }
}
