using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer renderer;
    [SerializeField]
    private GameObject tileHighlight;

    private TileType tileType = 0;
    private bool isWalkable = true;

    private bool canSpawnUnitOn = false;

    private PlayerTeam.Faction tileOwner = PlayerTeam.Faction.None;

    private Unit occupiedUnit = null;

    public enum TileType
    {
        Grass = 0,
        Mountain = 1
    }

    private void Start()
    {
        if (tileType == TileType.Grass)
        {
            this.renderer.color = Color.black;
        }
        else
        {
            this.renderer.color = Color.red;
            tileType = TileType.Mountain;
        }
    }

    private void OnMouseEnter()
    {
        tileHighlight.SetActive(true);
    }

    private void OnMouseExit()
    {
        tileHighlight.SetActive(false);
    }

    private void OnMouseOver()
    {
        if (GridManager.Instance.IsMapEditEnabled())
        {
            if (Input.GetMouseButtonDown(0))
            {
                this.renderer.color = Color.black;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                this.renderer.color = Color.red;
                tileType = TileType.Mountain;
            }
        }
    }

    public bool IsTileEmpty()
    {
        if(isWalkable && occupiedUnit == null)
        {
            return true;
        }

        return false;
    }

    public void SetIsWalkable(bool _isWalkable)
    {
        isWalkable = _isWalkable;
    }

    public void SetTileType(TileType type)
    {
        tileType = type;

        if (tileType == TileType.Grass)
        {
            this.renderer.color = Color.black;
        }
        else
        {
            this.renderer.color = Color.red;
        }
    }

    [System.Serializable]
    public class SaveObject
    {
        public TileType tileType;
        public bool isWalkable;
        public float posX;
        public float posY;
        public bool canSpawnUnitOn;
        public PlayerTeam.Faction tileOwner;
        public Unit.SaveObject occupiedUnit;
    }

    public SaveObject Save()
    {
        if(occupiedUnit != null)
        {
            return new SaveObject
            {
                tileType = tileType,
                isWalkable = isWalkable,
                posX = transform.position.x,
                posY = transform.position.y,
                canSpawnUnitOn = canSpawnUnitOn,
                tileOwner = tileOwner,
                occupiedUnit = occupiedUnit.Save()
            };
        }
        else
        {
            return new SaveObject
            {
                tileType = tileType,
                isWalkable = isWalkable,
                posX = transform.position.x,
                posY = transform.position.y,
                canSpawnUnitOn = canSpawnUnitOn,
                tileOwner = tileOwner,
                occupiedUnit = null,
            };
        }
    }
}
