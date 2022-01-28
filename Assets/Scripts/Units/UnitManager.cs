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
    private Unit[] units;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        redTeam.Initialize();
        blueTeam.Initialize();
    }

    public void AddUnit(Tile tile, PlayerTeam.Faction faction = PlayerTeam.Faction.Red)
    {
        Unit newUnit = Instantiate(units[0], new Vector3(tile.transform.position.x, tile.transform.position.y, -1), Quaternion.identity);
        if(faction == PlayerTeam.Faction.Red)
        {
            redTeam.AddUnit(newUnit);
            newUnit.SetTileLocation(tile);
        }
        else if(faction == PlayerTeam.Faction.Blue)
        {
            blueTeam.AddUnit(newUnit);
            newUnit.SetTileLocation(tile);
        }
    }

    public void RemoveUnit(Unit removedUnit)
    {
        if (removedUnit.GetFaction() == PlayerTeam.Faction.Red)
        {
            redTeam.RemoveUnit(removedUnit);
        }
        else if (removedUnit.GetFaction() == PlayerTeam.Faction.Blue)
        {
            blueTeam.RemoveUnit(removedUnit);
        }
    }

    public void SwapUnitTeam(Unit removedUnit)
    {
        if (removedUnit.GetFaction() == PlayerTeam.Faction.Red)
        {
            redTeam.RemoveUnit(removedUnit, false);
            blueTeam.AddUnit(removedUnit);
        }
        else if (removedUnit.GetFaction() == PlayerTeam.Faction.Blue)
        {
            blueTeam.RemoveUnit(removedUnit, false);
            redTeam.AddUnit(removedUnit);
        }
    }

    public void ClearLists()
    {
        redTeam.ClearLists();
        blueTeam.ClearLists();
    }
}
