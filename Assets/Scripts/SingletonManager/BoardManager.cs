using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
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

[Serializable]
public class BackgroundObject : WeightedGameObject
{
    public Vector2 LeftRange;
    public Vector2 RightRange;
    public int ThemeID = 0;

    public float GetRandomPosition()
    {
        float min = LeftRange.x;
        float leftBound = LeftRange.y;
        float rightBase = RightRange.x;
        float max = leftBound + RightRange.y - rightBase;
        float pos = UnityEngine.Random.Range(min, max);
        if (pos < leftBound) return pos;
        else return (pos - leftBound + rightBase);
    }
}

[Serializable]
public class StartScreenObject
{
    public int ObjectID;
    public Vector3 Position;
    public bool FlipX;
    public bool FlipY;
}

public class BoardManager : Singleton<BoardManager>
{
    public static readonly int WIDTH = 13;
    public static readonly int HEIGHT = 27;
    public static readonly int HEIGHT_BUFFER_TILES = 3;
    public static readonly float TILE_SIZE = .8f;
    public static readonly uint TILE_SIZE_PIXEL = 32;

    public float WalkSpeed = 30.0f;

    public WeightedGameObject[] GrassTiles; // 0
    public WeightedGameObject[] StoneTiles; // 1
    public GameObject[] GrassStoneBoundaryTiles; // 2

    public BackgroundObject[] SmallObjects; // 0
    public uint SmallObjectsWeight;
    public BackgroundObject[] MediumObjects; // 1
    public uint MediumObjectsWeight;
    public BackgroundObject[] LargeObjects; // 2
    public uint LargeObjectsWeight;

    // Number of background objects in a line of tiles.
    public float ObjectDensity = 1.2f;

    public Transform BoardContainerTransform;
    public Transform BackgroundObjectContainerTransform;
    //public Transform FXContainerTransform;

    private static readonly float PIXEL_SIZE = TILE_SIZE / TILE_SIZE_PIXEL;
    private static readonly float MIN_TOP_TILE_Y = 8.8f;
    private static readonly float MIN_BOTTOM_OBJECT_Y = -13.0f;
    private static readonly int MAX_SORTING_LAYER_INDEX = Int16.MaxValue;

    // This comparer makes the _boardObjects list a minheap.
    private class GameObjectYComparer : IComparer<GameObject>
    {
        public int Compare(GameObject a, GameObject b)
        {
            float aY = a.transform.position.y;
            float bY = b.transform.position.y;
            if (aY > bY) return -1;
            else if (aY < bY) return 1;
            else return 0;
        }
    }

    private GameObject[][] _board;
    private BinaryHeap<GameObject> _boardObjects;
    private GameObjectYComparer _gameObjectYComparer;
    private GameManager _gameManager;
    private uint[] _objectTypeWeights;
    private uint _topTileIndex;
    private float _pixelWalkDelay;
    private float _deltaTime = .0f;
    private float _meanNumObjects;
    private int _nextBGOSortingLayerIndex;

    void Awake()
    {
        _pixelWalkDelay = 1.0f / WalkSpeed;
        _objectTypeWeights = new uint[3] { SmallObjectsWeight, MediumObjectsWeight, LargeObjectsWeight };
        _meanNumObjects = HEIGHT_BUFFER_TILES * ObjectDensity;
        _gameManager = GameManager.Instance;
    }

