using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorButton : MonoBehaviour
{
    [SerializeField] private Color32 nodeColor;

    [SerializeField] private HUD hud;

    public void OnClick()
    {
        hud.NodeColor(nodeColor);
    }
}