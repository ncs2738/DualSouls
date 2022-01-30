using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PlayerTeam.Faction activePlayerTurn = PlayerTeam.Faction.Red;

    private List<PlayerTeam.Faction> playerTurns;

    [SerializeField]
    private Image RedTeamBG;

    [SerializeField]
    private Image BlueTeamBG;

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
            if(!isGameStarted)
            {
                StartGame();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                StartGame();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                EndTurn();
            }
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
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
        ShowTeam();
    }

    private void ShowTeam()
    {
        BlueTeamBG.enabled = activePlayerTurn.Equals(PlayerTeam.Faction.Blue);
        Debug.Log($"blueteam: `{BlueTeamBG.enabled}`");
        RedTeamBG.enabled = activePlayerTurn.Equals(PlayerTeam.Faction.Red);
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

        ShowTeam();

        activePlayerTurn = playerTurns[currentPlayer];
    }

    public void GameOver()
    {
        //TODO - add game over <3
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
