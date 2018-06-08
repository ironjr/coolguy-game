using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float MaxAmplitude = 0.2f;
    public float BurstDuration = 0.5f;
    public bool SmoothShaking = false;
    public float ShakeSmoothness = 1.0f;
    public bool SmoothRepositioning = true;
    public float RepositionSmoothness = 0.5f;

    private enum State
    {
        Idle,
        Shaking,
        Repositioning
    }

    private State _state = State.Idle;
    private Transform _transform;
    private SimpleTimer _shakeTimer;
    private Vector3 _repositionTargetPos;
    private Quaternion _repositionTargetRot;

    void Awake()
    {
        _transform = transform;
        _shakeTimer = GetComponent<SimpleTimer>();
        _shakeTimer.ResetTimer(BurstDuration);
        _shakeTimer.Alarms += RepositionCamera;
        _shakeTimer.SetActive(false);
        _repositionTargetPos = _transform.position;
        _repositionTargetRot = _transform.rotation;
    }

    void Update()
    {
        switch (_state)
        {
            case State.Idle:
                break;
            case State.Shaking:
                Vector2 rotationAmount2D = Random.insideUnitCircle * MaxAmplitude;
                Quaternion rotation = Quaternion.Euler(new Vector3(rotationAmount2D.x, rotationAmount2D.y));
                if (SmoothShaking)
                {
                    _transform.localRotation = Quaternion.Lerp(_transform.localRotation, rotation, Time.deltaTime * ShakeSmoothness);
                }
                else
                {
                    _transform.localRotation = rotation;
                }
                break;
            case State.Repositioning:
                if (SmoothRepositioning)
                {
                    if (Vector3.Distance(_transform.position, _repositionTargetPos) < 0.01f)
                    {
                        _transform.position = _repositionTargetPos;
                        _transform.rotation = _repositionTargetRot;
                        _state = State.Idle;
                    }
                    else
                    {
                        _transform.position = Vector3.Lerp(_transform.position, _repositionTargetPos, Time.deltaTime * RepositionSmoothness);
                        _transform.rotation = Quaternion.Lerp(_transform.rotation, _repositionTargetRot, Time.deltaTime * RepositionSmoothness);
                    }
                }
                else
                {
                    _transform.position = _repositionTargetPos;
                    _transform.rotation = _repositionTargetRot;
                    _state = State.Idle;
                }
                break;
            default:
                _state = State.Idle;
                break;
        }
    }

    public void ShakeCamera()
    {
        if (_state != State.Idle) return;
        _state = State.Shaking;
        _shakeTimer.SetActive(true);
    }

    public void ShakeCamera(float amplitude, float duration)
    {
        MaxAmplitude = amplitude;
        BurstDuration = duration;
        if (_state != State.Idle) return;
        _state = State.Shaking;
        _shakeTimer.ResetTimer(BurstDuration);
        _shakeTimer.SetActive(true);
    }

    public void RepositionCamera()
    {
        _state = State.Repositioning;
    }

    public void RepositionCamera(Vector3 position)
    {
        _state = State.Repositioning;
        _repositionTargetPos = position;
    }

    public void RepositionCamera(Vector3 position, Quaternion rotation)
    {
        _state = State.Repositioning;
        _repositionTargetPos = position;
        _repositionTargetRot = rotation;
    }

    public void RepositionCamera(Transform tf)
    {
        _state = State.Repositioning;
        _repositionTargetPos = tf.position;
        _repositionTargetRot = tf.rotation;
    }
}
