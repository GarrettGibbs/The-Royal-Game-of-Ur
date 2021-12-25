using UnityEngine;
using System.Collections.Generic;

public class AIPlayer
{
	public AIPlayer(){
		stateManager = GameObject.FindObjectOfType<StateManager>();
	}

	StateManager stateManager;

	virtual public void DoAI() {
		if(stateManager.IsDoneRolling == false) {
			//roll the dice
			DoRoll();
			return;
		}
		if(stateManager.IsDoneClicking == false) {
			//have roll need to pick a stone
			DoClick();
			return;
		}
	}

	virtual protected void DoRoll() {
		GameObject.FindObjectOfType<DiceRoller>().RollTheDice();
	}
	virtual protected void DoClick() {
		//pick a stone to move then click it
		PlayerStone[] legalStones = GetLegalMoves();
		if(legalStones == null || legalStones.Length == 0) {
			//no legal moves, should never get here?! delay from coroutine?
			return;
		}
		// basic AI will simply pick a random move at random
		PlayerStone pickedStone = PickStoneToMove(legalStones);

		pickedStone.MoveMe();
	}

	virtual protected PlayerStone PickStoneToMove(PlayerStone[] legalStones ) {
		return legalStones[Random.Range(0, legalStones.Length)];
	}

	
	protected PlayerStone[] GetLegalMoves() {
		List<PlayerStone> legalStones = new List<PlayerStone>();     

		if(stateManager.DiceTotal == 0) {
            return legalStones.ToArray();
        }
        PlayerStone[] pss = GameObject.FindObjectsOfType<PlayerStone>();
        foreach(PlayerStone ps in pss) {
            if(ps.PlayerId == stateManager.CurrentPlayerId){
                if(ps.CanLegallyMoveAhead(stateManager.DiceTotal)) {
					legalStones.Add(ps);
                }
            }
        }
		return legalStones.ToArray();
    }

}
