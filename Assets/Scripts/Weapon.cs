using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    public bool isActiveWeapon;
    public int weaponDamage;

    // Input
    private PlayerInput playerInput;
    public PlayerInput.OnFootActions onFoot;

    [Header("Shooting")]
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;

    [Header("Burst")]
    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;

    [Header("Spread")]
    public float spreadIntensity;
    public float hipSpreadIntensity;
    public float adsSpreadIntensity;

    [Header("Bullet")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 20f;
    public float bulletPrefabLifeTime = 3f;

    // Muzzle effect
    public GameObject muzzleEffect;
    
    // Recoil etc. animation
    internal Animator animator;

    [Header("Loading")]
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading = false;

    // Spawn position
    public Vector3 spawnPosition;
    public Vector3 spawnRotation;

    bool isADS;

    // Adjustments for resting position
    public Vector3 restingPositionOffset;
    public Vector3 restingRotationOffset;


    public enum WeaponModel
    {
        Pistol1911,
        M4
    }

    public WeaponModel thisWeaponModel;

    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }

    public ShootingMode currentShootingMode;

    void Awake()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;

        onFoot.Shoot.performed += ctx => StartShooting();
        onFoot.Shoot.canceled += ctx => StopShooting();

        onFoot.Reload.performed += ctx => ManualReload();

        onFoot.ADS.started += ctx => StartADS();
        onFoot.ADS.canceled += ctx => StopADS();

        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;

        animator = GetComponent<Animator>();

        bulletsLeft = magazineSize;

        spreadIntensity = hipSpreadIntensity;
    }

    private void OnEnable() 
    {
        onFoot.Enable();
    }

    private void OnDisable()
    {
        onFoot.Disable();
    }

    void Update()
    {
        if (isActiveWeapon) {
            // To disable weapon clipping on active weapons
            SetLayerRecursively(transform.gameObject, "WeaponRender");
            GetComponent<Outline>().enabled = false; // avoids a potential bug with remaining outline.
            if (bulletsLeft == 0 && isShooting) {
                SoundManager.Instance.emptyMagazineSound1911.Play();
            }

            // Automatic reload when magazine is empty
            // if (readyToShoot && !isShooting && !isReloading && bulletsLeft <=0) {
            //     Reload();
            // }

            if (readyToShoot && isShooting && bulletsLeft > 0) {
                burstBulletsLeft = bulletsPerBurst;
                FireWeapon();
            }
        } else {
            // Back to normal layer for these ones
            SetLayerRecursively(transform.gameObject, "Default");
        }
    }

    void SetLayerRecursively(GameObject obj, string layer) 
    {
        if (null == obj) {
            return;
        }
    
        int newLayer = LayerMask.NameToLayer(layer);

        obj.layer = newLayer;
    
        foreach (Transform child in obj.transform) {
            if (null == child) {
                continue;
            }
            SetLayerRecursively(child.gameObject, layer);
        }
    }


    public void StartShooting() 
    {
        if (isActiveWeapon) {
            isShooting = true;
        }
    }
    public void StopShooting()
    {
        if (isActiveWeapon) {
            isShooting = false;
        }
    }

    public void StartADS()
    {
        if (isActiveWeapon) {
            animator.SetBool("isADS", true);
            isADS = true;
            HUDManager.Instance.middleDot.SetActive(false);
            spreadIntensity = adsSpreadIntensity;
        }
    }

    public void StopADS()
    {
        if (isActiveWeapon) {
            animator.SetBool("isADS", false);
            isADS = false;
            HUDManager.Instance.middleDot.SetActive(true);
            spreadIntensity = hipSpreadIntensity;
        }
    }

    private void FireWeapon()
    {
        bulletsLeft--;

        muzzleEffect.GetComponent<ParticleSystem>().Play();
        if (isADS) {
            animator.SetTrigger("RECOIL_ADS");
        } else {
            animator.SetTrigger("RECOIL");
        }

        SoundManager.Instance.PlayShootingSound(thisWeaponModel);

        readyToShoot = false;

        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        // Instantiate the bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        
        Bullet bul = bullet.GetComponent<Bullet>();
        bul.bulletDamage = weaponDamage;

        // Pointing the bullet to face the shooting direction
        bullet.transform.forward = shootingDirection;

        // Shoot the bullet
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

        // Destroy the bullet after some time
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));

        if (allowReset) {
            Invoke(nameof(ResetShot), shootingDelay);
            allowReset = false;
        }

        // Burst Mode
        if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1) { // We already shoot once before this check
            burstBulletsLeft--;
            Invoke(nameof(FireWeapon), shootingDelay);
        }
    }

    private void ManualReload()
    {
        if (isActiveWeapon && bulletsLeft < magazineSize && !isReloading && WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > 0)
        {
            Reload();
        }
    }

    private void Reload()
    {
        SoundManager.Instance.PlayReloadSound(thisWeaponModel);

        animator.SetTrigger("RELOAD");

        isReloading = true;
        Invoke(nameof(ReloadCompleted), reloadTime);
    }

    private void ReloadCompleted()
    {
        if (WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > magazineSize) {
            bulletsLeft = magazineSize;
        } else {
            bulletsLeft = WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel);
        }
        WeaponManager.Instance.DecreaseTotalAmmo(bulletsLeft, thisWeaponModel);
        isReloading = false;
    }

    private void ResetShot() 
    {
        readyToShoot = true;
        allowReset = true;
    }

    // We use this code to help insure the bullet goes from the bullet spawn to the target at center of screen.
    public Vector3 CalculateDirectionAndSpread()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit)) {
            // Hitting something
            targetPoint = hit.point;
        } else {
            // Shooting at the air
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        // Returning the shooting direction and spread
        return direction + new Vector3(x, y, 0);
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay) 
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }

    

}
