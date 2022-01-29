using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardMouseInteraction : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private CardAppearance cardAppearance;
    private ConcreteCard card;

    [SerializeField]
    private Hand hand;

    void Start()
    {
        // weird that we get the card from the appearance and not
        // from some more neutral link, but it's fine for the jam
        card = cardAppearance.card;
    }

    //Detect if a click occurs
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("Unit selected for card `" + card.Name + "`");
            SelectUnitOfCard();
        } else if (pointerEventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("Spell selected for card `" + card.Name + "`");
            SelectSpellOfCard();
        }
    }

    public void SelectUnitOfCard()
    {
        UnitManager.Instance.SetSpawnCard(card);
    }

    public void SelectSpellOfCard()
    {
        UnitManager.Instance.SetSpellAndFace(card.Spell, card.face);
        UnitManager.Instance.ClearOnSpellCast();
        UnitManager.Instance.OnSpellCast += () => hand.RemoveCard(card);
    }
}
