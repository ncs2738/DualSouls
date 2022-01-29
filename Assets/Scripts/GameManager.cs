using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PlayerTeam.Faction playerTurn = PlayerTeam.Faction.Red;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            GridManager.Instance.Save();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("loaded");
            GridManager.Instance.Load();
        }
    }

    public PlayerTeam.Faction GetCurrentPlayerTurn()
    {
        return playerTurn;
    }
}
