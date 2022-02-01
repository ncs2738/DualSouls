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

    float countdown = 5f;
    float startTime = 5f;

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

        SoundManager.Instance.PlayAttackStartSound();

        if (ActivePlayerUnit.faction.Equals(PlayerTeam.Faction.Red))
        {
            RedPlayer.SetData(ActivePlayerUnit.elementOne, ActivePlayerUnit.elementTwo, ActivePlayerUnit);
            BluePlayer.SetData(EnemyPlayerUnit.elementOne, EnemyPlayerUnit.elementTwo, EnemyPlayerUnit);
        }
        else
        {
            BluePlayer.SetData(ActivePlayerUnit.elementOne, ActivePlayerUnit.elementTwo, ActivePlayerUnit);
            RedPlayer.SetData(EnemyPlayerUnit.elementOne, EnemyPlayerUnit.elementTwo, EnemyPlayerUnit);
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
                    SoundManager.Instance.PlayAttackDrawSound();
                }
                else if (RedPlayer.GetSelectedElement().Beats(BluePlayer.GetSelectedElement()))
                {
                    winner = PlayerTeam.Faction.Red;
                    BattleText.text = "Red wins!";
                    PlayWinSoundEffect();
                }
                else
                {
                    winner = PlayerTeam.Faction.Blue;
                    BattleText.text = "Blue wins!";
                    PlayWinSoundEffect();
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

                if (Input.GetKeyDown(KeyCode.Space) || countdown < 0)
                {
                    DisplayResults = false;
                    countdown = startTime;
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
        switch (CombatType)
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

        hasRedChosen = false;
        hasBlueChosen = false;
        InCombat = false;
        BattleIsDecided = false;
        CombatScreen.SetActive(false);
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
            GameManager.Instance.EndTurn();
        }
        //neither won; it's a draw
        else
        {
            //neither won; kill em all!
            UnitManager.Instance.RemoveUnit(ActivePlayerUnit);
            UnitManager.Instance.RemoveUnit(EnemyPlayerUnit);
            GameManager.Instance.EndTurn();
        }
    }

    private void PlayWinSoundEffect()
    {
        Elements elem; 

        if(ActivePlayerUnit.faction.Equals(winner))
        {
            elem = RedPlayer.GetSelectedElement();
        }
        else
        {
            elem = BluePlayer.GetSelectedElement();
        }

        switch(elem)
        {
            case Elements.FIRE:
            SoundManager.Instance.PlayAttackFireSound();
            break;

            case Elements.WATER:
            SoundManager.Instance.PlayAttackFireSound();
            break;

            case Elements.GRASS:
            SoundManager.Instance.PlayAttackGrassSound();
            break;
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
            //neither won
            GameManager.Instance.EndTurn();
        }
    }

    private void OneSideDefenseOutcomes()
    {
        //Check if the enemy won
        if (EnemyPlayerUnit.faction.Equals(winner))
        {
            //player lost, enemy won: kill player
            UnitManager.Instance.RemoveUnit(ActivePlayerUnit);
            GameManager.Instance.EndTurn();
        }
        else
        {
            //neither won
            GameManager.Instance.EndTurn();
        }
    }
}
