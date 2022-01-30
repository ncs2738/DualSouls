using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnumUtils
{
    private static Sprite fireSprite = null;
    private static Sprite waterSprite = null;
    private static Sprite grassSprite = null;

    public static Sprite Sprite(this Elements element)
    {
        switch (element)
        {
            case Elements.FIRE:
                if (fireSprite == null)
                {
                    fireSprite = Resources.Load<Sprite>("Sprites/fire");
                }
                return fireSprite;
            case Elements.WATER:
                if (waterSprite == null)
                {
                    waterSprite = Resources.Load<Sprite>("Sprites/water");
                }
                return waterSprite;
            case Elements.GRASS:
                if (grassSprite == null)
                {
                    grassSprite = Resources.Load<Sprite>("Sprites/grass");
                }
                return grassSprite;
            default:
                Debug.LogWarning("Bad switch statement in SpriteFromElement");
                fireSprite = Resources.Load<Sprite>("Sprites/fire");
                return fireSprite;
        }
    }

    // Rotate a vector so that the basis vector pointing to the right
    // points towards the given orientation.
    //
    // e.g. rotating to Orientation.EAST does nothing,
    // rotating to Orientation.WEST is a 180 degree turn.
    public static Vector2Int Rotate(this Vector2Int vec, Orientation o)
    {
        Vector2Int newVec;
        switch (o)
        {
            case Orientation.NORTH:
                // rotation matrix is
                //  0 1 (x goes to y)
                // -1 0 (y goes to x and is negated)
                newVec = new Vector2Int(-vec.y, vec.x);
                break;
            case Orientation.SOUTH:
                // rotation matrix is
                //  0 -1 (x goes to y and is negated)
                //  1  0 (y goes to x)
                newVec = new Vector2Int(vec.y, -vec.x);
                break;
            case Orientation.EAST:
                // rotation matrix is
                //  1 0 (x goes to x)
                //  0 1 (y goes to y)
                newVec = vec;
                break;
            case Orientation.WEST:
                // rotation matrix is
                // -1 0 (x stays in x and is negated)
                // -1 0 (y stays in y and is negated)
                newVec = new Vector2Int(-vec.x, -vec.y);
                break;
            default:
                newVec = vec;
                Debug.LogWarning("Bad rotation given to Vector2Int.Rotate(Orientation)");
                break;
        }

        return newVec;
    }

    public static Orientation ToOrientation(this Vector2Int vec)
    {
        if (vec.Equals(Vector2.up))
        {
            return Orientation.NORTH;
        } else if (vec.Equals(Vector2Int.down))
        {
            return Orientation.SOUTH;
        } else if (vec.Equals(Vector2Int.left))
        {
            return Orientation.WEST;
        } else if (vec.Equals(Vector2Int.right))
        {
            return Orientation.EAST;
        } else
        {
            Debug.LogWarning($"Bad call to ToOrientation with `{vec}`");
            return Orientation.EAST;
        }
    }

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
