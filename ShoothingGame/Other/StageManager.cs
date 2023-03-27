using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
	public int childNum;				//子供の数
	public int stagePartsNum;       //次に出すパーツの番号
	GameObject[] stageObjects;

	public bool isStageOn = false;

	void Start()
    {
		childNum = transform.childCount;
		stageObjects = new GameObject[childNum];

		for (int i = 0; i < childNum; i++)
		{
			stageObjects[i] = transform.GetChild(i).gameObject;
		}
    }


	void Update()
    {
		if (isStageOn)
		{
			if (!stageObjects[stagePartsNum].activeSelf)
			{
				stageObjects[stagePartsNum].SetActive(true);
			}
			stagePartsNum++;
			isStageOn = false;
		}
    }
}
