using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    [Header("WEAPON ON PLAYER")]
    public GameObject[] weapons;   

    public int currentWeaponIndex = -1;

    public void EquipWeapon(int weaponIndex)
    {
        if (weapons == null || weapons.Length == 0)
        {
            Debug.LogError("Weapons array belum diisi di Inspector!");
            return;
        }

        if (weaponIndex < 0 || weaponIndex >= weapons.Length)
        {
            Debug.LogError("Weapon index tidak valid: " + weaponIndex);
            return;
        }

        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(false);
        }

        weapons[weaponIndex].SetActive(true);
        currentWeaponIndex = weaponIndex;

        Debug.Log("Weapon equipped: index " + weaponIndex);
    }

    public void UnequipAll()
    {
        if (weapons == null) return;

        foreach (var w in weapons)
        {
            if (w != null) w.SetActive(false);
        }

        currentWeaponIndex = -1;
    }
}
