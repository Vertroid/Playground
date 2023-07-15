using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestAnchoredPosition : MonoBehaviour
{
    public RectTransform target;
    public Text text;

    private void Update()
    {
        this.text.text = target.anchoredPosition.ToString();
    }
}
