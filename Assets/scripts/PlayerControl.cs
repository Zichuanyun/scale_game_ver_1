using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

  public SpeedConfig speedConfig;
  public int playerNumber;
  public int initialCreepNumber = 5;
  public GameObject creepPrefab;
  public CreepGenerationConfig creepGenerationConfig;
  public GameController gameController;
  public bool disableControl = false;

  [System.Serializable]
  public class SpeedConfig
  {
    public float moveSpeed;
    public float dashSpeed;
  }

  [System.Serializable]
  public class CreepGenerationConfig
  {
    public float creepGenerationRange;
    public float yOffset;
  }

  [SerializeField]
  private int creepCount = 0;

  private HashSet<CreepController> creeps;
  [HideInInspector]
  public new Rigidbody rigidbody;

  // Use this for initialization
  void Start () {
    rigidbody = GetComponent<Rigidbody>();
    creeps = new HashSet<CreepController>();
    for (int i = 0; i < initialCreepNumber; ++i) {
      GenNewCreep();
    }
  }
	
	// Update is called once per frame
	void Update () {
    if (Input.GetKeyDown(KeyCode.Return)) {
      GenNewCreep();
    }
	}

  void FixedUpdate() {
    float moveH = Input.GetAxis("Horizontal");
    float moveV = Input.GetAxis("Vertical");

    Vector3 movement = new Vector3(moveH, 0.0f, moveV);
    if (disableControl) {
      movement = new Vector3();
    }
    rigidbody.velocity = movement.normalized * speedConfig.moveSpeed;
  }

  public HashSet<CreepController> GetCreeps() {
    return creeps;
  }

  public void AddCreep(CreepController creep) {
    if (!creeps.Contains(creep)) {
      ++creepCount;
      creeps.Add(creep);
    }
  }

  // reborn a new creep
  public void GenNewCreep() {
    Vector3 creepPosOffset = new Vector3(Random.Range(-1.0f, 1.0f), creepGenerationConfig.yOffset, Random.Range(-1.0f, 1.0f)) * creepGenerationConfig.creepGenerationRange;
    CreepController creep = Instantiate(creepPrefab, transform.position + creepPosOffset, Quaternion.identity).GetComponent<CreepController>();
    creep.SetPlayer(this);
    AddCreep(creep);
  }
}

