using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ThemeData))]
public class ThemeDataEditor : Editor
{
    private ThemeData _db;

    void Awake()
    {
        _db = (ThemeData)target;
    }

    public override void OnInspectorGUI()
    {
        GUILayout.BeginVertical();

        GUILayout.Label("Tiles");
        DisplayTileDict();
        GUILayout.Space(5);

        GUILayout.Label("BackgroundObjects");
        DisplayBGODict();
        GUILayout.Space(5);

        GUILayout.EndVertical();
    }

    private void DisplayTileDict()
    {
        // Display buttons.
        if (GUILayout.Button("New Tile Class", GUILayout.Height(25)))
        {
            // Add new TileClass.
            _db.Tiles.Add(new TileClass()
            {
                Name = null,
                GlobalWeight = 0,
                Tiles = new List<TileDatum>()
            });

            // Reserialize the target.
            EditorUtility.SetDirty(target);
        }

        if (_db.Tiles == null) return;
        if (_db.Tiles.Count == 0) return;
        int tileClassNum = _db.Tiles.Count;
        for (int i = 0; i < tileClassNum; ++i)
        {
            // Display class properties.
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ClassID :  " + i.ToString(), GUILayout.Width(90));
            if (GUILayout.Button("U", GUILayout.Width(20)))
            {
                // Move selected TileClass upward.
                if (i > 0)
                {
                    TileClass tmp = _db.Tiles[i];
                    _db.Tiles[i] = _db.Tiles[i - 1];
                    _db.Tiles[i - 1] = tmp;
                }
            }
            if (GUILayout.Button("D", GUILayout.Width(20)))
            {
                // Move selected TileClass upward.
                if (i < tileClassNum)
                {
                    TileClass tmp = _db.Tiles[i];
                    _db.Tiles[i] = _db.Tiles[i + 1];
                    _db.Tiles[i + 1] = tmp;
                }
            }
            if (GUILayout.Button("Del", GUILayout.Width(35)))
            {
                // Delete this tile
                _db.Tiles.RemoveAt(i);

                // Reserialize the target.
                EditorUtility.SetDirty(target);
                EditorGUILayout.EndHorizontal();
                return;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name :", GUILayout.Width(60));
            _db.Tiles[i].Name = EditorGUILayout.TextField(_db.Tiles[i].Name, GUILayout.Width(200));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Weight :", GUILayout.Width(60));
            _db.Tiles[i].GlobalWeight = (uint)EditorGUILayout.IntField(
                (int)_db.Tiles[i].GlobalWeight, GUILayout.Width(40));
            EditorGUILayout.EndHorizontal();

            ++EditorGUI.indentLevel;
            // Display field names
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ID", GUILayout.Width(50));
            EditorGUILayout.LabelField("TileObject", GUILayout.Width(150));
            if (GUILayout.Button("New", GUILayout.Width(35)))
            {
                // Add new tile.
                _db.Tiles[i].Tiles.Add(new TileDatum()
                {
                    ID = new int(),
                    TileObject = null
                });

                // Reserialize the target.
                EditorUtility.SetDirty(target);
            }
            EditorGUILayout.EndHorizontal();

            // Display every item in each class.
            if (_db.Tiles[i].Tiles == null) return;
            int tileNum = _db.Tiles[i].Tiles.Count;
            for (int j = 0; j < tileNum; ++j)
            {
                EditorGUILayout.BeginHorizontal();
                _db.Tiles[i].Tiles[j].ID = EditorGUILayout.IntField(
                    _db.Tiles[i].Tiles[j].ID, GUILayout.Width(50));
                _db.Tiles[i].Tiles[j].TileObject = (WeightedPO)EditorGUILayout.ObjectField(
                    _db.Tiles[i].Tiles[j].TileObject, typeof(WeightedPO), false, GUILayout.Width(150));
                if (GUILayout.Button("Del", GUILayout.Width(35)))
                {
                    // Delete this tile
                    _db.Tiles[i].Tiles.RemoveAt(j);

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

    private void DisplayBGODict()
    {
        // Display buttons.
        if (GUILayout.Button("New BackgroundObject Class", GUILayout.Height(25)))
        {
            // Add new BGObjectClass.
            _db.BGObjects.Add(new BGObjectClass()
            {
                Name = null,
                GlobalWeight = 0,
                BGObjects = new List<BGODatum>()
            });

            // Reserialize the target.
            EditorUtility.SetDirty(target);
        }

        if (_db.BGObjects == null) return;
        if (_db.BGObjects.Count == 0) return;
        int bgoClassNum = _db.BGObjects.Count;
        for (int i = 0; i < bgoClassNum; ++i)
        {
            // Display class properties.
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ClassID :  " + i.ToString(), GUILayout.Width(90));
            if (GUILayout.Button("U", GUILayout.Width(20)))
            {
                // Move selected BGObjectClass upward.
                if (i > 0)
                {
                    BGObjectClass tmp = _db.BGObjects[i];
                    _db.BGObjects[i] = _db.BGObjects[i - 1];
                    _db.BGObjects[i - 1] = tmp;
                }
            }
            if (GUILayout.Button("D", GUILayout.Width(20)))
            {
                // Move selected BGObjectClass upward.
                if (i < bgoClassNum)
                {
                    BGObjectClass tmp = _db.BGObjects[i];
                    _db.BGObjects[i] = _db.BGObjects[i + 1];
                    _db.BGObjects[i + 1] = tmp;
                }
            }
            if (GUILayout.Button("Del", GUILayout.Width(35)))
            {
                // Delete this bgo
                _db.BGObjects.RemoveAt(i);

                // Reserialize the target.
                EditorUtility.SetDirty(target);
                EditorGUILayout.EndHorizontal();
                return;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name :", GUILayout.Width(60));
            _db.BGObjects[i].Name = EditorGUILayout.TextField(_db.BGObjects[i].Name, GUILayout.Width(200));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Weight :", GUILayout.Width(60));
            _db.BGObjects[i].GlobalWeight = (uint)EditorGUILayout.IntField(
                (int)_db.BGObjects[i].GlobalWeight, GUILayout.Width(40));
            EditorGUILayout.EndHorizontal();

            ++EditorGUI.indentLevel;
            // Display field names
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ID", GUILayout.Width(50));
            EditorGUILayout.LabelField("BackgroundObject", GUILayout.Width(150));
            if (GUILayout.Button("New", GUILayout.Width(35)))
            {
                // Add new bgo.
                _db.BGObjects[i].BGObjects.Add(new BGODatum()
                {
                    ID = new int(),
                    BGObject = null
                });

                // Reserialize the target.
                EditorUtility.SetDirty(target);
            }
            EditorGUILayout.EndHorizontal();

            // Display every item in each class.
            if (_db.BGObjects[i].BGObjects == null) return;
            int bgoNum = _db.BGObjects[i].BGObjects.Count;
            for (int j = 0; j < bgoNum; ++j)
            {
                EditorGUILayout.BeginHorizontal();
                _db.BGObjects[i].BGObjects[j].ID = EditorGUILayout.IntField(
                    _db.BGObjects[i].BGObjects[j].ID, GUILayout.Width(50));
                _db.BGObjects[i].BGObjects[j].BGObject = (BackgroundObject)EditorGUILayout.ObjectField(
                    _db.BGObjects[i].BGObjects[j].BGObject, typeof(BackgroundObject), false, GUILayout.Width(150));
                if (GUILayout.Button("Del", GUILayout.Width(35)))
                {
                    // Delete this bgo
                    _db.BGObjects[i].BGObjects.RemoveAt(j);

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
