using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    private PlayerTeam redPlayer;
    private PlayerTeam bluePlayer;

    [SerializeField]
    private Unit[] units;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        redPlayer = new PlayerTeam(PlayerTeam.Faction.Red);
        bluePlayer = new PlayerTeam(PlayerTeam.Faction.Blue);
    }

    public void AddUnit(Tile tile, PlayerTeam.Faction faction = PlayerTeam.Faction.Red)
    {
        Unit newUnit = Instantiate(units[0], new Vector3(tile.transform.position.x, tile.transform.position.y, -1), Quaternion.identity);
        if(faction == PlayerTeam.Faction.Red)
        {
            Debug.Log("CALLED?");
            redPlayer.AddUnit(newUnit);
            tile.OccupyTile(newUnit);
        }
        else if(faction == PlayerTeam.Faction.Blue)
        {
            bluePlayer.AddUnit(newUnit);
            tile.OccupyTile(newUnit);
        }
    }

    public void RemoveUnit(Unit removedUnit)
    {
        if (removedUnit.GetFaction() == PlayerTeam.Faction.Red)
        {
            redPlayer.RemoveUnit(removedUnit);
        }
        else if (removedUnit.GetFaction() == PlayerTeam.Faction.Blue)
        {
            bluePlayer.RemoveUnit(removedUnit);
        }
    }

    public void SwapUnitTeam(Unit removedUnit)
    {
        if (removedUnit.GetFaction() == PlayerTeam.Faction.Red)
        {
            redPlayer.RemoveUnit(removedUnit, false);
            bluePlayer.AddUnit(removedUnit);
        }
        else if (removedUnit.GetFaction() == PlayerTeam.Faction.Blue)
        {
            bluePlayer.RemoveUnit(removedUnit, false);
            redPlayer.AddUnit(removedUnit);
        }
    }

    public void ClearLists()
    {
        redPlayer.ClearLists();
        bluePlayer.ClearLists();
    }
}
