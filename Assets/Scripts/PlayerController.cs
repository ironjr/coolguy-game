using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float CooldownTime = 0.25f;
    public SwingSkill SwingLeftAction;
    public SwingSkill SwingRightAction;

#if UNITY_ANDROID
    private int _screenWidth;
    private int _screenHeight;
#endif

    private float _cooldownTimeLeft = 0.0f;
    private Transform _swingLeftTransform;
    private Transform _swingRightTransform;
    private PlayerHealth _playerHealth;

    private bool _isSwingSkillActive
    {
        get
        {
            return SwingLeftAction.IsActive || SwingRightAction.IsActive;
        }
    }

    void Awake()
    {
        Transform swingSkillTransform = transform.GetChild(0);
        _swingLeftTransform = swingSkillTransform.GetChild(0);
        _swingRightTransform = swingSkillTransform.GetChild(1);
        _playerHealth = GetComponent<PlayerHealth>();

#if UNITY_ANDROID
        _screenWidth = Screen.width;
        _screenHeight = Screen.height;
#endif
    }

    void Update()
    {
        if (_playerHealth.IsDead) return;

        if (_cooldownTimeLeft <= 0.0f)
        {
            // Get input from various input devices.
            bool left = false;
            bool right = false;

#if UNITY_EDITOR || UNITY_STANDALONE
            left = Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetMouseButtonDown(0);
            right = Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || Input.GetMouseButtonDown(1);
#elif UNITY_ANDROID
            if (Input.touchCount > 0)
            {
                float touchPoint = Input.GetTouch(0).position.x - (_screenWidth / 2);
                left = touchPoint < 0.0f;
                right = touchPoint > 0.0f;
            }
#endif
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
        if (_isSwingSkillActive) return;
        SwingLeftAction.Activate(_swingLeftTransform);
    }

    private void SwingRight()
    {
        if (_isSwingSkillActive) return;
        SwingRightAction.Activate(_swingRightTransform);
    }
}
