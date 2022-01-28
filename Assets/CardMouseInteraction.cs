using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMouseInteraction : MonoBehaviour
{
    private ConcreteCard card;

    void Start()
    {
        // weird that we get the card from the appearance and not
        // from some more neutral link, but it's fine for the jam
        card = GetComponent<CardAppearance>().card;
    }
    
    public void SelectUnitOfCard()
    {
        Debug.Log("SelectUnitOfCard called");
        UnitManager.Instance.SetSpawnCard(card);
    }
}
