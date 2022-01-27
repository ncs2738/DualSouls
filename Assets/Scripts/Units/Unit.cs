using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    PlayerTeam.Faction playerFaction;

    public PlayerTeam.Faction GetFaction()
    {
        return playerFaction;
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
