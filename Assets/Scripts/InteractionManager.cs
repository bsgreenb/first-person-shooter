using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance {get; set;}

    public Weapon hoveredWeapon = null;
    private PlayerInput playerInput;
    public PlayerInput.OnFootActions onFoot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        } else {
            Instance = this;
        }

        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
    }

    private void OnEnable() 
    {
        onFoot.Enable();
    }

    private void OnDisable()
    {
        onFoot.Disable();
    }


    private void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        //Ray Debugging
        //Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green);

        if (Physics.Raycast(ray, out hit)) {
            GameObject objectHitByRaycast = hit.transform.gameObject;
            Weapon weaponHitByRaycast = objectHitByRaycast.GetComponent<Weapon>();

            if (weaponHitByRaycast && !weaponHitByRaycast.isActiveWeapon) {
                if (hoveredWeapon != weaponHitByRaycast) {
                    if (hoveredWeapon != null) {
                        hoveredWeapon.GetComponent<Outline>().enabled = false;
                    }

                    hoveredWeapon = weaponHitByRaycast;
                    hoveredWeapon.GetComponent<Outline>().enabled = true;
                }

                if (onFoot.Interact.triggered) {
                    WeaponManager.Instance.PickupWeapon(objectHitByRaycast);
                }
            }
            else if (hoveredWeapon) {
                hoveredWeapon.GetComponent<Outline>().enabled = false;
                hoveredWeapon = null;
            }
        }
    }
}