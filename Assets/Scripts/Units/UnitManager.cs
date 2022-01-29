using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    [SerializeField]
    private PlayerTeam redTeam;
    [SerializeField]
    private PlayerTeam blueTeam;

    [SerializeField]
    private GameObject unitPrefab;

    [SerializeField]
    private ConcreteCard spawnCard;

    [SerializeField]
    private SpellTypes? spell;
    public SpellTypes? SpellType => spell;
    private Faces spellFace;

    [SerializeField]
    private List<UnitKind> UnitTypes;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        redTeam.Initialize(PlayerTeam.Faction.Red);
        blueTeam.Initialize(PlayerTeam.Faction.Blue);
    }

    public void SetSpawnCard(ConcreteCard spawnCard)
    {
        this.spawnCard = spawnCard;
        this.spell = null;
    }

    public void SetSpellAndFace(SpellTypes spell, Faces face)
    {
        this.spawnCard = null;
        this.spell = spell;
        this.spellFace = face;
    }

    public void CastSpell(Tile tile, ConcreteCard card)
    {
        void WarriorSpell(Faces face, Tile t)
        {
            ;
        }
        void DragonSpell(Faces face, Tile t)
        {
            ;
        }
        void WizardSpell(Faces face, Tile t)
        {
            ;
        }
        void ThiefSpell(Faces face, ConcreteCard c)
        {
            ;
        }

        switch (spell)
        {
            case SpellTypes.Warrior:
                WarriorSpell(spellFace, tile);
                break;
            case SpellTypes.Wizard:
                WizardSpell(spellFace, tile);
                break;
            case SpellTypes.Dragon:
                DragonSpell(spellFace, tile);
                break;
            case SpellTypes.Thief:
                ThiefSpell(spellFace, card);
                break;
            default:
                // Do nothing -- spell might even be null.
                break;
        }
    }
    
    public void AddUnit(Tile tile, PlayerTeam.Faction faction = PlayerTeam.Faction.Red)
    {
        ConcreteUnit newUnit = Instantiate(unitPrefab, new Vector3(tile.transform.position.x, tile.transform.position.y, -1), Quaternion.identity)
            .GetComponent<ConcreteUnit>();

        newUnit.unitKind = spawnCard.UnitKind;
        newUnit.elementOne = spawnCard.elementOne;
        newUnit.elementTwo = spawnCard.elementTwo;
        newUnit.Location = tile;
        newUnit.orientation = faction == PlayerTeam.Faction.Red
            ? ConcreteUnit.Orientation.EAST
            : ConcreteUnit.Orientation.WEST;
        newUnit.faction = faction;

        newUnit.UpdateAppearance();

        if (faction == PlayerTeam.Faction.Red)
        {
            redTeam.AddUnit(newUnit);
            tile.OccupyTile(newUnit);
        }
        else if(faction == PlayerTeam.Faction.Blue)
        {
            blueTeam.AddUnit(newUnit);
            tile.OccupyTile(newUnit);
        }
    }

    public void LoadUnit(Tile tile, ConcreteUnit.SaveObject unitData)
    {
        ConcreteUnit newUnit = Instantiate(unitPrefab, new Vector3(tile.transform.position.x, tile.transform.position.y, -1), Quaternion.identity)
            .GetComponent<ConcreteUnit>();

        //newUnit.unitKind = UnitTypes[(int) unitData.unitType];
        newUnit.unitKind = unitData.unitKind;
        newUnit.elementOne = unitData.elementOne;
        newUnit.elementTwo = unitData.elementTwo;
        newUnit.Location = tile;
        newUnit.orientation = unitData.playerFaction == PlayerTeam.Faction.Red
            ? ConcreteUnit.Orientation.EAST
            : ConcreteUnit.Orientation.WEST;
        newUnit.faction = unitData.playerFaction;

        newUnit.UpdateAppearance();

        if (unitData.playerFaction == PlayerTeam.Faction.Red)
        {
            redTeam.AddUnit(newUnit);
            tile.OccupyTile(newUnit);
        }
        else if (unitData.playerFaction == PlayerTeam.Faction.Blue)
        {
            blueTeam.AddUnit(newUnit);
            tile.OccupyTile(newUnit);
        }
    }

    public void RemoveUnit(ConcreteUnit removedUnit)
    {
        if (removedUnit.faction == PlayerTeam.Faction.Red)
        {
            redTeam.RemoveUnit(removedUnit);
        }
        else if (removedUnit.faction == PlayerTeam.Faction.Blue)
        {
            blueTeam.RemoveUnit(removedUnit);
        }
    }

    public void SwapUnitTeam(ConcreteUnit unit)
    {
        if (unit.faction == PlayerTeam.Faction.Red)
        {
            redTeam.RemoveUnit(unit, false);
            blueTeam.AddUnit(unit);
        }
        else if (unit.faction == PlayerTeam.Faction.Blue)
        {
            blueTeam.RemoveUnit(unit, false);
            redTeam.AddUnit(unit);
        }
    }

    public void ClearLists()
    {
        redTeam.ClearLists();
        blueTeam.ClearLists();
    }

    public bool HasSelectedUnit()
    {
        if(spawnCard != null)
        {
            return true;
        }
        return false;
    }
}
