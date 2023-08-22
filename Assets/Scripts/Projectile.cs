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
        public string id;
        public string owner;

        private void Start()
        {
            Invoke("DestroyProjectile", lifeTime);
        }

        private void Update()
        {

            RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.right, distance, whatIsSolid);
            if (hitInfo.collider != null)
            {
                if (owner != User.getUsername() && hitInfo.collider.CompareTag("Player"))
                {
                    try
                    {
                        hitInfo.collider.GetComponent<PlayerMovement>().DeathByProjectile(id);
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

            if (owner == User.getUsername())
            {
                GameController.Instance.UpdateBullet(id, transform.position);
            }
        }

        void DestroyProjectile()
        {
            if (owner == User.getUsername())
            {
                GameController.Instance.DestroyBullet(id);
            }

            Destroy(gameObject);
        }
    }
}