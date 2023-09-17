
using System.Collections;
using UnityEngine;

namespace Assets.Game.Script.game.SwitchWeaponDemo
{
    public class Weapon : MonoBehaviour
    {
        public float fireRate;
        public GameObject bullet;
        public Transform muzzle;
        private float _nextFireTime;

        void Start()
        {
            _nextFireTime = Time.time + fireRate;
        }

        // Update is called once per frame
        void Update()
        {
            if (Time.time> _nextFireTime)
            {
                _nextFireTime = Time.time + fireRate;
                Fire();
            }
        }

        void Fire()
        {
            Instantiate(bullet, muzzle.position, muzzle.rotation);
        }
    }
}