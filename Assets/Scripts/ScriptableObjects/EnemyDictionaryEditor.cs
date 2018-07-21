using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyDictionary))]
public class EnemyDictionaryEditor : Editor
{
    private EnemyDictionary _db;

    void Awake()
    {
        _db = (EnemyDictionary)target;
    }

    public override void OnInspectorGUI()
    {
        GUILayout.BeginVertical();

        if (GUILayout.Button("New Enemy", GUILayout.Height(25)))
        {
            // Add new KeyValuePair.
            _db.Keys.Add(new int());
            _db.Values.Add(new BasicEnemyBehaviour());

            // Reserialize the target.
            EditorUtility.SetDirty(target);
        }
        GUILayout.Space(5);

        DisplayKeyValuePair();
        GUILayout.Space(5);

        GUILayout.EndVertical();
    }

    private void DisplayKeyValuePair()
    {
        int count = _db.Keys.Count;
        if (count == 0) return;
        if (count != _db.Values.Count)
        {
            Debug.LogError("Number of keys and values doesn't match.");
            return;
        }


        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("EnemyID", GUILayout.Width(70));
        EditorGUILayout.LabelField("EnemyObject");
        EditorGUILayout.EndHorizontal();
        for (int i = 0; i < count; ++i)
        {
            EditorGUILayout.BeginHorizontal();
            _db.Keys[i] = EditorGUILayout.IntField(_db.Keys[i], GUILayout.Width(70));
            _db.Values[i] = (BasicEnemyBehaviour)EditorGUILayout.ObjectField(_db.Values[i], typeof(BasicEnemyBehaviour), false);
            EditorGUILayout.EndHorizontal();
        }
    }
}
