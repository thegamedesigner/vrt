using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChatScript : MonoBehaviour
{
	public static string inputStr;
	public static string chatStr;
	bool enterTextMode = false;
	float timeSet = 0;
	bool showCursor = false;
	public static bool typing = false;
	
	string typingStr = "";
	void Update()
	{
		/*
		int index = 0;
		while (index < 8)
		{
			if ((strs.Count - 1 - index) >= 0)
			{
				str += strs[strs.Count - 1 - index];
			}

			index++;
		}*/

		if (Input.GetKeyDown(KeyCode.Return))
		{
			if (enterTextMode == false)
			{
				enterTextMode = true;
				typingStr = "";
				typing = true;
			}
			else
			{
				enterTextMode = false;
				typing = false;
				hl.hlObj.CmdChat(typingStr);
				typingStr = "";
				inputStr = typingStr;
			}
		}

		if (enterTextMode)
		{
			foreach (char c in Input.inputString)
			{
				if (c == "\b"[0])
				{
					if (typingStr.Length != 0)
					{
						typingStr = typingStr.Substring(0, typingStr.Length - 1);
					}
				}
				else
				{
					if (c == "\n"[0] || c == "\r"[0])
					{
					}
					else
					{
						typingStr += c;
					}
				}
			}
			inputStr = typingStr;

			float delay = 0.1f;
			if (showCursor)
			{
				delay = 0.1f;
			}
			if (Time.realtimeSinceStartup > (timeSet + delay))
			{
				timeSet = Time.realtimeSinceStartup;
				showCursor = !showCursor;
			}
			if (showCursor)
			{
				inputStr += "|";
			}
		}



	}

	public static void ChatLocallyAndLog(string str)
	{
		Debug.Log(str);
		ChatLocally(str);

	}
	public static void ChatLocally(string str)
	{
		chatStr += str + "\n";
	}

}
