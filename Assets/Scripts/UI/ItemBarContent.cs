using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemsBarContent : MonoBehaviour
{
    public List<TilemapButton> buttons { get; private set; }

    public void Init()
    {
        GetComponentsInChildren(true, buttons);
    }
}
