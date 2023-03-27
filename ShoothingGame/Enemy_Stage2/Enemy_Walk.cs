//作成者：川村良太
//歩く敵のスクリプト

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StorageReference;

public class Enemy_Walk : character_status
{
	//自分の状態
	public enum DirectionState
	{
		Left,			//左向き
		Right,		    //右向き
		Roll,			//回転中
		Stop,			//停止
	}

	public enum Direction_Vertical
	{
		Top,
		Under,
	}
	public　DirectionState direcState;        //状態変数
	public Direction_Vertical direction_Vertical;
	DirectionState saveDirection;           //状態を一時保存する変数

    public GameObject defaultParentObj;
	GameObject childObj;
	Vector3 velocity;

	CharacterController characterController;

	[Header("入力用　歩くスピード")]
	public float speedXMax;
	public float speedX;
	public float speedYMax;
	public float speedY;
	public float rotaY;					//角度
	[Header("入力用　回転スピード")]
	public float rotaSpeed;
	[Header("入力用　歩く最大時間（秒）")]
	public float walkTimeMax;
	public float walkTimeCnt;
	[Header("入力用　止まっている最大時間（秒）")]
	public float stopTimeMax;
	public float stopTimeCnt;			//止まっている時間カウント
	[Header("入力用　攻撃間隔")]
	public float attackTimeMax;
	public float attackTimeCnt;
	float rollDelayCnt;					//回転した後のカウント（回転直後に当たり判定をしないようにするため）

	//
	public Vector3 groundNormal = Vector3.zero;

	private Vector3 lastGroundNormal = Vector3.zero;
	public Vector3 lastHitPoint = new Vector3(Mathf.Infinity, 0, 0);

	public float groundAngle = 0;
	//

	//
	Vector3 normalVector = Vector3.zero;
	public Vector3 onPlane;
    //

    public Find_Angle angleScript;
	public Enemy_Roll roll_Script;
    public Quaternion diedAttackRota;           //死んだ時に出す弾の角度範囲

    public bool isRoll;			//回転中かどうか
	bool isRollEnd = false;     //回転が終わったかどうか
	bool isAttack = true;
    public bool Died_Attack = false;
    public bool once = true;

	new void Start()
    {
		characterController = GetComponent<CharacterController>();
		childObj = transform.GetChild(0).gameObject;
		walkTimeCnt = 0;
		stopTimeCnt = 0;
		rollDelayCnt = 0;
		isRoll = false;
		isAttack = true;

		base.Start();
	}

    new void Update()
    {
        //再起動時の処理
		if (once)
		{
            //回転の方向を変える
			switch(direcState)
			{
				case DirectionState.Left:
                    speedX = -2;

					switch(direction_Vertical)
					{
						case Direction_Vertical.Top:
                            if (roll_Script.rotaX_Value < 0)
                            {
                                roll_Script.rotaX_Value *= -1;
                            }
							break;

						case Direction_Vertical.Under:
                            if (roll_Script.rotaX_Value > 0)
                            {
                                roll_Script.rotaX_Value *= -1;
                            }
                            break;
					}
					break;

				case DirectionState.Right:
                    speedX = 1;

					switch (direction_Vertical)
					{
						case Direction_Vertical.Top:
                            if (roll_Script.rotaX_Value > 0)
                            {
                                roll_Script.rotaX_Value *= -1;
                            }

                            break;

						case Direction_Vertical.Under:
                            if (roll_Script.rotaX_Value < 0)
                            {
                                roll_Script.rotaX_Value *= -1;
                            }

                            break;
					}

					break;
			}

		}
		//this.controller.Move(Vector3.MoveTowards(this.transform.position, cameraPosition, delta) - this.transform.position + Physics.gravity);
		//characterController.Move(velocity * Time.deltaTime);
		//とりあえずすり抜けをなくす処理
		//if (transform.position.y < -4.15f)
		//{
		//	transform.position = new Vector3(transform.position.x, -4.15f, 0);
		//}
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

		//////
		// 平面に投影したいベクトルを作成
		Vector3 inputVector = Vector3.zero;
		inputVector.x = ControllerDevice.GetAxis("Horizontal", ePadNumber.ePlayer1);
		inputVector.z = ControllerDevice.GetAxis("Vertical", ePadNumber.ePlayer1);

		// 平面に沿ったベクトルを計算
		onPlane = Vector3.ProjectOnPlane(inputVector, normalVector);
		//////

		//if (direcState == DirectionState.Left || direcState == DirectionState.Right)
		//{
		//	walkTimeCnt += Time.deltaTime;
		//	if (walkTimeCnt > walkTimeMax)
		//	{
		//		walkTimeCnt = 0;
		//		direcState = DirectionState.Stop;
		//	}
		//}
		//動く関数
		Move();

        if (hp < 1)
        {
            if (Died_Attack)
            {
                //死亡時攻撃の処理
                int bulletSpread = 15;      //角度を広げるための変数
                for (int i = 0; i < 3; i++)
                {
                    //diedAttack_RotaZ = Random.Range(fd.degree - diedAttack_RotaValue, fd.degree + diedAttack_RotaValue);
                    //diedAttack_Transform.rotation = Quaternion.Euler(0, 0, diedAttack_RotaZ);
                    //diedAttackRota = Quaternion.Euler(0, 0, Random.Range(fd.degree - diedAttack_RotaValue, fd.degree + diedAttack_RotaValue));

                    diedAttackRota = Quaternion.Euler(0, 0, angleScript.degree + bulletSpread);

                    Object_Instantiation.Object_Reboot(Game_Master.OBJECT_NAME.eENEMY_BULLET, transform.transform.position, diedAttackRota);
                    bulletSpread -= 15;     //広げる角度を変える
                }

                //diedAttackRota = Quaternion.Euler(0, 0, Random.Range(fd.degree - diedAttack_RotaValue, fd.degree + diedAttack_RotaValue));
                //Object_Instantiation.Object_Reboot(Game_Master.OBJECT_NAME.eENEMY_BULLET, transform.position, diedAttackRota);

            }

            once = true;
            gameObject.transform.parent = defaultParentObj.transform;
            Died_Process();
        }
        else if (transform.position.x < -23)
        {
            once = true;
            gameObject.transform.parent = defaultParentObj.transform;
            gameObject.SetActive(false);
        }
        else if (transform.position.x > 23)
        {
            once = true;
            gameObject.transform.parent = defaultParentObj.transform;
            gameObject.SetActive(false);
        }

        base.Update();
	}

