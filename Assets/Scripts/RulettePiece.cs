using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RulettePiece : MonoBehaviour
{
    [SerializeField]
    private Image imageIcon;
    [SerializeField]
    private TextMeshProUGUI textDescription;

    public void Setup(RouletteDate data)
    {
        imageIcon.sprite = data.icon;
        textDescription.text = data.description;
    }
}
