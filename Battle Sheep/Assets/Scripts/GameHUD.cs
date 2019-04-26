using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameHUD : MonoBehaviour {

    [SerializeField]
    Text[] UIText;

    [SerializeField]
    Text victory_text;

    [SerializeField]
    private GameObject Button_ExitToMainMenu;
    [SerializeField]
    private GameObject Button_BackToMainMenu;

    [SerializeField]
    private Text Text_Timer;

    [SerializeField]
    private Transform[] Main_Scene;

	// Use this for initialization
	void Start ()
    {
        //disable victory text
        victory_text.gameObject.SetActive(false);

        if(GameMenu.GAME_MODE == 2)
        {
            UIText[4].transform.DOScale(new Vector3(0,0,0),0.1f);
            UIText[5].transform.DOScale(new Vector3(0,0,0),0.1f);
        }
        else if(GameMenu.GAME_MODE == 3)
        {
            UIText[5].transform.DOScale(new Vector3(0,0,0),0.1f);
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        //Update Timer Text
        Text_Timer.text = "" + GameData.PlayerTimer.ToString("0");

        //Update player scores
        for (int i = 2; i < UIText.Length; i++)
        {
            UIText[i].text = "Player " + (i - 1) + ": " + GameData.GetScores()[i - 2];
        }

        //Update the players turn
        UIText[0].text = "Player " + GameData.playersTurn + "'s turn! (Turn " + GameData.Turns + ")";

        //Display victory message
        if (GameData.GameOver)
        {
            //enable victory text
            victory_text.gameObject.SetActive(true);
            
            //Is player one the only winner?
            if (GameData.winners.Contains(1) && GameData.winners.Count == 1)
            {
                victory_text.text = "Congratulations!\nPlayer 1 Win!";
            }
            else if (GameData.winners.Contains(2) && GameData.winners.Count == 1)
            {
                victory_text.text = "Congratulations!\nPlayer 2 Win!";
            }
            else if (GameData.winners.Contains(3) && GameData.winners.Count == 1)
            {
                victory_text.text = "Congratulations!\nPlayer 3 Win!";
            }
            else if (GameData.winners.Contains(4) && GameData.winners.Count == 1)
            {
                victory_text.text = "Congratulations!\nPlayer 4 Win!";
            }
            else if (GameData.winners.Count > 1)
            {
                victory_text.text = "It's a Draw!";
            }
        }  
    }

    //Button Back To Main Menu Before Game End
    public void ExitToMainMenu ()
    {
        Button_ExitToMainMenu.transform.DOScale(new Vector3(0.8f,0.8f,1),0.3f);
        Button_ExitToMainMenu.transform.DOScale(new Vector3(1,1,1),0.3f).SetDelay(0.3f);

        StartCoroutine(DelayChangeSceneToMainMenu());
    }

    //Button Back To Main Menu After Game End
    public void BackToMainMenu ()
    {
        Button_BackToMainMenu.transform.DOScale(new Vector3(0.8f,0.8f,1),0.3f);
        Button_BackToMainMenu.transform.DOScale(new Vector3(1,1,1),0.3f).SetDelay(0.3f);

        StartCoroutine(DelayChangeSceneToMainMenu());
    }

    private IEnumerator DelayChangeSceneToMainMenu () 
    {
        GameMenu.GAME_MODE = 0;
        GameData.playersTurn = 1;

        //Stop Timer
        GameData.Countdown = false;
        GameData.PlayerTimer = 10;

        for(int i = 0; i < 5; i++)
        {
            Main_Scene[i].DOScale(new Vector3(1.2f,1.2f,1),0.5f).SetDelay(1);
            Main_Scene[i].DOScale(new Vector3(0,0,0),1).SetDelay(1.5f);
        }

        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene(0);
    }
}