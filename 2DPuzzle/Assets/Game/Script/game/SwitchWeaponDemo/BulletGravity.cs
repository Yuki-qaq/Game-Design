using System.Collections;
using UnityEngine;

namespace Assets.Game.Script.game.SwitchWeaponDemo
{
    public class BulletGravity : MonoBehaviour
    {
        public float startForce;
        void Start()
        {
            var rb = GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * startForce);
        }
    }
}