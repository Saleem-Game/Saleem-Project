using UnityEngine;
using TMPro;
using System;

public class DescriptionScript : MonoBehaviour
{
    public TextMeshProUGUI Nametext;
    public TextMeshProUGUI DescriptionText;

    public void changeText(string objName, string objDesc)
    {
        this.Nametext.text = objName;
        this.DescriptionText.text = objDesc;
    }
}
