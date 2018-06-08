using UnityEngine;

public class Blink : MonoBehaviour
{
    public float OnTime = 1.0f;
    public float OffTime = 0.5f;

    private float _deltaTime = 0.0f;
    private bool _enabled = true;
    private Renderer[] _renderers;

    void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>();
    }

    void Update()
    {
        _deltaTime += Time.deltaTime;

        if (_enabled)
        {
            if (_deltaTime > OnTime)
            {
                for (int i = 0; i < _renderers.Length; ++i)
                {
                    _renderers[i].enabled = false;
                }
                _enabled = false;
                _deltaTime = 0.0f;
            }
        }
        else
        {
            if (_deltaTime > OffTime)
            {
                for (int i = 0; i < _renderers.Length; ++i)
                {
                    _renderers[i].enabled = true;
                }
                _enabled = true;
                _deltaTime = 0.0f;
            }
        }
	}
}
