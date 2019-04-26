using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileData : MonoBehaviour 
{

    [SerializeField]
    int stack_size = 0;

    public int TempStackSize { get; set; }

    [SerializeField]
    byte playerOwner = 0;

    [SerializeField]
    ushort index = 0;

    [SerializeField]
    bool blank = false;

    [SerializeField]
    Sprite[] tileSprites;

    [SerializeField]
    int tile_value = 0;

    byte direction = 0;

    [SerializeField]
    int empty_tiles = 0;

    public bool Selected { get; set; }

    [SerializeField]
    List<GameObject> accessible_tiles;

    private GameObject checker;

    public GameObject stack_text;

    public GameObject stack_text_shadow;

    Quaternion default_rotation;

	// Use this for initialization
	void Start () 
    {
        TempStackSize = 0;
        Selected = false;
        stack_text = this.transform.GetChild(0).gameObject;
        stack_text_shadow = stack_text.transform.GetChild(0).gameObject;
        checker = this.transform.GetChild(1).gameObject;
        default_rotation = this.transform.rotation;
    }
	
	// Update is called once per frame
	void Update () 
    {
        //Update tiles to reflect who owns them
        UpdateOwnership();

        //determine the empty tiles around a tile
        empty_tiles = GetSurroundingEmptyTiles();   
    }

    public int StackSize
    {
        get
        {
            return stack_size;
        }
        set
        {
            stack_size = value;
        }
    }

    public byte Direction
    {
        get
        {
            return direction;
        }
        set
        {
            direction = value;
        }
    }

    public byte GetOwner
    {
        get
        {
            return playerOwner;
        }
    }

    public void DestroyArrow()
    {
        //Revert to the empty tile tag and defaul rotation for the tile
        direction = 0;
        this.gameObject.tag = "Tile";
        this.transform.rotation = default_rotation;
    }

    public GameObject TileChecker
    {
        get
        {
            return checker;
        }
    }

    public ushort ID
    {
        get
        {
            return index;
        }
        set
        {
            index = value;
        }
    }

    public bool RemoveTile
    {
        get
        {
            return blank;
        }
        set
        {
            blank = value;
        }
    }

    public int TileValue
    {
        get
        {
            return tile_value;
        }
        set
        {
            tile_value = value;
        }
    }

    public List<GameObject> GetAccessibleTiles()
    {
        return accessible_tiles;
    }

    public void ChangeOwnership(byte new_owner)
    {
        //tile's tag and owner is changed to the defined player ID
        playerOwner = new_owner;
        this.gameObject.tag = "Sheep" + new_owner;
    }

    public void SelectTile()
    {
        if(!Selected)
        {
            Selected = true;
        }
    }

    public void DeselectTile()
    {
        if (Selected)
        {
            Selected = false;
        }
    }

    //Confirms ownership and applies relevant data accordingly
    public void UpdateOwnership()
    {
        //There are only 4 players. Tile becomes unknowned
        if (playerOwner > GameMenu.GAME_MODE)
        {
            playerOwner = 0;
        }

        //checks that the tile isn't a temporary directional arrow
        if(this.gameObject.tag != "Arrow")
        {
            //If the tile is owned by the player, can move and it's the player's turn
            if (this.playerOwner == 1 && StackSize > 1 && accessible_tiles.Count > 0 && GameData.playersTurn == 1)
            {
                //Change the sprite to the glowing variant
                this.gameObject.GetComponent<SpriteRenderer>().sprite = tileSprites[7];
            }
            else if(this.playerOwner == 2 && StackSize > 1 && accessible_tiles.Count > 0 && GameData.playersTurn == 2)
            {
                //Change the sprite to the glowing variant
                this.gameObject.GetComponent<SpriteRenderer>().sprite = tileSprites[8];
            }
            else if(this.playerOwner == 3 && StackSize > 1 && accessible_tiles.Count > 0 && GameData.playersTurn == 3)
            {
                //Change the sprite to the glowing variant
                this.gameObject.GetComponent<SpriteRenderer>().sprite = tileSprites[9];
            }
            else if(this.playerOwner == 4 && StackSize > 1 && accessible_tiles.Count > 0 && GameData.playersTurn == 4)
            {
                //Change the sprite to the glowing variant
                this.gameObject.GetComponent<SpriteRenderer>().sprite = tileSprites[10];
            }
            else
            {
                //Set each sprite relative to its owner
                this.gameObject.GetComponent<SpriteRenderer>().sprite = tileSprites[playerOwner];
            }      
        }
        else
        {
            //Make the tile's sprite the directional arrow
            this.gameObject.GetComponent<SpriteRenderer>().sprite = tileSprites[6];
            return;
        }

        //If the tile is hidden, there is no sprite.
        if (blank)
            this.gameObject.GetComponent<SpriteRenderer>().sprite = null;

        //If the tile is owned
        if (stack_size > 0 && playerOwner > 0)
        {
            //change the tile's tag and object name based on its owner
            gameObject.tag = "Sheep" + playerOwner;
            gameObject.name = "Tile_Sheep_" + playerOwner;

            //The tile is owned and occupied, show it's stack size
            stack_text.SetActive(true);
            stack_text_shadow.SetActive(true);

            /*If the stack size is present (for the human player)
            we display that instead of the regular stack so the player
            knows how much of the s tack to move.
            */
            if (TempStackSize > 0)
            {
                stack_text.GetComponent<TextMesh>().text = "" + TempStackSize + "/" + stack_size;
                stack_text_shadow.GetComponent<TextMesh>().text = "" + TempStackSize + "/" + stack_size;
            }
            //if not, just display the regular stack
            else
            {
                stack_text.GetComponent<TextMesh>().text = "" + stack_size;
                stack_text_shadow.GetComponent<TextMesh>().text = "" + stack_size;
            }
                
        }
        //If the tile is unknowned but not blank
        else if (playerOwner == 0 && blank == false)
        {
            //give it appropriate tags and hide stack size - there is none
            gameObject.tag = "Tile";
            gameObject.name = "Tile";
            stack_text.SetActive(false);
            stack_text_shadow.SetActive(false);
        }
        else
        {
            //hide tile entirely if its blank
            gameObject.tag = "Empty";
            gameObject.name = "Blank_Tile";
            stack_text.SetActive(false);
            stack_text_shadow.SetActive(false);
        }
    }

    public int GetSurroundingEmptyTiles()
    {
        Collider[] tiles = Physics.OverlapSphere(this.transform.position, 2.0f);

        int emptyTiles = 0;

        //checks how many unoccupied tiles are are around a tile
        for (int i = 0; i < tiles.Length; i++)
        {
            TileData td = tiles[i].GetComponent<TileData>();

            //if the tile has no owner and hasnt been removed its an empty tile
            if (td != this && td.GetOwner == 0 && td.blank == false)
            {
                emptyTiles++;
            }
        }
        return emptyTiles;
    }

    //function to check which tiles any tile could perform a valid move to
    public void UpdateAccessibleTiles()
    {
        accessible_tiles.Clear();

        //Indices equal to list of direcions
        bool[] movable_direction = new bool[6];
        Collider[] collision;

        //Loop through each direction of movement
        for (int i = 0; i < movable_direction.Length; i++)
        {
            collision = Physics.OverlapSphere(this.transform.position + Directions.directions[i], 0.5f);

            //If a collision exists at the direction
            if (collision.Length == 1)
            {
                //and the object is an empty tile
                if (collision[0].gameObject.tag == "Tile")
                {
                    //This tile can move in that direction
                    movable_direction[i] = true;
                }
                else
                {
                    //This tile cannot move in that direction
                    movable_direction[i] = false;
                }
            }
            collision = null;
        }

        //For each of the six directions
        for (int i = 0; i < movable_direction.Length; i++)
        {
            //If its a direction this tile can move in
            if(movable_direction[i] == true)
            {
                bool furthest_found = false;
                int last_checked_ID = 0;
                checker.transform.position = this.transform.position;
                Collider[] checked_tile = null;

                //Consistently check tiles in that direction
                while (!furthest_found)
                {
                    checker.transform.position += Directions.directions[i];
                    checked_tile = Physics.OverlapSphere(checker.transform.position, 0.5f);

                    //If one is present
                    if(checked_tile.Length == 1)
                    {
                        //and it is owned
                        if(checked_tile[0].gameObject.tag == "Tile")
                            //It is queeried
                            last_checked_ID = checked_tile[0].GetComponent<TileData>().ID;
                        else
                            //We have moved as far as we can
                            furthest_found = true;
                    }
                    else
                    {
                        furthest_found = true;
                    }
                }
                //The tile is added to the list of tiles reachable by this tile
                accessible_tiles.Add(Game.GetBoard()[last_checked_ID]);
            }
        }
    }

    public void SetOwnership(byte playerID)
    {
        //Give the player a new stack of 16 sheep.
        playerOwner = playerID;
        stack_size = 16;
        this.gameObject.tag = "Sheep" + playerID;
    }
}