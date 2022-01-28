using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private PlayerTeam.Faction playerFaction;
    private Tile tileLocation;

    private List<Tile> availableMoves;
    private Vector2 currentPos;

    Vector2 leftVector = new Vector2(-1, 0);
    Vector2 rightVector = new Vector2(1, 0);
    Vector2 downVector = new Vector2(0, -1);
    Vector2 upVector = new Vector2(0, 1);

    Vector2 upperLeftDiagonalVector = new Vector2(-1, 1);
    Vector2 upperRightDiagonalVector = new Vector2(1, 1);
    Vector2 lowerLeftDiagonalVector = new Vector2(-1, -1);
    Vector2 lowerRightDiagonalVector = new Vector2(1, -1);

    public enum UnitMoveTypes
    {
        Warrior = 0,
        Wizard = 1,
        Dragon = 2,
    }

    private void Start()
    {
        availableMoves = new List<Tile>();
    }

    public void SetUnitFaction(PlayerTeam.Faction _playerFaction)
    {
        playerFaction = _playerFaction;
    }

    public PlayerTeam.Faction GetFaction()
    {
        return playerFaction;
    }

    public void SetTileLocation(Tile _tileLocation)
    {
        tileLocation = _tileLocation;
        currentPos = tileLocation.transform.position;
        tileLocation.OccupyTile(this);
    }

    public void ClearTile()
    {
        tileLocation.RemoveUnit();
    }

    public void MoveUnit(Tile _tileLocation)
    {
        ClearTile();
        SetTileLocation(_tileLocation);
        transform.position = new Vector3(currentPos.x, currentPos.y, -1);
    }

    public void ShowAvailableMoves(bool status)
    {
        for (int i = 0; i < availableMoves.Count; i++)
        {
            availableMoves[i].SetTileHighlight(status);
        }
    }

    public List<Tile> GetAvailableMoves(UnitMoveTypes moveType)
    {
        if(moveType.Equals(UnitMoveTypes.Dragon))
        {
            SetDragonMoves();
            ShowAvailableMoves(true);
            return availableMoves;
        }
        else if (moveType.Equals(UnitMoveTypes.Wizard))
        {
            SetWizardMoves();
            ShowAvailableMoves(true);
            return availableMoves;
        }

        return null;
    }

    public bool IsTileInMovePool(Tile selectedTile)
    {
        if(availableMoves.Contains(selectedTile))
        {
            return true;
        }

        return false;
    }

    private void SetDragonMoves()
    {
        availableMoves.Clear();

        FindNextTiles(leftVector);
        FindNextTiles(rightVector);
        FindNextTiles(upVector);
        FindNextTiles(downVector);
    }

    private void SetWizardMoves()
    {
        availableMoves.Clear();

        FindNextTiles(upperLeftDiagonalVector, 3);
        FindNextTiles(upperRightDiagonalVector, 3);
        FindNextTiles(lowerLeftDiagonalVector, 3);
        FindNextTiles(lowerRightDiagonalVector, 3);
    }


    private void FindNextTiles(Vector2 direction, int startVal = 0, int endVal = 3)
    {
        for (int i = startVal; i <= endVal; i++)
        {
            Vector2 nextTilePos = new Vector2(currentPos.x + (direction.x * i), currentPos.y + (direction.y * i));

            Tile nextTile = GridManager.Instance.GetTile(nextTilePos);

            if (nextTile != null && nextTile.IsPassable(playerFaction))
            {
                if (nextTile.IsTileEmpty())
                {
                    availableMoves.Add(nextTile);
                }
            }
            else
            {
                break;
            }
        }
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
