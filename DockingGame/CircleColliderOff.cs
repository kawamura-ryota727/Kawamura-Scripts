using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleColliderOff : MonoBehaviour
{
    GameObject cameraObj;
    GameObject circeGroupObj;
    CameraWork camera_Script;
    public CircleGroupManager circleGroup_Script;
    public  Collider[] myCollider;
    Light_Manager light_Script;

    public int cnt;
    float frame;
    public bool isCheck = false;
    bool isNumPlus = false;
    void Start()
    {
        cnt = 0;
        // myCollider = gameObject.GetComponent<CapsuleCollider>();
        cameraObj = GameObject.Find("Main Camera");
        camera_Script = cameraObj.GetComponent<CameraWork>();
        light_Script = gameObject.GetComponent<Light_Manager>();
        circeGroupObj = GameObject.Find("prismsetCircle_Group");
        circleGroup_Script = circeGroupObj.GetComponent<CircleGroupManager>();
    }

    void Update()
    {
        if(isNumPlus)
        {
            frame++;
            if (frame > 15)
            {
                circleGroup_Script.cnt++;
                isNumPlus = false;
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag=="Charger")
        {
            //cnt++;
            isCheck = true;
            isNumPlus = true;
            light_Script.isCheck = true;
            for (int i = 0; i < myCollider.Length; i++)
            {
                myCollider[i].enabled = false;
            }
            //myCollider.enabled = false;
        }
    }
}
