using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnableTile : Tile
{
    private List<Tile> neighboringTiles;

    [SerializeField]
    private PlayerTeam.Faction tileOwner = PlayerTeam.Faction.None;

    private void Start()
    {
        neighboringTiles = new List<Tile>();

        if (tileType.Equals(TileType.Fortress) || tileType.Equals(TileType.PlayerCastle))
        {
            Vector2 currentPos = transform.position;

            GetNeighboringTile(currentPos.x + 1, currentPos.y);
            GetNeighboringTile(currentPos.x - 1, currentPos.y);
            GetNeighboringTile(currentPos.x, currentPos.y + 1);
            GetNeighboringTile(currentPos.x, currentPos.y - 1);

            GetNeighboringTile(currentPos.x + 1, currentPos.y + 1);
            GetNeighboringTile(currentPos.x + 1, currentPos.y - 1);
            GetNeighboringTile(currentPos.x - 1, currentPos.y + 1);
            GetNeighboringTile(currentPos.x - 1, currentPos.y - 1);
        }
    }

    private void GetNeighboringTile(float x, float y)
    {
        Tile t = GridManager.Instance.GetTile(new Vector2(x, y));

        if(t != null)
        {
            neighboringTiles.Add(t);
        }
    }

    public void SetNeighboringTiles()
    {
        for(int i = 0; i < neighboringTiles.Count; i++)
        {
            TileType type = neighboringTiles[i].GetTileType();
            if (!type.Equals(TileType.Fortress) || !type.Equals(TileType.PlayerCastle) || !type.Equals(TileType.SpawnableTile))
            {
                //neighboringTiles[i].SetTileOwner(tileOwner);
            }
        }
    }

    public void ResetNeighboringTiles()
    {
        for (int i = 0; i < neighboringTiles.Count; i++)
        {
            TileType type = neighboringTiles[i].GetTileType();
            if (type.Equals(TileType.SpawnableTile))
            {
                //neighboringTiles[i].
            }
        }
    }

    public void ClaimFortress(PlayerTeam.Faction newOwner)
    {
        for (int i = 0; i < neighboringTiles.Count; i++)
        {
            TileType type = neighboringTiles[i].GetTileType();
            if (type.Equals(TileType.SpawnableTile))
            {
                SetTileOwner(newOwner);
            }
        }
    }

    public void SetTileOwner(PlayerTeam.Faction newOwner)
    {
        tileOwner = newOwner;

        if(neighboringTiles.Count > 0)
        {
            //SetNeighboringTiles();
        }
    }

    [System.Serializable]
    public class SaveObject
    {
        public PlayerTeam.Faction tileOwner;
    }

    public SaveObject Save()
    {
        return new SaveObject
        {
            tileOwner = tileOwner,
        };
    }
}
