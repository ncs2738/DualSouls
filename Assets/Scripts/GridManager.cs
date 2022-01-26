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
    private Transform camera;

    private Dictionary<Vector2, Tile> tiles;

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
            for (int y = 0; y < gridHeight; y++)
            {
                Tile newTile = Instantiate(tile, new Vector3(x, y), Quaternion.identity);
                newTile.name = $"X: {x} Y: {y}";
                newTile.transform.parent = gameObject.transform;

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
}
