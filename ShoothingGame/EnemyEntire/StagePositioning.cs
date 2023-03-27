//ステージ2の移動した分だけ移動する（ステージの子供にしない場合）

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagePositioning : MonoBehaviour
{
	GameObject stageObj;
	Vector3 stagePos;
	Vector3 stagePreviousPos;
	public Vector3 distance;

	public bool isStageMove = false;

	void Start()
    {
		stageObj = GameObject.Find("Stage_02_Map");
		stagePos = stageObj.transform.position;
		stagePreviousPos = stageObj.transform.position;
	}

	void Update()
    {
		stagePos = stageObj.transform.position;
		if (stagePos != stagePreviousPos && !isStageMove)
		{
			distance = stagePos - stagePreviousPos;
			stagePreviousPos = stagePos;
			isStageMove = true;
		}

		if (isStageMove)
		{
			transform.position += distance;
			isStageMove = false;
		}

	}
}
