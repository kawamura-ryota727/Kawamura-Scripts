using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrismMove : MonoBehaviour
{
    public CircleColliderOff circleCol_Script;
    public float speed;
    public float speedMax;
    public float addSpeed_Value;
    Vector3 velocity;

    void Start()
    {
        circleCol_Script = transform.parent.gameObject.GetComponent<CircleColliderOff>();
    }

    void Update()
    {
        if (circleCol_Script.isCheck)
        {
            speed += addSpeed_Value;
            if (speed > speedMax)
            {
                speed = speedMax;
            }
            velocity = gameObject.transform.rotation * new Vector3(speed, 0, 0);
            gameObject.transform.position += velocity * Time.deltaTime;
        }

    }
}
