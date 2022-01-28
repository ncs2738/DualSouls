using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerTeam", menuName = "ScriptableObjects/PlayerTeam")]
public class PlayerTeam : ScriptableObject
{
    [SerializeField]
    private Faction playerFaction;
    [SerializeField]
    private UnitRuntimeSet playerUnits;
    [SerializeField]
    // TODO: Rename this to playerOwnedForts maybe?
    private TileRuntimeSet playerFortress;

    public enum Faction
    {
        None = 0,
        Red = 1,
        Blue = 2,
    }

    public void Initialize()
    {
        playerUnits.Clear();
        playerFortress.Clear();
    }

    public void AddUnit(ConcreteUnit newUnit)
    {
        newUnit.faction = playerFaction;
        playerUnits.Add(newUnit);
    }

    public void RemoveUnit(ConcreteUnit removedUnit, bool destroyUnit = true)
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
            // TODO: The lose condition is different --
            // You lose if an enemy's unit reaches your castle
            // with a key
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
