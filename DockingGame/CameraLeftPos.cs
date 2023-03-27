//作成者：川村良太
//作成日：2019/10/07
//横から見るカメラ視点の位置オブジェクトの位置を決める

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLeftPos : MonoBehaviour
{
	[Header("手動で入れよう！スマホ")]
	public GameObject phoneObj;

	bool once = true;
	void Start()
	{

	}

	void Update()
	{
		if (once)
		{
			//transform.position = new Vector3(transform.position.x, phoneObj.transform.position.y, phoneObj.transform.position.z - 2f);
			transform.position = new Vector3(-11.5f, phoneObj.transform.position.y, -6f);
			once = false;
		}
	}
}
