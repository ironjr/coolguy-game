using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyDictionary : SerializableDictionary<int, BasicEnemyBehaviour>
{
    void OnGUI()
    {
        foreach (KeyValuePair<int, BasicEnemyBehaviour> kvp in InternalDictionary)
        {
            GUILayout.Label("ID: " + kvp.Key + "; Value: " + kvp.Value);
        }
    }
}
