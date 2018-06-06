/// <summary>
/// Interface for objects whose instances are managed by an object pool.
/// For those objects implementing object pool design concept, make them
/// have Poolable attribute.
/// </summary>
/// <example>
/// public class SampleObject : Monobehaviour, IPoolable
/// {
///     public void OnInstantiate() { }
///     public void OnCheckout() { }
///     public void OnReturn() { }
/// }
/// </example>
public interface IPoolable
{
    /// <summary>
    /// Called when this object is instantiated by the pool.
    /// </summary>
    void OnInstantiate();

    /// <summary>
    /// Called when this object is checked out from the pool.
    /// </summary>
    void OnCheckout();

    /// <summary>
    /// Called when this object is disabled and returning to the pool.
    /// </summary>
    void OnReturn();
}
