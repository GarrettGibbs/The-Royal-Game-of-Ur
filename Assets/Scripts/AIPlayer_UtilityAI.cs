using UnityEngine;
using System.Collections.Generic;

public class AIPlayer_UtilityAI : AIPlayer
{
	Dictionary<Tile, float> tileDanger;
	
	float aggressivenessBonus = .25f; //gets added for bops and removed for staying on safe spaces
	
	override protected PlayerStone PickStoneToMove(PlayerStone[] legalStones ) {
		//Debug.Log("Using utility logic");

		if(legalStones == null || legalStones.Length == 0) {
			Debug.LogError("Being asked to pick from no stones!?!");
			return null;
		}

		CalcTileDanger(legalStones[0].PlayerId);

		//give each legall stone a value, 1 is super awesome -1 is horrible
		PlayerStone bestStone = null;
		float goodness = -Mathf.Infinity;

		foreach(PlayerStone ps in legalStones) {
			float g = GetStoneGoodness(ps, ps.CurrentTile, ps.GetTileAhead());
			if(bestStone == null || g > goodness) {
				bestStone = ps;
				goodness = g;
			}
		}
		Debug.Log("Chosen Stone Goodness: " + goodness);
		return bestStone;
	}

	virtual protected void CalcTileDanger(int myPlayerId) {
		tileDanger = new Dictionary<Tile, float>();
		Tile[] tiles = GameObject.FindObjectsOfType<Tile>();

		foreach(Tile t in tiles) {
			tileDanger[t] = 0;
		}

		PlayerStone[] allStones = GameObject.FindObjectsOfType<PlayerStone>();
		foreach(PlayerStone stone in allStones) {
			//if enemy stone add a danger value to tiles in front of it, unless a safe tile
			if(stone.PlayerId == myPlayerId) {
				continue;
			}
			for (int i = 1; i <= 4; i++) {
				Tile t = stone.GetTileAhead(i);
				if(t == null) {
					break;
				}
				if(t.IsScoringSpace || t.IsSideline || t.IsRollAgain) {
					//this tile is not a danger zone, so we can ignore it
					continue;
				}
				//this tile is within bopping range of enemy, so is dangerous
				if(i == 2) {
					tileDanger[t] += 0.3f;
				} else if (i == 4) {
					tileDanger[t] += 0.15f;
				} else {
					tileDanger[t] += 0.2f;
				}
			}
		}
	}

	virtual protected float GetStoneGoodness( PlayerStone stone, Tile currentTile, Tile futureTile ) {
		//float goodness = Random.Range(-0.1f,0.1f);
		float goodness = 0;

		if(currentTile == null) {
			//not on board yet, always nice to add more to the board
			goodness += .10f;
		}

		if(currentTile != null && (currentTile.IsRollAgain == true && currentTile.IsSideline == false)) {
			//sitting in roll again middle space, resist because it blocks space form opponent
			goodness -= .10f;
		}
		if(futureTile.IsRollAgain == true) {
			goodness += .50f;
		}
		if(futureTile.PlayerStone != null && futureTile.PlayerStone.PlayerId != stone.PlayerId) {
			//enemy stone to bop
			goodness += .50f;
		}
		if(futureTile.IsScoringSpace == true) {
			goodness += .50f;
		}

		float currentDanger = 0;
		if(currentTile != null) {
			currentDanger = tileDanger[currentTile];
		}

		goodness += currentDanger - tileDanger[futureTile];

		//TODO: add goodness for behind enemys with future boppage potential
		//TODO: add goodness for moving a stone forward when we might be blocking friendly

		return goodness;
	}

}
