using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreepController : MonoBehaviour {

  public PlayerControl player;
  public CreepMoveConfig creepMoveConfig;
  public AttackConfig attackConfig;

  [System.Serializable]
  public class CreepMoveConfig
  {
    public float totalSpeed = 1.0f;
    public float catchPlayerFactor;
    public float keepDistanceFactor;
    public float keepDistanceRange;
    public float goCenterFactor;
    public float goCenterRange;
  }

  [System.Serializable]
  public class AttackConfig
  {
    public float searchEnemyRange = 1.0f;
  }

  private new Rigidbody rigidbody;

  private CreepController nearestEnemy;

  // Use this for initialization
  void Start () {
    rigidbody = GetComponent<Rigidbody>();
  }
	
	// Update is called once per frame
	void Update () {
    
	}

  void FixedUpdate() {
    if (player == null) {
      return;
    }
    // List<CreepController> neighbors = new List<CreepController>();
    Vector3 keepDistance = new Vector3();
    Vector3 creepCenter = new Vector3();
    int creepCenterCount = 0;

    foreach (CreepController creep in player.GetCreeps()) {
      if (creep == this) continue;
      float distance = Vector3.Distance(creep.transform.position, transform.position);
      if (distance < creepMoveConfig.keepDistanceRange) {
        keepDistance += (transform.position - creep.transform.position).normalized * (creepMoveConfig.keepDistanceRange - distance);
      }

      if (distance < creepMoveConfig.goCenterRange)
      {
        creepCenter += creep.transform.position;
        ++creepCenterCount;
      }
    }
    keepDistance *= creepMoveConfig.keepDistanceFactor;
    creepCenter /= creepCenterCount;

    // TODO 改成：判断一下速度点成，但是都至少有0.5
    Vector3 goCenter = (creepCenter - transform.position) * creepMoveConfig.goCenterFactor;
    /*
    Vector3 playerSpeed = player.rigidbody.velocity;
    float dotProductFactor = Vector3.Dot((transform.position - player.transform.position).normalized, playerSpeed.normalized);
    dotProductFactor = (dotProductFactor + 1) / 2;
    */
    Vector3 catchPlayer = (player.transform.position - transform.position) * creepMoveConfig.catchPlayerFactor * Vector3.Distance(player.transform.position, transform.position);
    Vector3 vel = (keepDistance + goCenter + catchPlayer) * creepMoveConfig.totalSpeed;
    vel.y = 0.0f;
    rigidbody.velocity = vel;


    // handle enemy
    nearestEnemy = null;
    float curNearestDistance = 999999.0f;
    foreach (PlayerControl other_player in player.gameController.players) {
      if (other_player == player) continue;
      foreach (CreepController creep in other_player.GetCreeps())
      {
        float distance = Vector3.Distance(creep.transform.position, transform.position);
        if (distance < attackConfig.searchEnemyRange && distance < curNearestDistance)
        {
          curNearestDistance = distance;
          nearestEnemy = creep;
        }
      }
    }
    if (nearestEnemy != null) {
      transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
    }
  }

  public void SetPlayer(PlayerControl player_in)
  {
    player = player_in;
  }
}




