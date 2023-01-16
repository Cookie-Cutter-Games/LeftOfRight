using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ButtonClicked : MonoBehaviour
{

    public EventHandler buttonClicked;

    public void onButtonClick()
    {
        buttonClicked?.Invoke(this, new EventArgs());
    }
}
