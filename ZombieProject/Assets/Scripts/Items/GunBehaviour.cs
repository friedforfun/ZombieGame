using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum WeaponTypes
{
    PISTOL,
    SMG,
    SHOTGUN
}

public enum FireMode
{
    SINGLE,
    AUTO
}

public class GunBehaviour : MonoBehaviour, IShootable
{
    [SerializeField] protected GameObject projectile;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private Transform muzzleLocation;

    [SerializeField] public int ClipRemaining; // Number of bullets in clip
    [SerializeField] public int MagazineSize; // Size of magazine
    [SerializeField] protected float fireRate; // Rate of fire (time between each shot)
    [SerializeField] public float reloadTime; // Duration of reload period
    [SerializeField] protected float recoilForce; // Modify recoil amount
    [SerializeField] protected int numProjectiles; // How many projectiles will be fired per shot
    [SerializeField] public FireMode fireMode;
    [SerializeField] protected float impactSpreadRadius;

    private ProjectileBehaviour projectileBehaviour;
    private bool canShoot = true;

    private bool reloading = false;
    private float reloadStartTime;

    void Start()
    {
        projectileBehaviour = projectile.GetComponent<ProjectileBehaviour>();
        if (projectileBehaviour is null)
            throw new MissingReferenceException();
    }

    /// <summary>
    /// Fire bullets, apply spread, and normalise magnitude
    /// </summary>
    /// <param name="targetDirection"></param>
    public void shoot(Vector3 targetDirection)
    {
        targetDirection = targetDirection.normalized;
        if (ClipRemaining > 0 && !reloading && canShoot)
        {
            canShoot = false;
            ClipRemaining--;
            for (int i = 0; i < numProjectiles; i++)
            {
                float x = Random.Range(-impactSpreadRadius, impactSpreadRadius);
                float y = Random.Range(-impactSpreadRadius, impactSpreadRadius);

                Vector3 directionWithSpread = targetDirection + transform.InverseTransformDirection(x, y, 0);

                GameObject bullet = Instantiate<GameObject>(projectile, muzzleLocation.position, Quaternion.LookRotation(directionWithSpread, Vector3.up));

                bullet.GetComponent<ProjectileBehaviour>().Fire(directionWithSpread);
                muzzleFlash.Play();
            }
            StartCoroutine(resetShot());
        }
    }

    public void reload()
    {
        reloadStartTime = Time.time;
        reloading = true;
        StartCoroutine(reloadFinished());
    }

    public float ReloadProgress()
    {
        float reloadDuration = (Time.time - reloadStartTime) / reloadTime;
        return reloadDuration;
    }

    public bool GetReloading()
    {
        return reloading;
    }

    private IEnumerator resetShot()
    {
        yield return new WaitForSeconds(fireRate);
        canShoot = true;
    }


    private IEnumerator reloadFinished()
    {
        yield return new WaitForSeconds(reloadTime);
        ClipRemaining = MagazineSize;
        reloading = false;
    }

}
