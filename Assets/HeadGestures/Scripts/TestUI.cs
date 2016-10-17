/* TestUI.cs
 * (c) 2014 Quantum Leap Computing (QLC)
 * Author: Dave Arendash
 */
 
 using UnityEngine;
using System.Collections;

public class TestUI : MonoBehaviour {

	enum Response { no, yes, dontknow };
	Response response = Response.no;
	bool gotResponse = false;
	Response response1 = Response.no;
	Response response2 = Response.no;
	bool gotResponse1 = false;
	bool gotResponse2 = false;
	int nods = 0;
	int shakes = 0;
	int sideNods = 0;

	// Use this for initialization
	void Start () {
		GetComponent<GUIText>().text = "Would you like to continue?";
	}
	
	// Update is called once per frame
	void Update () {
		if (!gotResponse1 && gotResponse)
		{
			response1 = response;
			gotResponse1 = true;
			gotResponse = false; // to wait for next response
			GetComponent<GUIText>().text = "Would you like to continue? "  + response1.ToString();
			GetComponent<GUIText>().text += "\nWould you like to start over?";
		}
		
		if (!gotResponse2 && gotResponse)
		{
			response2 = response;
			gotResponse2 = true;
			gotResponse = false;
			GetComponent<GUIText>().text = "Would you like to continue? "  + response1.ToString();
			GetComponent<GUIText>().text += "\nWould you like to start over? "  + response2.ToString ();
			if (response2 == Response.yes)
			{
				GetComponent<GUIText>().text = "Would you like to continue?";
				gotResponse1 = false;
				gotResponse2 = false;
			}
		}
		
	}
	
	void HeadFullNod(int fullNods)
	{
		gotResponse = true;
		response = Response.yes;
		nods = fullNods;
	}
	
	void HeadFullShake(int fullShakes)
	{
		gotResponse = true;
		response = Response.no;
		shakes = fullShakes;
	}
	
	void HeadFullSideNod(int fullSideNods)
	{
		gotResponse = true;
		response = Response.dontknow;
		sideNods = fullSideNods;
	}
	
	void OnGUI()
	{
		GUI.Label (new Rect (10,10,200,20), "Nods: "+nods);
		GUI.Label (new Rect (10,30,200,20), "Shakes: "+shakes);
		GUI.Label (new Rect (10,50,200,20), "SideNods: "+sideNods);
	}
}
