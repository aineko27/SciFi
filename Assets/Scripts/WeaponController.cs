using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public static Weapon[] EquippedWeapon = new Weapon[4];

    public Sprite[] IconWhitebackArrayTemp;
    public static Sprite[] IconWhitebackArray;


    public class Weapon: MonoBehaviour
    {
        public int index;
        public int baseDamage;
        public Sprite IconWhiteback;

        public Weapon MakeNewWeapon(int idx, int dmg)
        {
            Weapon thisWeapon = new Weapon();

            thisWeapon.index = idx;
            thisWeapon.baseDamage = dmg;
            thisWeapon.IconWhiteback = IconWhitebackArray[thisWeapon.index];
            
            return thisWeapon;
        }
    }

    private void Start()
    {
        IconWhitebackArray = IconWhitebackArrayTemp;
        Weapon WeaponObject = new Weapon();

        Weapon Punch = WeaponObject.MakeNewWeapon(0, 20);
        Weapon Laser = WeaponObject.MakeNewWeapon(1, 12);
        Weapon Beam = WeaponObject.MakeNewWeapon(2, 8);
        Weapon Mine = WeaponObject.MakeNewWeapon(3, 15);

        ArrangeEquippedWeapon(Punch, Laser, Beam, Mine);
    }

    public static void ArrangeEquippedWeapon(Weapon weapon0, Weapon weapon1, Weapon weapon2, Weapon weapon3)
    {
        EquippedWeapon[0] = weapon0;
        EquippedWeapon[1] = weapon1;
        EquippedWeapon[2] = weapon2;
        EquippedWeapon[3] = weapon3;
    }
}
