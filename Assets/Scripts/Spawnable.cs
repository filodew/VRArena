using UnityEngine;
using System.Collections;

public class Spawnable : MonoBehaviour {

    public delegate void OnDestroyedDelegate (Spawnable InObject);
	public static event OnDestroyedDelegate OnDestroyed;

    private float _TimeLifespan;
    private bool _WasSpawned;

    private Spawning _SpawnedBy;

    public Spawning SpawnedBy
    {
        get { return _SpawnedBy; }
        set { _SpawnedBy = value; }
    }

    public float TimeLifespan 
    { 
        get { return _TimeLifespan; }
        set { _TimeLifespan = value; }
    }

    public bool WasSpawned 
    { 
        get { return _WasSpawned; }
        set { _WasSpawned = value; }
    }

	private float CurrentLifespan;

	// Use this for initialization
	void Start () {
        CurrentLifespan = 0;
	}
	
	// Update is called once per frame
	void Update () {

        if (_WasSpawned == true)
        {
            CurrentLifespan += Time.deltaTime;

            if (CurrentLifespan >= TimeLifespan) 
            {
                CurrentLifespan = 0;
                _WasSpawned = false;
                StunPlayer();
                DestroySpawnable();
            }
        }

	}

    void StunPlayer()
    {
        GameObject PlayerObject = GameObject.Find("MainCamera");

        if (PlayerObject == null)
        {
            Debug.Log("FATAL ERROR ! PLAYER OBJECT NOT FOUND !");
            return;
        }

        Player PlayerComponent = PlayerObject.GetComponent<Player>();

        if (PlayerComponent == null)
        {
            Debug.Log("FATAL ERROR ! PLAYER COMPONENT NOT FOUND !");
            return;
        }

        PlayerComponent.IsStunned = true;
    }

    void OnDestroy()
    {
        GameObject SpawnManager = GameObject.Find("SpawnManager");
        if (SpawnManager == null)
        {
            Debug.Log("FATAL ERROR ! SPAWN MANAGER IS NOT FOUND !");

        }

        Spawning SpawningComponent = SpawnManager.GetComponent<Spawning>();

        if (SpawningComponent == null)
        {
            Debug.Log("FATAL ERROR ! SPAWNING COMPONENT IS NOT FOUND !");
        }

        SpawningComponent.IsSpawnedObjectAlive = false;
            
    }

    public void DestroySpawnable()
    {
        OnDestroyed (this);
        Destroy(gameObject);
    }
}
