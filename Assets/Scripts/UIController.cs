using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class UIController : MonoBehaviour
{
	public static int displayPop = 0;
	public static int displayMaxPop = 0;
	public static int displayMoney = 0;
	public static int displayMoneyIncrease = 0;
	public static int displayMunitions = 0;
	public static int displayMunitionsIncrease = 0;
	public static int displayFuel = 0;
	public static int displayFuelIncrease = 0;

	public UnityEngine.UI.Text popText;
	public UnityEngine.UI.Text moneyText;
	public UnityEngine.UI.Text muntionsText;
	public UnityEngine.UI.Text fuelText;
	public UnityEngine.UI.Text debugText;
	public UnityEngine.UI.Text debugText2;
	public UnityEngine.UI.Text debugText3;
	public UnityEngine.UI.Text chatInput;
	public UnityEngine.UI.Text chatText;
	public UnityEngine.UI.Text svTimeText;
	public UnityEngine.UI.Text teamText;
	public GameObject InGameMenu;
	public GameObject ChooseTeamMenu;
	public GameObject OptionsMenu;
	public GameObject CreditsMenu;
	public GameObject networkDisplay;

	void Start()
	{
		InitNetworkDisplay();
	}

	void Update()
	{
		UpdateNetworkDisplay();

		popText.text = "Pop: " + displayPop + "/" + displayMaxPop;//"Pop: " + xa.pop + "/" + xa.maxPop;
		moneyText.text = "Gold: " + displayMoney + " (+" + displayMoneyIncrease + ")";
		muntionsText.text = "Muntions: " + displayMunitions + " (+" + displayMunitionsIncrease + ")";
		fuelText.text = "Fuel: " + displayFuel + " (+" + displayFuelIncrease + ")";
		debugText.text = xa.debugStr;

		if (hl.globalInfo != null)
		{
			if (hl.globalInfo.currentLevel != null)
			{
				debugText3.text = "CurrentLevel: " + hl.globalInfo.currentLevel;
			}
			else
			{
				debugText3.text = "CurrentLevel is null";
			}
		}
		else
		{
			debugText3.text = "GlobalInfo is null";
		}

		debugText3.text += "\nLocal uId: " + hl.local_uId;



		string s = "Peer: ";
		if (hl.local_uId != -1)
		{
			s += "" + hl.local_uId + "\n";

			hl.Peer peer = hl.GetPeerForUID(hl.local_uId);
			if (peer != null)
			{
				s += "Team: " + peer.team;
			}
			else
			{
				s += "Team: Peer not found";
			}
		}
		else
		{
			s += " Peer is -1";
		}
		teamText.text = s;

		debugText2.text = PrintPeers().ToString();

		chatInput.text = ChatScript.inputStr;
		chatText.text = ChatScript.chatStr;

		svTimeText.text = "sv: " + hl.svTime + "\ncl: " + hl.clTime;
	}

	StringBuilder PrintPeers()
	{
		StringBuilder s = new StringBuilder();
		s.Append("Peers: \n");
		if (hl.peers == null) { s.Append("  is null"); return s; }
		if (hl.peers.Count == 0) { s.Append("  Count: 0"); return s; }
		for (int i = 0; i < hl.peers.Count; i++)
		{
			s.Append("Peer, uId: " + hl.peers[i].uId + ", Team: " + hl.peers[i].team + ", Pos: " + hl.peers[i].teamPlayerNum);
			s.Append("\n");
		}

		return s;
	}

	public void ClickedOnIGM()
	{
		if (InGameMenu.activeSelf)
		{
			InGameMenu.SetActive(false);
		}
		else
		{
			InGameMenu.SetActive(true);
		}
	}

	public void CloseIGM()
	{
		TurnOffInGameMenuSubTabs();
		InGameMenu.SetActive(false);
	}

	public void ClickedOnCT()
	{
		TurnOffInGameMenuSubTabs();

		ChooseTeamMenu.SetActive(true);
	}

	public void ClickedOnOptions()
	{
		TurnOffInGameMenuSubTabs();

		OptionsMenu.SetActive(true);
	}

	public void ClickedOnCredits()
	{
		TurnOffInGameMenuSubTabs();

		CreditsMenu.SetActive(true);
	}

	void TurnOffInGameMenuSubTabs()
	{
		ChooseTeamMenu.SetActive(false);
		OptionsMenu.SetActive(false);
		CreditsMenu.SetActive(false);
	}

	public void ToggleMusic()
	{
		xa.muteMusic = 1 - xa.muteMusic;
	}

	public void ToggleSounds()
	{
		xa.muteSounds = 1 - xa.muteSounds;
	}

	public void ChooseTeam0()
	{
		hl.TryToChangeTeam(0);
	}
	public void ChooseTeam1()
	{
		hl.TryToChangeTeam(1);
	}
	public void ChooseTeam2()
	{
		hl.TryToChangeTeam(2);
	}

	public static List<GameObject> networkDisplayPool;

	void InitNetworkDisplay()
	{
		networkDisplayPool = new List<GameObject>();
		for (int i = 0; i < 120; i++)
		{
			GameObject go = Instantiate(xa.de.networkDisplayPrefab);
			go.layer = LayerMask.NameToLayer("HUDstuff");
			go.transform.position = networkDisplay.transform.position;
			go.transform.AddX(i);
			go.transform.parent = networkDisplay.transform;
			networkDisplayPool.Add(go);

		}
	}

	void UpdateNetworkDisplay()
	{
		for (int i = 0; i < networkDisplayPool.Count; i++)
		{
			networkDisplayPool[i].transform.SetScaleY(Random.Range(0.1f, 10f));
			networkDisplayPool[i].transform.SetY(networkDisplay.transform.position.y + (networkDisplayPool[i].transform.localScale.y * 0.5f));
		}
	}
}
