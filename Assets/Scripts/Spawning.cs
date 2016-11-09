using UnityEngine;
using System.Collections;
using VRStandardAssets.ShootingGallery;

[RequireComponent(typeof(AudioSource))]
public class Spawning : MonoBehaviour {

    public GameObject PrefabToSpawn;
    public GameObject StunProgressBarPrefab;
    public Vector3 StunProgressBarPrefabOffset;
    public string[] SpawningPointsNames;

    public float SpawnDelay;
    public float SpawnedObjectLifespan;

    private ArrayList SpawningPoints = new ArrayList();
    private GameObject SpawnedObject;
    private GameObject SpawnedStunProgressBar;
    private float TimeTillSpawn;
    private int LastSpawnPointIndex = -1;

    private bool _IsSpawnedObjectAlive = false;
    private ShootingGalleryController SGCC;

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

        GameObject SGC = GameObject.Find("ShootingGalleryController");
        if (SGC == null)
        {
            Debug.Log("FATAL ERROR ! ShootingGalleryController NOT FOUND !");
        }

        SGCC = (ShootingGalleryController) SGC.GetComponent("ShootingGalleryController");
        if(SGCC == null)
        {
            Debug.Log("FATAL ERROR ! ShootingGalleryControllerComponent NOT FOUND !");
        }
    }

	// Update is called once per frame
	void Update () {
	    
        if (SGCC.IsPlaying == false)
        {
            return;
        }

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
            ObjectComponent.SpawnedBy = this;
            AudioSource Speaker = GetComponent<AudioSource>();
            CubeObject CubeObj = SpawnedObject.GetComponent<CubeObject>();
            if (CubeObj != null)
            {
                CubeObj.StunnerProgressBar = SpawnStunProgressBar();
            }
            else
            {
                Debug.Log("Spawned obj is not of type CubeObject.");
            }
            Speaker.Play();
        }
        else
        {
            Debug.Log("FATAL ERROR ! Spawned object dont have Spawnable component.");
        }

        IsSpawnedObjectAlive = true;
    }

    private StunnerProgressBar SpawnStunProgressBar()
    {
        Vector3 TargetPosition = Camera.main.WorldToViewportPoint(SpawnedObject.transform.position);
        Renderer SpawnedObjectRenderer = SpawnedObject.GetComponent<Renderer>();
        if (SpawnedObjectRenderer == null)
        {
            Debug.Log("Error. SpawnedObject Renderer is null.");
            return null;
        }

        Vector3 ObjectSize = new Vector3(0.0f, SpawnedObjectRenderer.bounds.size.y, 0.0f);

        SpawnedStunProgressBar = (GameObject) Instantiate(StunProgressBarPrefab, SpawnedObject.transform.position + ObjectSize / 2 + StunProgressBarPrefabOffset, SpawnedObject.transform.rotation);

        return SpawnedStunProgressBar.GetComponent<StunnerProgressBar>();
    }

    private void DestroyStunProgressBar()
    {
        if(SpawnedStunProgressBar == null)
        {
            Debug.Log("FATAL ERROR ! Stun progress bar is null.");
            return;
        }

        Destroy(SpawnedStunProgressBar);
    }

    void OnSpawnedObjectDestroyed(Spawnable InObject)
    {
        IsSpawnedObjectAlive = false;
        DestroyStunProgressBar();
        Spawnable.OnDestroyed -= OnSpawnedObjectDestroyed;
    }
}

