//ステージが次のパーツを出す地点を通過したか判定

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassingCheck : MonoBehaviour
{
	GameObject parentObj;
	StageManager stageManager_Script;

	public bool colcheck = false;
	public bool onceCheck = true;

    void Start()
    {
		parentObj = transform.parent.gameObject;
		stageManager_Script = parentObj.GetComponent<StageManager>();
    }

    void Update()
    {
        
    }
	//private void OnCollisionEnter(Collision collision)
	//{
	//	if (collision.gameObject.tag == "Check" && onceCheck)
	//	{
	//		stageManager_Script.isStageOn = true;
	//		colcheck = true;

	//		onceCheck = false;
	//	}

	//}

	private void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == "Check" && onceCheck)
		{
			stageManager_Script.isStageOn = true;
			colcheck = true;

			onceCheck = false;
		}
	}

	//private void OnTriggerExit(Collider col)
	//{
	//	if (col.gameObject.tag == "Player")
	//	{
	//		stageManager_Script.isStageOn = true;
	//		colcheck = true;
	//	}
	//}
}
