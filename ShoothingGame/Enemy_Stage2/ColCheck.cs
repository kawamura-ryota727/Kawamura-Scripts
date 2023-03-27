using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColCheck : MonoBehaviour
{
	Ray ray; //レイ
	Ray ray2;
	RaycastHit hit; //ヒットしたオブジェクト情報
	RaycastHit hit2;
	public float raydis = 3;

	Vector3 aaa;
	Vector3 bbb;
	public Vector3 groundAngle;
	public float angleCheck;
	public Quaternion hitAngle;

	string myName;

	public bool isVertical = false;		//縦
	public bool isHorizontal = false;	//横

	public bool isCheck = false;
	public int hitCnt;
    void Start()
    {
		myName = gameObject.name;

		if (myName == "Col_Top" || myName == "Col_Under")
		{
			isVertical = true;
		}
		else
		{
			isHorizontal = true;
		}

		hitCnt = 0;
    }


    void Update()
    {
		if (isVertical)
		{
			aaa = new Vector3(transform.position.x - 0.45f, transform.position.y, transform.position.z);
			bbb = new Vector3(transform.position.x + 0.45f, transform.position.y, transform.position.z);
		}
		else if (isHorizontal)
		{
			aaa = new Vector3(transform.position.x, transform.position.y - 0.45f, transform.position.z);
			bbb = new Vector3(transform.position.x, transform.position.y + 0.45f, transform.position.z);
		}
		RayTest();
	
		if (hitCnt > 0)
		{
			isCheck = true;
		}
		else
		{
			isCheck = false;
		}

		Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 1.0f);
	

	}

	void RayTest()
	{
		//Rayの作成　　　　　　　↓Rayを飛ばす原点　　　↓Rayを飛ばす方向
		//Ray ray = new Ray(transform.position, new Vector3(0, 1, 0));
		ray = new Ray(aaa, transform.TransformDirection(Vector3.forward));
		ray2 = new Ray(bbb, transform.TransformDirection(Vector3.forward));

		//レイキャスト（原点, 飛ばす方向, 衝突した情報, 長さ）
		if (Physics.Raycast(ray, out hit, raydis) || Physics.Raycast(ray2, out hit, raydis))
		{
			//groundAngle = hit.normal;
			//当たった時の処理
			if (!isCheck)
			{
				groundAngle = Quaternion.FromToRotation(transform.forward, hit.normal).eulerAngles;
				if (isVertical)
				{
					angleCheck = groundAngle.z - 180;
				}
				else if (isHorizontal)
				{
					angleCheck = groundAngle.z - 90;
				}

				if (hit.collider.tag == "Player")
				{
					isCheck = true;
					hitCnt++;
				}
			}
			//Debug.Log(hit.normal);
			//　衝突した面の前方方向と衝突した面の方向から角度を算出し確認
			//Debug.Log(Vector3.Angle(hit.transform.forward, hit.normal));
			//　レイを飛ばした方向と衝突した面の間の角度を算出する

		}
		else
		{
			isCheck = false;
			hitCnt = 0;
		}
		Debug.DrawRay(aaa, transform.TransformDirection(Vector3.forward) * raydis, Color.red);
		Debug.DrawRay(bbb, transform.TransformDirection(Vector3.forward) * raydis, Color.red);

		//Rayの飛ばせる距離
		//int distance = 10;

		//Rayの可視化    ↓Rayの原点　　　　↓Rayの方向　　　　　　　　　↓Rayの色
		//Debug.DrawLine(ray.origin, ray.direction * distance, Color.red);

		//もしRayにオブジェクトが衝突したら
		//                  ↓Ray  ↓Rayが当たったオブジェクト ↓距離
		//if (Physics.Raycast(ray, out hit, distance))
		//{
		//	//Rayが当たったオブジェクトのtagがPlayerだったら
		//	if (hit.collider.tag == "Player")
		//		Debug.Log("RayがPlayerに当たった");
		//}
	}
	//private void OnTriggerEnter(Collider col)
	//{
	//	if (col.gameObject.tag == "Player")
	//	{
	//		//isCheck = true;
	//		hitCnt++;
	//	}

	//}
	//private void OnTriggerStay(Collider col)
	//{
	//	//if (col.gameObject.tag == "Player")
	//	//{
	//	//	isCheck = true;
	//	//}
	//}
	//private void OnTriggerExit(Collider col)
	//{
	//	if (col.gameObject.tag == "Player")
	//	{
	//		//isCheck = false;
	//		hitCnt--;
	//	}
	//}


}
