using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private Transform TargetTransform;
    [SerializeField] private Transform WeaponMuzzle;

    [SerializeField] private GunBehaviour Pistol;
    [SerializeField] private GunBehaviour SMG;
    [SerializeField] private GunBehaviour Shotgun;

    public GunBehaviour CurrentWeaponMode;

    private bool autoShoot;
    // Start is called before the first frame update
    void Start()
    {
        CurrentWeaponMode = Pistol;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 trueGunOrientation = WeaponMuzzle.InverseTransformDirection(Vector3.forward);
        Vector3 desiredGunOrientation = TargetTransform.position - WeaponMuzzle.position;
        Debug.DrawRay(WeaponMuzzle.position, trueGunOrientation, Color.red);
        Debug.DrawRay(WeaponMuzzle.position, desiredGunOrientation, Color.green);

        if (autoShoot)
        {
            CurrentWeaponMode.shoot(TargetTransform.position - WeaponMuzzle.transform.position);
        }
    }

    public void SwapWeapon(WeaponTypes weapon)
    {
        switch (weapon)
        {
            case WeaponTypes.PISTOL:
                CurrentWeaponMode = Pistol;
                break;

            case WeaponTypes.SMG:
                CurrentWeaponMode = SMG;
                break;

            case WeaponTypes.SHOTGUN:
                CurrentWeaponMode = Shotgun;
                break;
        }


    }

    public void ShootDown() 
    {
        if (CurrentWeaponMode.fireMode == FireMode.SINGLE)
        {
            CurrentWeaponMode.shoot(TargetTransform.position - WeaponMuzzle.transform.position);
        }
        else if (CurrentWeaponMode.fireMode == FireMode.AUTO)
        {
            autoShoot = true;
        }
    }

    public void ShootUp()
    {
        autoShoot = false;
    }


}
