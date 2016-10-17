#pragma strict



function Update ()
{
	if (Input.GetKey(KeyCode.Escape) || Input.GetButtonDown("Back"))
	{
		Application.Quit();
	}

}