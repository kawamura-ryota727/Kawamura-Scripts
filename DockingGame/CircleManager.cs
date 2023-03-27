using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleManager : MonoBehaviour
{
    GameObject cameraObj;
    CameraWork camera_Script;

    public string myName;
    void Start()
    {
        cameraObj = GameObject.Find("Main Camera");
        camera_Script = cameraObj.GetComponent<CameraWork>();
        myName = gameObject.name;
    }

    void Update()
    {
        CircleDelete();
    }

    void CircleDelete()
    {
        switch (camera_Script.cameraPosNum)
        {
            case 0:
                //if (myName == "prismsetCircle") { isColor = true; }
                //else { isColor = false; }
                break;
            case 1:
                if (myName == "prismsetCircle") { Destroy(gameObject); }
                break;
            case 2:
                if (myName == "prismsetCircle (1)") { Destroy(gameObject); }
                break;
            case 3:
                if (myName == "prismsetCircle (2)") { Destroy(gameObject); }
                break;
            case 4:
                if (myName == "prismsetCircle (3)") { Destroy(gameObject); }
                break;
            case 5:
                if (myName == "prismsetCircle (4)") { Destroy(gameObject); }
                break;
            case 6:
                if (myName == "prismsetCircle (5)") { Destroy(gameObject); }
                break;
            case 7:
                if (myName == "prismsetCircle (6)") { Destroy(gameObject); }
                break;
        }

    }
}
