using UnityEngine;
using TMPro;  // Import TextMeshPro namespace

public class HUDManager : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerHealth;  // Reference to your PlayerHealth script
    public Weapons weapons;            // Reference to your Weapons script

    [Header("UI Elements (TextMeshPro)")]
    public TMP_Text healthText;        // TMP_Text to display Health
    public TMP_Text ammoText;          // TMP_Text to display Ammo count
    public TMP_Text weaponText;        // TMP_Text to display Current Weapon

    void Update()
    {
        UpdateHealthUI();
        UpdateAmmoUI();
        UpdateWeaponUI();
    }

    void UpdateHealthUI()
    {
        if (playerHealth != null && healthText != null)
        {
            healthText.text = "HP: " + Mathf.Ceil(playerHealth.CurrentHealth) + " / " + playerHealth.maxHealth;
        }
    }

    void UpdateAmmoUI()
    {
        if (weapons != null && ammoText != null)
        {
            string ammoDisplay = "";
            switch (weapons.currentWeapon)
            {
                case WeaponType.M1911:
                    ammoDisplay = weapons.M1911Ammo.ToString();
                    break;
                case WeaponType.Generic:
                    ammoDisplay = weapons.GenericAmmo.ToString();
                    break;
                case WeaponType.Sniper:
                    ammoDisplay = weapons.SniperAmmo.ToString();
                    break;
                case WeaponType.RPG7:
                    ammoDisplay = weapons.rpgIsLoaded ? "Loaded" : "Reloading";
                    break;
            }
            ammoText.text = "Ammo: " + ammoDisplay;
        }
    }

    void UpdateWeaponUI()
    {
        if (weapons != null && weaponText != null)
        {
            weaponText.text = "Weapon: " + weapons.currentWeapon.ToString();
        }
    }
}