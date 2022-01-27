using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardKind", menuName = "ScriptableObjects/CardKind")]
public class CardKind : ScriptableObject
{
    public new string name;
    public Sprite artwork;
    public UnitKind unit;
    public string frontDescription;
    public string backDescription;
}
