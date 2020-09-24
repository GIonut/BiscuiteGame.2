using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using Unity.Mathematics;
using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

public class GameController : MonoBehaviour
{
    // starting circle and ending circle will be the coordinates of the line to be drawn
    private GameObject startingCircle = null;
    private GameObject endingCircle = null;
    private int player1Score = 0;
    private int player2Score = 0;
    bool isPlayerTurn = true;
    int numberOfBiscuits = 0;

    public int height = 10;

    // reference
    public CreateCustomizedBiscuit createBiscuit;
    public PlaceBiscuits placeBiscuit;
    public Lines lines;
    public GameObject biscuitParent;
    public GameObject Player1Stats;
    public GameObject Player2Stats;
    public GameObject WinningPanel;
    public SceneLoader sceneLoader;

    private TextMeshProUGUI ScorePlayer1;
    private TextMeshProUGUI ScorePlayer2;
    private TextMeshProUGUI player1Turn;
    private TextMeshProUGUI player2Turn;
    private TextMeshProUGUI[] WinningPanelText;

    private BiscuitMatrix biscuitMatrix;
    private ActivateCircle startingCircleScript;
    private ActivateCircle endingCircleScript;
    
    public GameObject playerSymbol;
    public GameObject otherPlayerSymbol;


    private bool biscuitWasMarked = false;
    void Start()
    {
        // Instantiate the Avatars, the score and the whoseTurns message
        GameObject Player1StatsObject = Instantiate(Player1Stats, FindObjectOfType<Canvas>().transform);
        GameObject Player2StatsObject = Instantiate(Player2Stats, FindObjectOfType<Canvas>().transform);

        // reference the score and the whoseTurn message that are childs of the PlayerStatsObjects
        GameObject p1Score = Player1StatsObject.transform.GetChild(1).gameObject;
        ScorePlayer1 = p1Score.GetComponent<TextMeshProUGUI>();
        GameObject p1Turn = Player1StatsObject.transform.GetChild(2).gameObject;
        player1Turn = p1Turn.GetComponent<TextMeshProUGUI>();
        GameObject p2Score = Player2StatsObject.transform.GetChild(1).gameObject;
        ScorePlayer2 = p2Score.GetComponent<TextMeshProUGUI>();
        GameObject p2Turn = Player2StatsObject.transform.GetChild(2).gameObject;
        player2Turn = p2Turn.GetComponent<TextMeshProUGUI>();

        ScorePlayer1.text = "00";
        ScorePlayer2.text = "00";
        player1Turn.text = "Player 1 Turn";
        player2Turn.text = "";
        height = PlayerPrefs.GetInt("BiscuitSize") - 1;
        
        
        // having the height, is easy to compute the number of biscuits. The height is computed as the number of biscuits 
        // the matrix spread out from top to bottom or from left to right, minus one.
        // the formula for the number of all biscuits in the matrix is   ( 1 + 3 + 5 + height-1)*2 + height + 1
        // which can be simplyfied to the formula bellow;
        numberOfBiscuits = height * height / 2 + height + 1;

        createBiscuit.CreateBiscuit(height, null);
    }

    
    // a circle can be deactivated if the player changes its mind and want to chose another starting circle
    public bool CanBeDeactivated(GameObject circle)
    {
        if (startingCircle == circle)
        {
            startingCircle = null;
            return true;
        }
        return false;
    }

    // returns true if there is no startingCircle set 
    public bool CanBeActivated(GameObject circle)
    {
        if (startingCircle == null)
        {
            startingCircle = circle;
            startingCircleScript = startingCircle.GetComponent<ActivateCircle>();
            return true;
        }
        else if (CheckNeighbour(circle))
        {
           
            endingCircle = circle;
            endingCircleScript = endingCircle.GetComponent<ActivateCircle>();
            endingCircleScript.Activate();  // activate neighbour circle
            biscuitWasMarked = false;

            try
            {
                PlaceLine();
                if(!biscuitWasMarked)
                    SwitchTurns();
                CheckForWin();

            }
            catch (System.ArgumentException)
            { 

            }
            catch(KeyNotFoundException)
            {
               
            }
            DeactivateCircles();

            // set the circles to null
            startingCircle = null;
            endingCircle = null;
            return false;// returns false so that the onMouseDown method from ActivateCircle of ending circle wont activate it(not low coupling, sorry :'( )
        }
        else
        {
            return false;
        }
    }

    // checks if a circle is a neighbour of the startingCircle( the distance between their positions is 1f)
    private bool CheckNeighbour(GameObject circle)
    {
        
        if (startingCircle != null && UnityEngine.Vector3.Distance(startingCircle.transform.position, circle.transform.position) == 1f)
        {

            return true;
        }

        return false;
    }
       
