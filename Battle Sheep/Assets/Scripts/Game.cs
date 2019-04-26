using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {
    
    [SerializeField]
    GameObject tile;

    [SerializeField]
    GameObject emptytile;
    
    [SerializeField]
    static List<GameObject> board;

    Vector3 location = new Vector3(0, 0, 1);

    // Use this for initialization
    void Start()
    {
        //Create the game board
        board = new List<GameObject>();
        CreateGameBoard();

        //Populate the directions list
        Directions.directions = new List<Vector3>();
        Directions.directions.Add(new Vector3(1, 1.75f, 0)); //NE
        Directions.directions.Add(new Vector3(2, 0, 0)); //E
        Directions.directions.Add(new Vector3(1, -1.75f, 0)); //SE
        Directions.directions.Add(new Vector3(-1, -1.75f, 0)); //SW
        Directions.directions.Add(new Vector3(-2, 0f, 0)); //W
        Directions.directions.Add(new Vector3(-1, 1.75f, 0)); //NW

        //Initialise game data
        GameData.init();

        InitialiseGame();
        StartGame();
    }

    void InitialiseGame()
    {
        List<GameObject> border_tiles;

        //Determine which tiles are on the border of the grid
        border_tiles = new List<GameObject>();
        foreach (GameObject tile in board)
        {
            if (tile.GetComponent<TileData>().GetSurroundingEmptyTiles() <= 4)
            {
                border_tiles.Add(tile);
            }
        }

        //Decide a random number of tiles to remove to create a "random" board
        int removed_tiles = (int)UnityEngine.Random.Range(1, border_tiles.Count - 1);

        //Remove tiles
        for (int i = 0; i < removed_tiles; i++)
        {
            int index = UnityEngine.Random.Range(1, border_tiles.Count);
            index--;
            if (!border_tiles[index].GetComponent<TileData>().RemoveTile)
                border_tiles[index].GetComponent<TileData>().RemoveTile = true;
        }

        //Refresh border_tiles
        border_tiles.Clear();

        //redetermines border tiles, excluding removed tiles
        foreach (GameObject tile in board)
        {
            int empty_tiles = tile.GetComponent<TileData>().GetSurroundingEmptyTiles();
            bool removed = tile.GetComponent<TileData>().RemoveTile;
            int id = tile.GetComponent<TileData>().ID;
            if (empty_tiles <= 4 && !removed)
            {
                border_tiles.Add(tile);
            }
        }

        for(int i = 0; i < 4; i++)
        {
            int index = UnityEngine.Random.Range(1, border_tiles.Count);
            index--;

            if(i == 0)
            {
                border_tiles[index].GetComponent<TileData>().SetOwnership(1);
                border_tiles.Remove(border_tiles[index]);
            }
            else if(i == 1)
            {
                border_tiles[index].GetComponent<TileData>().SetOwnership(2);
                border_tiles.Remove(border_tiles[index]);
            }
            else if(i == 2)
            {
                border_tiles[index].GetComponent<TileData>().SetOwnership(3);
                border_tiles.Remove(border_tiles[index]);   
            }
            else if(i == 3)
            {
                border_tiles[index].GetComponent<TileData>().SetOwnership(4);
                border_tiles.Remove(border_tiles[index]);  
            }
        }

        //Refresh border_tiles
        border_tiles.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        //Player Timer
        if (GameData.Countdown == true)
        {
            GameData.PlayerTimer -= 1 * Time.deltaTime;

            if (GameData.PlayerTimer <= 0)
            {
                GameData.PlayerTimer -= 0 * Time.deltaTime;
                GameData.playersTurn++;
                GameData.NextTurn();

                GameData.PlayerTimer = 10;
            }
        }
        else
        {
            GameData.PlayerTimer -= 0 * Time.deltaTime;
            GameData.PlayerTimer = 10;
        }

        //See if the game is over
        if(!GameData.GameOver)
        {
            GameData.CheckWinner();
        }
    }

    void CreateGameBoard()
    {
        byte iterator = 0; //0 to 256
        ushort spawn_amount = 64; //0 and 65535
        sbyte x_location = 0; //-128 to 127    
        float y_distance = -1.75f;
        byte x_distance = 2;

        //Creates a grid
        for (ushort i = 0; i < spawn_amount; i++)
        {
            //creates a tile and adds it to the game board list
            GameObject instancedTile = Instantiate(tile, location, Quaternion.identity);
            instancedTile.GetComponent<TileData>().ID = i;
            board.Add(instancedTile);
            location.x += x_distance;
            iterator++;

            //Creates rows relative to the size of the grid
            if (iterator == Mathf.Sqrt(spawn_amount))
            {
                iterator = 0;
                if (x_location == 0)
                    x_location = -1;
                else
                    x_location = 0;
                location.x = x_location;
                location.y -= y_distance;
            }
        }
    }

    void StartGame()
    {
        //Update tiles and start game
        GameData.NextTurn();
    }

    public static List<GameObject> GetBoard()
    {
        return board;
    }
}