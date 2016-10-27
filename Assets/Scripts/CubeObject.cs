using UnityEngine;
using System.Collections;

using VRStandardAssets.Utils;

public class CubeObject : MonoBehaviour {

    public Rigidbody mRigidBody;
    public float    SwipeSensitivity = 1;
    public float RotationThreshold = 360.0f;

    private GameObject mPlayerCamera;
    private VREyeRaycaster mEyeRaycaster;
    private Vector3 AggregatedRotation; // Collects summary data on object rotation.

	// Use this for initialization
	void Start () {
        mPlayerCamera = GameObject.Find("MainCamera");
        if (mPlayerCamera != null)
        {
            mEyeRaycaster = mPlayerCamera.GetComponent<VREyeRaycaster>();
            if(mEyeRaycaster == null)
            {
                Debug.Log("FATAL ERROR ! EYE RAYCASTER NOT FOUND !");
            }
        }
        else
        {
            Debug.Log("FATAL ERROR ! PLAYER CAMERA NOT FOUND !");
        }
            
    }

	// Update is called once per frame
	void Update () {
        InputSwipe();

        if (AggregatedRotation.y >= RotationThreshold || AggregatedRotation.y <= -RotationThreshold)
        {
            Destroy(gameObject);
        }
	}
      
    void InputSwipe()
    {
        if (mEyeRaycaster.CurrentInteractible == null)
        {
            return;
        }

        if (mEyeRaycaster.CurrentInteractible.name == gameObject.name && Input.GetMouseButton(0) == true)
        {
            Vector3 CurrentRotationVector = (Vector3.up * -Input.GetAxis("Mouse X")) * SwipeSensitivity;

            mRigidBody.AddTorque(CurrentRotationVector);

            AggregatedRotation += CurrentRotationVector;
        }
            
    }
        
}
