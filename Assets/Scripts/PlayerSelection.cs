using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelection : MonoBehaviour
{
	public Dropdown ddPlayers;

	private void Start() {
		theStateManager = GameObject.FindObjectOfType<StateManager>();

		if(theStateManager == null) {
			Debug.Log("FUCKING WHY!!!");
		}

		ddPlayers.onValueChanged.AddListener(delegate {
			ddPlayersValueChangeHappened(ddPlayers);
		});
	}

	StateManager theStateManager;

	public void ddPlayersValueChangeHappened(Dropdown sender){
		Debug.Log("Selection: " + sender.value);
		theStateManager.PlayerSetting = sender.value;
		theStateManager.SetPlayers();
	}

}
