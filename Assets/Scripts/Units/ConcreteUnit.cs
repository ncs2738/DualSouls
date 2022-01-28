using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcreteUnit : MonoBehaviour
{
    public enum Orientation
    {
        NORTH = 0,
        SOUTH = 1,
        EAST = 2,
        WEST = 4
    }

    public UnitKind unitKind;
    public Sprite Appearance => unitKind.appearance;
    public Array2DEditor.Array2DInt AttackPattern => unitKind.attackPattern;

    public Elements elementOne;
    public Elements elementTwo;

    public Tile location;
    public Orientation orientation;

    public PlayerTeam.Faction faction;

    public void ClearTile()
    {
        location.RemoveUnit();
    }

    public void UpdateAppearance()
    {
        GetComponent<SpriteRenderer>().sprite = Appearance;
    }

    [System.Serializable]
    public class SaveObject
    {
        public UnitKind unitKind;

        public Elements elementOne;
        public Elements elementTwo;

        public Tile location;
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

            location = location,
            orientation = orientation,

            playerFaction = faction,
        };
    }
}
