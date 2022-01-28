using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcreteUnit : MonoBehaviour
{
    public enum Orientation
    {
        NORTH = 0,
        SOUTH = 1,
        EAST = 2,
        WEST = 4
    }

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

    private void Start()
    {
        availableMoves = new List<Tile>();
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

    public void ShowAvailableMoves(bool status)
    {
        for (int i = 0; i < availableMoves.Count; i++)
        {
            availableMoves[i].SetHoverHighlight(status);
        }
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
        if (moveType == null) return null;
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
    }

    [System.Serializable]
    public class SaveObject
    {
        public UnitKind unitKind;

        public Elements elementOne;
        public Elements elementTwo;

        public Tile location;
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

            location = Location,
            orientation = orientation,

            playerFaction = faction,
        };
    }
}
