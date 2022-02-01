using System;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    public event Action OnSpellCast;
    public event Action OnUnitSpawn;

    [SerializeField]
    private GameObject CardHolder;

    [SerializeField]
    private SpellTypes? spell;
    public SpellTypes? SpellType => spell;
    private Faces spellFace;
    public Faces SpellFace => spellFace;

    [SerializeField]
    private ConcreteCard spawnCard;

    private void Awake()
    {
        Instance = this;
    }

    public void SetSpawnCard(ConcreteCard spawnCard)
    {
        this.spawnCard = spawnCard;
        this.spell = null;
        UnitManager.Instance.ShowPlacementTiles(true);
        if(GridManager.Instance.GetSelectedUnit() != null)
        {
            GridManager.Instance.ClearUnitData();
        }
    }

    public void SetSpellAndFace(SpellTypes spell, Faces face)
    {
        if (GridManager.Instance.GetSelectedUnit() != null)
        {
            GridManager.Instance.ClearMovements();
        }

        this.spawnCard = null;
        this.spell = spell;
        this.spellFace = face;
        UnitManager.Instance.ShowPlacementTiles(false);
    }

    public void ClearOnSpellCast()
    {
        OnSpellCast = null;
        UnitManager.Instance.ShowPlacementTiles(false);
    }

    public void CastSpell(Tile tile, ConcreteCard card)
    {
        void WarriorSpell(Faces face, Tile t)
        {
            this.spell = null;
        }
        void DragonSpell(Faces face, Tile t)
        {
            this.spell = null;
        }
        void WizardSpell(Faces face, Tile t)
        {
            this.spell = null;
        }
        void ThiefSpell(Faces face, ConcreteCard c)
        {
            this.spell = null;
        }

        switch (spell)
        {
            case SpellTypes.Warrior:
                WarriorSpell(spellFace, tile);
                break;
            case SpellTypes.Wizard:
                WizardSpell(spellFace, tile);
                break;
            case SpellTypes.Dragon:
                DragonSpell(spellFace, tile);
                break;
            case SpellTypes.Thief:
                ThiefSpell(spellFace, card);
                break;
            default:
                // Do nothing -- spell might even be null.
                break;
        }

        Debug.Log("IMMENSE PAIN");
        OnSpellCast();
    }

    public ConcreteCard GetSpawnCard()
    {
        return spawnCard;
    }

    public bool HasSelectedUnit()
    {
        return spawnCard != null;
    }

    public void ActivateCardHolder(bool status)
    {
        CardHolder.SetActive(status);
    }

    public void ForgetUnit()
    {
        spawnCard = null;
    }
}
