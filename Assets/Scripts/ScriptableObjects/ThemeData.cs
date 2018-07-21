using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileDatum
{
    public int ID;
    public WeightedPO TileObject;
}

[System.Serializable]
public class TileClass
{
    public string Name;
    public uint GlobalWeight;
    public List<TileDatum> Tiles;

    public WeightedPO GetObjectByID(int id)
    {
        int len = Tiles.Count;
        for (int i = 0; i < len; ++i)
        {
            if (Tiles[i].ID == id) return Tiles[i].TileObject;
        }
        return null;
    }

    public WeightedPO GetRandomObject()
    {
        int len = Tiles.Count;
        uint sum = 0;
        for (int i = 0; i < len; ++i)
        {
            sum += Tiles[i].TileObject.Weight;
        }
        int random = (int)Random.Range(0, sum);

        int idx;
        for (idx = 0; random >= 0 && idx < len; ++idx)
        {
            random -= (int)Tiles[idx].TileObject.Weight;
        }
        return Tiles[idx - 1].TileObject;
    }
}

[System.Serializable]
public class BGODatum
{
    public int ID;
    public BackgroundObject BGObject;
}

[System.Serializable]
public class BGObjectClass
{
    public string Name;
    public uint GlobalWeight;
    public List<BGODatum> BGObjects;

    public BackgroundObject GetObjectByID(int id)
    {
        int len = BGObjects.Count;
        for (int i = 0; i < len; ++i)
        {
            if (BGObjects[i].ID == id) return BGObjects[i].BGObject;
        }
        return null;
    }

    public BackgroundObject GetRandomObject()
    {
        int len = BGObjects.Count;
        uint sum = 0;
        for (int i = 0; i < len; ++i)
        {
            sum += BGObjects[i].BGObject.Weight;
        }
        int random = (int)Random.Range(0, sum);

        int idx;
        for (idx = 0; random >= 0 && idx < len; ++idx)
        {
            random -= (int)BGObjects[idx].BGObject.Weight;
        }
        return BGObjects[idx - 1].BGObject;
    }
}

[CreateAssetMenu]
public class ThemeData : ScriptableObject
{
    public List<TileClass> Tiles;
    public List<BGObjectClass> BGObjects;

    public WeightedPO GetTileByID(int id)
    {
        int len = Tiles.Count;
        for (int i = 0; i < len; ++i)
        {
            WeightedPO tile = Tiles[i].GetObjectByID(id);
            if (tile) return tile;
        }
        return null;
    }

    public BackgroundObject GetBGObjectByID(int id)
    {
        int len = BGObjects.Count;
        for (int i = 0; i < len; ++i)
        {
            BackgroundObject bgo = BGObjects[i].GetObjectByID(id);
            if (bgo) return bgo;
        }
        return null;
    }

    public TileClass GetRandomTileClass()
    {
        int len = Tiles.Count;
        uint sum = 0;
        for (int i = 0; i < len; ++i)
        {
            sum += Tiles[i].GlobalWeight;
        }
        int random = (int)Random.Range(0, sum);

        int idx;
        for (idx = 0; random >= 0 && idx < len; ++idx)
        {
            random -= (int)Tiles[idx].GlobalWeight;
        }
        return Tiles[idx - 1];
    }

    public BGObjectClass GetRandomBGObjectClass()
    {
        int len = BGObjects.Count;
        uint sum = 0;
        for (int i = 0; i < len; ++i)
        {
            sum += BGObjects[i].GlobalWeight;
        }
        int random = (int)Random.Range(0, sum);

        int idx;
        for (idx = 0; random >= 0 && idx < len; ++idx)
        {
            random -= (int)BGObjects[idx].GlobalWeight;
        }
        return BGObjects[idx - 1];
    }

    public WeightedPO GetRandomTile()
    {
        TileClass tileClass = GetRandomTileClass();
        return tileClass.GetRandomObject();
    }

    public BackgroundObject GetRandomBGObject()
    {
        BGObjectClass bgoClass = GetRandomBGObjectClass();
        return bgoClass.GetRandomObject();
    }
}
