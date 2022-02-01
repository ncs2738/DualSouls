using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PlayerTeam.Faction activePlayerTurn;

    private List<PlayerTeam.Faction> playerTurns;

    [SerializeField]
    private Image RedTeamBG;

    [SerializeField]
    private Image BlueTeamBG;

    [SerializeField]
    private bool MapEditModeEnabled = true;
    [SerializeField]
    private bool isGameStarted = false;
    [SerializeField]
    public bool inMainMenu = true;

    public Hand redHand;
    public Hand blueHand;

    private void Awake()
    {
        Instance = this;
        playerTurns = new List<PlayerTeam.Faction>();
    }

    private void Start()
    {
        SoundManager.Instance.PlayMenuTheme();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitGame();
        }
        if (inMainMenu) return;
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
        BlueTeamBG.enabled = false;
        MapEditModeEnabled = false;
        isGameStarted = true;
        SoundManager.Instance.PlayGameTheme();

        activePlayerTurn = PlayerTeam.Faction.Red;
        ShowTeam();
    }

    private void ShowTeam()
    {
        BlueTeamBG.enabled = activePlayerTurn.Equals(PlayerTeam.Faction.Blue);
        RedTeamBG.enabled = activePlayerTurn.Equals(PlayerTeam.Faction.Red);
    }

    public void EndTurn()
    {
        UnitManager.Instance.OnTurnEnd();

        if (activePlayerTurn == PlayerTeam.Faction.Red)
        {
            redHand.DrawBackToFour();
            activePlayerTurn = PlayerTeam.Faction.Blue;
        } else
        {
            blueHand.DrawBackToFour();
            activePlayerTurn = PlayerTeam.Faction.Red;
        }

        GridManager.Instance.OnTurnEnd();

        ShowTeam();
    }

    public void GameOver()
    {
        //TODO - add game over
        //SoundManager.Instance.PlayWinGame();
        RestartGame();
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

    public void DisableMainMenu()
    {
        inMainMenu = false;
    }

    public void EnableMainManu()
    {
        inMainMenu = true;
    }


    public void ExitGame()
    {
        Application.Quit();
    }
}
