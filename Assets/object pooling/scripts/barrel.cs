using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace object_pool
{
public class barrel : MonoBehaviour
{
    public GameObject shooter;
    public GameObject target;
    private Vector3 target_direction;
    private Quaternion targetRotation;
    private float angleDifference;
    public GameObject target_pointer;
    public GameObject barrel_pointer;
    public float assest_max_angle = 3f;
    public GameObject pool;
    public bool shoot_bullet;
    private int bullets_interval_counter;
    public int timebetweenbullets = 5;
    public bool aim_assest_active;
    public float accuracy=2f;
    void Awake()
    {
        barrel_pointer.GetComponent<barrel_pointer>().target=target;
    }
    void FixedUpdate()
    {
        if (bullets_interval_counter < timebetweenbullets)
        {
            bullets_interval_counter++;
        }

        if (shoot_bullet && bullets_interval_counter == timebetweenbullets)
        {
            // Calculate direction to target pointer
            target_direction = target_pointer.transform.position - transform.position;

            // Calculate target rotation based on direction
            //if(target_direction!=Vector3.zero)
            targetRotation = Quaternion.LookRotation(target_direction);
            // Calculate angle difference on all axes
            angleDifference = Quaternion.Angle(transform.rotation, targetRotation);
            // Shoot bullet from pool
            GameObject currentbullet = pool.GetComponent<pool>().bulletdequeue();
            currentbullet.transform.position = transform.position;
            currentbullet.transform.rotation = transform.rotation;
            currentbullet.GetComponent<bullet>().pre_position = transform.position;
            currentbullet.GetComponent<bullet>().shooter=shooter;
            currentbullet.GetComponent<bullet>().target=target;
            // Add random dispersion
            Vector3 randomdis =new Vector3((Random.Range(-2, 3) + Random.Range(-2, 3))/2/accuracy,(Random.Range(-2, 3) + Random.Range(-2, 3))/2/accuracy,(Random.Range(-2, 3) + Random.Range(-2, 3))/2/accuracy);
            currentbullet.transform.Rotate(randomdis);

            // Check for aim assist
            if (angleDifference < assest_max_angle &&aim_assest_active)
            {
                currentbullet.GetComponent<bullet>().aim_assest_active = aim_assest_active;
            }
currentbullet.GetComponent<bullet>().accuracy=accuracy;
            currentbullet.GetComponent<bullet>().aim_assest();
            currentbullet.SetActive(true);
            currentbullet.GetComponent<bullet>().fire_sound.Play();

            // Reset bullet fire speed
            bullets_interval_counter = 0;
        }
    }
}
}