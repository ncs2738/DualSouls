using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private new SpriteRenderer renderer;
    [SerializeField]
    private GameObject tileHighlight;

    private TileType tileType = 0;
    private bool isWalkable = true;

    private PlayerTeam.Faction tileOwner = PlayerTeam.Faction.None;

    private Unit occupiedUnit = null;

    private TileType maxTypeVal;
    private TileType minTypeVal;

    public enum TileType
    {
        Grass = 0,
        Fortress = 1,
        MainFortress = 2,
        SpawnableTile = 3,
    }

    private void Start()
    {
        maxTypeVal = System.Enum.GetValues(typeof(TileType)).Cast<TileType>().Max();
        minTypeVal = System.Enum.GetValues(typeof(TileType)).Cast<TileType>().Min();

        SetTileType(tileType);
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
                tileType++;

                if (tileType > maxTypeVal)
                {
                    tileType = minTypeVal;
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                tileType--;

                if (tileType < minTypeVal)
                {
                    tileType = maxTypeVal;
                }
            }

            if (Input.GetMouseButtonDown(2))
            {
                if (occupiedUnit == null)
                {
                    UnitManager.Instance.AddUnit(this);
                }
                else
                {
                    Debug.Log(occupiedUnit);
                    UnitManager.Instance.RemoveUnit(occupiedUnit);
                }
            }

            if(occupiedUnit)
            {
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    UnitManager.Instance.SwapUnitTeam(occupiedUnit);
                }
            }


            SetTileType(tileType);
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

    public void OccupyTile(Unit newUnit)
    {
        occupiedUnit = newUnit;
        occupiedUnit.SetTileLocation(this);
    }

    public void RemoveUnit()
    {
        occupiedUnit = null;
    }

    public void SetTileType(TileType type)
    {
        tileType = type;

        switch(tileType)
        {
            case TileType.Grass:
            {
                renderer.color = Color.green;
                isWalkable = true;            
                break;
            }

            case TileType.Fortress:
            {
                renderer.color = Color.gray;
                isWalkable = true;
                break;
            }

            case TileType.MainFortress:
            {
                renderer.color = Color.black;
                isWalkable = true;
                break;
            }

            case TileType.SpawnableTile:
            {
                renderer.color = Color.yellow;
                isWalkable = true;
                break;
            }
        }
    }

    [System.Serializable]
    public class SaveObject
    {
        public TileType tileType;
        public bool isWalkable;
        public float posX;
        public float posY;
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
                tileOwner = tileOwner,
                occupiedUnit = null,
            };
        }
    }
}
