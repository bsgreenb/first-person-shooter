using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int HP = 100;

    public void TakeDamage(int damageAmount)
   {
        HP -= damageAmount;

        if (HP <= 0) {
            print("Player Dead");

            // Game over
            // ReSpawn Player
            // Dying Animation

        } else {
            print("Player Hit");
        }
   }

   private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("ZombieHand")) {
            TakeDamage(other.gameObject.GetComponent<ZombieHand>().damage);
        }
   }
}
