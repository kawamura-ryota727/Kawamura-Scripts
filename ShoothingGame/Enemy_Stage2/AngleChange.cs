using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleChange : MonoBehaviour
{
	public GameObject rayParentObj;
	RayParent rayParent_Script;

	public float angleZ;
	public float saveAngleZ;

	public int delayMax;
	public int delayCnt;

    void Start()
    {
		rayParent_Script = rayParentObj.GetComponent<RayParent>();
    }


    void Update()
    {
		//if (angleZ == saveAngleZ)
		//{

		//}
		//else if (angleZ != saveAngleZ)
		//{
		//	angleZ = saveAngleZ;
		//}
		//angleZ = rayParent_Script.angleZ - 90;
		//angleZ = rayParent_Script.angleZ;

		delayCnt++;
		transform.rotation = Quaternion.Euler(0, 0, angleZ);
    }
}
