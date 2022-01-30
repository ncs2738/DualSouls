using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardMouseInteraction : MonoBehaviour,
    IPointerClickHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField]
    private CardAppearance cardAppearance;
    private ConcreteCard card;

    [SerializeField]
    private Hand hand;


    private Vector3 originalParentPosition;
    void Start()
    {
        // weird that we get the card from the appearance and not
        // from some more neutral link, but it's fine for the jam
        card = cardAppearance.card;
        originalParentPosition = transform.parent.parent.position;
    }

    //Detect if a click occurs
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (Good())
        {
            if (CardManager.Instance.SpellType == SpellTypes.Thief
                && pointerEventData.button == PointerEventData.InputButton.Left)
            {
                card.Flip();
                CardManager.Instance.CastSpell(null, null);

            }
            else if (pointerEventData.button == PointerEventData.InputButton.Left)
            {
                Debug.Log("Unit selected for card `" + card.Name + "`");
                SelectUnitOfCard();

                hand.ResetHighlights();
                hand.SetCardHighlight(card, Hand.CardHighlightAction.left);
            }
            else if (pointerEventData.button == PointerEventData.InputButton.Right)
            {
                Debug.Log("Spell selected for card `" + card.Name + "`");
                SelectSpellOfCard();

                hand.ResetHighlights();
                hand.SetCardHighlight(card, Hand.CardHighlightAction.right);
            }
        }
    }

    public void OnPointerEnter(PointerEventData ped)
    {
        transform.parent.parent.position = new Vector3(
            transform.parent.parent.position.x,
            10,
            transform.parent.parent.position.z);
    }

    public void OnPointerExit(PointerEventData ped)
    {
        transform.parent.parent.position = originalParentPosition;
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

        CardManager.Instance.OnSpellCast += () => hand.RemoveCard(card);
    }

    private bool Good()
    {
        if (CardManager.Instance.SpellType == SpellTypes.Thief
            && CardManager.Instance.SpellFace == Faces.FRONT)
        {
            return hand.faction == GameManager.Instance.activePlayerTurn;
        }

        if (CardManager.Instance.SpellType == SpellTypes.Thief
            && CardManager.Instance.SpellFace == Faces.BACK)
        {
            return hand.faction != GameManager.Instance.activePlayerTurn;
        }

        return hand.faction == GameManager.Instance.activePlayerTurn;
    }
}
