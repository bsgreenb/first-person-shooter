using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance {get; set;}
    public List<GameObject> weaponSlots;
    public GameObject activeWeaponSlot;

    [Header("Ammo")]
    public int totalRifleAmmo = 0;
    public int totalPistolAmmo = 0;

    [Header("Throwables")]
    public int grenades = 0;
    public float throwForce = 10f;
    public GameObject grenadePrefab;
    public GameObject throwableSpawn;
    public float forceMultiplier = 0;
    public float forceMultiplierLimit = 3f;
    private bool isWindingUp = false;

    private PlayerInput playerInput;
    public PlayerInput.OnFootActions onFoot;

    private int totalWeaponSlots = 2;
    private int activeWeaponNum = 0;
    private float scrollAccumulator = 0f;
    public const float scrollThreshold = 20f;
    private const float resetSpeed = 3.0f;

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

        onFoot.Weapon1.performed += ctx => SwitchActiveSlot(0);
        onFoot.Weapon2.performed += ctx => SwitchActiveSlot(1);
        onFoot.WeaponRotateNext.performed += ctx => WeaponRotateNext();
        onFoot.WeaponRotatePrevious.performed += ctx => WeaponRotatePrevious();
        onFoot.WeaponRotate.performed += ctx => WeaponRotate(ctx);
        
        onFoot.Throw.started += ctx => WindUpThrow();
        onFoot.Throw.canceled += ctx => Throw();
    }

    private void OnEnable() 
    {
        onFoot.Enable();
    }

    private void OnDisable()
    {
        onFoot.Disable();
    }

    private void Start()
    {
        activeWeaponSlot = weaponSlots[0];
    }

    private void Update()
    {
        foreach (GameObject weaponSlot in weaponSlots) {
            if (weaponSlot == activeWeaponSlot) {
                weaponSlot.SetActive(true);
            } else {
                weaponSlot.SetActive(false);
            }
        }

        //decrease the accumulator over time to smooth out the experience
        if (scrollAccumulator != 0)
        {
            scrollAccumulator = Mathf.MoveTowards(scrollAccumulator, 0, Time.deltaTime * resetSpeed);
        }

        if (isWindingUp) {
            forceMultiplier += Time.deltaTime;

            if (forceMultiplier > forceMultiplierLimit) {
                forceMultiplier = forceMultiplierLimit;
            }
        }   
    }

    public void PickupWeapon(GameObject pickedupWeapon)
    {
        AddWeaponIntoActiveSlot(pickedupWeapon);
    }



    private void AddWeaponIntoActiveSlot(GameObject pickedupWeapon)
    {
        DropCurrentWeapon(pickedupWeapon);

        pickedupWeapon.transform.SetParent(activeWeaponSlot.transform, false);

        Weapon weapon = pickedupWeapon.GetComponent<Weapon>();

        pickedupWeapon.transform.SetLocalPositionAndRotation(new Vector3(weapon.spawnPosition.x, weapon.spawnPosition.y, weapon.spawnPosition.z), Quaternion.Euler(weapon.spawnRotation.x, weapon.spawnRotation.y, weapon.spawnRotation.z));
        weapon.isActiveWeapon = true;
        weapon.GetComponent<Outline>().enabled = false;
        weapon.enabled = true;
        weapon.animator.enabled = true;
        weapon.GetComponent<BoxCollider>().enabled = false;
    }

    internal void PickupAmmo(AmmoBox ammo)
    {
        switch (ammo.ammoType)
        {
            case AmmoBox.AmmoType.PistolAmmo:
                totalPistolAmmo += ammo.ammoAmount;
                break;
            case AmmoBox.AmmoType.RifleAmmo:
                totalRifleAmmo += ammo.ammoAmount;
                break;
        } 
    }

    private void DropCurrentWeapon(GameObject pickedupWeapon)
    {
        if (activeWeaponSlot.transform.childCount > 0) {
            GameObject weaponToDrop = activeWeaponSlot.transform.GetChild(0).gameObject;

            weaponToDrop.GetComponent<Weapon>().isActiveWeapon = false;
            weaponToDrop.GetComponent<Weapon>().enabled = false;
            weaponToDrop.GetComponent<Weapon>().animator.enabled = false;
            weaponToDrop.GetComponent<BoxCollider>().enabled = true;

            weaponToDrop.transform.SetParent(pickedupWeapon.transform.parent);

            Vector3 restingPositionOffset = weaponToDrop.GetComponent<Weapon>().restingPositionOffset - pickedupWeapon.GetComponent<Weapon>().restingPositionOffset;
            Vector3 restingRotationOffset = weaponToDrop.GetComponent<Weapon>().restingRotationOffset - pickedupWeapon.GetComponent<Weapon>().restingRotationOffset;

            weaponToDrop.transform.SetLocalPositionAndRotation(pickedupWeapon.transform.localPosition + restingPositionOffset, Quaternion.Euler(pickedupWeapon.transform.localRotation.eulerAngles + restingRotationOffset));
        }
    }

    private void WeaponRotateNext()
    {
        if (activeWeaponNum == (totalWeaponSlots - 1)) {
            SwitchActiveSlot(0);
        } else {
            SwitchActiveSlot(activeWeaponNum + 1);
        }
    }

    private void WeaponRotatePrevious()
    {
        if (activeWeaponNum == 0) {
            SwitchActiveSlot(totalWeaponSlots -1);
        } else {
            SwitchActiveSlot(activeWeaponNum - 1);
        }
    }

    private void WeaponRotate(InputAction.CallbackContext context)
    {
        Vector2 scrollValue = context.ReadValue<Vector2>();
        scrollAccumulator += scrollValue.y;

        if (scrollAccumulator > scrollThreshold) {
            WeaponRotateNext();
            scrollAccumulator = 0; 
        } else if (scrollAccumulator < -scrollThreshold) {
            WeaponRotatePrevious();
            scrollAccumulator = 0; 
        }
    }

    public void SwitchActiveSlot(int slotNumber)
    {
        if (activeWeaponSlot.transform.childCount > 0) {
            Weapon currentWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            currentWeapon.isActiveWeapon = false;
        }

        activeWeaponSlot = weaponSlots[slotNumber];
        activeWeaponNum = slotNumber;

        if (activeWeaponSlot.transform.childCount > 0) {
            Weapon newWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            newWeapon.isActiveWeapon = true;
        }
    }

    internal void DecreaseTotalAmmo(int bulletsToDecrease, Weapon.WeaponModel thisWeaponModel)
    {
        switch (thisWeaponModel) 
        {
            case Weapon.WeaponModel.M4:
                totalRifleAmmo -= bulletsToDecrease;
                break;
            case Weapon.WeaponModel.Pistol1911:
                totalPistolAmmo -= bulletsToDecrease;
                break;
        }
    }

    public int CheckAmmoLeftFor(Weapon.WeaponModel thisWeaponModel)
    {
        switch (thisWeaponModel)
        {
            case Weapon.WeaponModel.M4:
                return totalRifleAmmo;
            case Weapon.WeaponModel.Pistol1911:
                return totalPistolAmmo;
            default:
                return 0;
        }
    }

    #region || ---- Throwables ---- ||
    public void PickupThrowable(Throwable throwable)
    {
        switch (throwable.throwableType)
        {
            case Throwable.ThrowableType.Grenade:
                PickupGrenade();
                break;
        }
    }

    private void PickupGrenade()
    {
        grenades += 1;

        HUDManager.Instance.UpdateThrowables(Throwable.ThrowableType.Grenade);
    }

    private void WindUpThrow()
    {
        if (grenades > 0) {
            isWindingUp = true;
        }

    }

    private void Throw()
    {
        isWindingUp = false;
        if (grenades > 0) {
            ThrowLethal();
        }
        
        forceMultiplier = 0;
    }

    private void ThrowLethal()
    {
        GameObject lethalPrefab = grenadePrefab;

        GameObject throwable = Instantiate(lethalPrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);
        Rigidbody rb = throwable.GetComponent<Rigidbody>();

        rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);

        throwable.GetComponent<Throwable>().hasBeenThrown = true;

        grenades -= 1;
        HUDManager.Instance.UpdateThrowables(Throwable.ThrowableType.Grenade);
    }


    #endregion
}
