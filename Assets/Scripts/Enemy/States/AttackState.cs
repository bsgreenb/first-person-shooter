using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{
    private float moveTimer;
    private float losePlayerTimer;
    private float shotTimer;
    private float timeBeforeMove = 5f;
    public override void Enter()
    {

    }

    public override void Exit()
    {

    }

    public override void Perform() 
    {
        if (enemy.CanSeePlayer()) {
            losePlayerTimer = 0;
            moveTimer += Time.deltaTime;
            shotTimer += Time.deltaTime;
            enemy.transform.LookAt(enemy.Player.transform);
            if (shotTimer > enemy.fireRate)
            {
                Shoot();
            }

            if (moveTimer > timeBeforeMove) {
                enemy.Agent.SetDestination(enemy.transform.position +  (Random.insideUnitSphere * 5));
                moveTimer = 0;
            }
            enemy.LastKnownPos = enemy.Player.transform.position;
        } else { // lost sight of player
            losePlayerTimer += Time.deltaTime;
            if (losePlayerTimer > 8) {
                // Change to the search state.
                stateMachine.ChangeState(new SearchState());
            }
        }
    }

    public void Shoot()
    {
        // store reference to the gun barrel
        Transform gunbarrel = enemy.gunBarrel;

        // instantiate a new bullet
        GameObject bullet = GameObject.Instantiate(Resources.Load("Prefabs/Bullet") as GameObject, gunbarrel.position, enemy.transform.rotation);
        // calculate the direction to the player
        Vector3 shootDirection = (enemy.Player.transform.position - gunbarrel.transform.position).normalized;
        // add force to rigidbody of the bullet
        // we use Quaternion.AngleAxis to add some inaccuracy.
        bullet.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(Random.Range(-3f, 3f), Vector3.up) * shootDirection * 40;
        Debug.Log("Shoot");
        shotTimer = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
