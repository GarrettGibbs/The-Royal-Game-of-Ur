using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public void QuitGame(){
        Debug.Log("Quitting the Game now.");
        Application.Quit();
    }
}
