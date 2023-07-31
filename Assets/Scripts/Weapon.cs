using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class Weapon: MonoBehaviour
    {

        public GameObject projectile;
        public Transform shotPoint;

        private float timeBtwShots;
        public float startTimeBtwShots;

        private SpriteRenderer _spriteRenderer;
        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            Vector3 diffrence = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            float angle = Mathf.Atan2(diffrence.y, diffrence.x) * Mathf.Rad2Deg;
            
            float rotZ;
            if (!_spriteRenderer.flipY)
            {
                rotZ = Mathf.Max(-45, Mathf.Min(45, angle));
            }
            else
            {
                angle = 0 <= angle ? angle : angle + 360; 
                rotZ = Mathf.Max(135, Mathf.Min(225, angle));
            }
            
            transform.rotation = Quaternion.Euler(0f, 0f, rotZ);

            if (timeBtwShots <= 0)
            {
                if (Input.GetMouseButton(0))
                {
                    var p = Instantiate(projectile, shotPoint.position, transform.rotation).GetComponent<Projectile>();
                    p.isRight = !_spriteRenderer.flipY;
                    timeBtwShots = startTimeBtwShots;
                }
            }
            else
            {
                timeBtwShots -= Time.deltaTime;
            }
        }
    }
}