using UnityEngine;

public class BasicEnemyBehaviour : MonoBehaviour
{
    public int Score = 1;
    public uint EnemyLevel = 1u;
    public uint MaxHealth = 1u;
    public float WalkSpeed = 2.0f;
    public GameObject[] Bloodsheds;
    public float BloodshedSize = 1.0f;

    private static readonly float BLOODSHED_OFFSET_Y = +0.6f;

    private enum NavState
    {
        FreeToGo,
        ObstacleEncountered
    }

    private NavState _navState = NavState.FreeToGo;
    private int _health;
    private Transform _transform;
    private static GameManager _gameManager;

    #region IPoolable
    public void OnInstantiate() { }
    
    public void OnCheckout() { }

    public void OnReturn() { }
    #endregion

    void Awake()
    {
        _health = (int)MaxHealth;
        _transform = transform;
        _gameManager = GameObject.Find("_GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        switch (_navState)
        {
            case NavState.FreeToGo:
                _transform.localPosition += new Vector3(0.0f, -1.0f, 0.1f) *
                    Time.deltaTime * WalkSpeed;
                break;
            case NavState.ObstacleEncountered:
                // Move to the center.
                Vector3 direction;
                if (_transform.position.x < 0) direction = Vector3.right;
                else direction = Vector3.left;
                _transform.localPosition += direction * Time.deltaTime * WalkSpeed;
                break;
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("BackgroundObject"))
        {
            _navState = NavState.ObstacleEncountered;
        }
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("BackgroundObject"))
        {
            _navState = NavState.FreeToGo;
        }
    }

    public void ReceiveDamage(int damage, GameObject causedBy)
    {
        _health -= damage;
        if (Bloodsheds != null)
        {
            GameObject bloodShed = Instantiate(Bloodsheds[Random.Range(0, Bloodsheds.Length)]);
            Transform bloodShedTransform = bloodShed.transform;
            bloodShedTransform.SetPositionAndRotation(
                _transform.position - new Vector3(0, BLOODSHED_OFFSET_Y),
                Quaternion.Euler(new Vector3(180.0f * Random.Range(0, 2), 180.0f * Random.Range(0, 2))));
            bloodShedTransform.localScale = new Vector3(BloodshedSize, BloodshedSize);
        }
        if (_health <= 0)
        {
            // Dead!
            Die(causedBy);
        }
    }

    private void Die(GameObject causedBy)
    {
        // TODO Maybe an animation for dying state?
        if (causedBy.CompareTag("Player"))
        {
            // Add score.
            _gameManager.GainScore(Score);
        }
        Destroy(gameObject);
    }
}
