using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        theStateManager = GameObject.FindObjectOfType<StateManager>();
    }

    StateManager theStateManager;

    // Update is called once per frame
    void Update()
    {
        string winner = null;
        if(theStateManager.CurrentPlayerId == 0) {
            winner = "White";
        } else {
            winner = "Black";
        }
        GetComponent<Text>().text = winner + " Player Wins!";
    }
}
