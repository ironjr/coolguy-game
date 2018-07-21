using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BoardManager : Singleton<BoardManager>
{
    public static readonly int WIDTH = 13;
    public static readonly int HEIGHT = 27;
    public static readonly int HEIGHT_BUFFER_TILES = 3;
    public static readonly float TILE_SIZE = .8f;
    public static readonly uint TILE_SIZE_PIXEL = 32;

    public StartScreenData[] StartScreens;
    public ThemeData[] Themes;
    public int ThemeID = 0;
    public float WalkSpeed = 30.0f;

    // Number of background objects in a line of tiles.
    public float ObjectDensity = 1.2f;

    public Transform BoardContainerTransform;
    public Transform BackgroundObjectContainerTransform;
    //public Transform FXContainerTransform;
    
    private static readonly float PIXEL_SIZE = TILE_SIZE / TILE_SIZE_PIXEL;
    private static readonly float MIN_TOP_TILE_Y = 8.8f;
    private static readonly float MIN_BOTTOM_OBJECT_Y = -13.0f;
    private static readonly int MAX_SORTING_LAYER_INDEX = System.Int16.MaxValue;

    // This comparer makes the _boardObjects list a minheap.
    private class PooledObjectYComparer : IComparer<PooledObject>
    {
        public int Compare(PooledObject a, PooledObject b)
        {
            float aY = a.transform.position.y;
            float bY = b.transform.position.y;
            if (aY > bY) return -1;
            else if (aY < bY) return 1;
            else return 0;
        }
    }

    private StartScreenData _startScreen;
    private ThemeData _theme;
    private PooledObject[][] _board;
    private BinaryHeap<PooledObject> _boardObjects;
    private PooledObjectYComparer _gameObjectYComparer;
    private uint _topTileIndex;
    private float _pixelWalkDelay;
    private float _deltaTime = .0f;
    private float _meanNumObjects;
    private int _nextBGOSortingLayerIndex;

    private WeightedPO _leftEdge;
    private WeightedPO _rightEdge;

    void Awake()
    {
        _pixelWalkDelay = 1.0f / WalkSpeed;
        _meanNumObjects = HEIGHT_BUFFER_TILES * ObjectDensity;

        // Load data from asset database.
        LoadTheme(ThemeID);
    }

    // Assume these scripts have been executed: GameManager::Start
    void Start()
    {
        // Initiate the start screen tileset.
        int halfWidth = (WIDTH - 1) / 2;
        int halfHeight = (HEIGHT - 1) / 2;
        PooledObject tile;
        _board = new PooledObject[HEIGHT][];
		for (int i = halfHeight; i >= -halfHeight; --i)
        {
            _board[i + halfHeight] = new PooledObject[WIDTH];
            for (int j = -halfWidth; j <= halfWidth; ++j)
            {
                tile = GetTileByID(_startScreen.Tiles[halfHeight - i][j + halfWidth])
                    .GetObject(BoardContainerTransform);
                tile.transform.SetPositionAndRotation(
                    new Vector3(j * TILE_SIZE, i * TILE_SIZE),
                    new Quaternion());
                _board[i + halfHeight][j + halfWidth] = tile;
            }
        }
        _topTileIndex = (uint)HEIGHT - 1;

        // Initiate the start screen objects.
        _gameObjectYComparer = new PooledObjectYComparer();
        _boardObjects = new BinaryHeap<PooledObject>(_gameObjectYComparer);
        _nextBGOSortingLayerIndex = MAX_SORTING_LAYER_INDEX;
        BinaryHeap<PooledObject> sortingHeap = new BinaryHeap<PooledObject>(_gameObjectYComparer);
        if (_startScreen.BGObjects != null)
        {
            PooledObject backgroundObject;
            int len = _startScreen.BGObjects.Count;
            for (int i = 0; i < len; ++i)
            {
                StartScreenObject tempObject = _startScreen.BGObjects[i];
                backgroundObject = GetBackgroundObjectFromID(tempObject.ObjectID)
                    .GetObject(BackgroundObjectContainerTransform);
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
        _startScreen = StartScreens[ThemeID];
        _theme = Themes[ThemeID];
        _leftEdge = _theme.GetTileByID(36);
        _rightEdge = _theme.GetTileByID(32);
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

    private void LoadTheme(int themeID)
    {
        _startScreen = StartScreens[themeID];
        _theme = Themes[themeID];

        // TODO state-specific.
        _leftEdge = _theme.GetTileByID(36);
        _rightEdge = _theme.GetTileByID(32);
    }

    private void GenerateNewTerrain(float topY)
    {
        
        uint top = _topTileIndex;
        int halfWidth = (WIDTH - 1) / 2;
        PooledObject newTile;
        PooledObject oldTile;
        for (int i = 0; i < HEIGHT_BUFFER_TILES; ++i)
        {
            top = (uint)((top + 1) % HEIGHT);
            topY += TILE_SIZE;
            for (int j = -halfWidth; j <= halfWidth; ++j)
            {
                if (j < -2 || j > 2)
                {
                    newTile = _theme.Tiles[0].GetRandomObject()
                        .GetObject(BoardContainerTransform);
                }
                else if (j == -2)
                {
                    newTile = _leftEdge
                        .GetObject(BoardContainerTransform);
                }
                else if (j == 2)
                {
                    newTile = _rightEdge
                        .GetObject(BoardContainerTransform);
                }
                else
                {
                    newTile = _theme.Tiles[1].GetRandomObject()
                        .GetObject(BoardContainerTransform);
                }
                newTile.transform.SetPositionAndRotation(
                    new Vector3(j * TILE_SIZE, topY),
                    new Quaternion());
                oldTile = _board[top][j + halfWidth];
                _board[top][j + halfWidth] = newTile;
                oldTile.ReturnToPool();
            }
        }
        _topTileIndex = top;
    }

    private void GenerateNewObject(float topY)
    {
        // Remove offscreen objects.
        PooledObject bottomObject;
        while (!_boardObjects.IsEmpty())
        {
            bottomObject = _boardObjects.Peek();
            if (bottomObject.transform.position.y < MIN_BOTTOM_OBJECT_Y)
            {
                _boardObjects.RemoveRoot().ReturnToPool();
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
        BackgroundObject newObject;
        BinaryHeap<PooledObject> sortingHeap = new BinaryHeap<PooledObject>(_gameObjectYComparer);
        for (int i = 0; i < numObjects; ++i)
        {
            newObject = (BackgroundObject)_theme.GetRandomBGObject()
                .GetObject(BackgroundObjectContainerTransform);

            float randY = UnityEngine.Random.Range(minY, maxY);
            newObject.transform.SetPositionAndRotation(
                new Vector3(newObject.GetRandomPosition(), randY),
                Quaternion.Euler(new Vector3(0, 180.0f * UnityEngine.Random.Range(0, 2), 0)));

            sortingHeap.Insert(newObject);
        }

        // Maximum sorting layer index is going to be occupied. Should
        // rearrange the whole sorting layer for nextly created objects.
        {
            if (_nextBGOSortingLayerIndex - numObjects <= 0)
            {
                int index;
                int len = _boardObjects.Count;
                int lim = MAX_SORTING_LAYER_INDEX - len;
                PooledObject backgroundObject;
                BinaryHeap<PooledObject> newBoardObject = new BinaryHeap<PooledObject>(_gameObjectYComparer);
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
            PooledObject backgroundObject;
            for (index = _nextBGOSortingLayerIndex; index > lim; --index)
            {
                backgroundObject = sortingHeap.RemoveRoot();
                backgroundObject.GetComponent<SpriteRenderer>().sortingOrder = index;
                _boardObjects.Insert(backgroundObject);
            }
            _nextBGOSortingLayerIndex = index;
        }
    }

    private PooledObject GetTileByID(int id)
    {
        int type = id >> 4;
        int num = id & 0xf;
        return _theme.Tiles[type].Tiles[num].TileObject;
    }

    private PooledObject GetBackgroundObjectFromID(int id)
    {
        int type = id >> 4;
        int num = id & 0xf;
        return _theme.BGObjects[type].BGObjects[num].BGObject;
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
}
