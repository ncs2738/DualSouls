using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ConcreteUnit : MonoBehaviour
{
    public UnitKind unitKind;
    public Sprite Appearance => unitKind.appearance;
    public Array2DEditor.Array2DInt AttackPattern => unitKind.attackPattern;

    public Elements elementOne;
    public Elements elementTwo;

    private Tile _location;
    public Tile Location
    {
        get => _location;
        set
        {
            _location = value;
            _location.OccupyTile(this);
        }
    }

    public Vector2 CurrentPos => Location.transform.position;

    private List<Tile> availableMoves;
    public Orientation orientation;

    public PlayerTeam.Faction faction;

    public SpriteRenderer elementOneRenderer;
    public SpriteRenderer elementTwoRenderer;

    private void Start()
    {
        availableMoves = new List<Tile>();
    }

    int everyTen = 0;
    private void Update()
    {
        if (everyTen == 0)
        {
            UpdateAppearance();
        }

        everyTen++;

        everyTen = everyTen % 10;
    }

    public void ClearTile()
    {
        Location.RemoveUnit();
        // WARNING: Should this set _location to null, too?
    }

    public void MoveUnit(Tile newLocation)
    {
        Debug.Log("MoveUnit called");
        ClearTile();
        Location = newLocation;
        float oldZ = transform.position.z;
        transform.position = new Vector3 (CurrentPos.x, CurrentPos.y, oldZ);
    }

    public void ShowAttackedTiles(bool status)
    {
        foreach (Tile tile in GetTilesThisAttacks())
        {
            tile.SetHoverHighlight(status);
        }
    }

    public void ShowAvailableMoves(bool status)
    {
        for (int i = 0; i < availableMoves.Count; i++)
        {
            availableMoves[i].SetHoverHighlight(status);
        }
    }

    public List<Tile> GetTilesThisAttacks()
    {
        List<Tile> attackedTiles = new List<Tile>();
        // This could become a performance bottleneck. If it does,
        // we can 
        foreach (Vector2Int rightAttack in GetAttackPatternVectorList())
        {
            Vector2Int directedAttack = rightAttack.Rotate(orientation);
            Tile attackedTile = GridManager.Instance.GetTile(CurrentPos + directedAttack);

            if (attackedTile != null)
            {
                attackedTiles.Add(attackedTile);
            }
        }

        return attackedTiles;
    }

    public List<Vector2Int> GetAttackPatternVectorList()
    {
        List<Vector2Int> relativeAttackPositions = new List<Vector2Int>();
        Vector2Int unitPositionInPattern = Vector2Int.zero;
        bool unitPositionFound = false;

        for (int xi = 0; xi < unitKind.attackPattern.GridSize.x; xi++)
        {
            for (int yi = 0; yi < unitKind.attackPattern.GridSize.y; yi++)
            {
                int num = unitKind.attackPattern.GetCell(x: xi, y: yi);
                if (num == 0)
                {
                    // ignore zeroes
                } else if (num == 1 && !unitPositionFound)
                {
                    unitPositionInPattern = new Vector2Int (xi, yi);
                    unitPositionFound = true;
                } else if (num == 2)
                {
                    relativeAttackPositions.Add(new Vector2Int(xi, yi));
                } else
                {
                    Debug.LogWarning("Bad unit attack pattern for  `" + unitKind.name + "`");
                }
            }
        }

        if (!unitPositionFound)
        {
            Debug.LogWarning("Bad unit attack pattern for  `" + unitKind.name + "`");
        }

        return relativeAttackPositions.Select(p => p - unitPositionInPattern).ToList();
    }

    private void SetDragonMoves()
    {
        availableMoves.Clear();

        FindNextTiles(Vector2.left);
        FindNextTiles(Vector2.right);
        FindNextTiles(Vector2.up);
        FindNextTiles(Vector2.down);
    }

    private void SetWizardMoves()
    {
        availableMoves.Clear();

        FindNextTiles(Vector2.left + Vector2.up, 3);
        FindNextTiles(Vector2.right + Vector2.up, 3);
        FindNextTiles(Vector2.right + Vector2.down, 3);
        FindNextTiles(Vector2.left + Vector2.down, 3);
    }

    public bool IsTileInMovePool(Tile tile) => availableMoves.Contains(tile);

    public List<Tile> GetAvailableMoves(SpellTypes? moveType)
    {
        if (moveType == null)
        {
            return null;
        }
        if (moveType.Equals(SpellTypes.Dragon))
        {
            SetDragonMoves();
            ShowAvailableMoves(true);
            return availableMoves;
        }
        else if (moveType.Equals(SpellTypes.Wizard))
        {
            SetWizardMoves();
            ShowAvailableMoves(true);
            return availableMoves;
        }

        return null;
    }

    private void FindNextTiles(Vector2 direction, int startVal = 0, int endVal = 3)
    {
        for (int i = startVal; i <= endVal; i++)
        {
            Vector2 nextTilePos = new Vector2(CurrentPos.x + (direction.x * i), CurrentPos.y + (direction.y * i));

            Tile nextTile = GridManager.Instance.GetTile(nextTilePos);

            if (nextTile != null && nextTile.IsPassable(faction))
            {
                if (nextTile.IsTileEmpty())
                {
                    availableMoves.Add(nextTile);
                }
            }
            else
            {
                break;
            }
        }
    }

    public void UpdateAppearance()
    {

        GetComponent<SpriteRenderer>().sprite = Appearance;
        elementOneRenderer.sprite = elementOne.Sprite();
        elementTwoRenderer.sprite = elementTwo.Sprite();

        switch (orientation)
        {
            case Orientation.NORTH:
                transform.right = Vector2.up;
                break;
            case Orientation.SOUTH:
                transform.right = Vector2.down;
                break;
            case Orientation.EAST:
                transform.right = Vector2.right;
                break;
            case Orientation.WEST:
                transform.right = Vector2.left;
                break;
            default:
                break;
        }
    }

    [System.Serializable]
    public class SaveObject
    {
        public UnitKind unitKind;

        public Elements elementOne;
        public Elements elementTwo;

        public Orientation orientation;

        public PlayerTeam.Faction playerFaction;
    }

    public SaveObject Save()
    {
        return new SaveObject
        {
            unitKind = unitKind,
            elementOne = elementOne,
            elementTwo = elementTwo,

            orientation = orientation,

            playerFaction = faction,
        };
    }
}
