using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StartScreenData))]
public class StartScreenDataEditor : Editor
{
    private StartScreenData _db;

    void Awake()
    {
        _db = (StartScreenData)target;
    }

    public override void OnInspectorGUI()
    {
        GUILayout.BeginVertical();

        // Tiles at startup.
        GUILayout.Label("Tile Grid Arrangement");
        DisplayTile();
        GUILayout.Space(5);

        // Background objects at startup.
        GUILayout.Label("Background Object Placement");
        DisplayBGObject();

        GUILayout.EndVertical();
    }

    private void DisplayTile()
    {
        int height = _db.Tiles.Length;
        for (int i = 0; i < height; ++i)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(35));
            int width = _db.Tiles[i].Data.Length;
            for (int j = 0; j < width; ++j)
            {
                _db.Tiles[i].Data[j] = EditorGUILayout.IntField(
                    _db.Tiles[i].Data[j], GUILayout.Width(35));
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DisplayBGObject()
    {
        int idWidth = 35;
        int floatWidth = 60;
        int boolWidth = 40;
        if (GUILayout.Button("New Background Object", GUILayout.Height(25),
            GUILayout.Width(idWidth + floatWidth + floatWidth + boolWidth + boolWidth)))
        {
            // Add new BackgroundObject.
            _db.BGObjects.Add(new StartScreenObject());

            // Reserialize the target.
            EditorUtility.SetDirty(target);
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("ID", GUILayout.Width(idWidth));
        GUILayout.Label("X", GUILayout.Width(floatWidth));
        GUILayout.Label("Y", GUILayout.Width(floatWidth));
        GUILayout.Label("Flip X", GUILayout.Width(boolWidth));
        GUILayout.Label("Flip Y", GUILayout.Width(boolWidth));
        GUILayout.EndHorizontal();

        int height = _db.BGObjects.Count;
        for (int i = 0; i < height; ++i)
        {
            EditorGUILayout.BeginHorizontal();
            _db.BGObjects[i].ObjectID = EditorGUILayout.IntField(
                _db.BGObjects[i].ObjectID, GUILayout.Width(idWidth));
            _db.BGObjects[i].Position.x = EditorGUILayout.FloatField(
                _db.BGObjects[i].Position.x, GUILayout.Width(floatWidth));
            _db.BGObjects[i].Position.y = EditorGUILayout.FloatField(
                _db.BGObjects[i].Position.y, GUILayout.Width(floatWidth));
            _db.BGObjects[i].FlipX = EditorGUILayout.Toggle(
                _db.BGObjects[i].FlipX, GUILayout.Width(boolWidth));
            _db.BGObjects[i].FlipY = EditorGUILayout.Toggle(
                _db.BGObjects[i].FlipY, GUILayout.Width(boolWidth));
            if (GUILayout.Button("del", GUILayout.Width(30)))
            {
                // Delete specified BackgroundObject.
                _db.BGObjects.RemoveAt(i);

                // Reserialize the target.
                EditorUtility.SetDirty(target);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
