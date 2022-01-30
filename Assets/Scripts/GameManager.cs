using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PlayerTeam.Faction activePlayerTurn = PlayerTeam.Faction.Red;

    private List<PlayerTeam.Faction> playerTurns;

    [SerializeField]
    private bool MapEditModeEnabled = true;
    [SerializeField]
    private bool isGameStarted = false;

    private int currentPlayer;

    private void Awake()
    {
        Instance = this;
        playerTurns = new List<PlayerTeam.Faction>();
    }

    // Update is called once per frame
    void Update()
    {
        if(MapEditModeEnabled)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                GridManager.Instance.Save();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                Debug.Log("loaded");
                GridManager.Instance.Load();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                StartGame();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                EndTurn();
            }
        }
    }

    public void AddPlayer(PlayerTeam.Faction playerTeam)
    {
        playerTurns.Add(playerTeam);
    }

    public void StartGame()
    {
        MapEditModeEnabled = false;
        isGameStarted = true;

        //currentPlayer = Random.Range(0, playerTurns.Count);
        currentPlayer = 0;
        activePlayerTurn = playerTurns[currentPlayer];
    }

    public void HasTurnEnded()
    {
        //TODO
    }

    public void EndTurn()
    {
        UnitManager.Instance.OnTurnEnd();

        currentPlayer++;
        if(currentPlayer >= playerTurns.Count)
        {
            currentPlayer = 0;
        }

        activePlayerTurn = playerTurns[currentPlayer];
    }

    public void GameOver()
    {
        //TODO - add game over <3
    }

    public void RestartGame()
    {
        //TODO <3!
    }

    public bool IsMapEditEnabled()
    {
        return MapEditModeEnabled;
    }

    public bool IsGameStarted()
    {
        return isGameStarted;
    }
}
