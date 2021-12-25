using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentPlayerDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        theStateManager = GameObject.FindObjectOfType<StateManager>();
        myText = GetComponent<Text>();
    }

    StateManager theStateManager;
    
    Text myText;

    string[] numberWords = {"White", "Black"};

    // Update is called once per frame
    void Update()
    {
       myText.text = "Current Player: " + numberWords[theStateManager.CurrentPlayerId]; 
    }
}
