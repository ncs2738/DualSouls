using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatPlayer : MonoBehaviour
{
    [SerializeField]
    private TMP_Text TeamTitle;

    [SerializeField]
    private PlayerTeam.Faction playerFaction;

    [SerializeField]
    private Image sprite;

    [SerializeField]
    private TMP_Text ChoiceStatus;

    private Elements elem1;
    private Elements elem2;

    [SerializeField]
    Image elem1Image;
    [SerializeField]
    Image elem2Image;

    [SerializeField]
    Sprite[] elementImages;

    private Elements selectedElement;

    public enum SelectedElement
    {
        left = 0,
        right = 1,
    }

    public void SetData(Elements _elem1, Elements _elem2)
    {
        elem1 = _elem1;
        SetImage(elem1, elem1Image);

        elem2 = _elem2;
        SetImage(elem2, elem2Image);

        ChoiceStatus.text = "Choose.";
    }

    private void SetImage(Elements element, Image elementImage)
    {
        switch(element)
        {
            case Elements.FIRE:
                elementImage.sprite = elementImages[0];
                break;
            case Elements.WATER:
                elementImage.sprite = elementImages[1];
                break;
            default:
                elementImage.sprite = elementImages[2];
                break;
        }
    }

    public void SetElement(SelectedElement elem)
    {
        if(elem.Equals(SelectedElement.left))
        {
            selectedElement = elem1;
        }
        else
        {
            selectedElement = elem2;
        }

        ChoiceStatus.text = "C H O S E N";
    }

    public Elements GetSelectedElement()
    {
        return selectedElement;
    }
}
