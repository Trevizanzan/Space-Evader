using UnityEngine;

public static class UnlockManager
{
    private const string LifetimeBossKey = "lifetime_boss_kills";

    public static bool IsWeaponUnlocked(WeaponData weapon)
    {
        if (weapon == null) return false;
        if (weapon.alwaysUnlocked) return true;
        return PlayerPrefs.GetInt(WeaponKey(weapon), 0) == 1;
    }

    public static void UnlockWeapon(WeaponData weapon)
    {
        if (weapon == null || weapon.alwaysUnlocked) return;
        PlayerPrefs.SetInt(WeaponKey(weapon), 1);
        PlayerPrefs.Save();
    }

    public static int IncrementAndGetLifetimeBossKills()
    {
        int kills = PlayerPrefs.GetInt(LifetimeBossKey, 0) + 1;
        PlayerPrefs.SetInt(LifetimeBossKey, kills);
        PlayerPrefs.Save();
        return kills;
    }

    public static void CheckWeaponUnlocks(int lifetimeBossKills, WeaponData spreadGun, WeaponData railgun)
    {
        if (lifetimeBossKills >= 1 && spreadGun != null)
            UnlockWeapon(spreadGun);
        if (lifetimeBossKills >= 2 && railgun != null)
            UnlockWeapon(railgun);
    }

    private static string WeaponKey(WeaponData weapon) =>
        "unlock_" + weapon.weaponName.ToLower().Replace(" ", "_");
}
