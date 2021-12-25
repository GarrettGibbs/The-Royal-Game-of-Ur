using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StateManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerAIs = new AIPlayer[NumberOfPlayers];
    }

    public int NumberOfPlayers = 2;
    public int CurrentPlayerId = 0;
    
    public int PlayerSetting = 0;

    AIPlayer[] PlayerAIs;
    
    public int DiceTotal;

    public bool IsPlaying = false;
    public bool IsDoneRolling = false;
    public bool IsDoneClicking = false;
    public bool GameOver = false;
    //public bool IsDoneAnimating = false;
    public int AnimationsPlaying = 0;

    public GameObject NoLegalMovesPopup;
    public GameObject Menu;
    public GameObject GameMenu;
    public GameObject RulesMenu;
    public GameObject WinMessage;
    public GameObject Diagram;
    public AudioSource VictorySFX;

    public void SetPlayers() {
        //PlayerAIs[0] = new AIPlayer_UtilityAI(); //null is human player
        //PlayerAIs[0] = null; //null is human player
        //PlayerAIs[1] = new AIPlayer_UtilityAI();
        if(PlayerSetting == 0) {
            PlayerAIs[0] = null;
            PlayerAIs[1] = null;
        } else if (PlayerSetting == 1) {
            PlayerAIs[0] = null;
            PlayerAIs[1] = new AIPlayer();
        } else if (PlayerSetting == 2) {
            PlayerAIs[0] = null;
            PlayerAIs[1] = new AIPlayer_UtilityAI();
        } else if (PlayerSetting == 3) {
            PlayerAIs[0] = new AIPlayer_UtilityAI();
            PlayerAIs[1] = new AIPlayer_UtilityAI();
        }
    }

    public void NewTurn() {
       //This is the start of a player turn, no roll yet
       IsDoneRolling = false;
       IsDoneClicking = false;
       //IsDoneAnimating = false;

       CurrentPlayerId = (CurrentPlayerId + 1) % NumberOfPlayers;
    }

    public void RollAgain() {
        IsDoneRolling = false;
        IsDoneClicking = false;
        //IsDoneAnimating = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(IsPlaying == false) {
            //not playing yet
            return;
        }

        //is turn done?
        if(IsDoneRolling && IsDoneClicking && AnimationsPlaying == 0) {
            NewTurn();
            return;
        }

        if(PlayerAIs[CurrentPlayerId] != null) {
            PlayerAIs[CurrentPlayerId].DoAI();
        }
    }

    public void CheckLegalMoves() {
        if(DiceTotal == 0) {
            StartCoroutine(NoLegalMoveCoroutine());
            return;
        }
        PlayerStone[] pss = GameObject.FindObjectsOfType<PlayerStone>();
        bool hasLegalMove = false;
        foreach(PlayerStone ps in pss) {
            if(ps.PlayerId == CurrentPlayerId){
                if(ps.CanLegallyMoveAhead(DiceTotal)) {
                    hasLegalMove = true;
                }
            }
        }
        if(hasLegalMove == false) {
            StartCoroutine(NoLegalMoveCoroutine());
            return;
        }
    }

    IEnumerator NoLegalMoveCoroutine(){
        NoLegalMovesPopup.SetActive(true);
        yield return new WaitForSeconds(1f);
        NoLegalMovesPopup.SetActive(false);
        NewTurn();
    }

    public void StartGame() {
        Menu.SetActive(false);
        GameMenu.SetActive(true);
        IsPlaying = true;
    }
    public void OpenMenu() {
        Menu.SetActive(true);
        GameMenu.SetActive(false);
        IsPlaying = false;
    }

    public void OpenRules() {
        RulesMenu.SetActive(true);
        Menu.SetActive(false);
    }

    public void CloseRules() {
        RulesMenu.SetActive(false);
        Menu.SetActive(true);
    }

    public void OpenDiagram() {
        if(Diagram.activeSelf == true) {
             Diagram.SetActive(false);
        } else {
            Diagram.SetActive(true);
        }
    }

    public IEnumerator CheckWinCondition() {
        PlayerStone[] pss = GameObject.FindObjectsOfType<PlayerStone>();
        int playerScore = 0;
        foreach(PlayerStone ps in pss) {
            if(ps.PlayerId == CurrentPlayerId){
                if(ps.HasBeenScored == true) {
                    playerScore++;
                }
            }
        }
        //Debug.Log(playerScore);
        if(playerScore == 6) {
            GameOver = true;
            WinMessage.SetActive(true);
            IsPlaying = false;
            VictorySFX.Play();
            yield return new WaitForSeconds(5f);
            ResetGame();
        }
    }

    public void ResetGame(){
        SceneManager.LoadScene("GameScene");
    }
}
