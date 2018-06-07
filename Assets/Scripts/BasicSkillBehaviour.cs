using UnityEngine;

/// <summary>
/// Simplest properties of the skill of a game.
/// </summary>
public abstract class BasicSkillBehaviour : PooledObject
{
    /// <summary>
    /// Lifespan of the skill.
    /// </summary>
    public float Lifespan;

    /// <summary>
    /// CooldownTime of the skill.
    /// </summary>
    public float CooldownTime;

    /// <summary>
    /// Timer for cooldown effect.
    /// </summary>
    protected SimpleTimer _cooldownTimer;

    /// <summary>
    /// Timer for lifespan of the skill.
    /// Automatically call Deactivate() after the timer expires.
    /// </summary>
    protected SimpleTimer _activationTimer;

    /// <summary>
    /// Skill instance from the pool. Number of the instances live in
    /// the Scene is managed by the PoolManager.
    /// </summary>
    public BasicSkillBehaviour Instance { get; protected set; }

    /// <summary>
    /// Current activeness of the skill instance.
    /// </summary>
    public bool IsActive
    {
        get
        {
            return (Instance != null) && Instance.isActiveAndEnabled;
        }
    }

    /// <summary>
    /// Installs timers for the skill instance.
    /// </summary>
    public override void OnInstantiate()
    {
        SimpleTimer[] timers = GetComponents<SimpleTimer>();
        _cooldownTimer = timers[0];
        _activationTimer = timers[1];
        _cooldownTimer.ResetTimer(CooldownTime);
        _activationTimer.ResetTimer(Lifespan);
        _activationTimer.Alarms += Deactivate;
        _activationTimer.SetActive(false);
    }

    /// <summary>
    /// Fetching from pool act as activation of the skill.
    /// Override this method to implement preset of the skill.
    /// </summary>
    public override void OnFetchFromPool()
    {
        base.OnFetchFromPool();
    }

    /// <summary>
    /// Activate the skill.
    /// </summary>
    /// <returns>True when succeeded to activate a new skill.
    /// False if failed due to duplicate instances.</returns>
    public bool Activate()
    {
        // Failed to activate the skill: there is already an active
        // instance.
        if (IsActive) return false;
        OnActivate();
        Instance = (BasicSkillBehaviour)GetObject();
        Instance._activationTimer.SetActive(true);
        Instance._cooldownTimer.SetActive(true);
        return true;
    }

    /// <summary>
    /// Activate the skill.
    /// </summary>
    /// <param name="parentTransform">Parent transform to spawn the
    /// skill.</param>
    /// <returns>True when succeeded to activate a new skill.
    /// False if failed due to duplicate instances.</returns>
    public bool Activate(Transform parentTransform)
    {
        // Failed to activate the skill: there is already an active
        // instance.
        if (IsActive) return false;
        OnActivate();
        Instance = (BasicSkillBehaviour)GetObject(parentTransform);
        Instance._activationTimer.SetActive(true);
        Instance._cooldownTimer.SetActive(true);
        return true;
    }

    /// <summary>
    /// Activate the skill.
    /// </summary>
    /// <param name="parentTransform">Parent transform to spawn the
    /// skill.</param>
    /// <param name="localPosition">Local position of the spawned
    /// skill.</param>
    /// <returns>True when succeeded to activate a new skill.
    /// False if failed due to duplicate instances.</returns>
    public bool Activate(Transform parentTransform, Vector3 localPosition)
    {
        // Failed to activate the skill: there is already an active
        // instance.
        if (IsActive) return false;
        OnActivate();
        Instance = (BasicSkillBehaviour)GetObject(parentTransform);
        Instance.transform.localPosition = localPosition;
        Instance._activationTimer.SetActive(true);
        Instance._cooldownTimer.SetActive(true);
        return true;
    }

    /// <summary>
    /// Called when the skill is activated by Activate().
    /// Override this to implement pre-modified settings for the
    /// skill instance.
    /// </summary>
    public virtual void OnActivate() { }

    /// <summary>
    /// Called when the activation timer expires.
    /// Returns the skill object to its pool.
    /// </summary>
    protected void Deactivate()
    {
        OnDeactivate();
        ReturnToPool();
    }

    /// <summary>
    /// Called before the skill is deactivated by Deactivate().
    /// Override this to implement modification for the skill
    /// instance before it returns to the pool.
    /// </summary>
    protected virtual void OnDeactivate() { }
}
