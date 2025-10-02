using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace object_pool
{
public class object_pooling_example_controller : MonoBehaviour
{
public barrel br;
        public GameObject target;
        public Vector3 positionA;
    public Vector3 positionB;
    public float speed = 2.0f;
        private bool movingToB = true;
    private float lerpTime = 0.0f;
        void Update()
    {
        MoveObject();
    }

    private void MoveObject()
    {
        lerpTime += Time.deltaTime * speed * (movingToB ? 1 : -1);

        lerpTime = Mathf.Clamp01(lerpTime);

        target.transform.position = Vector3.Lerp(positionA, positionB, lerpTime);

        if (lerpTime >= 1.0f || lerpTime <= 0.0f)
        {
            movingToB = !movingToB;
        }
    }
public void toggle_barrel_activation()
{
br.shoot_bullet=!br.shoot_bullet;
}
}
}