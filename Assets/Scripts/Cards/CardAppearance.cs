using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardAppearance : MonoBehaviour
{
    public ConcreteCard card;

    public Image frame;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Image elementOneImage;
    public Image elementTwoImage;
    public Image artImage;
    public GameObject leftClickHighlight;
    public GameObject rightClickHighlight;

    void Start()
    {
        Refresh();
        if (gameObject.activeSelf)
        {
            card.OnModified += Refresh;
        }
    }

    public void Refresh()
    {
        if (card == null)
        {
            gameObject.SetActive(false);
            return;
        } else
        {
            gameObject.SetActive(true);
        }
        frame.color = card.face.GetColor();
        nameText.color = card.face.Opposite().GetColor();
        descriptionText.color = card.face.Opposite().GetColor();

        nameText.text = card.Name;
        descriptionText.text = card.face == Faces.FRONT
            ? card.FrontDescription
            : card.BackDescription;

        elementOneImage.sprite = card.elementOne.Sprite();
        elementTwoImage.sprite = card.elementTwo.Sprite();

        artImage.sprite = card.Artwork;
    }


    public void SetLeftClicKHighlight()
    {
        leftClickHighlight.SetActive(true);
    }

    public void SetRightClicKHighlight()
    {
        rightClickHighlight.SetActive(true);
    }

    public void ResetHighlights()
    {
        leftClickHighlight.SetActive(false);
        rightClickHighlight.SetActive(false);
    }
}
