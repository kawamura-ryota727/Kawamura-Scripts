//作成者：川村良太
//作成日：2019/10/07
//上から見るカメラ視点の位置オブジェクトの位置を決める

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTopPos : MonoBehaviour
{
	[Header("手動で入れよう！チャージャー")]
	public GameObject chargerObj;					//チャージャーオブジェクト
	[Header("手動で入れよう！挿入口")]
	public GameObject phoneObj;                 //スマホオブジェクト

	public Vector3 chargerPos;
	public Vector3 phonePos;

	[Header("スマホとチャージャーの最大距離")]
	public float distanceMax;
	[Header("スマホとチャージャーの最小距離")]
	public float distanceMin;
	[Header("スマホとチャージャーの今の距離")]
	public float distance;

	public float posZ;

	bool once = true;
	void Start()
    {
		chargerPos = new Vector3(chargerObj.transform.position.x, phoneObj.transform.position.y, chargerObj.transform.position.z);
		phonePos = phoneObj.transform.position;
		distanceMax = 19.53912f;
		distanceMin = 4.5f;
	}

	void Update()
    {
        if(once)
		{
			//transform.position = new Vector3(phoneObj.transform.position.x, transform.position.y, phoneObj.transform.position.z - 2f);
			transform.position = new Vector3(phoneObj.transform.position.x, 19f, -7f);
			once = false;
		}

		chargerPos = new Vector3(chargerObj.transform.position.x, phoneObj.transform.position.y, chargerObj.transform.position.z);
		distance = Vector3.Distance(chargerPos, phonePos);

		posZ = distanceMax - distance * 0.598f - 9f;

	}
}
