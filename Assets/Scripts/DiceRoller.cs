using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceRoller : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //theStateManager = GameObject.FindObjectOfType<StateManager>();
        
        DiceValues = new int[4];
    }

    public StateManager theStateManager;

    public int[] DiceValues;

    public Sprite[] DiceImageOne;
    public Sprite[] DiceImageZero;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RollTheDice() {

        if (theStateManager.IsDoneRolling == true) {
            return;
            //already rolled this turn
        }
        //Roll the 4 techahedron dice which have half the corners have value of 1 and half 0

       theStateManager.DiceTotal = 0;

       for (int i = 0; i < DiceValues.Length; i++) {
            DiceValues[i] = Random.Range( 0, 2 );
            theStateManager.DiceTotal += DiceValues[i];
            
            //update visual for die Roll

            if(DiceValues[i] == 0) {
                this.transform.GetChild(i).GetComponent<Image>().sprite = DiceImageZero[Random.Range(0, DiceImageZero.Length)];
            } else {
                this.transform.GetChild(i).GetComponent<Image>().sprite = DiceImageOne[Random.Range(0, DiceImageOne.Length)];
            }
       }
       //theStateManager.DiceTotal = 15;
       theStateManager.IsDoneRolling = true;
       theStateManager.CheckLegalMoves();
    }
}
