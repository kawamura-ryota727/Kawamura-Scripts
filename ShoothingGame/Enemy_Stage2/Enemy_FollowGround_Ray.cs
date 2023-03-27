using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_FollowGround_Ray : MonoBehaviour
{
	public GameObject angleObj;

	Vector3 velocity;
	Vector3 velocity_norm;

	Collider coll;

	public RayCheck TopCheck;           //上側のコライダーチェック用
	public RayCheck UnderCheck;     //下側のコライダーチェック用
	public RayCheck LeftCheck;          //左側のコライダーチェック用
	public RayCheck RightCheck;     //右側のコライダーチェック用

	public float speedX_Max;        //Xスピードマックス
	public float speedX;            //Xスピード
	public float speedY_Max;        //Yスピードマックス
	public float speedY;            //Yスピード

	float changeDelayCnt;                   //移動向きが変わったときに連続で切り替わらないようにするためのディレイカウント
	public float chamgeDelayMax;        //ディレイの最大

	public int hitTotal;                        //いくつのオブジェクトに当たっているかの合計

	public float angle;

	bool isTop;         //上の判定
	bool isUnder;       //下の判定
	bool isLeft;        //左の判定
	bool isRight;       //右の判定
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		velocity = angleObj.transform.rotation * new Vector3(speedX, 0, 0);
		velocity_norm = velocity.normalized;
		gameObject.transform.position += velocity_norm * Time.deltaTime;

	}
}
