using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ConcreteCard : MonoBehaviour
{
    public CardKind cardKind;
    public string Name => cardKind.name;
    public Sprite Artwork => cardKind.artwork;
    public UnitKind UnitKind => cardKind.unit;
    public string FrontDescription => cardKind.frontDescription;
    public string BackDescription => cardKind.backDescription;
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
