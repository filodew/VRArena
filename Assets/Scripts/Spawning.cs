using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Spawning : MonoBehaviour {

    public GameObject PrefabToSpawn;
    public string[] SpawningPointsNames;

    public float SpawnDelay;
    public float SpawnedObjectLifespan;

    private ArrayList SpawningPoints = new ArrayList();
    private GameObject SpawnedObject;
    private float TimeTillSpawn;
    private int LastSpawnPointIndex = -1;

    private bool _IsSpawnedObjectAlive = false;

    public bool IsSpawnedObjectAlive
    {
        get { return _IsSpawnedObjectAlive; }
        set { _IsSpawnedObjectAlive = value; }
    }

    private GameObject Player;

	// Use this for initialization
	void Start () {
        InitializeSpawners();
        Player = GameObject.Find("ShooterWeapon");
    }
	
	// Update is called once per frame
	void Update () {
	    
        if(IsSpawnedObjectAlive == false)
        {
            TimeTillSpawn += Time.deltaTime;
        }

        if(TimeTillSpawn >= SpawnDelay)
        {
            GameObject PickedSpawnPoint = PickSpawnPoint();

            if(PrefabToSpawn != null)
            {
                SpawnObject(PickedSpawnPoint);
            }
            else
            {
                Debug.Log("Object type to spawn is not set !");
                return;
            }

            TimeTillSpawn = 0;
        }

	}

    private void InitializeSpawners()
    {
        foreach (string Name in SpawningPointsNames)
        {
            GameObject Obj = GameObject.Find(Name);
            if (Obj != null)
            {
                SpawningPoints.Add(Obj);
            }
        }
    }

    private GameObject PickSpawnPoint()
    {
        int CurrentSpawnPointIndex = -1;

        do
        {
            CurrentSpawnPointIndex = Random.Range(0, SpawningPoints.Capacity);
        }
        while (CurrentSpawnPointIndex == LastSpawnPointIndex);

        LastSpawnPointIndex = CurrentSpawnPointIndex;
        GameObject ReturnObj = (GameObject) SpawningPoints[CurrentSpawnPointIndex];
        return ReturnObj;
    }

    private void SpawnObject(GameObject InSpawnPoint)
    {
        SpawnedObject = (GameObject) Instantiate(PrefabToSpawn, InSpawnPoint.transform.position, InSpawnPoint.transform.rotation);
        SpawnedObject.transform.LookAt(Player.transform.position);
        Spawnable ObjectComponent = SpawnedObject.GetComponent<Spawnable>();
        if (ObjectComponent != null)
        {
            ObjectComponent.TimeLifespan = SpawnedObjectLifespan;
            ObjectComponent.WasSpawned = true;
            Spawnable.OnDestroyed += OnSpawnedObjectDestroyed;
            AudioSource Speaker = GetComponent<AudioSource>();
            Speaker.Play();
        }
        else
        {
            Debug.Log("FATAL ERROR ! Spawned object dont have Spawnable component.");
        }

        IsSpawnedObjectAlive = true;
    }

    void OnSpawnedObjectDestroyed(Spawnable InObject)
    {
        IsSpawnedObjectAlive = false;
        Spawnable.OnDestroyed -= OnSpawnedObjectDestroyed;
    }
}