	//----------------ここから関数----------------
	void OnControllerColliderHit(ControllerColliderHit hit)
	{
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

		// 現在の接地面の角度を取得
		groundAngle = Vector3.Angle(hit.normal, Vector3.up);
	}

	//動く関数
	void Move()
	{
		switch(direcState)
		{
			//左向きの時移動する
			case DirectionState.Left:
				switch(direction_Vertical)
				{
					case Direction_Vertical.Top:
						velocity = gameObject.transform.rotation * new Vector3(speedX, speedY, 0);

						break;

					case Direction_Vertical.Under:
						velocity = gameObject.transform.rotation * new Vector3(speedX, -speedY, 0);

						break;
				}
				//gameObject.transform.position += velocity * Time.deltaTime;
				//坂を上り下りできる移動
				characterController.Move(velocity * Time.deltaTime);

				if (characterController.collisionFlags != CollisionFlags.None)
				{
					speedY = 0;
				}
				else
				{
					speedY = 3f;
				}
				walkTimeCnt += Time.deltaTime;
				if (walkTimeCnt > walkTimeMax)
				{
					walkTimeCnt = 0;
					saveDirection = direcState;
					direcState = DirectionState.Stop;
				}
				break;

			//右向きの時移動する
			case DirectionState.Right:
				switch (direction_Vertical)
				{
					case Direction_Vertical.Top:
						velocity = gameObject.transform.rotation * new Vector3(speedX, speedY, 0);

						break;

					case Direction_Vertical.Under:
						velocity = gameObject.transform.rotation * new Vector3(speedX, -speedY, 0);

						break;
				}
				//gameObject.transform.position += velocity * Time.deltaTime;
				characterController.Move(velocity * Time.deltaTime);
				if (characterController.collisionFlags != CollisionFlags.None)
				{
					speedY = 0;
				}
				else
				{
					speedY = 3f;
				}

				walkTimeCnt += Time.deltaTime;
				if (walkTimeCnt > walkTimeMax)
				{
					walkTimeCnt = 0;
					saveDirection = direcState;
					direcState = DirectionState.Stop;
				}
				break;

			//回転する
			case DirectionState.Roll:
				//直前の状態が左向きだったら
				if (saveDirection == DirectionState.Left)
				{
					//向きをマイナス
					rotaY -= rotaSpeed;
					if (rotaY < -180f)
					{
						rotaY = -180f;
						direcState = DirectionState.Right;
						isRoll = false;
						isRollEnd = true;
					}
					transform.rotation = Quaternion.Euler(0, rotaY, 0);
				}
				//直前の状態が右向きだったら
				else if (saveDirection == DirectionState.Right)
				{
					//向きをプラス
					rotaY += rotaSpeed;
					if (rotaY > 0)
					{
						rotaY = 0;
						direcState = DirectionState.Left;
						isRoll = false;
						isRollEnd = true;
					}
					transform.rotation = Quaternion.Euler(0, rotaY, 0);
				}
				break;

			case DirectionState.Stop:
				stopTimeCnt += Time.deltaTime;
				attackTimeCnt += Time.deltaTime;

				if (stopTimeCnt > stopTimeMax)
				{
					direcState = saveDirection;
					stopTimeCnt = 0;
					attackTimeCnt = 0;
					isAttack = true;
				}
				if (isAttack && attackTimeCnt > attackTimeMax)
				{
					Object_Instantiation.Object_Reboot(Game_Master.OBJECT_NAME.eENEMY_BULLET,childObj.transform.position,childObj.transform.rotation);
					isAttack = false;
				}
				break;
		}
	}

	//private void OnTriggerEnter(Collider col)
	//{

	//	//当たったら
	//	if (col.gameObject.name == "Stage02TableB_v2.0")
	//	{
	//		if (!isRollEnd && !isRoll)
	//		{
	//			saveDirection = direcState;
	//			direcState = DirectionState.Roll;
	//			isRoll = true;
	//		}
	//	}

	//}
	//private void OnCollisionStay(Collision collision)
	//{
	//	// 衝突した面の、接触した点における法線を取得
	//	normalVector = collision.contacts[0].normal;
	//}
}
