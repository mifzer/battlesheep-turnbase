using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameMenu : MonoBehaviour 
{
    //How Many Player In Game
    //GAME_MODE = 2 -> 2 Player In Game
    //GAME_MODE = 3 -> 3 Player In Game
    //GAME_MODE = 4 -> 4 Player In Game
    public static int GAME_MODE = 0;

    [SerializeField]
    private Transform Transform_Menu;
    [SerializeField]
    private GameObject Button_2_Player;
    [SerializeField]
    private GameObject Button_3_Player;
    [SerializeField]
    private GameObject Button_4_Player;

    private void Start ()
    {
        Transform_Menu.DOScale(new Vector3(1.2f,1.2f,1),1);
        Transform_Menu.DOScale(new Vector3(1,1,1),0.5f).SetDelay(1);
    }

    //Play With 2 Player
    public void Choose_2_Player ()
    {   
        //Animation Tween Button Click
        Button_2_Player.transform.DOScale(new Vector3(0.8f,0.8f,1),0.3f);
        Button_2_Player.transform.DOScale(new Vector3(1,1,1),0.3f).SetDelay(0.3f);

        //Change State
        GAME_MODE = 2;
        AllButtonUninteractable();
        StartCoroutine(DelayChangeScene());

        Debug.Log(GAME_MODE);
    }   

    //Play With 3 Player
    public void Choose_3_Player ()
    {
        //Animation Tween Button Click
        Button_3_Player.transform.DOScale(new Vector3(0.8f,0.8f,1),0.3f);
        Button_3_Player.transform.DOScale(new Vector3(1,1,1),0.3f).SetDelay(0.3f);

        //Change State
        GAME_MODE = 3;
        AllButtonUninteractable();
        StartCoroutine(DelayChangeScene());

        Debug.Log(GAME_MODE);
    }

    //Play With 4 Player
    public void Choose_4_Player ()
    {
        //Animation Tween Button Click
        Button_4_Player.transform.DOScale(new Vector3(0.8f,0.8f,1),0.3f);
        Button_4_Player.transform.DOScale(new Vector3(1,1,1),0.3f).SetDelay(0.3f);

        //Change State
        GAME_MODE = 4;
        AllButtonUninteractable();
        StartCoroutine(DelayChangeScene());

        Debug.Log(GAME_MODE);
    }

    //Off All Button After Choose
    private void AllButtonUninteractable ()
    {
        Button_2_Player.GetComponent<Button>().interactable = false;
        Button_3_Player.GetComponent<Button>().interactable = false;
        Button_4_Player.GetComponent<Button>().interactable = false;
    }

    private IEnumerator DelayChangeScene ()
    {
        Transform_Menu.DOScale(new Vector3(1.2f,1.2f,1),0.5f).SetDelay(1);
        Transform_Menu.DOScale(new Vector3(0,0,0),1).SetDelay(1.5f);

        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene(1);
    }
}