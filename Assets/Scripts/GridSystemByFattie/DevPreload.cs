using UnityEngine;
using UnityEngine.SceneManagement;

public class DevPreload : MonoBehaviour
{
    void Awake()
    {
        GameObject check = GameObject.Find("__App");
        if (check == null)
        {
            SceneManager.LoadScene("_preload");
        }
    }
}
