using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRandomAudio : MonoBehaviour
{
    public AudioClip[] AudioClipList;

    private AudioSource _audioSource;

    void Awake()
    {
        _audioSource = gameObject.GetComponent<AudioSource>();
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
}
