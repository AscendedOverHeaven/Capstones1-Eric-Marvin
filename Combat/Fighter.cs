using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {   
        [SerializeField] float timeBetweenAttacks = .5f;
        [SerializeField] Transform leftTransform = null;
        [SerializeField] Transform rightTransform = null;
        [SerializeField] WeaponConfig defaultWeapon = null;

        Health target;
        float timeSinceLastAttacks = Mathf.Infinity;
        WeaponConfig currentWeapon = null;

        private void Start() 
        {   
            if (currentWeapon == null)
            {
                EquipWeapon(defaultWeapon);
            }
        }

        private void Update() 
        {   
            timeSinceLastAttacks += Time.deltaTime;

            if (target == null) return;
            if (target.IsDead()) return;

            if (!GetIsInRange())
            {
                GetComponent<Mover>().MoveTo(target.transform.position, 1f);
            }
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }

        public void EquipWeapon(WeaponConfig weapon)
        {   
            currentWeapon = weapon;
            Animator animator = GetComponent<Animator>();
            weapon.Spawn(leftTransform, rightTransform, animator);
            
        }

        public Health GetTarget()
        {
            return target;
        }  

        private void AttackBehaviour()
        {   
            transform.LookAt(target.transform);
            if (timeSinceLastAttacks > timeBetweenAttacks) 
            { 
                TriggerAttack();
                timeSinceLastAttacks = 0;
            }
            
        }

        private void TriggerAttack()
        {
            if (leftTransform == null || rightTransform == null) return;
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

    void Hit()
    {   
        if (target == null) { return; }

        float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
        
        if(currentWeapon.HasProjectile())
        {
            currentWeapon.LaunchProjectile(rightTransform, leftTransform, target, gameObject, damage);
        }
        else
        {
            target.TakeDamage(gameObject, damage);
        }

    }
    void Shoot()
    {
        Hit();
    }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.transform.position, target.transform.position) < currentWeapon.GetRange();
        }
        
        public bool CanAttack(GameObject combatTarget)
        {   
        if (combatTarget == null) { return false; }
        Health targetToTest = combatTarget.GetComponent<Health>();
        if (targetToTest != null && !targetToTest.IsDead())
        {
            return true;
        }
        else
        {
            StopAttack();
            return false;
        }
    }

        public void Attack(GameObject combatTarget)
        {   
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {   
            StopAttack();
            target = null;
        }

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.GetPercentageBonus();
            }
        }

        public object CaptureState()
        {
            return currentWeapon.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            WeaponConfig weapon = Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }

    }   
}



