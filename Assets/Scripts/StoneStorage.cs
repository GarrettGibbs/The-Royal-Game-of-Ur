using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneStorage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Create a stone for each placeholder spot
        for (int i = 0; i < this.transform.childCount; i++) {
            GameObject theStone = Instantiate(StonePrefab); 
            theStone.GetComponent<PlayerStone>().StartingTile = this.StartingTile;
            theStone.GetComponent<PlayerStone>().MyStoneStorage = this;
            AddStoneToStorage(theStone, this.transform.GetChild(i));
        }

    }

    public GameObject StonePrefab;
    public Tile StartingTile;


    public void AddStoneToStorage(GameObject theStone, Transform thePlaceholder = null) {
        //find first emtpy placeholder, parent and reset local position in placeholder
        if(thePlaceholder == null) {
            for (int i = 0; i < this.transform.childCount; i++) {
                Transform p = this.transform.GetChild(i);
                if(p.childCount == 0) {
                    thePlaceholder = p;
                    break;
                }
            }

           // if(thePlaceholder == null){
              //  Debug.LogError("Trying to add a stone, but no empty places?!?!");
               // return;
            //}
        }
        theStone.transform.SetParent(thePlaceholder);
        theStone.transform.localPosition = Vector3.zero;
        
    }
}
