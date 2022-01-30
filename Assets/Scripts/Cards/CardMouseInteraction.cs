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
        CardManager.Instance.SetSpawnCard(card);
        UnitManager.Instance.ClearOnUnitSpawn();
        if (GameManager.Instance.IsGameStarted())
        {
            UnitManager.Instance.OnUnitSpawn += () => hand.RemoveCard(card);
        }
    }

    public void SelectSpellOfCard()
    {
        CardManager.Instance.SetSpellAndFace(card.Spell, card.face);
        CardManager.Instance.ClearOnSpellCast();
        if (GameManager.Instance.IsGameStarted())
        {
            CardManager.Instance.OnSpellCast += () => hand.RemoveCard(card);
        }
    }
}
