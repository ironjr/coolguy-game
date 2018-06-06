using UnityEngine;

public class LoopAudio : MonoBehaviour
{
    public AudioClip[] AudioClipList;

    private AudioClip _audioClip;
    private AudioSource _audioSource;
    private uint[] _orderList;
    private uint _index;

	void Start()
    {
        _orderList = new uint[AudioClipList.Length];
        RandomizeTrack();
        _index = 0;
        _audioSource = gameObject.GetComponent<AudioSource>();
        Play();
	}

    private void Play()
    {
        // The track is fully traversed.
        if (_index == AudioClipList.Length)
        {
            RandomizeTrack();
            _index = 0;
        }

        // Play the audio of the right index of the track.
        _audioClip = AudioClipList[_orderList[_index]];
        _audioSource.clip = _audioClip;
        _audioSource.Play();
        ++_index;

        // Schedule next audio.
        Invoke("Play", _audioSource.clip.length);
    }

    // RandomizeTrack - Generate random permutation to randomize the audio track.
    private void RandomizeTrack()
    {
        // Shuffle the track order list.
        int len = AudioClipList.Length;
        for (int i = 0; i < len; ++i)
        {
            _orderList[i] = (uint)i;
        }
        for (int i = len - 1; i >= 0; --i)
        {
            int j = Random.Range(0, i + 1);
            uint temp = _orderList[i];
            _orderList[i] = _orderList[j];
            _orderList[j] = temp;
        }
    }
}