    // Assume these scripts have been executed: GameManager::Start
    void Start()
    {
        StartScreenData startScreenData = new StartScreenData();
        StartScreenLoader.LoadScreen(_gameManager.CurrentThemeID, ref startScreenData);
        if (startScreenData == null)
        {
            Debug.LogError("Start screen data is not well prepared.");
            Application.Quit();
        }

        // Initiate the start screen tileset.
        int halfWidth = (WIDTH - 1) / 2;
        int halfHeight = (HEIGHT - 1) / 2;
        GameObject tile;
        _board = new GameObject[HEIGHT][];
		for (int i = halfHeight; i >= -halfHeight; --i)
        {
            _board[i + halfHeight] = new GameObject[WIDTH];
            for (int j = -halfWidth; j <= halfWidth; ++j)
            {
                tile = Instantiate(
                    GetTileFromID(startScreenData.Tiles[halfHeight - i][j + halfWidth]),
                    BoardContainerTransform);
                tile.transform.SetPositionAndRotation(
                    new Vector3(j * TILE_SIZE, i * TILE_SIZE),
                    new Quaternion());
                _board[i + halfHeight][j + halfWidth] = tile;
            }
        }
        _topTileIndex = (uint)HEIGHT - 1;

        // Initiate the start screen objects.
        _gameObjectYComparer = new GameObjectYComparer();
        _boardObjects = new BinaryHeap<GameObject>(_gameObjectYComparer);
        _nextBGOSortingLayerIndex = MAX_SORTING_LAYER_INDEX;
        BinaryHeap<GameObject> sortingHeap = new BinaryHeap<GameObject>(_gameObjectYComparer);
        if (startScreenData.BackgroundObjects != null)
        {
            GameObject backgroundObject;
            int len = startScreenData.BackgroundObjects.Length;
            for (int i = 0; i < len; ++i)
            {
                StartScreenObject tempObject = startScreenData.BackgroundObjects[i];
                backgroundObject = Instantiate(
                    GetBackgroundObjectFromID(tempObject.ObjectID),
                    BackgroundObjectContainerTransform);
                backgroundObject.transform.SetPositionAndRotation(
                    tempObject.Position,
                    Quaternion.Euler(new Vector3(tempObject.FlipX ? 1 : 0, tempObject.FlipY ? 1 : 0)));
                sortingHeap.Insert(backgroundObject);
            }
            int index;
            int lim = MAX_SORTING_LAYER_INDEX - len;
            for (index = MAX_SORTING_LAYER_INDEX; index > lim; --index)
            {
                backgroundObject = sortingHeap.RemoveRoot();
                backgroundObject.GetComponent<SpriteRenderer>().sortingOrder = index;
                _boardObjects.Insert(backgroundObject);
            }
            _nextBGOSortingLayerIndex = index;
        }
	}

    void Update()
    {
        _deltaTime += Time.deltaTime;

        while (_deltaTime >= _pixelWalkDelay)
        {
            // Update deltaTime.
            _deltaTime -= _pixelWalkDelay;

            // Move all tiles one pixel downward.
            foreach (Transform tile in BoardContainerTransform)
            {
                tile.localPosition += new Vector3(0, -PIXEL_SIZE, 0);
            }

            // Move all object one pixel downward.
            foreach (Transform bgObject in BackgroundObjectContainerTransform)
            {
                bgObject.localPosition += new Vector3(0, -PIXEL_SIZE, 0);
            }

            // Move all effects one pixel downward.
            GameObject[] fxObjects = GameObject.FindGameObjectsWithTag("FX");
            if (fxObjects != null)
            {
                for (int i = 0; i < fxObjects.Length; ++i)
                {
                    fxObjects[i].transform.localPosition += new Vector3(0, -PIXEL_SIZE, 0);
                }
            }
        }

        // If the tiles were moved downward more than a certain limit, generate
        // new terrain.
        float topY = _board[_topTileIndex][0].transform.position.y;
        if (topY < MIN_TOP_TILE_Y)
        {
            GenerateNewTerrain(topY);
            GenerateNewObject(topY);
        }
    }

