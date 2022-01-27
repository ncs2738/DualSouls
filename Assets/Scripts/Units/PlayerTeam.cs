using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeam : MonoBehaviour
{
    private Faction playerFaction;
    private List<Unit> playerUnits;
    private List<Tile> playerFortress;

    public enum Faction
    {
        None = 0,
        Red = 1,
        Blue = 2,
    }

    public PlayerTeam(Faction _playerFaction)
    {
        playerFaction = _playerFaction;
        playerUnits = new List<Unit>();
        playerFortress = new List<Tile>();
    }

    public void AddUnit(Unit newUnit)
    {
        newUnit.SetUnitFaction(playerFaction);
        playerUnits.Add(newUnit);
    }

    public void RemoveUnit(Unit removedUnit, bool destroyUnit = true)
    {
        if(playerUnits.Contains(removedUnit))
        {
            playerUnits.Remove(removedUnit);

            if(destroyUnit)
            {
                removedUnit.ClearTile();
                Destroy(removedUnit.gameObject);
            }
        }
    }

    public void AddFortress(Tile newFortress)
    {
        playerFortress.Add(newFortress);
    }

    public void RemoveFortress(Tile removedFortress)
    {
        if (playerFortress.Contains(removedFortress))
        {
            playerFortress.Remove(removedFortress);
        }

        if(playerFortress.Count <= 0)
        {
            //gameover code here!
        }
    }

    public void ClearLists()
    {
        for(int i = 0; i < playerUnits.Count; i++)
        {
            Destroy(playerUnits[i].gameObject);
        }

        for (int i = 0; i < playerFortress.Count; i++)
        {
            Destroy(playerFortress[i].gameObject);
        }

        playerUnits.Clear();
        playerFortress.Clear();
    }
}
