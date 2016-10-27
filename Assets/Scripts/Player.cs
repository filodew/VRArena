using UnityEngine;
using System.Collections;

using VRStandardAssets.Utils;

public class Player : MonoBehaviour {

    public float StunTime;

    private float CurrentStunTime;
    private bool _IsStunned;
    private bool IsStunActive;

    Renderer mShootingWeaponRenderer;

    public bool IsStunned
    {
        get
        {
            return _IsStunned;
        }

        set
        {
            _IsStunned = value;
        }
    }

    private VRInput Controller;

	// Use this for initialization
	void Start () 
    {
        Controller = GetComponent<VRInput>();
        if (Controller == null)
        {
            Debug.Log("FATAL ERROR ! CONTROLLER IS NOT FOUND !");
            return;
        }

        GameObject ShootingWeapon = GameObject.Find("ShooterFPSWeapon");
        if (ShootingWeapon == null)
        {
            Debug.Log("FATAL ERROR ! SHOOTING WEAPON NOT FOUND !");
            return;
        }
        mShootingWeaponRenderer = (Renderer) ShootingWeapon.GetComponent<Renderer>();
        if (mShootingWeaponRenderer == null)
        {
            Debug.Log("FATAL ERROR ! SHOOTING WEAPON RENDERER NOT FOUND ");
            return;
        }

	}
	
	// Update is called once per frame
	void Update () {
	    
        if (IsStunned == true)
        {
            if (IsStunActive == false)
            {
                PreStun();
                IsStunActive = true;
            }

            CurrentStunTime += Time.deltaTime;

            if (CurrentStunTime >= StunTime)
            {
                PostStun();
                IsStunActive = false;
                IsStunned = false;
                CurrentStunTime = 0;
            }

        }

	}

    private void PreStun()
    {
        Controller.ShootingDisabled = true;
        mShootingWeaponRenderer.material.SetColor("_EmissionColor", Color.red);

    }

    private void PostStun()
    {
        Controller.ShootingDisabled = false;
        mShootingWeaponRenderer.material.SetColor("_EmissionColor", Color.black);
    }
}
