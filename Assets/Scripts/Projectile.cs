using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class Projectile: MonoBehaviour
    {
        public float speed;
        public float lifeTime;
        public float distance;
        public LayerMask whatIsSolid;
        public bool isRight = false;

        private void Start()
        {
            Invoke("DestroyProjectile", lifeTime);
        }

        private void Update()
        {

            RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.right, distance, whatIsSolid);
            if (hitInfo.collider != null)
            {
                if (hitInfo.collider.CompareTag("Rival"))
                {
                    try
                    {
                        hitInfo.collider.GetComponent<MatchRival>().DeathByProjectile();
                    }
                    catch (Exception e)
                    {
                        
                    }
                }
                DestroyProjectile();
            }

            if (isRight)
            {
                transform.Translate(transform.right * speed * Time.deltaTime);
            }
            else
            {
                transform.Translate(-transform.right * speed * Time.deltaTime);
            }
            
        }

        void DestroyProjectile()
        {
            Destroy(gameObject);
        }
    }
}