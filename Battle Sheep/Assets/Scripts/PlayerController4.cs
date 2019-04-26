using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController4 : MonoBehaviour
{
    [SerializeField]
    private Text Stack_UI_Text;

    enum PlayerState
    {
        IDLE = 0,
        TILE_CHOSEN = 1,
        DIRECTION_CHOSEN = 2
    }

    PlayerState state = PlayerState.IDLE;

    [SerializeField]
    int score;

    bool taken_turn = false;

    [SerializeField]
    List<GameObject> owned_tiles;

    TileData chosen_tile_data;

    public bool CanMove { get; set; }

    // Use this for initialization
    void Start()
    {
        GameObject UIStackText = GameObject.FindGameObjectWithTag("UIStack");
        Stack_UI_Text = UIStackText.GetComponent<Text>();

        CanMove = true;
        owned_tiles = new List<GameObject>();
        score = 0;
    }

    void Update()
    {
        //Update the player's ability to move and score in the game data
        GameData.GetScores()[3] = CalculateScore();
        GameData.GetCanMove()[3] = CanMove;

        //Skip if the game is ended or it's not the player's turn
        if (GameData.GameOver || GameData.playersTurn != 4)
            return;

        //Make sure the player hasnt taken their turn
        if(!taken_turn)
        {
            taken_turn = true;
            CalculateScore();
            UpdateTiles();
        }

        //DEBUG - Skip your turn
        if (Input.GetKeyDown("space"))
        {
            CalculateScore();
            EndTurn();
        }

        //Mouse button one is pressed
        if (Input.GetButtonDown("Fire1"))
        {
            //Make sure it's the player's turn
            if (GameData.playersTurn == 4)
            {
                HandleClick();
                Stack_UI_Text.text = "" + chosen_tile_data.TempStackSize;
            }
        }
    }

    bool HandleClick()
    {
        //cast the mouse position to world space
        Vector3 target = new Vector3
            (
            this.gameObject.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition).x,
            this.gameObject.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition).y, 0
            );

        //raycast to the mouse position in world space
        RaycastHit hit;
        Physics.Raycast(target, transform.TransformDirection(Vector3.forward), out hit, 100f);

        //ignore if no tiles were hit
        if (hit.collider == null)
            return false;

        //store the data of the tile hit otherwise
        TileData data = hit.collider.gameObject.GetComponent<TileData>();

        //If the player hasn't chosen a move yet
        if (state == PlayerState.IDLE)
        {
            //Ignore if the player didn't click one of their tiles
            if (hit.collider.gameObject.tag != "Sheep4")
                return false;

            //Ignore if the player's tile can't perform a valid move
            if (data.GetAccessibleTiles().Count < 1 || data.StackSize < 2)
                return false;

            //Resize the text and display how many tiles the player wants to move
            chosen_tile_data = data;
            chosen_tile_data.stack_text.GetComponent<TextMesh>().fontSize = 60;
            chosen_tile_data.stack_text_shadow.GetComponent<TextMesh>().fontSize = 60;
            chosen_tile_data.TempStackSize = chosen_tile_data.StackSize - 1;

            Collider[] collision;

            //Create directional arrows based on the directions the tile can move in
            for (int i = 0; i < Directions.directions.Count; i++)
            {
                collision = Physics.OverlapSphere(data.transform.position + Directions.directions[i], 0.5f);

                if (collision.Length == 1)
                {
                    if (collision[0].gameObject.tag == "Tile")
                    {
                        collision[0].gameObject.tag = "Arrow";
                        collision[0].gameObject.transform.Rotate(new Vector3(0, 0, -60 * i));
                        collision[0].GetComponent<TileData>().Direction = (byte)i;
                    }
                }
                collision = null;
            }
            //The player has chosen a tile & returns so the move isnt automatically made
            state = PlayerState.TILE_CHOSEN;
            return false;
        }

        //Makes sure the player has chosen a tile
        if(state == PlayerState.TILE_CHOSEN)
        {   
            //If the previous raycast has hit a directional arrow
            if(hit.collider.gameObject.tag == "Arrow")
            {
                bool furthest_found = false;
                int last_checked_ID = 0;
                chosen_tile_data.TileChecker.transform.position = chosen_tile_data.transform.position;
                Collider[] checked_tile = null;

                //check the furthest position the player can move to in the chosen direction
                while (!furthest_found)
                {
                    chosen_tile_data.TileChecker.transform.position += Directions.directions[data.Direction];
                    checked_tile = Physics.OverlapSphere(chosen_tile_data.TileChecker.transform.position, 0.5f);

                    //There is a tile here 
                    if (checked_tile.Length == 1)
                    {
                        if (checked_tile[0].gameObject.tag == "Tile" || checked_tile[0].gameObject.tag == "Arrow")
                            last_checked_ID = checked_tile[0].GetComponent<TileData>().ID;
                        else
                            furthest_found = true;
                    }
                    //no more tiles found, the last tile queried is the furthest the player can move
                    else
                    {
                        furthest_found = true;
                    }
                }

                //as long as at least one tile was queried
                if (last_checked_ID >= 0)
                {
                    //Decrease the stack of the moving tile by the amount designated by the player
                    chosen_tile_data.StackSize -= chosen_tile_data.TempStackSize;

                    //Change ownership of the empty tile to the player, with its stack size equalling the amount moved by the player
                    Game.GetBoard()[last_checked_ID].GetComponent<TileData>().ChangeOwnership(4);
                    Game.GetBoard()[last_checked_ID].GetComponent<TileData>().StackSize = chosen_tile_data.TempStackSize;

                    //Player is no longer moving
                    state = PlayerState.IDLE;

                    //Remove the changes made to the font on the tile
                    chosen_tile_data.stack_text.GetComponent<TextMesh>().fontSize = 30;
                    chosen_tile_data.stack_text_shadow.GetComponent<TextMesh>().fontSize = 30;
                    chosen_tile_data.TempStackSize = 0;

                    //Destroy the arrows created for directional movement
                    foreach (GameObject tile in Game.GetBoard())
                    {
                        tile.GetComponent<TileData>().DestroyArrow();
                    }

                    //End the player's turn
                    EndTurn();
                    return true;
                }
            }
            //Clicking the selected tile decreases the amount of tiles you want to move
            else if (hit.collider.gameObject.GetComponent<TileData>() == chosen_tile_data)
            {
                chosen_tile_data.TempStackSize--;
                if (chosen_tile_data.TempStackSize < 1)
                    chosen_tile_data.TempStackSize = chosen_tile_data.StackSize - 1;
            }
            //clicking anywhere else voids your current selection
            else
            {
                foreach (GameObject tile in Game.GetBoard())
                {
                    tile.GetComponent<TileData>().DestroyArrow();
                }
                state = PlayerState.IDLE;
                chosen_tile_data.stack_text.GetComponent<TextMesh>().fontSize = 30;
                chosen_tile_data.stack_text_shadow.GetComponent<TextMesh>().fontSize = 30;
                chosen_tile_data.TempStackSize = 0;
            }
        }
        return false;
    }

    int CalculateScore()
    {   
        //Player's score is equal to the number of tiles they own.
        score = GetOwnedTiles().Count;
        return score;
    }

    public void EndTurn()
    {
        //Reset Player Timer
        GameData.PlayerTimer = 10;

        //Start the next player's turn
        taken_turn = false;
        GameData.playersTurn++;
        GameData.NextTurn();
    }
    
    void UpdateTiles()
    {
        int accessible_tiles = 0;

        //Determine which tiles can be moved to from each tile the player owns
        foreach (GameObject tile in owned_tiles)
        {
            tile.GetComponent<TileData>().UpdateAccessibleTiles();

            if(tile.GetComponent<TileData>().StackSize > 1)
                accessible_tiles += tile.GetComponent<TileData>().GetAccessibleTiles().Count;
        }

        //If no tiles can be moved to, the player cannot move.
        if (accessible_tiles <= 0)
        {
            CanMove = false;
            EndTurn();
            Debug.Log("DIJALANKAN 4");
        }   
    }

    List<GameObject> GetOwnedTiles()
    {
        owned_tiles.Clear();

        //Loop through all tiles, ones tagged for the player are tiles the player owns.
        foreach (GameObject tile in Game.GetBoard())
        {
            if (tile.gameObject.tag == "Sheep4")
            {
                owned_tiles.Add(tile);
            }
        }
        return owned_tiles;
    }
}