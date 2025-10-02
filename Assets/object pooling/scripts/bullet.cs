using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace object_pool
{
public class bullet : MonoBehaviour
{
    public GameObject pool;
    public GameObject shooter;
    public float speed;
    public int damage;
    public int timealive;
    public int timebeforedestruction;
    public Vector3 pre_position;
    public bool fully_active = false;
    public int timebeforeactivition;
    public bool aim_assest_active;
    public GameObject target;
    public AudioSource fire_sound;
    public float accuracy;
    public GameObject destroy_effect;
    private Vector3 randomdis;
    private RaycastHit hit;
    public void call_destroy_effects()
    {
        Instantiate(destroy_effect, transform.position, transform.rotation);
    }
    public void aim_assest()
    {
        if (aim_assest_active)
        {
            transform.LookAt(target.transform.position);
             randomdis =new Vector3((Random.Range(-2, 3) + Random.Range(-2, 3))/2/accuracy,(Random.Range(-2, 3) + Random.Range(-2, 3))/2/accuracy,(Random.Range(-2, 3) + Random.Range(-2, 3))/2/accuracy);
            transform.Rotate(randomdis);
        }
    }
    void FixedUpdate()
    {
        transform.position += transform.forward * speed;
        if (timealive == timebeforeactivition)
        {
            fully_active = true;
            pre_position = transform.position;
        }
            if (Physics.Raycast(transform.position, (pre_position - transform.position).normalized, out hit, (transform.position - pre_position).magnitude, 1 << 3))
            {
                if (hit.collider.gameObject != shooter)
                {
                    call_destroy_effects();
                    this.gameObject.SetActive(false);
                    //damage the player
                }
            }
        pre_position = transform.position;
        if (timealive >= timebeforedestruction)
        {
            this.gameObject.SetActive(false);
        }
        timealive++;
    }
    private void OnDisable()
    {
        timealive = 0;
        fully_active = false;
        if(pool)
        pool.GetComponent<pool>().bulletenqueue(this.gameObject);
        aim_assest_active = false;
    }
}
}