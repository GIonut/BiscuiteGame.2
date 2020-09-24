using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    public void LoadWinningScene()
    {
        SceneManager.LoadScene("WinningScene");
    }
    public void LoadStartScene()
    {
        
        SceneManager.LoadScene("StartScene");
        //gameController.ResetGame();
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void LoadSettingsScene()
    {
        SceneManager.LoadScene("SettingsScene");
    }

    public void BiscuitSizeScene()
    {
        SceneManager.LoadScene("BiscuitSizeScene");
    }

    public void BackgroundScene()
    {
        SceneManager.LoadScene("BackgroundScene");
    }

    public void LoadLoginScene()
    {
        SceneManager.LoadScene("LoginScene");
    }
}

