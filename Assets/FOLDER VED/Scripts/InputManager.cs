using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private bool[] clicked = new bool[4];
    private bool[] wasClicked = new bool[4];
    public bool Clicked => IsInputClicked();
    public bool Held => IsInputHeld();
    

    private bool IsInputClicked()
    {
        for(int i = 0; i < clicked.Length; i++)
        {
            if (clicked[i] && !wasClicked[i]) return true;
        }
        return false;
    }

    private bool IsInputHeld()
    {
        for (int i = 0; i < clicked.Length; i++)
        {
            if (clicked[i] && wasClicked[i]) return true;
        }
        return false;
    }

    public void UpdateInput()
    {
        for (int i = 0; i < clicked.Length; i++)
        {
            wasClicked[i] = clicked[i];
        }

        clicked[0] = Input.GetKey(KeyCode.LeftArrow);
        clicked[1] = Input.GetKey(KeyCode.RightArrow);
        clicked[2] = Input.GetKey(KeyCode.Mouse0);
        clicked[3] = Input.GetKey(KeyCode.Mouse1);
        
    } 
}
