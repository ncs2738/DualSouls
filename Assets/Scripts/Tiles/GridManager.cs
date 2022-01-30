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
    private int gridWidth;
    [SerializeField]
    private int gridHeight;

    [SerializeField]
    private new Transform camera;

    private Dictionary<Vector2, Tile> tiles;

    public event EventHandler OnLoaded;

    private Tile currentSelectedTile;
    private ConcreteUnit currentSelectedUnit;

    private Dictionary<Tile, HashSet<ConcreteUnit>> availableUnitMoves;
    private List<Tile> availableUnitRotations;

    private List<ConcreteUnit> selectedEnemyUnits;
    private List<Tile> enemyUnitMoves;

    private bool hasSelected = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        availableUnitMoves = new Dictionary<Tile, HashSet<ConcreteUnit>>();
        availableUnitRotations = new List<Tile>();
        selectedEnemyUnits = new List<ConcreteUnit>();
        enemyUnitMoves = new List<Tile>();

        //GenerateGrid();
        Load();
    }

    public void SetTileType(Tile tile, Tile.TileType tileType)
    {
        Tile newTile = CreateNewTile(tile.gameObject.transform.parent, (int) tileType, (int)tile.transform.position.x, (int)tile.transform.position.y);
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

    public void SetUnitData(Tile newSelectedTile, ConcreteUnit newSelectedUnit)
    {
        //update the unit data
        currentSelectedTile = newSelectedTile;
        currentSelectedUnit = newSelectedUnit;

        currentSelectedUnit.ShowAttackedTiles(true);

        if (CardManager.Instance.SpellType == SpellTypes.Thief)
        {
            // the thief ability can't act on the board
            return;
        }

        Faces validFace =
            GameManager.Instance.activePlayerTurn == newSelectedUnit.faction ?
            Faces.FRONT : Faces.BACK;
        if (CardManager.Instance.SpellFace != validFace)
        {
            // Not a valid spell target.
            return;
        }

        if (CardManager.Instance.SpellType.Equals(SpellTypes.Dragon) || CardManager.Instance.SpellType.Equals(SpellTypes.Wizard))
        {
            //grab & show it's move-pool
            availableUnitMoves = currentSelectedUnit.GetAvailableMoves(CardManager.Instance.SpellType);
            currentSelectedUnit.ShowAvailableMoves(true);
        } else if (CardManager.Instance.SpellType == SpellTypes.Warrior)
        {
            availableUnitRotations = currentSelectedUnit.GetAvailableRotations();
            currentSelectedUnit.ShowAvailableRotations(true);
        }
    }

    public void ClearMovements()
    {
        currentSelectedUnit.ShowAvailableMoves(false);
        availableUnitMoves.Clear();
        currentSelectedUnit.ShowAvailableRotations(false);
        availableUnitRotations.Clear();
    }

    public void ClearHighlights()
    {
        currentSelectedUnit.ShowAvailableMoves(false);
        availableUnitMoves.Clear();
        currentSelectedUnit.ShowAvailableRotations(false);
        availableUnitRotations.Clear();
        currentSelectedUnit.ShowAttackedTiles(false);
    }

    public void ClearUnitData()
    {
        ClearHighlights();
        currentSelectedTile = null;
        currentSelectedUnit = null;
    }

    public void EnterCombatChoice(ConcreteUnit unit, ConcreteUnit.CombatKind combatKind)
    {
        ClearHighlights();
        unit.Location.SetMovementHighlight(true);

        foreach (ConcreteUnit possibleOpponent in unit.possibleOpponents)
        {
            possibleOpponent.Location.SetAttackHighlight(unit.faction, true);
        }

        if (combatKind == ConcreteUnit.CombatKind.DUEL)
        {
            foreach (ConcreteUnit possibleOpponent in unit.possibleOpponents)
            {
                possibleOpponent.SetDuelCrosshair(true);
            }
            unit.SetDuelCrosshair(true);
            CombatManager.Instance.SelectingComponent = true;
            CombatManager.Instance.CombatType = ConcreteUnit.CombatKind.DUEL;

        } else if (combatKind == ConcreteUnit.CombatKind.ONE_SIDED_DEFENSE)
        {
            unit.SetOneSidedCrosshair(true);
            CombatManager.Instance.SelectingComponent = true;
            CombatManager.Instance.CombatType = ConcreteUnit.CombatKind.ONE_SIDED_DEFENSE;

        } else if (combatKind == ConcreteUnit.CombatKind.ONE_SIDED_ATTACK)
        {
            foreach (ConcreteUnit possibleOpponent in unit.possibleOpponents)
            {
                possibleOpponent.SetOneSidedCrosshair(true);
            }

            CombatManager.Instance.SelectingComponent = true;
            CombatManager.Instance.CombatType = ConcreteUnit.CombatKind.ONE_SIDED_ATTACK;
        }
        else
        {
            //Clear the unit data - there's nothing else to do!
            ClearUnitData();
        }
    }

    public void LeftClickInputHandler(Tile newSelectedTile, ConcreteUnit newSelectedUnit = null)
    {
        Debug.Log("LeftClickInputHandler");
        //check to see if we've selected a unit yet...
        if (currentSelectedUnit != null)
        {
            Debug.Log("Selected unit isn't null");
            //we have a selected unit, so check if the tile we selected is in the unit's movement list
            HashSet<ConcreteUnit> attackersOfMove = currentSelectedUnit.AttackersOfMoveTo(newSelectedTile);
            Orientation? newOrientation = currentSelectedUnit.RotationTo(newSelectedTile);
            if (attackersOfMove != null)
            {
                Debug.Log("selected unit is movin");

                //it is! this is a valid selection! First clear the attack-ranges!
                currentSelectedUnit.ShowAvailableMoves(false);
                currentSelectedUnit.ShowAttackedTiles(false);

                //next move the unit
                currentSelectedUnit.MoveUnit(newSelectedTile, attackersOfMove);
                CardManager.Instance.CastSpell(tile: newSelectedTile, card: null);
            } 
            else if (newOrientation != null)
            {
                Debug.Log($"newOrientation: {newOrientation}");
                currentSelectedUnit.ShowAvailableMoves(false);
                currentSelectedUnit.ShowAttackedTiles(false);
                currentSelectedUnit.ShowAvailableRotations(true);
                currentSelectedUnit.RotateUnit(newOrientation ?? currentSelectedUnit.orientation);
                CardManager.Instance.CastSpell(tile: null, card: null);
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
                //if(card.color == black && newSelectedUnit.GetFaction().Equals(activeteam))
                //they do! -this is a valid selection. First clear the currently selected unit's data
                ClearUnitData();
                //set the selected unit &tile
                SetUnitData(newSelectedTile, newSelectedUnit);
            }
        }
        else
        {
            if (CardManager.Instance.HasSelectedUnit()
                && UnitManager.Instance.CanPlayerSpawnUnit(newSelectedTile)
                && newSelectedTile.OccupiedUnit == null)
            {
                UnitManager.Instance.AddUnit(newSelectedTile, GameManager.Instance.activePlayerTurn);
            }
            else
            {
                Debug.Log("Tried to select a unit");
                //we haven't a selected unit yet, so check if the tile we selected has a unit...
                if (newSelectedUnit != null)
                {
                    //it does - this is a valid selection. set the selected unit & tile
                    SetUnitData(newSelectedTile, newSelectedUnit);
                }
            }
        }
    }

    public void RightClickInputHandler()
    {

    }

    public bool IsTileInMovePool(Tile selectedTile)
    {
        if(availableUnitMoves.Keys.Contains(selectedTile))
        {
            return true;
        }

        return false;
    }

    public bool IsUnitSelected(ConcreteUnit hoveredUnit)
    {
        if (currentSelectedUnit != null && currentSelectedUnit.Equals(hoveredUnit))
        {
            return true;
        }

        return false;
    }

    public void OnTurnEnd()
    {
        foreach(Tile tile in tiles.Values)
        {
            tile.OnTurnEnd();
        }
    }

    public ConcreteUnit GetSelectedUnit()
    {
        return currentSelectedUnit;
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

    private Dictionary<Tile, ConcreteUnit.SaveObject> unitsToLoad;
    private Tile LoadNewTile(Tile.SaveObject savedTile, Transform parentObject)
    {
        int x = (int)savedTile.posX;
        int y = (int)savedTile.posY;

        Tile newTile = Instantiate(TileTypes[(int) savedTile.tileType], new Vector3(x,y,-1), Quaternion.identity);
        newTile.name = $"Y: {savedTile.posY}";
        newTile.transform.parent = parentObject;

        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);

        tiles.Add(newTile.transform.position, newTile);

        if (!savedTile.occupiedUnit.playerFaction.Equals(PlayerTeam.Faction.None))
        {
            unitsToLoad[newTile] = savedTile.occupiedUnit;
        }

        if (!savedTile.spawnableTileData.tileOwner.Equals(PlayerTeam.Faction.None))
        {
            newTile.LoadSpawnableTile(savedTile.spawnableTileData);
        }

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
        CardManager.Instance.ClearOnSpellCast();

        if (tiles != null && tiles.Count != 0)
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

        SaveObject saveObject = SaveSystem.LoadResource();

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


        unitsToLoad = new Dictionary<Tile, ConcreteUnit.SaveObject>();
        foreach (Tile.SaveObject savedTile in saveObject.savedLevel)
        {
            LoadNewTile(savedTile, tileRows[(int)savedTile.posX].transform);
        }

        foreach (KeyValuePair<Tile, ConcreteUnit.SaveObject> kv in unitsToLoad)
        {
            ConcreteUnit.SaveObject unit = kv.Value;
            tiles[kv.Key.transform.position].LoadUnit(unit);
        }
    }

    public class SaveObject
    {
        public Tile.SaveObject[] savedLevel;
        public int gridWidth;
        public int gridHeight;
    }
}
 