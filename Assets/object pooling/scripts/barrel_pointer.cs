using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace object_pool
{
public class barrel_pointer : MonoBehaviour
{
public GameObject target;
   private void FixedUpdate()
    {
      transform.LookAt(target.transform.position);
    }
}
}