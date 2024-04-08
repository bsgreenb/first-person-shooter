using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int bulletDamage;

    private void OnCollisionEnter(Collision objectWeHit) 
    {
        if (objectWeHit.gameObject.CompareTag("Target"))
        {
            print("hit " + objectWeHit.gameObject.name + " !");

            CreateBulletImpactEffect(objectWeHit);

            Destroy(gameObject); // destroy upon collision
        }


        if (objectWeHit.gameObject.CompareTag("Wall"))
        {
            print("hit a wall");
            
            CreateBulletImpactEffect(objectWeHit);
            
            Destroy(gameObject); // destroy upon collision
        }

        if (objectWeHit.gameObject.CompareTag("Beer"))
        {
            print("hit a beer bottle");
            
            objectWeHit.gameObject.GetComponent<BeerBottle>().Shatter();

            // We will not destroy the bullet on impact, it will get destroyed according to its lifetime.
        }

        if (objectWeHit.gameObject.CompareTag("Enemy"))
        {
            objectWeHit.gameObject.GetComponent<Enemy>().TakeDamage(bulletDamage);

            Destroy(gameObject);
        }

    }

    void CreateBulletImpactEffect(Collision objectWeHit)
    {
        ContactPoint contact = objectWeHit.contacts[0];

        GameObject hole = Instantiate(
            GlobalReferences.Instance.bulletImpactEffectPrefab,
            contact.point,
            Quaternion.LookRotation(contact.normal)
        );

        hole.transform.SetParent(objectWeHit.gameObject.transform);
    }
}
