using UnityEngine;
using RPG.Attributes;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "WeaponConfig", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject 
    {
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] Weapon leftEquippedPrefab = null;
        [SerializeField] Weapon rightEquippedPrefab = null;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float weaponDamage = 25f;
        [SerializeField] float percentageBonus = 0;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "Weapon";

        public Weapon Spawn(Transform leftTransform, Transform rightTransform, Animator animator)
        {
            DestroyOldWeapon(leftTransform, rightTransform);

            Weapon weapon = null;
            Transform handTransform = GetTransform(leftTransform, rightTransform);

            if (handTransform == null)
            {
                return weapon;
            }

            if (leftEquippedPrefab != null && rightEquippedPrefab == null)
            {
                //left 
                weapon = Instantiate(leftEquippedPrefab, handTransform);
                if (handTransform.CompareTag("OG"))
                {
                    handTransform.GetComponent<MeshRenderer>().enabled = false;
                }
            }
            else if (leftEquippedPrefab == null && rightEquippedPrefab != null)
            {
                //right 
                weapon = Instantiate(rightEquippedPrefab, handTransform);
                if (handTransform.CompareTag("OG"))
                {
                    handTransform.GetComponent<MeshRenderer>().enabled = false;
                }
            }
            else if (leftEquippedPrefab != null && rightEquippedPrefab != null)
            {
                //left and right
                weapon =Instantiate(leftEquippedPrefab, leftTransform);
                Instantiate(rightEquippedPrefab, rightTransform);
                if (leftTransform.CompareTag("OG"))
                {
                    leftTransform.GetComponent<MeshRenderer>().enabled = false;
                }
                if (rightTransform.CompareTag("OG"))
                {
                    rightTransform.GetComponent<MeshRenderer>().enabled = false;
                }
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            } 
            else if (overrideController != null)
            {
    
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
                
            }

            return weapon;

        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public void LaunchProjectile(Transform rightEquippedPrefab, Transform leftEquippedPrefab, Health target, GameObject instigator, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, GetTransform(rightEquippedPrefab, leftEquippedPrefab).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, calculatedDamage);
        }

        public float GetDamage()
        {
            return weaponDamage;
        }

        public float GetPercentageBonus()
        {
            return percentageBonus;
        }

        public float GetRange()
        {
            return weaponRange;
        }

        private Transform GetTransform(Transform leftTransform, Transform rightTransform)
        {
            if (leftEquippedPrefab != null && rightEquippedPrefab == null)
            {
                //left 
                return leftTransform;
            }
            else if (leftEquippedPrefab == null && rightEquippedPrefab != null)
            {
                //right 
                return rightTransform;
            }
            else if (leftEquippedPrefab != null && rightEquippedPrefab != null)
            {
                //left and right
                return leftTransform; // or rightTransform
            }

            return null;
        }

        private void DestroyOldWeapon(Transform leftTransform, Transform rightTransform)
        {
            if (leftTransform.childCount > 0)
            {
                Destroy(leftTransform.GetChild(0).gameObject);
            }

            if (rightTransform.childCount > 0)
            {
                Destroy(rightTransform.GetChild(0).gameObject);
            }
        }
    }
}