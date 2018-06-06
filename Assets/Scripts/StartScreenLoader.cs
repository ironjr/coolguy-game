using UnityEngine;

public static class StartScreenLoader
{
    public static void LoadScreen(int themeID, ref StartScreenData startScreen)
    {
        TextAsset data = Resources.Load<TextAsset>("StartScreen" + themeID);
        string[] lines = data.text.Split(new char[] { '\n' });

        char[] comma = new char[] { ',' };

        // Receive screen dimensions.
        string[] dimension = lines[0].Split(comma);
        int width;
        int height;
        int numObjects;
        int.TryParse(dimension[1], out width);
        int.TryParse(dimension[3], out height);
        int.TryParse(dimension[5], out numObjects);

        // Parse the start screen tile data.
        startScreen.Tiles = new IntRow[height];
        for (int i = 0; i < height; ++i)
        {
            string[] mapRow = lines[i + 1].Split(comma);
            startScreen.Tiles[i] = new IntRow();
            startScreen.Tiles[i].Data = new int[width];
            for (int j = 0; j < width; ++j)
            {
                int.TryParse(mapRow[j], out startScreen.Tiles[i].Data[j]);
            }
        }

        // Parse the start screen objects data.
        startScreen.BackgroundObjects = new StartScreenObject[numObjects];
        for (int i = 0; i < numObjects; ++i)
        {
            string[] objectEntry = lines[i + height + 2].Split(comma);
            float x;
            float y;
            int flipX;
            int flipY;
            float.TryParse(objectEntry[1], out x);
            float.TryParse(objectEntry[2], out y);
            int.TryParse(objectEntry[3], out flipX);
            int.TryParse(objectEntry[4], out flipY);

            startScreen.BackgroundObjects[i] = new StartScreenObject();
            int.TryParse(objectEntry[0], out startScreen.BackgroundObjects[i].ObjectID);
            startScreen.BackgroundObjects[i].Position = new Vector3(x, y);
            startScreen.BackgroundObjects[i].FlipX = (flipX != 0);
            startScreen.BackgroundObjects[i].FlipY = (flipY != 0);
        }
    }
}
