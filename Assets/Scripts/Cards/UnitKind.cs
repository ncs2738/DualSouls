using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitKind", menuName = "ScriptableObjects/UnitKind")]
public class UnitKind : ScriptableObject
{
    public Array2DEditor.Array2DInt attackPattern;
    public Sprite appearance;
}
