using UnityEngine;
using System.Collections;

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
	public UnityEngine.UI.Text chatInput;
	public UnityEngine.UI.Text chatText;
	public UnityEngine.UI.Text svTimeText;
	public GameObject InGameMenu;
	public GameObject ChooseTeamMenu;
	public GameObject OptionsMenu;
	public GameObject CreditsMenu;

	void Update()
	{
		popText.text = "Pop: " + displayPop + "/" + displayMaxPop;//"Pop: " + xa.pop + "/" + xa.maxPop;
		moneyText.text = "Gold: " + displayMoney + " (+" + displayMoneyIncrease + ")";
		muntionsText.text = "Muntions: " + displayMunitions + " (+" + displayMunitionsIncrease + ")";
		fuelText.text = "Fuel: " + displayFuel + " (+" + displayFuelIncrease + ")";
		debugText.text = xa.debugStr;

		chatInput.text = ChatScript.inputStr;
		chatText.text = ChatScript.chatStr;

		svTimeText.text = "sv: " + hl.svTime + "\ncl: " + hl.clTime;

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

	}
