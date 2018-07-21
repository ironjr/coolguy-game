using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemySpawnPatternData))]
public class EnemySpawnPatternDataEditor : Editor
{
    private EnemySpawnPatternData _db;
    private EnemyDictionary _enemyDict;

    void Awake()
    {
        _db = (EnemySpawnPatternData)target;
    }

    public override void OnInspectorGUI()
    {
        // Load from the global enemy dictionary.
        _enemyDict = AssetDatabase.LoadAssetAtPath<EnemyDictionary>(
            "Assets/Data/EnemyDictionary.asset");

        GUILayout.BeginVertical();

        // Buttons.
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("New Empty Pattern", GUILayout.Height(25)))
        {
            // Add new BackgroundObject.
            _db.Patterns.Add(new EnemySpawnPattern()
            {
                Level = 0,
                Instances = new List<EnemySpawnPatternEntry>()
            });

            // Reserialize the target.
            EditorUtility.SetDirty(target);
        }
        if (GUILayout.Button("New Pattern from Selection", GUILayout.Height(25)))
        {
            // Add new BackgroundObject.
            EnemySpawnPattern pattern = new EnemySpawnPattern()
            {
                Level = 0,
                Instances = new List<EnemySpawnPatternEntry>()
            };

            foreach (GameObject go in Selection.gameObjects)
            {
                Transform tf = go.transform;
                BasicEnemyBehaviour beh = go.GetComponent<BasicEnemyBehaviour>();
                if (!beh) continue;
                pattern.Instances.Add(new EnemySpawnPatternEntry()
                {
                    EnemyID = beh.EnemyID,
                    SpawnX = tf.position.x,
                    SpawnY = tf.position.y,
                    WaitForNextEntry = false,
                    WaitAmount = 0
                });
            }

            _db.Patterns.Add(pattern);

            // Reserialize the target.
            EditorUtility.SetDirty(target);
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(5);

        DisplayPattern();
        GUILayout.Space(5);

        GUILayout.EndVertical();
    }

    private void DisplayPattern()
    {
        int numPatterns = _db.Patterns.Count;
        for (int i = 0; i < numPatterns; ++i)
        {
            EnemySpawnPattern pattern = _db.Patterns[i];

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Pattern ID :  " + i.ToString(), GUILayout.Width(110));
            EditorGUILayout.LabelField("Level", GUILayout.Width(50));
            EditorGUI.BeginChangeCheck();
            pattern.Level = (uint)EditorGUILayout.IntField(
                (int)pattern.Level, GUILayout.Width(50));
            if (EditorGUI.EndChangeCheck())
            {
                uint level = pattern.Level;

                // Re-sort the pattern database with the level properties.
                int j;
                j = i;
                while (j < numPatterns - 1 && level > _db.Patterns[j + 1].Level)
                {
                    EnemySpawnPattern tmp = _db.Patterns[j];
                    _db.Patterns[j] = _db.Patterns[j + 1];
                    _db.Patterns[j + 1] = tmp;
                    ++j;
                }

                j = i;
                while (j > 0 && level < _db.Patterns[j - 1].Level)
                {
                    EnemySpawnPattern tmp = _db.Patterns[j];
                    _db.Patterns[j] = _db.Patterns[j - 1];
                    _db.Patterns[j - 1] = tmp;
                    --j;
                }

                EditorGUILayout.EndHorizontal();
                return;
            }
            if (GUILayout.Button("New Entry", GUILayout.Width(75)))
            {
                // Add new BackgroundObject.
                _db.Patterns[i].Instances.Add(new EnemySpawnPatternEntry());

                // Reserialize the target.
                EditorUtility.SetDirty(target);
            }
            if (GUILayout.Button("Del", GUILayout.Width(35)))
            {
                // Delete this tile
                _db.Patterns.RemoveAt(i);

                // Reserialize the target.
                EditorUtility.SetDirty(target);
                EditorGUILayout.EndHorizontal();
                return;
            }
            EditorGUILayout.EndHorizontal();

            if (pattern.Instances == null) return;

            ++EditorGUI.indentLevel;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("EnemyID", GUILayout.Width(80));
            EditorGUILayout.LabelField("EnemyName", GUILayout.Width(150));
            EditorGUILayout.LabelField("SpawnX", GUILayout.Width(70));
            EditorGUILayout.LabelField("SpawnY", GUILayout.Width(70));
            EditorGUILayout.LabelField("WaitNext", GUILayout.Width(75));
            EditorGUILayout.LabelField("WaitAmount", GUILayout.Width(85));
            EditorGUILayout.EndHorizontal();

            int len = pattern.Instances.Count;
            for (int j = 0; j < len; ++j)
            {
                EditorGUILayout.BeginHorizontal();
                pattern.Instances[j].EnemyID = EditorGUILayout.IntField(
                    pattern.Instances[j].EnemyID, GUILayout.Width(80));
                EditorGUILayout.LabelField(
                    _enemyDict.InternalDictionary[pattern.Instances[j].EnemyID].PoolID,
                    GUILayout.Width(150));
                pattern.Instances[j].SpawnX = EditorGUILayout.FloatField(
                    pattern.Instances[j].SpawnX, GUILayout.Width(70));
                pattern.Instances[j].SpawnY = EditorGUILayout.FloatField(
                    pattern.Instances[j].SpawnY, GUILayout.Width(70));
                pattern.Instances[j].WaitForNextEntry = EditorGUILayout.Toggle(
                    pattern.Instances[j].WaitForNextEntry, GUILayout.Width(75));
                pattern.Instances[j].WaitAmount = EditorGUILayout.FloatField(
                    pattern.Instances[j].WaitAmount, GUILayout.Width(85));
                if (GUILayout.Button("U", GUILayout.Width(20)))
                {
                    // Move selected TileClass upward.
                    if (j > 0)
                    {
                        EnemySpawnPatternEntry tmp = _db.Patterns[i].Instances[j];
                        _db.Patterns[i].Instances[j] = _db.Patterns[i].Instances[j - 1];
                        _db.Patterns[i].Instances[j - 1] = tmp;
                    }
                }
                if (GUILayout.Button("D", GUILayout.Width(20)))
                {
                    // Move selected EnemySpawnPatternEntry upward.
                    if (j < len)
                    {
                        EnemySpawnPatternEntry tmp = _db.Patterns[i].Instances[j];
                        _db.Patterns[i].Instances[j] = _db.Patterns[i].Instances[j + 1];
                        _db.Patterns[i].Instances[j + 1] = tmp;
                    }
                }
                if (GUILayout.Button("Del", GUILayout.Width(35)))
                {
                    // Delete this tile
                    _db.Patterns[i].Instances.RemoveAt(j);

                    // Reserialize the target.
                    EditorUtility.SetDirty(target);
                    EditorGUILayout.EndHorizontal();
                    --EditorGUI.indentLevel;
                    return;
                }
                EditorGUILayout.EndHorizontal();
            }
            --EditorGUI.indentLevel;
        }
    }
}
