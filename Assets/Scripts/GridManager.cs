using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [SerializeField]
    private bool MapEditModeEnabled = true;

    [SerializeField]
    private int gridWidth;
    [SerializeField]
    private int gridHeight;

    [SerializeField]
    private Tile tile;

    [SerializeField]
    private new Transform camera;

    private Dictionary<Vector2, Tile> tiles;

    public event EventHandler OnLoaded;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        tiles = new Dictionary<Vector2, Tile>();

        for (int x = 0; x < gridWidth; x++)
        {
            GameObject newColumn = new GameObject();
            newColumn.name = "X: " + x;
            newColumn.transform.parent = this.gameObject.transform;

            for (int y = 0; y < gridHeight; y++)
            {
                Tile newTile = Instantiate(tile, new Vector3(x, y), Quaternion.identity);
                newTile.name = $"Y: {y}";
                newTile.transform.parent = newColumn.transform;

                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);

                tiles.Add(newTile.transform.position, newTile);
            }
        }

        camera.transform.position = new Vector3((float)gridWidth / 2 -.5f, (float)gridHeight / 2 -.5f, -10f);
    }

    private Vector3 GetWorldPosition (int x, int y)
    {
        return new Vector3(x - .5f, y -.5f);
    }

    public Tile GetTile(Vector2 pos)
    {
        if(tiles.TryGetValue(pos, out Tile tile))
        {
            return tile;
        }

        return null;
    }

    public bool IsMapEditEnabled()
    {
        return MapEditModeEnabled;
    }

    public void Save()
    {
        List<Tile.SaveObject> savedTiles = new List<Tile.SaveObject>();
        foreach (KeyValuePair<Vector2, Tile> tile in tiles)
        {
            savedTiles.Add(tile.Value.Save());
        }

        SaveObject savedLevel = new SaveObject { savedLevel = savedTiles.ToArray(), gridHeight = gridHeight, gridWidth = gridWidth };

        SaveSystem.SaveObject(savedLevel);
    }

    public void Load()
    {
        if(tiles != null && tiles.Count != 0)
        {
            foreach (KeyValuePair<Vector2, Tile> tile in tiles)
            {
                Destroy(tile.Value.gameObject);
            }

            foreach (Transform child in gameObject.transform)
            {
                Destroy(child.gameObject);
            }

            tiles.Clear();
        }
        else
        {
            tiles = new Dictionary<Vector2, Tile>();
        }

        SaveObject saveObject = SaveSystem.LoadMostRecentObject<SaveObject>();

        gridWidth = saveObject.gridWidth;
        gridHeight = saveObject.gridHeight;

        GameObject[] tileRows = new GameObject[gridWidth];

        for (int x = 0; x < gridWidth; x++)
        {
            GameObject newColumn = new GameObject();
            newColumn.name = "X: " + x;
            newColumn.transform.parent = this.gameObject.transform;
            tileRows[x] = newColumn;
        }
        

        foreach (Tile.SaveObject savedTile in saveObject.savedLevel)
        {
            Tile newTile = Instantiate(tile, new Vector3(savedTile.posX, savedTile.posY), Quaternion.identity);
            newTile.name = $"X: {savedTile.posX} Y: {savedTile.posY}";
            newTile.transform.parent = tileRows[(int)savedTile.posX].gameObject.transform;
            newTile.SetTileType(savedTile.tileType);

            int x = (int) savedTile.posX;
            int y = (int) savedTile.posY;

            Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);

            tiles.Add(newTile.transform.position, newTile);
        }

        OnLoaded?.Invoke(this, EventArgs.Empty);
    }

    public class SaveObject
    {
        public Tile.SaveObject[] savedLevel;
        public int gridWidth;
        public int gridHeight;
    }
}