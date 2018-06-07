using UnityEngine;

/// <summary>
/// Inherit this class to make a class singleton.
/// This will not prevent a non-singleton constructor such as
///     T myT = new T();
/// To prevent this to happen, add
///     protected T() { }
/// to your singleton class.
/// MonoBehaviour is inherited because of possible need of Coroutines.
/// </summary>
/// <typeparam name="T">Your singleton class.</typeparam>
public abstract class Singleton<T> : Singleton where T : MonoBehaviour
{
    #region Fields
    private static T _instance;
    private static readonly object _lock = new object();
    [SerializeField]
    private bool _persistent = true;
    #endregion // Fields

    #region Properties
    public static T Instance
    {
        get
        {
            if (Quitting)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                    "' already destroyed on quitting the application. " +
                    "Will not create this again, returing null.");
                return null;
            }
            
            lock (_lock)
            {
                if (_instance == null)
                {
                    T[] instances = (T[])FindObjectsOfType<T>();
                    int count = instances.Length;

                    // Only one instance exists.
                    if (count == 1)
                    {
                        return _instance = instances[0];
                    }

                    // More than one instances exist.
                    if (count > 1)
                    {
                        Debug.LogError("[Singleton] There exist more than one objects! " +
                            "Try to reopen the scene to fix it.");
                        for (int i = count - 1; i > 0; --i)
                        {
                            Destroy(instances[i]);
                        }
                        return _instance = instances[0];
                    }
                    
                    // No instances exist.
                    GameObject singleton = new GameObject();
                    _instance = singleton.AddComponent<T>();
                    singleton.name = "(Singleton) " + typeof(T).ToString();

                    Debug.Log("[Singleton] An instance of " + typeof(T) +
                        " is created with DontDestroyOnLoad.");
                    return _instance;
                }

                // Instance is matched.
                return _instance;
            }
        }
    }
    #endregion // Properties

    #region Methods
    void Awake()
    {
        if (_persistent)
        {
            DontDestroyOnLoad(gameObject);
        }
        OnAwake();
    }

    protected virtual void OnAwake() { }
    #endregion // Methods
}

public abstract class Singleton : MonoBehaviour
{
    #region Properties
    public static bool Quitting { get; private set; }
    #endregion // Properties

    #region Methods
    private void OnApplicationQuit()
    {
        Quitting = true;
    }
    #endregion // Methods
}