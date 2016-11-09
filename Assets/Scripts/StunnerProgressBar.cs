using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StunnerProgressBar : MonoBehaviour {

    public Slider ProgressBar;
    private GameObject Enemy;

    private float _PreValue;
    private float _PostValue;

    public float PreValue
    {
        get { return _PreValue; }
        set { _PreValue = value; }
    }

    public float PostValue
    {
        get { return _PostValue; }
        set { _PostValue = value; }
    }

    public GameObject _Enemy
    {
        get { return Enemy; }
        set { Enemy = value; }
    }

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update () {
        ProgressBar.value = Mathf.Lerp(ProgressBar.value, _PostValue, Time.deltaTime);
	}
}
