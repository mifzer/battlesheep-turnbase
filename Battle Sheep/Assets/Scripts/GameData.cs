using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour {

    //Player Turn
    public static int playersTurn = 1;

    //Player Timer
    public static float PlayerTimer = 10;
    public static bool Countdown = false;

    static int playerCount;
    static int turns;
    static bool game_over;
    static int[] scores;
    static bool[] can_move;
    public static List<int> winners = new List<int>();

    //Use this for initialization
    public static void init()
    {
        //initialise variables
        playersTurn = 1;
        playerCount = 5;
        turns = 0;
        game_over = false;
        scores = new int[5];
        can_move = new bool[5];

        //Clear all winner may for previous game
        winners.Clear();

        for (int i = 0; i < scores.Length; i++)
        {
            scores[i] = 0;
        }

        for (int i = 0; i < can_move.Length; i++)
        {
            can_move[i] = true;
        }

        //Init Countdown Timer
        GameData.Countdown = true;
    }

    public static int[] GetScores()
    {
        return scores;
    }

    public static bool[] GetCanMove()
    {
        return can_move;
    }

    public static void CheckWinner()
    {
        //Check that no players can make valid moves
        if (!can_move[0] && !can_move[1] && !can_move[2] && !can_move[3] && !can_move[4])
        {
            //Stop Timer
            GameData.Countdown = false;
            GameData.PlayerTimer = 10;

            int high_score = 0;

            //Check what the highest score achieves is
            for (int i = 0; i < playerCount; i++)
            {
                if (scores[i] > high_score)
                {
                    high_score = scores[i];
                }
            }

            //Any player(s) that got this score are winners
            for (int i = 0; i < playerCount; i++)
            {
                if (scores[i] == high_score)
                {
                    winners.Add(i + 1);
                }
            }

            //Declare the victor and end the game
            if (!GameOver)
            {
                GameData.GameOver = true;
                if(winners.Count == 1)
                {
                    Debug.Log("Game over! Player " + winners[0] + " is the winner with " + high_score + " points !");
                }
                else
                {
                    Debug.Log("Game over! There was a draw! The following players scored " + high_score + "points :");
                    for(int i = 0; i < winners.Count; i++)
                    {
                        Debug.Log(winners[i]);
                    }
                }
            }
        }
    }

    public static bool GameOver
    {
        get
        {
            return game_over;
        }
        set
        {
            game_over = value;
        }
    }

    public static int Turns
    {
        get
        {
            return turns;
        }
        set
        {
            turns = value;
        }
    }

    public static int PlayerCount
    {
        get
        {
            return playerCount;
        }
        set
        {
            playerCount = value;
        }
    }

    public static void NextTurn()
    {
        //Make sure the game is running
        if (game_over)
        {
            return;
        }

        //Debug Player Turn
        Debug.Log("Turn " + Turns + ": player " + playersTurn + "'s turn!");
    }
}