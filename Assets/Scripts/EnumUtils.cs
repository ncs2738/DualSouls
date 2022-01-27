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
}
