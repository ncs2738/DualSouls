using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [SerializeField]
    private List<Tile> TileTypes;

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

    private Tile currentSelectedTile;
    private Unit currentSelectedUnit;
    private List<Tile> availableUnitMoves;

    private List<Unit> selectedEnemyUnits;
    private List<Tile> enemyUnitMoves;

    private bool hasSelected = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        availableUnitMoves = new List<Tile>();
        selectedEnemyUnits = new List<Unit>();
        enemyUnitMoves = new List<Tile>();

        GenerateGrid();
    }

    public void SetTileType(Tile tile, Tile.TileType tileType)
    {
        int newTileType = 0;

        switch (tileType)
        {
            case Tile.TileType.Fortress:
                newTileType = (int) Tile.TileType.Fortress;
                break;

            case Tile.TileType.PlayerCastle:
                newTileType = (int)Tile.TileType.PlayerCastle;
                break;

            case Tile.TileType.SpawnableTile:
                newTileType = (int)Tile.TileType.SpawnableTile;
                break;

            default:
                newTileType = 0;
                break;   
        }

        Tile newTile = CreateNewTile(tile.gameObject.transform.parent, newTileType, (int)tile.transform.position.x, (int)tile.transform.position.y);
        tiles[tile.transform.position] = newTile;

        Destroy(tile.gameObject);
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
                Tile newTile = Instantiate(TileTypes[0], new Vector3(x, y), Quaternion.identity);
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

    public void SetUnitData(Tile newSelectedTile, Unit newSelectedUnit)
    {
        //update the unit data
        currentSelectedTile = newSelectedTile;
        currentSelectedUnit = newSelectedUnit;

        //grab & show it's move-pool 
        availableUnitMoves = currentSelectedUnit.GetAvailableMoves(Unit.UnitMoveTypes.Wizard);
        currentSelectedUnit.ShowAvailableMoves(true);
    }

    public void ClearUnitData()
    {
        currentSelectedUnit.ShowAvailableMoves(false);
        availableUnitMoves.Clear();
        currentSelectedTile = null;
        currentSelectedUnit = null;
    }

    public void LeftClickInputHandler(Tile newSelectedTile, Unit newSelectedUnit = null)
    {
        //check to see if we've selected a unit yet...
        if(currentSelectedUnit != null)
        {
            //we have a selected unit, so check if the tile we selected is in the unit's movement list
            if(currentSelectedUnit.IsTileInMovePool(newSelectedTile))
            {
                //it is! this is a valid selection! First move the unit
                currentSelectedUnit.MoveUnit(newSelectedTile);

                //then clear the unit data
                ClearUnitData();
            }
            //check if the player clicked on an empty spot or on the same unit...
            else if (newSelectedTile.IsTileEmpty() || newSelectedUnit.Equals(currentSelectedUnit))
            {
                //they did, so let's let the player unselect what they've chosen
                ClearUnitData();
            }
            //The player clicked on another unit...
            else
            {
                //check if the player can select the unit...
                //CURRNT PLACEHOLDER - swap the faction out with the current active team
                //if(card.color == black && newSelectedUnit.GetFaction().Equals(activeteam))
                if (newSelectedUnit.GetFaction() == PlayerTeam.Faction.Red)
                {
                    //they do! -this is a valid selection. First clear the currently selected unit's data
                    ClearUnitData();
                    //set the selected unit &tile
                    SetUnitData(newSelectedTile, newSelectedUnit);
                }
            }
        }
        else
        {
            //we haven't a selected unit yet, so check if the tile we selected has a unit...
            if (newSelectedUnit != null)
            {
                //it does - this is a valid selection. set the selected unit & tile
                SetUnitData(newSelectedTile, newSelectedUnit);
            }
        }
    }

    public void RightClickInputHandler()
    {

    }

    public bool IsTileInMovePool(Tile selectedTile)
    {
        if(availableUnitMoves.Contains(selectedTile))
        {
            return true;
        }

        return false;
    }

    public bool IsUnitSelected(Unit hoveredUnit)
    {
        if (currentSelectedUnit != null && currentSelectedUnit.Equals(hoveredUnit))
        {
            return true;
        }

        return false;
    }

    private Tile CreateNewTile(Transform parentObject, int tileType, int x, int y)
    {
        Tile newTile = Instantiate(TileTypes[tileType], new Vector3(x, y), Quaternion.identity);
        newTile.name = $"Y: {y}";
        newTile.transform.parent = parentObject;

        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);

        return newTile;
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
            UnitManager.Instance.ClearLists();
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
            CreateNewTile(tileRows[(int)savedTile.posX].transform, 0, (int)savedTile.posX, (int)savedTile.posY);

            /*
            Tile newTile = Instantiate(tile, new Vector3(, ), Quaternion.identity);
            newTile.name = $"X: {savedTile.posX} Y: {savedTile.posY}";
            newTile.transform.parent = ;
            
            //TODOOOOOO
            //newTile.SetTileType(savedTile.tileType);

            int x = (int) savedTile.posX;
            int y = (int) savedTile.posY;

            Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);

            tiles.Add(newTile.transform.position, newTile);

            /*
            if(savedTile.occupiedUnit.playerFaction != PlayerTeam.Faction.None)
            {
                UnitManager.Instance.AddUnit(newTile, savedTile.occupiedUnit.playerFaction);
            }
            */
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