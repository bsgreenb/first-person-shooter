using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
   [SerializeField] private int HP = 100;
   private Animator animator;
   private NavMeshAgent navAgent;

   private void Start()
   {
    animator = GetComponent<Animator>();
    navAgent = GetComponent<NavMeshAgent>();
   }

   public void TakeDamage(int damageAmount)
   {
        HP -= damageAmount;

        if (HP <= 0) {

            int randomValue = Random.Range(0,2);

            if (randomValue == 0) {
                animator.SetTrigger("DIE1");
            } else {
                animator.SetTrigger("DIE2");
            }

        } else {
            animator.SetTrigger("DAMAGE");
        }
   }

   private void Update()
   {
        if (navAgent.velocity.magnitude > 0.1f) {
            animator.SetBool("isWalking", true);
        } else {
            animator.SetBool("isWalking", false);
        }
   }
}
