using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float CooldownTime = 0.25f;
    public GameObject SwingLeftAction;
    public GameObject SwingRightAction;
    public Transform SwingLeftTransform;
    public Transform SwingRightTransform;

    private float _cooldownTimeLeft = 0.0f;
    private PlayerHealth _playerHealth;
    private GameObject _swingLeft;
    private GameObject _swingRight;

    void Awake()
    {
        _playerHealth = GetComponent<PlayerHealth>();
        _swingLeft = Instantiate(SwingLeftAction, SwingLeftTransform);
        _swingRight = Instantiate(SwingRightAction, SwingRightTransform);
        _swingLeft.SetActive(false);
        _swingRight.SetActive(false);
    }

    void Update()
    {
        if (_playerHealth.IsDead) return;

        if (_cooldownTimeLeft <= 0.0f)
        {
            // Get input from various input devices.
            bool left = Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetMouseButtonDown(0);
            bool right = Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || Input.GetMouseButtonDown(1);
            if (!left && !right)
                return;
            else if (left && !right) SwingLeft();
            else if (!left && right) SwingRight();
            else if (left && right)
            {
                // If both keys are pressed, do the random swing.
                float rand = Random.value;
                if (rand < 0.5) SwingLeft();
                else SwingRight();
            }
            _cooldownTimeLeft = CooldownTime;
        }
        else
        {
            _cooldownTimeLeft -= Time.deltaTime;
        }
	}

    private void SwingLeft()
    {
        _swingLeft.SetActive(true);
    }

    private void SwingRight()
    {
        _swingRight.SetActive(true);
    }
}
