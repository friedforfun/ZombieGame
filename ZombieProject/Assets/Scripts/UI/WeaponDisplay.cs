using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponDisplay : MonoBehaviour
{
    [SerializeField] private Image PistolImage;
    [SerializeField] private Image SMGImage;
    [SerializeField] private Image ShotgunImage;
    [SerializeField] private Text LHAmmoText;
    [SerializeField] private Text RHAmmoText;

    [SerializeField] private PlayerCombat playerCombat;

    [SerializeField] private Slider PistolReload;
    [SerializeField] private Slider SMGReload;
    [SerializeField] private Slider ShotgunReload;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateReloading();
        UpdateAmmoText();
        SetCurrentWeapon();
    }

    private void UpdateReloading()
    {
        if (playerCombat.RhGun.GetPistol().GetReloading())
        {
            PistolReload.value = playerCombat.RhGun.GetPistol().ReloadProgress();
        }
        if (playerCombat.RhGun.GetSMG().GetReloading())
        {
            SMGReload.value = playerCombat.RhGun.GetSMG().ReloadProgress();
        }
        if (playerCombat.RhGun.GetShotgun().GetReloading())
        {
            ShotgunReload.value = playerCombat.RhGun.GetShotgun().ReloadProgress();
        }
    }

    private void UpdateAmmoText()
    {
        int LCurrentAmmo = playerCombat.LhGun.CurrentWeaponMode.ClipRemaining;
        int LMaxAmmo = playerCombat.LhGun.CurrentWeaponMode.MagazineSize;
        LHAmmoText.text = $"{LCurrentAmmo} / {LMaxAmmo}";

        int RCurrentAmmo = playerCombat.RhGun.CurrentWeaponMode.ClipRemaining;
        int RMaxAmmo = playerCombat.RhGun.CurrentWeaponMode.MagazineSize;
        RHAmmoText.text = $"{RCurrentAmmo} / {RMaxAmmo}";
    }

    private void deSelectWeapons()
    {
        Color alpha = Color.white;
        alpha.a = 0.4f;
        PistolImage.color = alpha;
        SMGImage.color = alpha;
        ShotgunImage.color = alpha;
    }

    private void SetCurrentWeapon()
    {
        deSelectWeapons();
        switch (playerCombat.RhGun.CurrentWeaponMode.weaponType)
        {
            case WeaponTypes.PISTOL:
                {
                    SelectPistol();
                    break;
                }
            case WeaponTypes.SMG:
                {
                    SelectSMG();
                    break;
                }
            case WeaponTypes.SHOTGUN:
                {
                    SelectShotgun();
                    break;
                }
        }
    }

    private void SelectPistol()
    {
        //Debug.Log("Pistol Selected");
        Color alpha = PistolImage.color;
        alpha.a = 1f;
        PistolImage.color = alpha;
    }

    private void SelectSMG()
    {
        //Debug.Log("SMG Selected");
        Color alpha = SMGImage.color;
        alpha.a = 1f;
        SMGImage.color = alpha;
    }

    private void SelectShotgun()
    {
        //Debug.Log("Shotgun Selected");
        Color alpha = ShotgunImage.color;
        alpha.a = 1f;
        ShotgunImage.color = alpha;

    }

}
