using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchWeapon : MonoBehaviour
{

    [System.Serializable]
    public class WeaponInfo
    {
        public string key;
        public GameObject view;
        public string pose;
    }

    public WeaponInfo[] weapons;

    public GameObject cap1;
    public GameObject cap2;
    public WeaponInfo crtWeapon;

    void Start()
    {
    }

    void Update()
    {
        foreach (WeaponInfo w in weapons)
        {
            if (Input.GetKeyDown(w.key))
                SetToWeapon(w);
        }
    }

    void SetToWeapon(WeaponInfo weaponToActive)
    {
        crtWeapon = weaponToActive;

        foreach (WeaponInfo w in weapons)
            w.view.SetActive(false);

        weaponToActive.view.SetActive(true);

        switch (weaponToActive.pose)
        {
            case "stand1":
                cap1.SetActive(true);
                cap2.SetActive(false);
                break;

            case "stand2":
                cap1.SetActive(false);
                cap2.SetActive(true);
                break;
        }
    }
}
