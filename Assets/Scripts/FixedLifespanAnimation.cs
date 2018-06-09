using System.Collections;
using UnityEngine;

public class FixedLifespanAnimation : FixedLifespanPO
{
    public float AnimationLength = 1.0f;

    protected PlayRandomAudio _playRandomAudio = null;
    protected Animator _animator = null;
    protected Renderer _renderer = null;

    void Awake()
    {
        _playRandomAudio = GetComponent<PlayRandomAudio>();
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<Renderer>();
    }

    public override void OnFetchFromPool()
    {
        base.OnFetchFromPool();
        if (_playRandomAudio != null)
        {
            _playRandomAudio.SetVolume();
            _playRandomAudio.Play();
        }
        if (_animator != null)
        {
            _animator.Rebind();
        }
        if (_renderer != null)
        {
            _renderer.enabled = true;
        }
        StartCoroutine(HideAnimation());
    }

    public void SetVolume()
    {
        if (_playRandomAudio != null)
        {
            _playRandomAudio.SetVolume();
        }
    }

    public void SetVolume(bool absolute, float volume)
    {
        if (_playRandomAudio != null)
        {
            _playRandomAudio.SetVolume(absolute, volume);
        }
    }

    public IEnumerator HideAnimation()
    {
        yield return new WaitForSeconds(AnimationLength);
        if (_renderer != null)
        {
            _renderer.enabled = false;
        }
    }
}
