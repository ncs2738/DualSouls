using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnableTile : Tile
{
    [SerializeField]
    private Dictionary<Vector2, Tile> neighboringTiles;

    [SerializeField]
    private PlayerTeam.Faction tileOwner = PlayerTeam.Faction.None;

    [SerializeField]
    private GameObject BlueTeamColorTint;
    [SerializeField]
    private GameObject RedTeamColorTint;

    private PlayerTeam.Faction occupiedUnitFaction;

    private void Start()
    {
        neighboringTiles = new Dictionary<Vector2, Tile>();

        SetTeamColor();

        if(!tileOwner.Equals(PlayerTeam.Faction.None))
        {
            UnitManager.Instance.AddNewTile(this, tileOwner);
        }
    }

    private void GetNeighboringTiles()
    {
        neighboringTiles.Clear();

        Vector2 currentPos = transform.position;

        GetTile(currentPos.x + 1, currentPos.y);
        GetTile(currentPos.x - 1, currentPos.y);
        GetTile(currentPos.x, currentPos.y + 1);
        GetTile(currentPos.x, currentPos.y - 1);

        GetTile(currentPos.x + 1, currentPos.y + 1);
        GetTile(currentPos.x + 1, currentPos.y - 1);
        GetTile(currentPos.x - 1, currentPos.y + 1);
        GetTile(currentPos.x - 1, currentPos.y - 1);
    }

    private void GetTile(float x, float y)
    {
        Tile t = GridManager.Instance.GetTile(new Vector2(x, y));

        if (t != null)
        {
            neighboringTiles.Add(t.transform.position, t);
        }
    }

    public void ClaimTile(PlayerTeam.Faction newOwner)
    {
        SetTileOwner(newOwner);
        UnitManager.Instance.ClaimNewTile(this, newOwner);

        GetNeighboringTiles();

        foreach (Tile tile in neighboringTiles.Values)
        {
            TileType type = tile.GetTileType();
            if (type.Equals(TileType.SpawnableTile))
            {
                UnitManager.Instance.ClaimNewTile(tile, newOwner);
                SpawnableTile t = tile as SpawnableTile;
                t.SetTileOwner(newOwner);
            }
        }
    }

    public void SetTileOwner(PlayerTeam.Faction newOwner)
    {
        tileOwner = newOwner;
        SetTeamColor();
    }

    private void SetTeamColor()
    {
        switch(tileOwner)
        {
            case PlayerTeam.Faction.Red:
                RedTeamColorTint.SetActive(true);
                BlueTeamColorTint.SetActive(false);
                break;

            case PlayerTeam.Faction.Blue:
                RedTeamColorTint.SetActive(false);
                BlueTeamColorTint.SetActive(true);
                break;

            default:
                RedTeamColorTint.SetActive(false);
                BlueTeamColorTint.SetActive(false);
                break;
        }
    }

    protected override void OnUnitEnter(ConcreteUnit occupiedUnit)
    {
        occupiedUnitFaction = occupiedUnit.faction;

        if (tileType.Equals(TileType.Fortress))
        {
            if(tileOwner.Equals(occupiedUnit.faction))
            {
                occupiedUnit.GiveUnitKey();
            }
            else
            {
                ClaimTile(occupiedUnit.faction);
            }
        }
        else if (tileType.Equals(TileType.PlayerCastle))
        {
            if(occupiedUnit.DoesUnitHaveKey())
            {
                ClaimTile(occupiedUnit.faction);
                //remove from list
            }
        }
    }

    protected override void OnUnitExit()
    {
        if(!tileOwner.Equals(PlayerTeam.Faction.None))
        {
            occupiedUnitFaction = PlayerTeam.Faction.None;
            UnitManager.Instance.AddNewTile(this, occupiedUnitFaction);
        }
    }

    public PlayerTeam.Faction CycleTileOwner()
    {
        PlayerTeam.Faction newFaction;
        switch(tileOwner)
        {
            case PlayerTeam.Faction.None:
                newFaction = PlayerTeam.Faction.Red;
                break;

            case PlayerTeam.Faction.Red:
                newFaction = PlayerTeam.Faction.Blue;
                break;

            default:
                newFaction = PlayerTeam.Faction.None;
                break;
        }

        ClaimTile(newFaction);

        return  newFaction;
    }

    [System.Serializable]
    public class SaveTileObject
    {
        public PlayerTeam.Faction tileOwner;
    }

    public SaveTileObject SaveTile()
    {
        return new SaveTileObject
        {
            tileOwner = tileOwner,
        };
    }
}
