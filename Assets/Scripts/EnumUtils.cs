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
        List<Elements> elementList = new List<Elements>(k);
        List<Elements> combination = new List<Elements>(k);

        while (elementList.Count > 0)
        {
            combination.Add(elementList[Random.Range(0,elementList.Count)]);
            elementList.RemoveAt(elementList.Count - 1);
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
}
