using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Target selection, and shooting
/// </summary>
public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private PlayerCamera PlayerCam;
    [SerializeField] private GameObject TargetReticle;
    [SerializeField] private GameObject AimSource;
    [SerializeField] private float AimRange;

    [SerializeField] private PlayerWeapon LhGun;
    [SerializeField] private PlayerWeapon RhGun;

    [SerializeField] private float AutoTargetRange = 10f;

    private PlayerRigging PlayerRig;

    private Vector3 CrosshairPoint;
    private GameObject closestHostile = null;

    // Start is called before the first frame update
    void Start()
    {
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
        offhandTarget(CrosshairPoint);

        if (closestHostile is null)
        {
            
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
                if (Mathf.Abs(angle) > 80)
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

}
