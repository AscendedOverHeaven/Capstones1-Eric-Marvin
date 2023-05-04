using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {   
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float timeBetweenAttacks = .5f;
        [SerializeField] float weaponDamage = 25f;

        Health target;
        float timeSinceLastAttacks = Mathf.Infinity;

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
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        void Hit()
        {   
            if(target == null) { return; }
            target.TakeDamage(weaponDamage);
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.transform.position, target.transform.position) < weaponRange;
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

    }   
}
