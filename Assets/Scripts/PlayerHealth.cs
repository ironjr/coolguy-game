using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    /**
     * Setup maximum hit point the player can have in the game. The HP of the
     * player restores itself one unit per fixed length of period.
     */
    public uint MaxHealth = 5u;
    public float HealthRegenPeriod = 1.0f;
    public float PositionMultiplier = 0.9f;
    public float Speed = 0.5f;
    public PooledObject[] Bloodsheds;
    public Transform BackgroundObjectContainerTransform;

    public bool IsDead
    {
        get
        {
            return _state == State.Dying || _state == State.Dead;
        }
    }

    private static readonly float BLOODSHED_OFFSET_Y = -0.6f;

    private enum State
    {
        Living,
        Dying,
        Dead
    }

    private int _health;
    private int _regenAmount;
    private State _state = State.Living;
    private float _regenTimer;
    private float _deathZoneY;
    private Transform _transform;
    private DeathZone _deathZone;

    void Awake()
    {
        _transform = transform;
        _deathZone = DeathZone.Instance;
        _deathZoneY = _deathZone.DeathZoneBoundaryY;
        _health = (int)MaxHealth;
        _regenAmount = 1;
        _regenTimer = .0f;
        _transform.SetPositionAndRotation(GetPlayerPositionFromHealth(), new Quaternion());
    }

	void Update()
    {
        float deltaTime = Time.deltaTime;

        Vector3 playerPos;
        Vector3 relativePos;
        switch (_state)
        {
            case State.Living:
                // Regenerate player health first.
                if (_health < MaxHealth)
                {
                    _regenTimer += deltaTime;
                    while (_regenTimer >= HealthRegenPeriod)
                    {
                        _regenTimer -= HealthRegenPeriod;

                        // Regenerate health
                        _health += _regenAmount;
                    }
                }

                if (_health > MaxHealth)
                {
                    _health = (int)MaxHealth;
                }

                // Move the player character to the appropriate hit point line.
                playerPos = _transform.position;
                relativePos = GetPlayerPositionFromHealth() - playerPos;
                _transform.localPosition += relativePos * Speed * deltaTime;
                break;
            case State.Dying:
                if (_transform.position.y <= (_deathZoneY + 1.0))
                {
                    // Kill player character and end the game if the player is dead and
                    // the player character is moved into the deathzone.
                    Die();
                }

                // Move the player character to the appropriate hit point line.
                playerPos = _transform.position;
                relativePos = GetPlayerPositionFromHealth() - playerPos;
                _transform.localPosition += relativePos * Speed * 3.0f * deltaTime;
                break;
            case State.Dead:
                //_transform.localPosition += Vector3.down * Speed * deltaTime;
                break;
        }
	}

    public void ReceiveDamage(int damage, GameObject causedBy)
    {
        // Damage matters only when you are living.
        if (_state == State.Living)
        {
            _health -= damage;
            if (_health <= 0)
            {
                _health = 0;
                _state = State.Dying;
            }
            if (Bloodsheds != null)
            {
                // Player bleeds a lot.
                PooledObject bloodShed = Bloodsheds[Random.Range(0, Bloodsheds.Length)].GetObject();
                bloodShed.transform.SetPositionAndRotation(
                    _transform.position - new Vector3(0, BLOODSHED_OFFSET_Y),
                    Quaternion.Euler(new Vector3(180.0f * Random.Range(0, 2), 180.0f * Random.Range(0, 2))));
            }
        }
    }

    private Vector3 GetPlayerPositionFromHealth()
    {
        return new Vector3(0, (_health * PositionMultiplier) + _deathZoneY);
    }

    private void Die()
    {
        // Some effects make player dying majestically.
        _deathZone.GenerateExplosion(_transform.position, new Vector3(3, 3));
        PooledObject bloodShed = Bloodsheds[Random.Range(0, Bloodsheds.Length)].GetObject();
        Transform bloodShedTransform = bloodShed.transform;
        bloodShedTransform.SetPositionAndRotation(
            _transform.position - new Vector3(0, BLOODSHED_OFFSET_Y),
            Quaternion.Euler(new Vector3(180.0f * Random.Range(0, 2), 180.0f * Random.Range(0, 2))));
        bloodShedTransform.localScale = new Vector3(3, 3);

        // Player is now dead. Which is irreversible.
        _state = State.Dead;

        // This cause the board manager to move the player remnant down with
        // the same speed as other board objects.
        _transform.SetParent(BackgroundObjectContainerTransform);

        // Destroy player object after certain amount of time.
        Destroy(gameObject, 10.0f);

        // Send GameManager the game is now over.
        GameManager.Instance.GameOver();
    }
}
