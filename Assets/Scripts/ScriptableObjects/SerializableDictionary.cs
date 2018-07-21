using System.Collections.Generic;
using UnityEngine;

public class SerializableDictionary<TKey, TValue> : ScriptableObject, ISerializationCallbackReceiver
{
    public List<TKey> Keys;
    public List<TValue> Values;
    public bool SaveBeforeSerialize = false;

    [System.NonSerialized]
    public Dictionary<TKey, TValue> InternalDictionary = new Dictionary<TKey, TValue>();

    public virtual void OnAfterDeserialize()
    {
        int len = Keys.Count;
        if (len != Values.Count)
        {
            Debug.LogError("Internal dictionary is not properly initiated");
            Application.Quit();
        }

        for (int i = 0; i < len; ++i)
        {
            InternalDictionary.Add(Keys[i], Values[i]);
        }
    }

    public virtual void OnBeforeSerialize()
    {
        if (SaveBeforeSerialize)
        {
            Keys.Clear();
            Values.Clear();

            foreach (KeyValuePair<TKey, TValue> kvp in InternalDictionary)
            {
                Keys.Add(kvp.Key);
                Values.Add(kvp.Value);
            }
        }
    }

    void OnGUI()
    {
        foreach (KeyValuePair<TKey, TValue> kvp in InternalDictionary)
        {
            GUILayout.Label("Key: " + kvp.Key + "; Value: " + kvp.Value);
        }
    }
}