    private void GenerateNewTerrain(float topY)
    {
        uint top = _topTileIndex;
        int halfWidth = (WIDTH - 1) / 2;
        GameObject newTile;
        GameObject oldTile;
        for (int i = 0; i < HEIGHT_BUFFER_TILES; ++i)
        {
            top = (uint)((top + 1) % HEIGHT);
            topY += TILE_SIZE;
            for (int j = -halfWidth; j <= halfWidth; ++j)
            {
                if (j < -2 || j > 2)
                {
                    newTile = Instantiate(PickRandomTile(0),
                        BoardContainerTransform);
                }
                else if (j == -2)
                {
                    newTile = Instantiate(GrassStoneBoundaryTiles[4],
                        BoardContainerTransform);
                }
                else if (j == 2)
                {
                    newTile = Instantiate(GrassStoneBoundaryTiles[0],
                        BoardContainerTransform);
                }
                else
                {
                    newTile = Instantiate(PickRandomTile(1),
                        BoardContainerTransform);
                }
                newTile.transform.SetPositionAndRotation(
                    new Vector3(j * TILE_SIZE, topY),
                    new Quaternion());
                oldTile = _board[top][j + halfWidth];
                _board[top][j + halfWidth] = newTile;
                Destroy(oldTile);
            }
        }
        _topTileIndex = top;
    }

    private void GenerateNewObject(float topY)
    {
        // Remove offscreen objects.
        GameObject bottomObject;
        while (!_boardObjects.IsEmpty())
        {
            bottomObject = _boardObjects.Peek();
            if (bottomObject.transform.position.y < MIN_BOTTOM_OBJECT_Y)
            {
                Destroy(_boardObjects.RemoveRoot());
            }
            else
            {
                break;
            }
        }

        // Generate new objects according to the random weights.
        float minY = topY + 0.5f * TILE_SIZE;
        float maxY = minY + HEIGHT_BUFFER_TILES * TILE_SIZE;
        int numObjects = (int)Mathf.Round(_meanNumObjects +
            (float)RandomDistributionGenerator.NextGaussianDouble(new System.Random()));

        // No new object needed to be created. Procedure ends.
        if (numObjects < 0)
        {
            return;
        }

        // There exist some background objects to be created.
        GameObject newObject;
        BinaryHeap<GameObject> sortingHeap = new BinaryHeap<GameObject>(_gameObjectYComparer);
        for (int i = 0; i < numObjects; ++i)
        {
            int objectType = GetIndexFromWeightedRandom(_objectTypeWeights);
            int objectIndex;
            float randY;
            switch (objectType)
            {
                case 0: // SmallObjects
                    objectIndex = GetIndexFromWeightedRandom(SmallObjects);
                    newObject = Instantiate(
                        SmallObjects[objectIndex].Object,
                        BackgroundObjectContainerTransform);
                    randY = UnityEngine.Random.Range(minY, maxY);
                    newObject.transform.SetPositionAndRotation(
                        new Vector3(SmallObjects[objectIndex].GetRandomPosition(), randY),
                        Quaternion.Euler(new Vector3(0, 180.0f * UnityEngine.Random.Range(0, 2), 0)));
                    sortingHeap.Insert(newObject);
                    break;
                case 1: // MediumObjects
                    objectIndex = GetIndexFromWeightedRandom(MediumObjects);
                    newObject = Instantiate(
                        MediumObjects[objectIndex].Object,
                        BackgroundObjectContainerTransform);
                    randY = UnityEngine.Random.Range(minY, maxY);
                    newObject.transform.SetPositionAndRotation(
                        new Vector3(MediumObjects[objectIndex].GetRandomPosition(), randY),
                        Quaternion.Euler(new Vector3(0, 180.0f * UnityEngine.Random.Range(0, 2), 0)));
                    sortingHeap.Insert(newObject);
                    break;
                case 2: // LargeObjects
                    objectIndex = GetIndexFromWeightedRandom(LargeObjects);
                    newObject = Instantiate(
                        LargeObjects[objectIndex].Object,
                        BackgroundObjectContainerTransform);
                    randY = UnityEngine.Random.Range(minY, maxY);
                    newObject.transform.SetPositionAndRotation(
                        new Vector3(LargeObjects[objectIndex].GetRandomPosition(), randY),
                        Quaternion.Euler(new Vector3(0, 180.0f * UnityEngine.Random.Range(0, 2), 0)));
                    sortingHeap.Insert(newObject);
                    break;
                default:
                    break;
            }
        }

        // Maximum sorting layer index is going to be occupied. Should
        // rearrange the whole sorting layer for nextly created objects.
        {
            if (_nextBGOSortingLayerIndex - numObjects <= 0)
            {
                int index;
                int len = _boardObjects.Count;
                int lim = MAX_SORTING_LAYER_INDEX - len;
                GameObject backgroundObject;
                BinaryHeap<GameObject> newBoardObject = new BinaryHeap<GameObject>(_gameObjectYComparer);
                for (index = MAX_SORTING_LAYER_INDEX; index > lim; --index)
                {
                    backgroundObject = _boardObjects.RemoveRoot();
                    backgroundObject.GetComponent<SpriteRenderer>().sortingOrder = index;
                    newBoardObject.Insert(backgroundObject);
                }

                // Migrate.
                _boardObjects = newBoardObject;
                _nextBGOSortingLayerIndex = index;
            }
        }

        // Add newly created objects to the _boardObjects list.
        {
            int index;
            int lim = _nextBGOSortingLayerIndex - sortingHeap.Count;
            GameObject backgroundObject;
            for (index = _nextBGOSortingLayerIndex; index > lim; --index)
            {
                backgroundObject = sortingHeap.RemoveRoot();
                backgroundObject.GetComponent<SpriteRenderer>().sortingOrder = index;
                _boardObjects.Insert(backgroundObject);
            }
            _nextBGOSortingLayerIndex = index;
        }
    }

