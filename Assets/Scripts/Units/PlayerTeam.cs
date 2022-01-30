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
    private TileRuntimeSet playerCastles;
    [SerializeField]
    private TileRuntimeSet playerOwnedTiles;

    public enum Faction
    {
        None = 0,
        Red = 1,
        Blue = 2,
    }


    public void Initialize(Faction _playerFaction = Faction.None)
    {
        playerUnits = CreateInstance<UnitRuntimeSet>();
        playerCastles = CreateInstance<TileRuntimeSet>();
        playerOwnedTiles = CreateInstance<TileRuntimeSet>();

        if (!playerFaction.Equals(Faction.None))
        {
            playerFaction = _playerFaction;
        }
    }

    public void AddUnit(ConcreteUnit newUnit)
    {
        newUnit.faction = playerFaction;
        newUnit.SetUnitTeamTint();
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

    public void AddCastle(Tile newCastle)
    {
        playerCastles.Add(newCastle);
    }

    public void RemoveCastle(Tile removedCastle)
    {
        if (playerCastles.Contains(removedCastle))
        {
            playerCastles.Remove(removedCastle);
        }

        if(playerCastles.Count <= 0)
        {
            // TODO: The lose condition is different --
            // You lose if an enemy's unit reaches your castle
            // with a key
            //gameover code here!
        }
    }

    public void AddOwnedTile(Tile newTile)
    {
        playerOwnedTiles.Add(newTile);
    }

    public void RemoveOwnedTile(Tile removedTile)
    {
        if (playerOwnedTiles.Contains(removedTile))
        {
            playerOwnedTiles.Remove(removedTile);
        }
    }

    public void ClearLists()
    {
        for(int i = 0; i < playerUnits.Count; i++)
        {
            Destroy(playerUnits[i].gameObject);
        }

        for (int i = 0; i < playerOwnedTiles.Count; i++)
        {
            Destroy(playerOwnedTiles[i].gameObject);
        }

        playerUnits.Clear();
        playerCastles.Clear();
        playerOwnedTiles.Clear();
    }

    public bool DoesPlayerOwnTile(Tile tile)
    {
        return playerOwnedTiles.Contains(tile);
    }

    public List<Tile> GetPlaceableTiles()
    {
        List<Tile> placeableTiles = new List<Tile>();

        for(int i = 0; i < playerOwnedTiles.Count; i++)
        {
            if(playerOwnedTiles[i].IsTileEmpty())
            {
                placeableTiles.Add((Tile)playerOwnedTiles[i]);
            }
        }

        return placeableTiles;
    }

    public void OnTurnEnd()
    {
        for (int i = 0; i < playerUnits.Count; i++)
        {
            playerUnits[i].Location.OnTurnEnd();
        }
    }
}
