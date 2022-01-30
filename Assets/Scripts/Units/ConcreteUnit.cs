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

    [SerializeField]
    private GameObject CaptureKey;

    [SerializeField]
    private GameObject duelCrosshair;
    [SerializeField]
    private GameObject oneSidedCrosshair;

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

    // Map from the terminal tile of each move,
    // To the set of attackers the unit is vulnerable to
    // upon landing in the terminal tile.
    private Dictionary<Tile, HashSet<ConcreteUnit>> availableMoves;
    public Orientation orientation;

    private List<Tile> availableRotations;

    public List<ConcreteUnit> possibleOpponents;

    public PlayerTeam.Faction faction;

    public SpriteRenderer elementOneRenderer;
    public SpriteRenderer elementTwoRenderer;
    private bool UnitHasKey = false;

    [SerializeField]
    private Color BlueTeamTint;
    [SerializeField]
    private Color RedTeamTint;
    [SerializeField]
    private SpriteRenderer tintSprite;

    private void Start()
    {
        availableMoves = new Dictionary<Tile, HashSet<ConcreteUnit>>();
        availableRotations = new List<Tile>();
        CaptureKey.SetActive(false);
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
        UnmarkAttackedTiles();
        Location.RemoveUnit();
        // WARNING: Should this set _location to null, too?
    }

    public void MoveUnit(Tile newLocation, ISet<ConcreteUnit> attackersOfMove)
    {
        Debug.Log("MoveUnit called");
        ClearTile();
        Location = newLocation;
        float oldZ = transform.position.z;
        transform.position = new Vector3 (CurrentPos.x, CurrentPos.y, oldZ);

        MarkAttackedTiles();

        InitiateCombatSelection(
            attackers: attackersOfMove.Where(a => a.faction != this.faction).ToHashSet(),
            victims: GetUnitsThisAttacks().Where(a => a.faction != this.faction).ToHashSet());

        availableMoves.Clear();

        if (!CombatManager.Instance.SelectingComponent)
        {
            GridManager.Instance.ClearUnitData();
        }
    }

    public void MarkAttackedTiles()
    {
        foreach (Tile attackedTile in GetTilesThisAttacks())
        {
            attackedTile.AddAttacker(this);
            Debug.Log($"added <{attackedTile.transform.name},{attackedTile.transform.parent.name}>");
        }
    }

    public void UnmarkAttackedTiles()
    {
        foreach (Tile attackedTile in GetTilesThisAttacks())
        {
            attackedTile.RemoveAttacker(this);
            Debug.Log($"remove <{attackedTile.transform.name},{attackedTile.transform.parent.name}>");
        }
    }

    public void RotateUnit(Orientation newOrientation)
    {
        if (newOrientation == orientation)
        {
            Debug.LogWarning($"Tried a useless rotation of `{unitKind}` from `{orientation}` to {orientation}");
        }

        // Unit is no longer attacking the tiles it used to attack
        UnmarkAttackedTiles();

        // Turn the unit
        orientation = newOrientation;

        // Unit is now attacking some new tiles
        MarkAttackedTiles();

        MoveUnit(Location, Location.AttackingUnits.ToHashSet());

        // Initiate combat
        //InitiateCombatSelection(
        //    attackers: Location.AttackingUnits.ToHashSet(),
        //    victims: GetUnitsThisAttacks());
    }

    private void InitiateCombatSelection(ISet<ConcreteUnit> attackers, ISet<ConcreteUnit> victims)
    {
        attackers.Remove(this);
        victims.Remove(this);
        HashSet<ConcreteUnit> oneSidedAttackers;
        HashSet<ConcreteUnit> oneSidedVictims;
        HashSet<ConcreteUnit> duelists;

        oneSidedAttackers = attackers.Except(victims).ToHashSet();
        oneSidedVictims = victims.Except(attackers).ToHashSet();
        duelists = attackers.Intersect(victims).ToHashSet();

        // DEBUG ONLY
        foreach (ConcreteUnit osAttacker in oneSidedAttackers)
        {
            Debug.DrawLine(osAttacker.transform.position, transform.position, Color.red, 3, false);
        }

        foreach (ConcreteUnit osVictim in oneSidedVictims)
        {
            Debug.DrawLine(osVictim.transform.position, transform.position, Color.green, 3, false);
        }

        foreach (ConcreteUnit duelist in duelists)
        {
            Debug.DrawLine(duelist.transform.position, transform.position, Color.blue, 3, false);
        }
        // DEBUG ONLY


        if (duelists.Count > 0)
        {
            possibleOpponents = duelists.ToList();
            GridManager.Instance.EnterCombatChoice(this, CombatKind.DUEL);
            // force a fight with one of the duelists
        } else if (oneSidedAttackers.Count > 0)
        {
            possibleOpponents = oneSidedAttackers.ToList();
            GridManager.Instance.EnterCombatChoice(this, CombatKind.ONE_SIDED_DEFENSE);
            // force a fight with one of the attackers
        }
        else if (oneSidedVictims.Count > 0)
        {
            possibleOpponents = oneSidedVictims.ToList();
            GridManager.Instance.EnterCombatChoice(this, CombatKind.ONE_SIDED_ATTACK);
            // force a fight with one of the victims
        }
        else
        {
            // no fight occurs
        }
    }

    public void SetDuelCrosshair(bool status)
    {
        duelCrosshair.SetActive(status);
    }

    public void SetOneSidedCrosshair(bool status)
    {
        oneSidedCrosshair.SetActive(status);
    }

    public void ClearCrosshairs()
    {
        SetDuelCrosshair(false);
        SetOneSidedCrosshair(false);
    }

    public enum CombatKind
    {
        DUEL = 0,
        ONE_SIDED_DEFENSE = 1,
        ONE_SIDED_ATTACK = 2,
    }

    // Should only be called if other is a valid combatant
    private void InitiateCombat(ConcreteUnit other, CombatKind combatKind)
    {
        //TODO: handle combat
    }

    public void ShowAttackedTiles(bool status)
    {
        if(GameManager.Instance.IsGameStarted())
        {
            foreach (Tile tile in GetTilesThisAttacks())
            {
                tile.SetAttackHighlight(faction, status);
            }
        }
    }

    public void ShowAvailableMoves(bool status)
    {
        foreach (Tile tile in availableMoves.Keys)
        {
            tile.SetMovementHighlight(status);
        }
    }

    public void ShowAvailableRotations(bool status)
    {
        foreach (Tile tile in availableRotations)
        {
            tile.SetRotationHighlight(status);
        }
    }

    public ISet<ConcreteUnit> GetUnitsThisAttacks() =>
        GetTilesThisAttacks()
        .Select(t => t.OccupiedUnit)
        .Where(u => u != null)
        .ToHashSet();

    public ISet<Tile> GetTilesThisAttacks()
    {
        HashSet<Tile> attackedTiles = new HashSet<Tile>();
        // This could become a performance bottleneck. If it does,
        // we can 
        foreach (Vector2Int rightAttack in GetAttackPatternVectorList())
        {
            Vector2Int directedAttack = rightAttack.Rotate(orientation);
            Tile attackedTile = GridManager.Instance.GetTile(CurrentPos + directedAttack);

            if (attackedTile != null)
            {
                attackedTiles.Add(attackedTile);
            } else
            {
                Debug.Log($"nu~rupo! `{CurrentPos + directedAttack}`");
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

    public HashSet<ConcreteUnit> AttackersOfMoveTo(Tile tile) => availableMoves.ContainsKey(tile)
        ? availableMoves[tile]
        : null;

    public Dictionary<Tile, HashSet<ConcreteUnit>> GetAvailableMoves(SpellTypes? moveType)
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

    public List<Tile> GetAvailableRotations()
    {
        List<Vector2Int> offsets = new List<Vector2Int>();
        // it must be a warrior
        if (orientation == Orientation.EAST || orientation == Orientation.WEST)
        {
            offsets.Add(Vector2Int.up);
            offsets.Add(Vector2Int.down);
        } else
        {
            offsets.Add(Vector2Int.left);
            offsets.Add(Vector2Int.right);
        }

        foreach (Vector2Int offset in offsets)
        {
            Tile nextTile = GridManager.Instance.GetTile(CurrentPos + offset);
            if (nextTile != null)
            {
                availableRotations.Add(nextTile);
            }
        }

        return availableRotations;
    }

    public Orientation? RotationTo(Tile other)
    {
        if (!availableRotations.Contains(other))
        {
            return null;
        }

        Vector2Int offset =
            Vector2Int.RoundToInt(other.transform.position - Location.transform.position);

        return offset.ToOrientation();
    }

    private void FindNextTiles(Vector2 direction, int startVal = 0, int endVal = 3)
    {
        HashSet<ConcreteUnit> moveAttackers = new HashSet<ConcreteUnit>();
        for (int i = startVal; i <= endVal; i++)
        {
            Vector2 nextTilePos = new Vector2(CurrentPos.x + (direction.x * i), CurrentPos.y + (direction.y * i));

            Tile nextTile = GridManager.Instance.GetTile(nextTilePos);

            if (nextTile != null && nextTile.IsPassable(faction))
            {
                moveAttackers.UnionWith(nextTile.AttackingUnits);
                if (nextTile.IsTileEmpty())
                {
                    availableMoves[nextTile] = new HashSet<ConcreteUnit>();
                    availableMoves[nextTile].UnionWith(moveAttackers);
                }
            }
            else
            {
                break;
            }
        }
    }

    public bool DoesUnitHaveKey()
    {
        return UnitHasKey;
    }

    public void GiveUnitKey()
    {
        UnitHasKey = true;
        CaptureKey.SetActive(true);
    }

    public void UpdateAppearance()
    {
        GetComponent<SpriteRenderer>().sprite = Appearance;
        tintSprite.sprite = Appearance;
        elementOneRenderer.sprite = elementOne.Sprite();
        elementTwoRenderer.sprite = elementTwo.Sprite();
        SetUnitTeamTint();

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

    public void SetUnitTeamTint()
    {
        if(faction.Equals(PlayerTeam.Faction.Red))
        {
            tintSprite.color = RedTeamTint;
        }
        else
        {
            tintSprite.color = BlueTeamTint;
        }
    }
}