    private GameObject GetTileFromID(int id)
    {
        int type = id >> 4;
        int num = id & 0xf;
        switch (type)
        {
            case 0: // Grass
                return GrassTiles[num].Object;
            case 1: // Stone
                return StoneTiles[num].Object;
            case 2: // Grass-Stone boundary
                return GrassStoneBoundaryTiles[num];
            default:
                return null;
        }
    }

    private GameObject GetBackgroundObjectFromID(int id)
    {
        int type = id >> 4;
        int num = id & 0xf;
        switch (type)
        {
            case 0: // SmallObjects
                return SmallObjects[num].Object;
            case 1: // MediumObjects
                return MediumObjects[num].Object;
            case 2: // LargeObjects
                return LargeObjects[num].Object;
            default:
                return null;
        }
    }

    private GameObject PickRandomTile(int type)
    {
        int index;
        switch (type)
        {
            case 0: // Grass
                index = GetIndexFromWeightedRandom(GrassTiles);
                return GrassTiles[index].Object;
            case 1: // Stone
                index = GetIndexFromWeightedRandom(StoneTiles);
                return StoneTiles[index].Object;
            case 2: // Grass-Stone boundary
                return null;
            default:
                return null;
        }
    }

    private int GetIndexFromWeightedRandom(uint[] weights)
    {
        int len = weights.Length;
        uint sum = 0;
        for (int i = 0; i < len; ++i)
        {
            sum += weights[i];
        }
        int random = (int)UnityEngine.Random.Range(0, sum);

        int idx;
        for (idx = 0; random >= 0 && idx < len; ++idx)
        {
            random -= (int)weights[idx];
        }
        return idx - 1;
    }

    private int GetIndexFromWeightedRandom(WeightedGameObject[] list)
    {
        int len = list.Length;
        uint sum = 0;
        for (int i = 0; i < len; ++i)
        {
            sum += list[i].Weight;
        }
        int random = (int)UnityEngine.Random.Range(0, sum);

        int idx;
        for (idx = 0; random >= 0 && idx < len; ++idx)
        {
            random -= (int)list[idx].Weight;
        }
        return idx - 1;
    }
}
