using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace object_pool
{
    public class pool : MonoBehaviour
    {
        public int bulletpoolsize = 25;
        private Queue<GameObject> bulletpool;
        public GameObject bullet;

        public void bulletenqueue(GameObject x)
        {
            x.transform.position = transform.position;
            x.SetActive(false);
            bulletpool.Enqueue(x);
        }

        public GameObject bulletdequeue()
        {
            if (bulletpool.Count > 0)
            {
                GameObject x = bulletpool.Dequeue();
                x.SetActive(true);
                return x;
            }
            return null;
        }

        void Start()
        {
            bulletpool = new Queue<GameObject>(bulletpoolsize);
            for (int i = 0; i < bulletpoolsize; i++)
            {
                GameObject instatiated_bullet = Instantiate(bullet, transform.position, transform.rotation);
                instatiated_bullet.GetComponent<bullet>().pool = this.gameObject;
                instatiated_bullet.SetActive(false);
                bulletpool.Enqueue(instatiated_bullet);
            }
        }
    }
}
