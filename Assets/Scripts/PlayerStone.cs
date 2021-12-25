using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStone : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        theStateManager = GameObject.FindObjectOfType<StateManager>();

        targetPosition = this.transform.position;
    }

    public Tile StartingTile;
    public Tile CurrentTile {get; protected set;}

    public int PlayerId;
    public StoneStorage MyStoneStorage;

    public bool HasBeenScored = false;

    bool ScoreMe = false;

    StateManager theStateManager;

    Tile[] moveQueue;
    int moveQueueIndex;

    bool isAnimating = false;

    Vector3 targetPosition;
    Vector3 velocity;
    float smoothTime = .2f;
    float smoothTimeVertical = .1f;
    float smoothDistance = .01f;
    float smoothHeight = .5f;

    PlayerStone stoneToBop;

    // Update is called once per frame
    void Update()
    {
        if (isAnimating == false) {
            return;
        }
        
        if(Vector3.Distance(new Vector3(this.transform.position.x, targetPosition.y, this.transform.position.z), targetPosition) < smoothDistance){
            //reached the target, do we still have moves in queue?
            if((moveQueue == null || moveQueueIndex == (moveQueue.Length)) && (this.transform.position.y-smoothDistance) > targetPosition.y) {
                //out of moves need to drop down
                this.transform.position = Vector3.SmoothDamp(this.transform.position, new Vector3(this.transform.position.x, targetPosition.y, this.transform.position.z), ref velocity, smoothTimeVertical);
                if(stoneToBop != null) {
                    stoneToBop.ReturnToStorage();
                    stoneToBop = null;
                }
            } else {
                //right position right height
                AdvanceMoveQueue();
            }  
        } 
        //rise before moving sideways
        else if(this.transform.position.y < (smoothHeight - smoothDistance)) {
             this.transform.position = Vector3.SmoothDamp(this.transform.position, new Vector3(this.transform.position.x, smoothHeight, this.transform.position.z), ref velocity, smoothTimeVertical);
        } else {
            this.transform.position = Vector3.SmoothDamp(this.transform.position, new Vector3(targetPosition.x, smoothHeight, targetPosition.z), ref velocity, smoothTime);
        }
    }

    void AdvanceMoveQueue() {
         if (moveQueue != null && moveQueueIndex < moveQueue.Length) { 
                Tile nextTile = moveQueue[moveQueueIndex];
                if(nextTile == null) {
                    //probably scoring???
                    //TODO move to score pile
                    Debug.Log("Scoring Tile!");
                    SetNewTargetPosition(this.transform.position + Vector3.right*10f);
                } else {
                    SetNewTargetPosition(nextTile.transform.position);
                moveQueueIndex++;
                }
            } else {
                //move queue is empty
                this.isAnimating = false;
                theStateManager.AnimationsPlaying--;

                //roll again tile?
                if(CurrentTile != null && CurrentTile.IsRollAgain == true){
                    theStateManager.RollAgain();
                }
                if(CurrentTile != null) {
                    if(CurrentTile.IsScoringSpace == true) {
                    this.HasBeenScored = true; 
                    Rigidbody stoneRigidbody = this.GetComponent<Rigidbody>();
                    stoneRigidbody.isKinematic = false;

                    StartCoroutine(theStateManager.CheckWinCondition());
                    }
                }
            }
    }

    void SetNewTargetPosition(Vector3 pos) {
        targetPosition = pos;
        velocity = Vector3.zero;
        isAnimating = true;
    }

    void OnMouseUp() {
    //TODO is the mouse over a UI element? if so ignore click
        MoveMe();
    }
    
    public void MoveMe() {
        if(theStateManager.CurrentPlayerId != PlayerId) {
            return;
            //wrong stone
        }

        if (theStateManager.IsDoneRolling == false) {
            //cant move yet
            return;
        }
        if(theStateManager.IsDoneClicking == true) {
            //we've already done a move 
            return;
        }

        int spacesToMove = theStateManager.DiceTotal;

        if (spacesToMove == 0) {
            return;
        }

        //Where should we end up?

        moveQueue = GetTilesAhead(spacesToMove);
        Tile finalTile = moveQueue[moveQueue.Length-1];
        

        if(finalTile == null) {
            //we are scoring this stone
            ScoreMe = true;
            return;
        }
        else {
            if(CanLegallyMoveTo(finalTile) == false) {
                finalTile = CurrentTile;
                moveQueue = null;
                return;
            }
            if(finalTile.PlayerStone != null){
                //finalTile.PlayerStone.ReturnToStorage();
                stoneToBop = finalTile.PlayerStone;
                stoneToBop.CurrentTile.PlayerStone = null;
                stoneToBop.CurrentTile = null;
            }
        }

        this.transform.SetParent(null); //become Batman
        
        if(CurrentTile != null){
            CurrentTile.PlayerStone = null;
        }
        
        //even before animation is done set current tile to the new tile
        CurrentTile = finalTile;
        if(finalTile.IsScoringSpace == false) {
            //does not add stone to tile if final tile
            finalTile.PlayerStone = this;
        }

        moveQueueIndex = 0;
        theStateManager.IsDoneClicking = true;
        this.isAnimating = true;
        theStateManager.AnimationsPlaying++;
    }

    //returns lists of tiles ahead of stone
    Tile[] GetTilesAhead(int spacesToMove){
         if (spacesToMove == 0) {
            return null;
        }

        //Where should we end up?

        Tile[] listOfTiles = new Tile[spacesToMove];
        Tile finalTile = CurrentTile;
        
        for (int i = 0; i < spacesToMove; i++) {
            if(finalTile == null) {
               finalTile = StartingTile;
            } else {
                if(finalTile.NextTiles == null || finalTile.NextTiles.Length == 0){
                    break;
                } else if (finalTile.NextTiles.Length > 1) {
                    //branch based on player ID
                    finalTile = finalTile.NextTiles[PlayerId];
                } else {
                    finalTile = finalTile.NextTiles[0];
                }
            }
            listOfTiles[i] = finalTile;
        }
        return listOfTiles;
    }

    public Tile GetTileAhead () {
        return GetTileAhead (theStateManager.DiceTotal);
    }
    
    public Tile GetTileAhead (int spacesToMove) {
        Tile[] tiles = GetTilesAhead(spacesToMove);
        if(tiles == null){
            return CurrentTile;
        }
        return tiles[tiles.Length-1];
    }

    public bool CanLegallyMoveAhead( int spacesToMove ){
        if(CurrentTile != null && CurrentTile.IsScoringSpace == true){
            //in scoring Tile
            return false;
        }
        Tile theTile = GetTileAhead(spacesToMove);
        return CanLegallyMoveTo(theTile);
    }

    bool CanLegallyMoveTo(Tile destinationTile) {
        if(destinationTile == null){
            //pvershooting the victory roll, not legal
            return false;
        }
        
        if(destinationTile.PlayerStone == null){
            return true;
        }
        if(destinationTile.PlayerStone.PlayerId == this.PlayerId){
            return false;
        }
        if(destinationTile.IsRollAgain == true){
            return false;
            //safe squares??
        }
        return true;
    }

    public void ReturnToStorage() {
        //CurrentTile.PlayerStone = null;
        //CurrentTile = null;
        this.isAnimating = true;
        theStateManager.AnimationsPlaying++;

        moveQueue = null;
        Vector3 savePosition = this.transform.position;
        MyStoneStorage.AddStoneToStorage(this.gameObject);
        SetNewTargetPosition(this.transform.position);
        this.transform.position = savePosition;

        //TODO maybe animate??
    }

}
