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

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        redTeam.Initialize();
        blueTeam.Initialize();
    }

    public void SetSpawnCard(ConcreteCard spawnCard)
    {
        this.spawnCard = spawnCard;
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
}
