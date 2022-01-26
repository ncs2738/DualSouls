using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer renderer;
    [SerializeField]
    private GameObject tileHighlight;

    private void OnMouseEnter()
    {
        tileHighlight.SetActive(true);
    }

    private void OnMouseExit()
    {
        tileHighlight.SetActive(false);
    }

    private void OnMouseOver()
    {
        if (GridManager.Instance.IsMapEditEnabled())
        {
            if (Input.GetMouseButtonDown(0))
            {
                this.renderer.color = Color.black;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                this.renderer.color = Color.red;
            }
        }
    }
}