    // draw the line between the starting and the ending circle
    private void PlaceLine()
    {
        float startingCircleY = startingCircle.transform.position.y;
        float startingCircleX = startingCircle.transform.position.x;

        float endingCircleY = endingCircle.transform.position.y;
        float endingCircleX = endingCircle.transform.position.x;

        // if the circles are aligned horizontaly, place the line accordingly
        if(startingCircleY == endingCircleY)
        {
            // if we want to draw a line from left to right, move it a bit to the right so that the center of the line
            // is not placed exactly on the startingircle, but in-between the circles;
            if (startingCircleX - endingCircleX == -1f)
            {
                
                //SetNeighbours(1);// 1 means endingCircle is the right neighbour of startingCircle 
                lines.DrawLine(startingCircleX + 0.5f, startingCircleY, false);
                MarkBiscuitsSide(startingCircleX + 0.5f, startingCircleY, false);
            }
            // if we want to draw the line from left to right, move it a little to the left
            else
            {
                //SetNeighbours(3);// 3 means endingCircle is the left neighbour of startingCircle
                lines.DrawLine(startingCircleX - 0.5f, startingCircleY, false);
                MarkBiscuitsSide(startingCircleX - 0.5f, startingCircleY, false);
            }
        }
        // otherwise place the line verticaly( the offset 0.5f  is used to place it right in between the circles)
        else
        {
            // draw the line from bottom to top
            if (startingCircleY - endingCircleY == -1f)
            {
                //SetNeighbours(0); // 0 means endingCircle is the upper neighbour of startingCircle
                lines.DrawLine(startingCircleX, startingCircleY + 0.5f, true);
                MarkBiscuitsSide(startingCircleX, startingCircleY + 0.5f, true);
            }
            // draw the line from top to bottom
            else
            {
                //SetNeighbours(2); // 2 means endingCircle is the bottom neighbour of startingCircle
                lines.DrawLine(startingCircleX, startingCircleY - 0.5f, true);
                MarkBiscuitsSide(startingCircleX, startingCircleY - 0.5f, true);
            }
        }
    }


    private void DeactivateCircles()
    {
        endingCircleScript.Deactivate();
        startingCircleScript.Deactivate();
    }

    /*private void SetNeighbours(int startingCircleNeighbourDirection)
    {
        // 0-top, 1-right, 2-bottom, 3-left

        // if there's no neighobur in that direction, then set it. Otherwise don't
        if (!startingCircleScript.GetNeighbour(startingCircleNeighbourDirection))
        {
            startingCircleScript.SetNeighbour(startingCircleNeighbourDirection);
            endingCircleScript.SetNeighbour((startingCircleNeighbourDirection + 2) % 4);
        }
        else
        {
            throw new System.ArgumentException("invalid neighbour");
        }
    }*/

    private void MarkBiscuitsSide(float lineX, float lineY, bool lineIsRotated)
    {
        biscuitMatrix = createBiscuit.GetBiscuitMatrix();
        int top = 0, right = 1, bottom = 2, left = 3;
        
        if (lineIsRotated) // the squares are left-right neighbours
        {
            float y = lineY;
            float xOfFirstSquare = lineX - 0.5f;
            float xOfSecondSquare = lineX + 0.5f;
            biscuitMatrix.SetLine(xOfFirstSquare, y, right);
            CheckForSquare(xOfFirstSquare, y);
            biscuitMatrix.SetLine(xOfSecondSquare, y, left);
            CheckForSquare(xOfSecondSquare, y);
        }
        else // the squares are top-bottom neighbours
        {
            
            float x = lineX;
            float yOfFirstSquare = lineY - 0.5f;
            float yOfSecondSquare = lineY + 0.5f;
            biscuitMatrix.SetLine(x, yOfFirstSquare, top);
            CheckForSquare(x, yOfFirstSquare);
            biscuitMatrix.SetLine(x, yOfSecondSquare, bottom);
            CheckForSquare(x, yOfSecondSquare);
        }
    }

    private void CheckForSquare(float x, float y)
    {
        if(biscuitMatrix.CheckClosedSquare(x, y))
        {
             MarkBiscuit(x, y);
             biscuitWasMarked = true;
        }
    }

    void MarkBiscuit(float x, float y)
    {
        if (isPlayerTurn)
        {
            Instantiate(playerSymbol, new UnityEngine.Vector3(x, y, 0), transform.rotation);
            player1Score++;
            string score = "";
            if (player1Score < 10)
                score += "0";
            score += player1Score.ToString();
            ScorePlayer1.text = score;
        }
        else
        {
            Instantiate(otherPlayerSymbol, new UnityEngine.Vector3(x, y, 0), transform.rotation);
            player2Score++;
            string score = "";
            if (player2Score < 10)
                score += "0";
            score += player2Score.ToString();
            ScorePlayer2.text = score;
        }
    }

    private void LoadWinningScene()
    {
        sceneLoader.LoadWinningScene();
    }
    private void SwitchTurns()
    {
        if (isPlayerTurn == true)
        {
            // Player's turn
            player1Turn.text = "";
            player2Turn.text = "Player 2 Turn";
            isPlayerTurn = false;
        }
        else
        {
            // AI's turn
            player1Turn.text = "Player 1 Turn";
            player2Turn.text = "";

            isPlayerTurn = true;
        }
    }

    private void CheckForWin()
    {
        if (player1Score + player2Score == numberOfBiscuits)
        {
            WinningPanelText = WinningPanel.GetComponentsInChildren<TextMeshProUGUI>();
            WinningPanelText[2].text = player1Score.ToString();
            WinningPanelText[3].text = player2Score.ToString();
            if (player1Score > player2Score)
            {
                WinningPanelText[0].text = "Player 1 Has Won!";

            }
            else
            {
                WinningPanelText[0].text = "Player 2 Has Won!";
            }
            Invoke("LoadWinningScene", 0.2f);

        }
    }
}
