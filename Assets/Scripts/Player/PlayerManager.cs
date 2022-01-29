using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public Player redTeam;
    public Player blueTeam;

    [SerializeField]
    private PlayerTeam redTeamObject;
    [SerializeField]
    private PlayerTeam blueTeamObject;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        redTeam = new Player(PlayerTeam.Faction.Red, redTeamObject);
        blueTeam = new Player(PlayerTeam.Faction.Blue, blueTeamObject);

        UnitManager.Instance.SetTeams();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class Player
{
    PlayerTeam.Faction playerFaction;
    private PlayerTeam playerTeam;

    public Player(PlayerTeam.Faction _playerFaction, PlayerTeam _playerTeam)
    {
        playerFaction = _playerFaction;
        playerTeam = _playerTeam;
        playerTeam.Initialize(_playerFaction);
    }

    public PlayerTeam GetTeam()
    {
        return playerTeam;
    }
}
