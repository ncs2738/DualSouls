using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ConcreteCard : MonoBehaviour
{
    public List<CardKind> cardKinds;
    public CardKind cardKind;
    public string Name => cardKind.name;
    public Sprite Artwork => cardKind.artwork;
    public UnitKind UnitKind => cardKind.unit;
    public string FrontDescription => cardKind.frontDescription;
    public string BackDescription => cardKind.backDescription;
    public SpellTypes Spell => cardKind.spell;
    public Elements elementOne;
    public Elements elementTwo;
    public Faces face;

    public void Flip()
    {
        face = face.Opposite();

        if (OnModified != null)
        {
            OnModified();
        }
    }

    public void Reshuffle()
    {
        cardKind = cardKinds[UnityEngine.Random.Range(0, cardKinds.Count)];
        ReshuffleNonKindParts();
    }

    public void ReshuffleNonKindParts()
    {
        List<Elements> randomElements = EnumUtils.RandomElementCombination(2);
        elementOne = randomElements[0];
        elementTwo = randomElements[1];

        face = EnumUtils.RandomFace();

        if (OnModified != null)
        {
            OnModified();
        }
    }

    public event Action OnModified; 
}
