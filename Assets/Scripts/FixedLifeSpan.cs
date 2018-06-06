using UnityEngine;

public class FixedLifeSpan : MonoBehaviour
{
    public float Lifespan = 1.0f;

	void OnEnable()
    {
        Invoke("Disable", Lifespan);
	}

    void OnDisable()
    {
        CancelInvoke();
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }
}
