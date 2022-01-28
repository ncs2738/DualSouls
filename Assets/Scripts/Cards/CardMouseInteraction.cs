using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardMouseInteraction : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private CardAppearance cardAppearance;
    private ConcreteCard card;

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
            SelectUnitOfCard();
        } else if (pointerEventData.button == PointerEventData.InputButton.Right)
        {
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
    }
}
