using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Entities.Data;
using UnityEngine.SceneManagement;


public class MultiplayerGameController : MonoBehaviour
{
    private SmartFox sfs;
    private int whoseTurn;
    private int myPlayerID;
    private string player1Name;
    private string player2Name;

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

    /**
	 * Initialize the game.
	 */
    public void Start()
    {
        // Register to SmartFox events
        if (SmartFoxConnection.IsInitialized)
        {
            sfs = SmartFoxConnection.Connection;
            sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
        }
        else
        {
            SceneManager.LoadScene("LoginScene");
            return;
        }
        
        // Setup my properties
        myPlayerID = sfs.MySelf.PlayerId;

        // Reset game board
        ResetGameBoard();

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

        Debug.Log("here");
        // Tell extension I'm ready to play
        sfs.Send(new ExtensionRequest("ready", new SFSObject(), sfs.LastJoinedRoom));
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
        if (whoseTurn != myPlayerID)
            return false;

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

            float startingCircleY = startingCircle.transform.position.y;
            float startingCircleX = startingCircle.transform.position.x;

            float endingCircleY = endingCircle.transform.position.y;
            float endingCircleX = endingCircle.transform.position.x;

            
            try
            {
                //PlaceLine();
                PlayerMoveMade(startingCircleX, startingCircleY, endingCircleX, endingCircleY);
            }
            catch (System.ArgumentException)
            { }
            catch (KeyNotFoundException)
            { }

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

    /**
	 * On board click, send move to other players.
	 */
    public void PlayerMoveMade(float startingCircleX, float startingCircleY, float endingCircleX, float endingCircleY)
    {

        SFSObject obj = new SFSObject();
        obj.PutFloat("x1", startingCircleX);
        obj.PutFloat("y1", startingCircleY);
        obj.PutFloat("x2", endingCircleX);
        obj.PutFloat("y2", endingCircleY);

        sfs.Send(new ExtensionRequest("move", obj, sfs.LastJoinedRoom));
    }

    private void DeactivateCircles()
    {
        endingCircleScript.Deactivate();
        startingCircleScript.Deactivate();
    }

    /**
	 * Handle the opponent move.
	 */
    private void MoveReceived(float x, float y, bool direction, int turn, float squareX, float squareY, int player1Score, int player2Score)
    {
        lines.DrawLine(x, y, direction);
        MarkBiscuit(squareX, squareY, player1Score, player2Score, turn);
        SwitchTurns(turn);
    }

    void MarkBiscuit(float x, float y, int player1Score, int player2Score, int turn)
    {
        if (turn == 1)
        {
            Instantiate(playerSymbol, new UnityEngine.Vector3(x, y, 0), transform.rotation);
            string score = "";
            if (player1Score < 10)
                score += "0";
            score += player1Score.ToString();
            ScorePlayer1.text = score;
        }
        else
        {
            Instantiate(otherPlayerSymbol, new UnityEngine.Vector3(x, y, 0), transform.rotation);
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
    private void SwitchTurns(int turn)
    {
        if (turn == 1)
        {
            // Player's turn
            player1Turn.text = "";
            player2Turn.text = player2Name + " Turn";
            isPlayerTurn = false;
        }
        else
        {
            // AI's turn
            player1Turn.text = player1Name + " Turn";
            player2Turn.text = "";

            isPlayerTurn = true;
        }
    }

    /**
	 * Destroy the game instance.
	 */
    public void DestroyGame()
    {
        sfs.RemoveEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
    }

    /**
	 * Start the game.
	 */
    private void StartGame(int whoseTurn, int p1Id, int p2Id, string p1Name, string p2Name)
    {
        this.whoseTurn = whoseTurn;
        player1Name = p1Name;
        player2Name = p2Name;
        createBiscuit.CreateBiscuit(height, sfs);
    }

    /**
	 * Clear the game board.
	 */
    public void ResetGameBoard()
    {
        createBiscuit.CreateBiscuit(height, sfs);
    }

    /**
	 * Declare game winner.
	 */
    private void ShowWinner(string cmd, int winnerId)
    {

        if (cmd == "win")
        {
            WinningPanelText = WinningPanel.GetComponentsInChildren<TextMeshProUGUI>();
            WinningPanelText[2].text = player1Score.ToString();
            WinningPanelText[3].text = player2Score.ToString();
            if (winnerId == 1)
            {
                WinningPanelText[0].text = player1Name + " Has Won!";

            }
            else
            {
                WinningPanelText[0].text = player2Name + " Has Won!";
            }
            Invoke("LoadWinningScene", 0.2f);

        }
    }

    /**
	 * Restart the game.
	 */
    public void RestartGame()
    {
        sfs.Send(new ExtensionRequest("restart", new SFSObject(), sfs.LastJoinedRoom));
    }

    /**
	 * One of the players left the game.
	 */
    private void UserLeft()
    {
        // Update interface
       
    }

    /**
	 * Handle responses from server side Extension.
	 */
    public void OnExtensionResponse(BaseEvent evt)
    {
        string cmd = (string)evt.Params["cmd"];
        SFSObject dataObject = (SFSObject)evt.Params["params"];

        switch (cmd)
        {
            case "start":
                StartGame(dataObject.GetInt("t"),
                    dataObject.GetInt("p1i"),
                    dataObject.GetInt("p2i"),
                    dataObject.GetUtfString("p1n"),
                    dataObject.GetUtfString("p2n")
                    );
                break;

            case "stop":
                UserLeft();
                break;

            case "move":
                MoveReceived(dataObject.GetFloat("x"),  // line coordinates 
                    dataObject.GetFloat("y"),           //
                    dataObject.GetBool("d"),            // line direction
                    dataObject.GetInt("t"),             // player id
                    dataObject.GetFloat("xSq"),         // square coordinates
                    dataObject.GetFloat("ySq"),         //
                    dataObject.GetInt("s1"),            // player 1 score
                    dataObject.GetInt("s2")             // player 2 score
                    );
                break;

            case "win":
                ShowWinner(cmd, (int)dataObject.GetInt("w"));
                break;

           
        }
    }
}
