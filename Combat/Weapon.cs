using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Weapon : MonoBehaviour
    {       
        [SerializeField] UnityEvent onHit;
        [SerializeField] GameObject weaponHitEffect = null;

        public void OnHit()
        {
            onHit.Invoke(); 

            if (weaponHitEffect == null)
            {
                return;
            }
            Instantiate(weaponHitEffect, transform.position, Quaternion.identity);
            
            
        }
    }
}
