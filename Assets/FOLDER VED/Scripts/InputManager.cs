using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public int InstrumentClicked => WasInstrumentClicked();

    private bool[] instrumentClick = new bool[3];
    private bool[] clicked = new bool[4];
    private bool[] wasClicked = new bool[4];
    public bool Clicked => IsInputClicked();
    public bool Held => IsInputHeld();


    private bool IsInputClicked()
    {
        for (int i = 0; i < clicked.Length; i++)
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

    private int WasInstrumentClicked()
    {
        for (int i = 0; i < instrumentClick.Length; i++)
        {
            if (instrumentClick[i]) return i;
        }
        return -1;
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

        instrumentClick[0] = Input.GetKeyDown(KeyCode.Alpha1);
        instrumentClick[1] = Input.GetKeyDown(KeyCode.Alpha2);
        instrumentClick[2] = Input.GetKeyDown(KeyCode.Alpha3);
    }
}
