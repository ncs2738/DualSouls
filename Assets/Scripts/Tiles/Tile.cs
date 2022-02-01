using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    [SerializeField]
    private GameObject hoverHighlight;
    [SerializeField]
    private GameObject movementHighlight;
    [SerializeField]
    private GameObject rotationHighlight;

    [SerializeField]
    private GameObject blueAttackHighlight;
    [SerializeField]
    private GameObject redAttackHighlight;

    [SerializeField]
    private GameObject bluePlacementHighlight;
    [SerializeField]
    private GameObject redPlacementHighlight;

    [SerializeField]
    protected TileType tileType;
    [SerializeField]
    protected bool isWalkable = true;

    [SerializeField]
    protected ConcreteUnit occupiedUnit = null;
    public ConcreteUnit OccupiedUnit => occupiedUnit;

    private TileType maxTypeVal;
    private TileType minTypeVal;

    private bool isHovered = false;

    // TODO: HashSet would be better, but List is easier to debug for now.
    [SerializeField]
    protected List<ConcreteUnit> attackingUnits;
    public List<ConcreteUnit> AttackingUnits => attackingUnits;

    public enum TileType
    {
        Grass = 0,
        Fortress = 1,
        PlayerCastle = 2,
        SpawnableTile = 3,
    }

    private void Start()
    {
        attackingUnits = new List<ConcreteUnit>();
        maxTypeVal = System.Enum.GetValues(typeof(TileType)).Cast<TileType>().Max();
        minTypeVal = System.Enum.GetValues(typeof(TileType)).Cast<TileType>().Min();
    }

    protected void OnMouseEnter()
    {
        if (GameManager.Instance.inMainMenu) return;

        //First, check if there is a unit on the tile
        if (occupiedUnit != null)
        {
            //Then check to make sure we've not hovered over unit currently selected...
            if(!GridManager.Instance.IsUnitSelected(occupiedUnit))
            {
                isHovered = true;
                occupiedUnit.OnHovered(true);
            }
        }

        SetHoverHighlight(true);
    }

    protected void OnMouseExit()
    {
        if(isHovered)
        {
            isHovered = false;

            //First, check if there is a unit on the tile
            if (occupiedUnit != null)
            {
                //Then check to make sure we've not hovered over unit currently selected...
                if (!GridManager.Instance.IsUnitSelected(occupiedUnit))
                {
                    occupiedUnit.OnHovered(false);
                }
            }
        }

        //if(!GridManager.Instance.IsTileInMovePool(this))
        {
            SetHoverHighlight(false);
        }
    }

    public void SetHoverHighlight(bool status)
    {
        hoverHighlight.SetActive(status);
    }

    public void SetMovementHighlight(bool status)
    {
        movementHighlight.SetActive(status);
    }

    public void SetRotationHighlight(bool status)
    {
        rotationHighlight.SetActive(status);
    }

    public void SetAttackHighlight(PlayerTeam.Faction playerFaction, bool status)
    {
        if(playerFaction.Equals(PlayerTeam.Faction.Red))
        {
            redAttackHighlight.SetActive(status);
        }
        else
        {
            blueAttackHighlight.SetActive(status);
        }
    }

    public void SetPlacementHighlight(PlayerTeam.Faction playerFaction, bool status)
    {
        if (playerFaction.Equals(PlayerTeam.Faction.Red))
        {
            redPlacementHighlight.SetActive(status);
        }
        else
        {
            bluePlacementHighlight.SetActive(status);
        }
    }

    protected void OnMouseOver()
    {
        if (GameManager.Instance.IsMapEditEnabled())
        {
            EditModeInputs();
        }
        else
        {
            GameModeInputs();
        }
    }

    private void EditModeInputs()
    {
        void AddUnit()
        {
            if (occupiedUnit == null)
            {
                if (CardManager.Instance.HasSelectedUnit())
                {
                    UnitManager.Instance.AddUnit(this, GameManager.Instance.activePlayerTurn);
                }
            }
            else
            {
                Debug.Log(occupiedUnit);
                UnitManager.Instance.RemoveUnit(occupiedUnit);
                this.OnUnitExit();
            }
        }

        void IncrementTileType()
        {
            tileType++;

            /* I'm really not sure why this isn't working... for some reason the maxTypeVal keeps changing? it's quite odd. I'm just gonna hardcode it.
            if (tileType > maxTypeVal) */
            if ((int) tileType > 3)
            {
                tileType = minTypeVal;
            }

            GridManager.Instance.SetTileType(this, tileType);
        }

        void DecrementTileType()
        {
            tileType--;

            if (tileType < minTypeVal)
            {
                tileType = maxTypeVal;
            }

            GridManager.Instance.SetTileType(this, tileType);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (Input.GetKey(KeyCode.U))
            {
                AddUnit();
            }
            else if(occupiedUnit == null)
            {
                IncrementTileType();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            DecrementTileType();
        }

        if (Input.GetMouseButtonDown(2))
        {
            AddUnit();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(tileType.Equals(TileType.Fortress) || tileType.Equals(TileType.PlayerCastle))
            {
                SpawnableTile t = this as SpawnableTile;
                PlayerTeam.Faction newFaction = t.CycleTileOwner();
                if (occupiedUnit && !occupiedUnit.faction.Equals(newFaction))
                {
                    UnitManager.Instance.SwapUnitTeam(occupiedUnit);
                }
            }
            else if (occupiedUnit)
            {
                UnitManager.Instance.SwapUnitTeam(occupiedUnit);
            }
        }
    }

    private void GameModeInputs()
    {
        if (GameManager.Instance.inMainMenu) return;

        if (!CombatManager.Instance.InCombat)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (CombatManager.Instance.SelectingComponent)
                {
                    Debug.Log($"occupiedUnit {occupiedUnit}");
                    if (occupiedUnit != null)
                    {
                        ConcreteUnit selectedUnit = GridManager.Instance.GetSelectedUnit();
                        Debug.Log($"selectedUnit {selectedUnit}");
                        if (selectedUnit.possibleOpponents.Contains(occupiedUnit)
                            && !selectedUnit.Equals(occupiedUnit)
                            && !occupiedUnit.faction.Equals(selectedUnit.faction))
                        {
                            // sometimes the selected unit is an enemy unit
                            if (selectedUnit.faction != GameManager.Instance.activePlayerTurn)
                            {
                                // in those cases, the active unit is flipped (so the combat kind is flipped as well).
                                if (CombatManager.Instance.CombatType == ConcreteUnit.CombatKind.ONE_SIDED_ATTACK)
                                {
                                    CombatManager.Instance.CombatType = ConcreteUnit.CombatKind.ONE_SIDED_DEFENSE;
                                } else if (CombatManager.Instance.CombatType == ConcreteUnit.CombatKind.ONE_SIDED_DEFENSE)
                                {
                                    CombatManager.Instance.CombatType = ConcreteUnit.CombatKind.ONE_SIDED_ATTACK;
                                }

                                CombatManager.Instance.StartCombat(occupiedUnit, selectedUnit);
                            } else
                            {
                                CombatManager.Instance.StartCombat(selectedUnit, occupiedUnit);
                            }
                        }
                    }
                }
                else
                {
                    GridManager.Instance.LeftClickInputHandler(this, occupiedUnit);
                }
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

    public bool IsPassable(PlayerTeam.Faction faction)
    {
        if (isWalkable)
        {
            if((occupiedUnit == null || occupiedUnit.faction.Equals(faction)))
            {
                return true;
            }
            return false;
        }

        return false;
    }

    public void OccupyTile(ConcreteUnit newUnit)
    {
        occupiedUnit = newUnit;
        OnUnitEnter(occupiedUnit);
    }

    protected virtual void OnUnitEnter(ConcreteUnit occupiedUnit)
    {
        return;
    }

    public void RemoveUnit()
    {
        OnUnitExit();
        occupiedUnit = null;
    }

    protected virtual void OnUnitExit()
    {
        return;
    }

    public virtual void OnTurnEnd()
    {
        return;
    }

    public TileType GetTileType()
    {
        return tileType;
    }

    [System.Serializable]
    public class SaveObject
    {
        public TileType tileType;
        public bool isWalkable;
        public float posX;
        public float posY;
        public ConcreteUnit.SaveObject occupiedUnit;
        public SpawnableTile.SaveTileObject spawnableTileData;
    }

    public SaveObject Save()
    {
        SpawnableTile spawnTile = this as SpawnableTile;
        return new SaveObject
        {
            tileType = tileType,
            isWalkable = isWalkable,
            posX = transform.position.x,
            posY = transform.position.y,
            occupiedUnit = occupiedUnit != null ? occupiedUnit.Save() : null,
            spawnableTileData = spawnTile != null ? spawnTile.SaveTile() : null,
        };
    }

    public void LoadUnit(ConcreteUnit.SaveObject loadedUnit)
    {
        UnitManager.Instance.LoadUnit(this, loadedUnit);
    }

    public void LoadSpawnableTile(SpawnableTile.SaveTileObject spawnableTileData)
    {
        SpawnableTile spawnableTile = this as SpawnableTile;

        if (spawnableTile != null)
        {
            spawnableTile.SetTileOwner(spawnableTileData.tileOwner);
            UnitManager.Instance.ClaimNewTile(spawnableTile, spawnableTileData.tileOwner); 
        }
    }

    public void AddAttacker(ConcreteUnit unit)
    {
        if (attackingUnits.Contains(unit))
        {
            Debug.LogWarning($"Attacker `{unit.unitKind.name}` getting added twice to tile at "+
                $"`{this.transform},{this.transform.parent}`. This shouldn't happen.");
        } else
        {
            attackingUnits.Add(unit);
        }
    }

    public void RemoveAttacker(ConcreteUnit unit)
    {
        bool success = attackingUnits.Remove(unit);
        if (!success)
        {
            Debug.LogWarning($"Tried to remove `{unit.unitKind.name}` from attackers of "+
                $"`{this.transform.name},{this.transform.parent.name} when it wasn't there. This shouldn't happen.`");
        }
    }
}
