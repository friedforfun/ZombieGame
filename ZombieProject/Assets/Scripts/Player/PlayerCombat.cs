using System.Collections;
using System;
using UnityEngine;

/// <summary>
/// Target selection, shooting, and changing weapons
/// </summary>
public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private PlayerCamera PlayerCam;
    [SerializeField] private GameObject TargetReticle;
    [SerializeField] private GameObject AimSource;
    [SerializeField] private float AimRange;

    [SerializeField] public PlayerWeapon LhGun;
    [SerializeField] public PlayerWeapon RhGun;

    [SerializeField] private float AutoTargetRange = 50f;

    private PlayerRigging PlayerRig;
    private PlayerStateManager PlayerState;
    private bool canCycleWeapon = true;
    private Vector3 CrosshairPoint;
    private GameObject closestHostile = null;
    private WeaponTypes currentWeapon = WeaponTypes.PISTOL;

    // Start is called before the first frame update
    void Start()
    {
        PlayerState = GetComponent<PlayerStateManager>();
        if (PlayerState is null)
            throw new UnassignedReferenceException();

        PlayerRig = GetComponent<PlayerRigging>();
        if (PlayerRig is null)
        {
            throw new UnassignedReferenceException();
        }

        if (PlayerCam is null)
        {
            throw new UnassignedReferenceException();
        }
    }

    // Update is called once per frame
    void Update()
    {
        CrosshairPoint = CrosshairAimAt();
        closestHostile = findClosestHostile();

        mainhandTarget(CrosshairPoint);

        if (closestHostile is null || PlayerState.aimHeld)
        {
            offhandTarget(CrosshairPoint);
            SetHeadTarget(CrosshairPoint);
        }
        else
        {
            //Debug.Log("Should aim at enemy");
            offhandTarget(closestHostile.transform.position);
            SetHeadTarget(closestHostile.transform.position);
        }

    }

    public void Reload()
    {
        RhGun.CurrentWeaponMode.reload();
        LhGun.CurrentWeaponMode.reload();
    }

    public void CycleWeaponForwards()
    {
        if (canCycleWeapon)
        {
            canCycleWeapon = false;
            StartCoroutine(CycleForwards());
        }
    }

    public void CycleWeaponBackwards()
    {
        if (canCycleWeapon)
        {
            canCycleWeapon = false;
            StartCoroutine(CycleBackwards());
        }
    }

    public void ShootDown()
    {
        LhGun.ShootDown();
        RhGun.ShootDown();
    }

    public void ShootUp()
    {
        LhGun.ShootUp();
        RhGun.ShootUp();
    }

    /// <summary>
    /// Raycast from camera to the point in the world the crosshair is over
    /// </summary>
    /// <returns>point in the world the crosshair hit, or at the max range</returns>
    public Vector3 CrosshairAimAt()
    {

        Ray r = Camera.main.ScreenPointToRay(TargetReticle.transform.position);
        RaycastHit hit;
        //Debug.DrawRay(r);

        if (Physics.Raycast(r, out hit))
        {
            return hit.point;
        }
        else
        {
            return r.GetPoint(AimRange);
        }

    }

    private void SetWeapons()
    {
        LhGun.SwapWeapon(currentWeapon);
        RhGun.SwapWeapon(currentWeapon);
    }

    /// <summary>
    /// Loop over all hostiles, find the closest one that is also in line of sight
    /// </summary>
    /// <returns>closest hostile</returns>
    private GameObject findClosestHostile()
    {
        GameObject[] hostiles = GameObject.FindGameObjectsWithTag("HostileRigTarget");
        GameObject closestHostile = null;
        if (hostiles is null || hostiles.Length == 0)
        {
            return null;
        }
        foreach (GameObject hostile in hostiles)
        {
            if (hostile is null)
                continue;

            IKillable deathCheck = hostile.GetComponentInParent<IKillable>();
            if (deathCheck != null)
            {
                if (deathCheck.IsDead())
                {
                    continue;
                }
            }

            Vector3 directionToTarget = targetDirection(hostile);
            float angle = Vector3.Angle(transform.forward, directionToTarget);
            float distance = directionToTarget.magnitude;
            // Target within range?
            if (distance < AutoTargetRange)
            {
                // Target in front of player?
                if (Mathf.Abs(angle) > 80 && InLineOfSight(hostile))
                {
                    if (closestHostile is null)
                    {
                        closestHostile = hostile;
                    }
                    else if (distance < targetDirection(closestHostile).magnitude)
                    {
                        closestHostile = hostile;
                    }
                }
            }
        }
        return closestHostile;
    }

    /// <summary>
    /// Find if other is in line of sight (from players head)
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    private bool InLineOfSight(GameObject other)
    {
        GameObject target = other.GetComponent<ParentPointer>().Parent();
        Vector3 directionToOther = other.transform.position - AimSource.transform.position;
        Debug.DrawRay(AimSource.transform.position, directionToOther, Color.blue);
        RaycastHit hit;
        Ray los = new Ray(AimSource.transform.position, directionToOther);
        if (Physics.Raycast(los, out hit))
        {
            //Debug.Log($"Hit name: {hit.transform.name}");
            //Debug.Log($"Other name: {other.transform.name}");
            if (hit.transform.name == target.transform.name)
                return true;
        }
        return false;
    }

    private void SetHeadTarget(Vector3 target)
    {
        PlayerRig.lookAtTarget(target);
    }

    private void offhandTarget(Vector3 target)
    {
        if (PlayerCam.rightShoulderCam)
        {
            // Left hand is offhand
            PlayerRig.SetLeftHandTarget(target);
        }
        else
        {
            // Right hand is offhand
            PlayerRig.SetRightHandTarget(target);
        }
    }

    private void mainhandTarget(Vector3 target)
    {
        if (PlayerCam.rightShoulderCam)
        {
            // Right hand is mainhand
            PlayerRig.SetRightHandTarget(target);
        }
        else
        {
            // Left hand is mainhand
            PlayerRig.SetLeftHandTarget(target);
        }
    }

    private Vector3 targetDirection(GameObject target)
    {
        return transform.position - target.transform.position;
    }

    private IEnumerator CycleForwards()
    {
        int nextWeapon = ((int)currentWeapon + 1) % Enum.GetNames(typeof(WeaponTypes)).Length;
        currentWeapon = (WeaponTypes) nextWeapon;
        SetWeapons();
        yield return new WaitForSeconds(0.15f);
        canCycleWeapon = true;
    }

    private IEnumerator CycleBackwards()
    {
        
        int previousWeapon = (int)currentWeapon - 1;
        if (previousWeapon < 0)
            previousWeapon = Enum.GetNames(typeof(WeaponTypes)).Length - 1;
        currentWeapon = (WeaponTypes) previousWeapon;
        SetWeapons();
        yield return new WaitForSeconds(0.15f);
        canCycleWeapon = true;
    }

}
