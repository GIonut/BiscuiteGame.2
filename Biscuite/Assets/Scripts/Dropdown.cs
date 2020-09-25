using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dropdown : MonoBehaviour
{
    private void Start()
    {
        PlayerPrefs.SetInt("BiscuitSize", 11);
    }

    public void DropdownHandler(int value)
    {
        switch (value)
        {
            case 1:
                PlayerPrefs.SetInt("BiscuitSize", 15);
                break;
            case 2:
                PlayerPrefs.SetInt("BiscuitSize", 13);
                break;
            case 3:
                PlayerPrefs.SetInt("BiscuitSize", 11);
                break;
            case 4:
                PlayerPrefs.SetInt("BiscuitSize", 9);
                break;
            case 5:
                PlayerPrefs.SetInt("BiscuitSize", 7);
                break;
        }
    }
}
