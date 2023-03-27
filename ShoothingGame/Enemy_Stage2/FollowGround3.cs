using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StorageReference;

public class FollowGround3 : MonoBehaviour
{
	//自分の状態
	public enum DirectionState
	{
		Left,           //左向き
		Right,      //右向き
		Roll,           //回転中
		Stop,           //停止
	}

	public DirectionState direcState;       //状態変数
	DirectionState saveDirection;           //状態を一時保存する変数

	public GameObject angleObj;
	GameObject childObj;
	Vector3 velocity;

	CharacterController characterController;
	public AngleChange2 angleChange2_Script;

	[Header("入力用　歩くスピード")]
	public float speedXMax;
	public float speedX;
	public float speedYMax;
	public float speedY;
	public float rotaY;                 //角度
	[Header("入力用　回転スピード")]
	public float rotaSpeed;
	[Header("入力用　歩く最大時間（秒）")]
	public float walkTimeMax;
	public float walkTimeCnt;
	[Header("入力用　攻撃間隔")]
	public float attackTimeMax;
	public float attackTimeCnt;
	float rollDelayCnt;                 //回転した後のカウント（回転直後に当たり判定をしないようにするため）
	public int hitDelayMax;
	public int hitDelayCnt;
	public int NotHitMax;
	public int notHitCnt;

	//
	public Vector3 groundNormal = Vector3.zero;

	private Vector3 lastGroundNormal = Vector3.zero;
	public Vector3 lastHitPoint = new Vector3(Mathf.Infinity, 0, 0);

	public float groundAngle = 0;
	public float saveAngle;
	public float angle_sin = 0;
	public float angle_cos = 0;
	//

	//
	public Vector3 normalVector = Vector3.zero;
	public Vector3 onPlane;
	//

	public bool isRoll;         //回転中かどうか
	bool isRollEnd = false;     //回転が終わったかどうか
	bool isAttack = true;
	public bool cccc = false;
	public bool isHit = false;
	public bool isHitP = false;
	public bool tttttssss = false;
	void Start()
	{
		characterController = GetComponent<CharacterController>();
		childObj = transform.GetChild(0).gameObject;
		walkTimeCnt = 0;
		rollDelayCnt = 0;
		isRoll = false;
		isAttack = true;
	}

	void Update()
	{
		//this.controller.Move(Vector3.MoveTowards(this.transform.position, cameraPosition, delta) - this.transform.position + Physics.gravity);
		//characterController.Move(velocity * Time.deltaTime);
		//とりあえずすり抜けをなくす処理
		if (transform.position.y < -4.15f)
		{
			transform.position = new Vector3(transform.position.x, -4.15f, 0);
		}
		//回転が終わった後当たり判定に間を空けるためカウント
		if (isRollEnd)
		{
			rollDelayCnt++;
			if (rollDelayCnt > 5)
			{
				isRollEnd = false;
				rollDelayCnt = 0;
			}
		}
		angle_sin = (Mathf.Asin(normalVector.y) * Mathf.Rad2Deg);
		angle_cos= (Mathf.Asin(normalVector.x) * Mathf.Rad2Deg);

		//////
		// 平面に投影したいベクトルを作成
		Vector3 inputVector = Vector3.zero;
		inputVector.x = ControllerDevice.GetAxis("Horizontal", ePadNumber.ePlayer1);
		inputVector.z = ControllerDevice.GetAxis("Vertical", ePadNumber.ePlayer1);

		// 平面に沿ったベクトルを計算
		onPlane = Vector3.ProjectOnPlane(inputVector, normalVector);
		//////

		//動く関数
		Move();

		if(isHitP)
		{
			hitDelayCnt++;
			notHitCnt = 0;
		}
		else
		{
			notHitCnt++;
		}
	}

	//----------------ここから関数----------------

