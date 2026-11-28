
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextFaderHandler : MonoBehaviour
{
    public void DisplayText(string text)
    {
        TextFader.Singleton.ShowText(text);

    }
}
