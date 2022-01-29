using System;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    public event Action OnSpellCast;

    [SerializeField]
    private SpellTypes? spell;
    public SpellTypes? SpellType => spell;
    private Faces spellFace;

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
    }

    public void SetSpellAndFace(SpellTypes spell, Faces face)
    {
        this.spawnCard = null;
        this.spell = spell;
        this.spellFace = face;
    }

    public void ClearOnSpellCast()
    {
        OnSpellCast = null;
    }

    public void CastSpell(Tile tile, ConcreteCard card)
    {
        void WarriorSpell(Faces face, Tile t)
        {
            OnSpellCast();
            this.spell = null;
        }
        void DragonSpell(Faces face, Tile t)
        {
            OnSpellCast();
            this.spell = null;
        }
        void WizardSpell(Faces face, Tile t)
        {
            OnSpellCast();
            this.spell = null;
        }
        void ThiefSpell(Faces face, ConcreteCard c)
        {
            OnSpellCast();
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
    }

    public ConcreteCard GetSpawnCard()
    {
        return spawnCard;
    }

    public bool HasSelectedUnit()
    {
        if (spawnCard != null)
        {
            return true;
        }
        return false;
    }
}
