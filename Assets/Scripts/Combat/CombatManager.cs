using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    public ConcreteUnit.CombatKind CombatType;

    public bool InCombat = false;
    public bool SelectingComponent = false;
    public bool BattleIsDecided = false;
    private bool DisplayResults = false;

    [SerializeField]
    private GameObject CombatScreen;

    [SerializeField]
    private TMP_Text BattleText;

    [SerializeField]
    private CombatPlayer RedPlayer;
    [SerializeField]
    private CombatPlayer BluePlayer;

    private bool hasRedChosen = false;
    private bool hasBlueChosen = false;

    private PlayerTeam.Faction winner;
    private ConcreteUnit ActivePlayerUnit;
    private ConcreteUnit EnemyPlayerUnit;

    float countdown = 10f;
    float startTime = 10f;

    private void Awake()
    {
        Instance = this;
    }

    public void StartCombat(ConcreteUnit _ActivePlayerUnit, ConcreteUnit _EnemyPlayerUnit)
    {
        SelectingComponent = false;
        CardManager.Instance.ActivateCardHolder(false);

        ActivePlayerUnit = _ActivePlayerUnit;
        EnemyPlayerUnit = _EnemyPlayerUnit;

        if (ActivePlayerUnit.faction.Equals(PlayerTeam.Faction.Red))
        {
            RedPlayer.SetData(ActivePlayerUnit.elementOne, ActivePlayerUnit.elementTwo);
            BluePlayer.SetData(EnemyPlayerUnit.elementOne, EnemyPlayerUnit.elementTwo);
        }
        else
        {
            BluePlayer.SetData(ActivePlayerUnit.elementOne, ActivePlayerUnit.elementTwo);
            RedPlayer.SetData(EnemyPlayerUnit.elementOne, EnemyPlayerUnit.elementTwo);
        }

        CombatScreen.SetActive(true);
        BattleText.text = "Battle!";

        foreach (ConcreteUnit possibleOpponent in ActivePlayerUnit.possibleOpponents)
        {
            possibleOpponent.Location.SetAttackHighlight(ActivePlayerUnit.faction, false);
            possibleOpponent.SetDuelCrosshair(false);
            possibleOpponent.SetOneSidedCrosshair(false);
        }

        ActivePlayerUnit.Location.SetMovementHighlight(false);
        ActivePlayerUnit.SetDuelCrosshair(false);
        ActivePlayerUnit.SetOneSidedCrosshair(false);

        //EnemyPlayerUnit.SetDuelCrosshair(false);
        //EnemyPlayerUnit.SetOneSidedCrosshair(false);

        InCombat = true;
    }

    private void Update()
    {
        if(InCombat && !BattleIsDecided)
        {
            if(!hasRedChosen)
            {
                if(Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
                {
                    RedPlayer.SetElement(CombatPlayer.SelectedElement.left);
                    hasRedChosen = true;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                {
                    RedPlayer.SetElement(CombatPlayer.SelectedElement.right);
                    hasRedChosen = true;
                }
            }

            if (!hasBlueChosen)
            {
                if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
                {
                    BluePlayer.SetElement(CombatPlayer.SelectedElement.left);
                    hasBlueChosen = true;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9))
                {
                    BluePlayer.SetElement(CombatPlayer.SelectedElement.right);
                    hasBlueChosen = true;
                }
            }

            if(hasRedChosen && hasBlueChosen)
            {
                if(RedPlayer.GetSelectedElement().Equals(BluePlayer.GetSelectedElement()))
                {
                    winner = PlayerTeam.Faction.None;
                    BattleText.text = "Draw!";
                }
                else if (RedPlayer.GetSelectedElement().Beats(BluePlayer.GetSelectedElement()))
                {
                    winner = PlayerTeam.Faction.Red;
                    BattleText.text = "Red wins!";
                }
                else
                {
                    winner = PlayerTeam.Faction.Blue;
                    BattleText.text = "Blue wins!";
                }

                BattleIsDecided = true;
                DisplayResults = true;
            }
        }

        if(BattleIsDecided)
        {
            if(DisplayResults)
            {
                countdown -= Time.deltaTime;

                if (Input.GetKeyDown(KeyCode.Space) || Time.deltaTime < 0f)
                {
                    countdown = startTime;
                    DisplayResults = false;
                }
            }
            else
            {
                OnCombatEnd();
            }
        }
    }

    private void OnCombatEnd()
    {
        switch(CombatType)
        {
            case ConcreteUnit.CombatKind.DUEL:
                DuelOutComes();
                break;
            case ConcreteUnit.CombatKind.ONE_SIDED_ATTACK:
                OneSideAttackdOutcomes();
                break;
            case ConcreteUnit.CombatKind.ONE_SIDED_DEFENSE:
                OneSideDefenseOutcomes();
                break;
        }

        InCombat = false;
        BattleIsDecided = false;
        CardManager.Instance.ActivateCardHolder(true);
    }

    private void DuelOutComes()
    {
        //Check if the player won
        if (ActivePlayerUnit.faction.Equals(winner))
        {
            //player won, enemy lost: kill enemy
            UnitManager.Instance.RemoveUnit(EnemyPlayerUnit);
        }
        //check if the enemy won
        else if (EnemyPlayerUnit.faction.Equals(winner))
        {
            //player lost, enemy won: kill player
            UnitManager.Instance.RemoveUnit(ActivePlayerUnit);
        }
        //neither won; it's a draw
        else
        {
            //neither won; kill em all!
            UnitManager.Instance.RemoveUnit(ActivePlayerUnit);
            UnitManager.Instance.RemoveUnit(EnemyPlayerUnit);
        }
    }

    private void OneSideAttackdOutcomes()
    {
        //Check if the player won
        if (ActivePlayerUnit.faction.Equals(winner))
        {
            //player won, enemy lost: kill enemy
            UnitManager.Instance.RemoveUnit(EnemyPlayerUnit);
        }
        else
        {
            //neither won; do nothing
        }
    }

    private void OneSideDefenseOutcomes()
    {
        //Check if the enemy won
        if (EnemyPlayerUnit.faction.Equals(winner))
        {
            //player lost, enemy won: kill player
            UnitManager.Instance.RemoveUnit(ActivePlayerUnit);
        }
        else
        {
            //neither won; do nothing
        }
    }
}
