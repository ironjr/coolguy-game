using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IntRow
{
    public int[] Data;

    public int this[int x]
    {
        get
        {
            return Data[x];
        }
        set
        {
            Data[x] = value;
        }
    }
}

[System.Serializable]
public class StartScreenObject
{
    public int ObjectID;
    public Vector3 Position;
    public bool FlipX;
    public bool FlipY;
}

[CreateAssetMenu]
public class StartScreenData : ScriptableObject
{
    public IntRow[] Tiles;
    public List<StartScreenObject> BGObjects;
}
