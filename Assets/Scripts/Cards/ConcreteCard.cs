using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcreteCard : MonoBehaviour
{
    public CardKind cardKind;
    public Elements elementOne;
    public Elements elementTwo;
    public Faces face;

    public void Flip()
    {
        face = face.Opposite();
    }

    public void Reshuffle()
    {
        List<Elements> randomElements = EnumUtils.RandomElementCombination(2);
        elementOne = randomElements[0];
        elementTwo = randomElements[1];

        face = EnumUtils.RandomFace();
    }
}
