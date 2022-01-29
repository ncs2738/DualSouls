using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnableTile : Tile
{
    [SerializeField]
    private List<Tile> neighboringTiles;

    [SerializeField]
    private PlayerTeam.Faction tileOwner = PlayerTeam.Faction.None;

    private bool isCapturable = false;

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

            isCapturable = true;
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
        List<Tile> newNeighbors = new List<Tile>();

        int neighborCount = neighboringTiles.Count;

        for (int i = 0; i < neighborCount; i++)
        {
            TileType type = neighboringTiles[i].GetTileType();
            if (type.Equals(TileType.Grass))
            {
                Tile removedTile = neighboringTiles[i];
                Vector2 removedTilePos = removedTile.transform.position;
                neighboringTiles.RemoveAt(i);
                GridManager.Instance.SetTileType(removedTile, TileType.Grass);

                GetNeighboringTile(removedTilePos.x, removedTilePos.y);

                SetNeighboringTileOwner(neighboringTiles[i], tileOwner);
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

    public void SetTileOwner(PlayerTeam.Faction newOwner)
    {
        tileOwner = newOwner;

        if(neighboringTiles.Count > 0)
        {
            for (int i = 0; i < neighboringTiles.Count; i++)
            {
                SetNeighboringTileOwner(neighboringTiles[i], newOwner);
            }
        }
    }

    public void SetNeighboringTileOwner(Tile tile, PlayerTeam.Faction newOwner)
    {
        TileType type = tile.GetTileType();
        if (type.Equals(TileType.SpawnableTile))
        {
            SpawnableTile t = tile as SpawnableTile;
            t.SetTileOwner(newOwner);
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
