using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    private ConcreteCard[] oldCards;
    public ConcreteCard[] cards;
    public CardAppearance[] cardAppearances;

    void Awake()
    {
        oldCards = (ConcreteCard[]) cards.Clone();
        ReconnectAppearances();
    }

    private void Update()
    {
        bool shouldRefresh = false;

        // This is a super gross way of doing this, but it's
        // also a quick way to implement it, and this *is* a jam
        for (int i = 0; i < 4; i++)
        {
            if (oldCards[i] != cards[i])
            {
                shouldRefresh = true;
            }

            oldCards[i] = cards[i];
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
            cardAppearances[i].card = cards[i];
            cardAppearances[i].Refresh();
        }
    }
}
