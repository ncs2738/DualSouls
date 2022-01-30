using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    private ConcreteCard[] oldCards;
    public ConcreteCard[] cards;
    public ConcreteCard[] activeCards;
    public CardAppearance[] cardAppearances;

    public GameObject cardPrefab;
    public PlayerTeam.Faction faction;

    public enum CardHighlightAction
    {
        left = 0,
        right = 1,
        reset = 2,
    }

    void Awake()
    {
        oldCards = (ConcreteCard[]) cards.Clone();
        activeCards = (ConcreteCard[]) cards.Clone();
        ReconnectAppearances();
    }

    public void RemoveCard(ConcreteCard c)
    {
        for (int i = 0; i < 4; i++)
        {
            if (activeCards[i] == c)
            {
                activeCards[i] = null;
                return;
            }
        }
    }

    public void ResetHighlights()
    {
        for (int i = 0; i < 4; i++)
        {
            cardAppearances[i].ResetHighlights();
        }
    }

    public void SetCardHighlight(ConcreteCard c, CardHighlightAction action)
    {
        for (int i = 0; i < 4; i++)
        {
            if (activeCards[i] == c)
            {
                switch(action)
                {
                    case CardHighlightAction.left:
                        cardAppearances[i].SetLeftClicKHighlight();
                        break;

                    case CardHighlightAction.right:
                        cardAppearances[i].SetRightClicKHighlight();
                        break;

                    case CardHighlightAction.reset:
                        cardAppearances[i].ResetHighlights();
                        break;
                }
            }
        }
    }

    public void DiscardAll()
    {
        for (int i = 0; i < 4; i++)
        {
            activeCards[i] = null;
        }
    }

    public void DrawBackToFour()
    {
        for (int i = 0; i < 4; i++)
        {
            if (activeCards[i] == null)
            {
                cards[i].Reshuffle();
                activeCards[i] = cards[i];
            }
        }
    }

    public bool IsHandEmpty()
    {
        bool empty = true;
        for (int i = 0; i < 4; i++)
        {
            if (activeCards[i] != null)
            {
                empty = false;
            }
        }

        return empty;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (faction == GameManager.Instance.activePlayerTurn)
            {
                DrawBackToFour();
            }
        }

        bool shouldRefresh = false;

        // This is a super gross way of doing this, but it's
        // also a quick way to implement it, and this *is* a jam
        for (int i = 0; i < 4; i++)
        {
            if (oldCards[i] != activeCards[i])
            {
                shouldRefresh = true;
            }

            oldCards[i] = activeCards[i];
        }

        if (shouldRefresh)
        {
            ReconnectAppearances();
        }
    }

    void ReconnectAppearances()
    {
        for (int i = 0; i < 4; i++)
        {
            cardAppearances[i].card = activeCards[i];
            cardAppearances[i].Refresh();
        }
    }
}
