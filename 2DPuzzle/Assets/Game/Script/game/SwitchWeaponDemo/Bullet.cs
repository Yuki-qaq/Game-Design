using System.Collections;
using UnityEngine;

namespace Assets.Game.Script.game.SwitchWeaponDemo
{
    public class Bullet : MonoBehaviour
    {
        public float speed;

        private void Update()
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }
}