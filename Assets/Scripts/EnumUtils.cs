using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnumUtils
{
    public static bool Beats(this Elements a, Elements b)
    {
        return (a == Elements.FIRE) && (b == Elements.GRASS)
            || (a == Elements.GRASS) && (b == Elements.WATER)
            || (a == Elements.WATER) && (b == Elements.FIRE);
    }

    public static bool IsBeatenBy(this Elements a, Elements b)
    {
        return !a.Beats(b) && a != b;
    }

    public static Faces Opposite(this Faces f)
    {
        return f == Faces.FRONT ? Faces.BACK : Faces.FRONT;
    }

    /// <summary>
    /// Gets a combination of k random elements. Usually k = 1 or 2,
    /// calling with k=3 is useless
    /// </summary>
    /// <param name="k">How many random elements to take</param>
    /// <returns>A list of k randomly chosen elements, in order</returns>
    public static List<Elements> RandomElementCombination(int k) {
        List<Elements> elementList = new List<Elements>(k) {
            Elements.FIRE,
            Elements.WATER,
            Elements.GRASS
        };
        List<Elements> combination = new List<Elements>(k);

        while (elementList.Count > 0 && combination.Count < k)
        {
            int randomIndex = Random.Range(0, elementList.Count);
            combination.Add(elementList[randomIndex]);
            elementList.RemoveAt(randomIndex);
        }

        // Ensure "Fire-Water" and "Water-Fire" look the same after the function,
        // so that we're giving combinations and not permutations.
        combination.Sort();

        return combination;
    }

    public static Faces RandomFace()
    {
        return Random.Range(0, 2) == 0 ? Faces.FRONT : Faces.BACK;
    }

    // Used for now instead of actual images
    public static Color GetColor(this Faces face)
    {
        switch (face)
        {
            case Faces.FRONT:
                return Color.white;
            case Faces.BACK:
                return Color.black;
            default:
                // impossible case but keeps the compiler happy
                return Color.magenta;
        }
    }

    // Used for now instead of actual images
    public static Color GetColor(this Elements element)
    {
        switch (element)
        {
            case Elements.FIRE:
                return Color.red;
            case Elements.WATER:
                return Color.blue;
            case Elements.GRASS:
                return Color.green;
            default:
                // impossible case but keeps the compiler happy
                return Color.magenta;
        }
    }
}