	public void SetDirection(DirectionState direc)
	{
		direcState = direc;
	}

	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		cccc = true;
		isHitP = true;
		angleChange2_Script.hithit = true;
		if (hit.normal.y > 0 && hit.moveDirection.y < 0)
		{
			if ((hit.point - lastHitPoint).sqrMagnitude > 0.001f || lastGroundNormal == Vector3.zero)
			{
				groundNormal = hit.normal;
			}
			else
			{
				groundNormal = lastGroundNormal;
			}

			lastHitPoint = hit.point;
		}

		normalVector = hit.normal;
		if (normalVector.x > -0.0001f && normalVector.x < 0.0001f)
		{
			normalVector.x = 0;
		}
		if (normalVector.y > -0.0001f && normalVector.y < 0.0001f)
		{
			normalVector.y = 0;
		}

		//normalVector = hit.normal;
		//angleeeeeeeeeeeeeeee = Mathf.Asin(normalVector.x * Mathf.Rad2Deg);

		// 現在の接地面の角度を取得
		groundAngle = Vector3.Angle(hit.normal, Vector3.up);
		saveAngle = Vector3.Angle(hit.normal, Vector3.up);
		groundAngle = Mathf.Round(groundAngle * 10);
		groundAngle /= 10;


	}

	//動く関数
	void Move()
	{
		switch (direcState)
		{
			//左向きの時移動する
			case DirectionState.Left:
				velocity = angleObj.transform.rotation * new Vector3(-speedX, -speedY, 0);
				//gameObject.transform.position += velocity * Time.deltaTime;
				transform.position += angleObj.transform.right.normalized * speedX;
				//坂を上り下りできる移動
				characterController.Move(velocity * Time.deltaTime);

				//angleChange2_Script.angleZ = -groundAngle;
				
				walkTimeCnt += Time.deltaTime;
				//if (walkTimeCnt > walkTimeMax)
				//{
				//	walkTimeCnt = 0;
				//	saveDirection = direcState;
				//	direcState = DirectionState.Stop;
				//}
				break;

			//右向きの時移動する
			case DirectionState.Right:
				velocity = angleObj.transform.rotation * new Vector3(-speedX, -speedY, 0);
				//gameObject.transform.position += velocity * Time.deltaTime;
				transform.position += angleObj.transform.right.normalized * speedX;
				characterController.Move(velocity * Time.deltaTime);

				//angleChange2_Script.angleZ = groundAngle;

				walkTimeCnt += Time.deltaTime;
				//if (walkTimeCnt > walkTimeMax)
				//{
				//	walkTimeCnt = 0;
				//	saveDirection = direcState;
				//	direcState = DirectionState.Stop;
				//}
				break;

		}
	}

	//private void OnCollisionStay(Collision collision)
	//{
	//	//normalVector = collision.contacts[0].normal;
	//	if (collision.gameObject.tag=="Wall")
	//	{
	//		isHitP = true;

	//	}
	//}

	private void OnCollisionEnter(Collision collision)
	{
		//normalVector = collision.contacts[0].normal;
		if (collision.gameObject.tag == "Wall")
		{
			//isHitP = true;
			tttttssss = true;
		}

	}

	//private void OnTriggerEnter(Collider other)
	//{
	//	if (other.gameObject.tag == "Wall")
	//	{
	//		//isHitP = true;
	//		tttttssss = true;
	//		angleChange2_Script.hithit = true;
	//	}
	//}

	//private void OnTriggerExit(Collider other)
	//{
	//	if (other.gameObject.tag == "Wall")
	//	{
	//		//isHitP = true;
	//		tttttssss = false ;
	//		angleChange2_Script.hithit = false;
	//	}
	//}

	//private void OnCollisionExit(Collision collision)
	//{
	//	if (collision.gameObject.tag == "Wall")
	//	{
	//		isHitP = true;

	//	}
	//}
	//private void OnCollision(Collision collision)
	//{
	//	// 衝突した面の、接触した点における法線を取得
	//	normalVector = collision.contacts[0].normal;
	//}
}
