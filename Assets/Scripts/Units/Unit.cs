using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    PlayerTeam.Faction playerFaction;
    Tile tileLocation;

    public void SetUnitFaction(PlayerTeam.Faction _playerFaction)
    {
        playerFaction = _playerFaction;
        GetComponent<SpriteRenderer>().color =
            playerFaction == PlayerTeam.Faction.Red
                ? Color.red
                : Color.blue;
    }

    public PlayerTeam.Faction GetFaction()
    {
        return playerFaction;
    }

    public void SetTileLocation(Tile _tileLocation)
    {
        tileLocation = _tileLocation;
    }

    public void ClearTile()
    {
        tileLocation.RemoveUnit();
    }

    [System.Serializable]
    public class SaveObject
    {
        public PlayerTeam.Faction playerFaction;
    };

    public SaveObject Save()
    {
        return new SaveObject
        {
            playerFaction = playerFaction,
        };
    }
}
