using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinningController : MonoBehaviour
{
    //Parameters
    public TextMeshProUGUI player1Score;
    public TextMeshProUGUI player2Score;
    public TextMeshProUGUI winner;
    // Start is called before the first frame update
    void Start()
    {
        player1Score.text = PlayerPrefs.GetInt("p1").ToString();
        player2Score.text = PlayerPrefs.GetInt("p2").ToString();
        winner.text = "Player " + PlayerPrefs.GetInt("win").ToString() + " has won!";
    }
}
