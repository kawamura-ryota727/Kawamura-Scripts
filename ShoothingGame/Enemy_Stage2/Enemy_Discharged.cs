using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StorageReference;

public class Enemy_Discharged : character_status
{
	//動きタイプ
	public enum MoveType
	{
		LeftCurveUp_90,				//上に上がって90度左に曲がる
		LeftCueveUp_180,				//右に出て左回りに180度曲がる
		LeftCurveDown_90,			//下に下がって90度左に曲がる
		LeftCueveDown_180,			//右に出て右回りに180度曲がる
		RightCurveUp_90,				//上に上がって90度右に曲がる
		RightCueveUp_180,			//左に出て右曲がりに180度曲がる
		RightCurveDown_90,			//下に下がって90度左に曲がる
		RightCueveDown_180,		//左に出て左回りに180度に曲がる
		FreeLeft90,
		FreeRight90,
	}
	public MoveType moveType;		//動きタイプ変数


	public enum MoveState
	{
		LeftXMove,			//左移動
		RightXMove,			//右移動
		UpYMove,				//上移動
		DownYMove,			//下移動
		MiddleMove,			//上下移動から横移動に移るまでの間の移動
	}

	public MoveState moveState;
	public MoveState saveMoveState;

    public GameObject defaultParentObj;
	public GameObject modelObj;         //モデルオブジェクト（主に左右の動きで向きを変えるのに使う）
	Enemy_Roll roll_Script;

	Vector3 velocity;

	//ここから90度カーブ用
	[Header("入力用　Xスピード")]
	public float speedX;
	[Header("入力用　	最大Xスピード")]
	public float speedXMax;
	public float defaultSpeedX;					//X初期スピード
	[Header("入力用　Xスピードの増減値")]
	public float changeSpeedX_value;
	[Header("入力用　Yスピード")]
	public float speedY;
	public float defaultSpeedY;					//Y初期スピード
	[Header("入力用　Yスピードの増減値")]
	public float changeSpeedY_value;

	[Header("入力用　横移動の最大時間　秒")]
	public float XMoveTimeMax;
	public float XMoveTimeCnt;					//横に動いている時間カウント
	[Header("入力用　上下移動の最大時間　秒")]
	public float YMoveTimeMax;
	public float YMoveTimeCnt;                  //上下に動いている時間カウント

	bool isSpeedXCangeEnd = false;          //横移動のスピードが変化し終わったか用
	bool isSpeedYCangeEnd = false;			//上下移動のスピードが変化し終わったかどうか用
	//ここまで90度カーブ用

	//ここから180度カーブ用
	[Header("入力用　180Xスピード")]
	public float speedX180;
	[Header("入力用　180最大Xスピード")]
	public float speedXMax180;
	public float defaultSpeedX180;
	[Header("入力用　180Xスピードの増減値")]
	public float changeSpeedX_value180;

	public float speedY180;
	[Header("入力用　180初期Yスピード")]
	public float defaultSpeedY180;
	public float speedYMax180;
	[Header("入力用　180Yスピードの増減値")]
	public float changeSpeedY_value180;

	[Header("入力用　180横移動の最大時間　秒")]
	public float XMoveTimeMax180 = 0;
	public float XMoveTimeCnt180 = 0;

	public int speedStateCnt = 0;

	//ここまで180度カーブ用

	//死亡時処理用
	public Find_Angle fd;						//プレイヤーの方向を向くスクリプト取得用（死亡時攻撃に使う） パブリックで入れて
	public Quaternion diedAttackRota;			//死んだ時に出す弾の角度範囲
	public bool Died_Attack = false;
	[Header("死亡時の弾発射の角度範囲()")]
	public float diedAttack_RotaValue;

	public bool once = true;        //一回だけ行う処理用
	public bool isRotaReset = true;

	new void Start()
	{
		//モデル取得
		modelObj = transform.GetChild(0).gameObject;
		roll_Script = modelObj.GetComponent<Enemy_Roll>();

		defaultSpeedX = speedX;
		defaultSpeedY = speedY;
		XMoveTimeCnt = 0;
		YMoveTimeCnt = 0;

		once = true;
		isSpeedYCangeEnd = false;
		isSpeedXCangeEnd = false;

		base.Start();
	}

	new void Update()
	{
		//一回だけ行う
		if (once)
		{
			//動きのタイプで最初上に動くか下に動くか決める
			if (moveType == Enemy_Discharged.MoveType.LeftCurveUp_90 || moveType == Enemy_Discharged.MoveType.LeftCueveUp_180 || moveType == Enemy_Discharged.MoveType.RightCurveUp_90 || moveType == Enemy_Discharged.MoveType.RightCueveUp_180)
			{
				moveState = Enemy_Discharged.MoveState.UpYMove;
			}
			else if (moveType == Enemy_Discharged.MoveType.LeftCurveDown_90 || moveType == Enemy_Discharged.MoveType.LeftCueveDown_180|| moveType == Enemy_Discharged.MoveType.RightCurveDown_90 || moveType == Enemy_Discharged.MoveType.RightCueveDown_180)
			{
				moveState = Enemy_Discharged.MoveState.DownYMove;
			}

			//動きの種類でモデルの向きを変える
			if (moveType == MoveType.LeftCurveUp_90 || moveType == MoveType.LeftCueveUp_180 || moveType == MoveType.LeftCurveDown_90 || moveType == MoveType.LeftCueveDown_180)
			{
				if (isRotaReset)
				{
					modelObj.transform.rotation = Quaternion.Euler(0, -90, 0);
					roll_Script.rotaY = -90;
					//modelObj.transform.rotation = Quaternion.Euler(0, 0, 0);
					transform.rotation = Quaternion.Euler(0, 0, 0);
				}
				else
				{
					modelObj.transform.rotation = Quaternion.Euler(0, 90, 0);
					transform.rotation = Quaternion.Euler(0, 180, 0);
				}
			}
			else if (moveType == MoveType.RightCurveUp_90 || moveType == MoveType.RightCueveUp_180 || moveType == MoveType.RightCurveDown_90 || moveType == MoveType.RightCueveDown_180)
			{
				if (isRotaReset)
				{
					modelObj.transform.rotation = Quaternion.Euler(0, -270, 0);
					roll_Script.rotaY = -270;
					transform.rotation = Quaternion.Euler(0, 180, 0);
				}
				else
				{
					modelObj.transform.rotation = Quaternion.Euler(0, 90, 0);
					//transform.rotation = Quaternion.Euler(0, 180, transform.rotation.z);

				}
			}
			else if (moveType == MoveType.FreeLeft90)
			{
				
			}
			else if (moveType == MoveType.FreeRight90)
			{
				modelObj.transform.rotation = Quaternion.Euler(0, 90, 0);
				roll_Script.rotaY = 90;
			}

            speedX = defaultSpeedX;
            speedY = defaultSpeedY;

            //上下の移動のスピードを決める（プラスマイナスがあっていなかったら変える）
            if (moveState == MoveState.UpYMove && speedY < 0)
			{
				speedY *= -1;
			}
			else if (moveState == MoveState.DownYMove && speedY > 0)
			{
				speedY *= -1;
			}

            //defaultSpeedX = speedX;
            //defaultSpeedY = speedY;
			XMoveTimeCnt = 0;
			YMoveTimeCnt = 0;

			speedX180 = defaultSpeedX180;
			speedY180 = 0;
			XMoveTimeCnt180 = 0;
			speedStateCnt = 0;
            isSpeedYCangeEnd = false;
            isSpeedXCangeEnd = false;

            once = false;
		}

		//velocity = gameObject.transform.rotation * new Vector3(speedX, speedY, 0);
		//gameObject.transform.position += velocity * Time.deltaTime;

		SpeedCange();

		if (hp < 1)
		{
			if (Died_Attack)
			{
				//死亡時攻撃の処理
				//int bulletSpread = 15;      //角度を広げるための変数
				//複数発出すよう
				//for (int i = 0; i < 3; i++)
				//{
				//	//diedAttack_RotaZ = Random.Range(fd.degree - diedAttack_RotaValue, fd.degree + diedAttack_RotaValue);
				//	//diedAttack_Transform.rotation = Quaternion.Euler(0, 0, diedAttack_RotaZ);
				//	//diedAttackRota = Quaternion.Euler(0, 0, Random.Range(fd.degree - diedAttack_RotaValue, fd.degree + diedAttack_RotaValue));

				//	diedAttackRota = Quaternion.Euler(0, 0, angle_Script.degree + bulletSpread);

				//	Object_Instantiation.Object_Reboot(Game_Master.OBJECT_NAME.eENEMY_BULLET, transform.position, diedAttackRota);
				//	bulletSpread -= 15;     //広げる角度を変える
				//}

				diedAttackRota = Quaternion.Euler(0, 0, Random.Range(fd.degree - diedAttack_RotaValue, fd.degree + diedAttack_RotaValue));
				Object_Instantiation.Object_Reboot(Game_Master.OBJECT_NAME.eENEMY_BULLET, transform.position, diedAttackRota);

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

	//----------------------ここから関数-------------------------
	//スピード変化関数
	void SpeedCange()
	{
		//動きのタイプを見る
		switch(moveType)
		{
			//90度左に曲がる
			case MoveType.FreeLeft90:
				//上に上がる状態なら
				if (moveState == MoveState.UpYMove)
				{
					//上に上がっている時間を数える
					YMoveTimeCnt += Time.deltaTime;
					//上がる最大時間を超えたら
					if (YMoveTimeCnt > YMoveTimeMax)
					{
						//状態を変える（曲がる動きの状態へ）
						moveState = MoveState.MiddleMove;
					}
				}
				//曲がる動きの状態なら
				else if (moveState == MoveState.MiddleMove)
				{
					//Xスピードの変化が終わっていないなら
					if (!isSpeedXCangeEnd)
					{
						//マイナス方向へのスピードを増やす
						speedX -= changeSpeedX_value;
						//マイナス方向補スピード最大値を超えたら
						if (speedX < -speedXMax)
						{
							//Xスピード変化終わり判定
							isSpeedXCangeEnd = true;
							//スピードを最大値にする
							speedX = -speedXMax;
						}
					}
					//Yスピードの変化が終わっていないなら
					if (!isSpeedYCangeEnd)
					{
						//yスピードを減らす
						speedY -= changeSpeedX_value;
						//Yスピードが0より小さくなったら
						if (speedY < 0)
						{
							//Yスピード変化終わり判定
							isSpeedYCangeEnd = true;
							//Yスピードを0に直す
							speedY = 0;
						}
					}
					//XスピードとYスピードどちらの変化も終わっていたら
					if (isSpeedXCangeEnd && isSpeedYCangeEnd)
					{
						//動きの状態を左移動状態にする
						moveState = MoveState.LeftXMove;
					}
				}
				//移動
				velocity = gameObject.transform.rotation * new Vector3(-speedX, speedY, 0);
				gameObject.transform.position += velocity * Time.deltaTime;

				break;


			//90度左に曲がる
			case MoveType.LeftCurveUp_90:
				//上に上がる状態なら
				if (moveState == MoveState.UpYMove)
				{
					//上に上がっている時間を数える
					YMoveTimeCnt += Time.deltaTime;
					//上がる最大時間を超えたら
					if (YMoveTimeCnt > YMoveTimeMax)
					{
						//状態を変える（曲がる動きの状態へ）
						moveState = MoveState.MiddleMove;
					}
				}
				//曲がる動きの状態なら
				else if (moveState == MoveState.MiddleMove)
				{
					//Xスピードの変化が終わっていないなら
					if (!isSpeedXCangeEnd)
					{
						//マイナス方向へのスピードを増やす
						speedX -= changeSpeedX_value;
						//マイナス方向補スピード最大値を超えたら
						if (speedX < -speedXMax)
						{
							//Xスピード変化終わり判定
							isSpeedXCangeEnd = true;
							//スピードを最大値にする
							speedX = -speedXMax;
						}
					}
					//Yスピードの変化が終わっていないなら
					if (!isSpeedYCangeEnd)
					{
						//yスピードを減らす
						speedY -= changeSpeedX_value;
						//Yスピードが0より小さくなったら
						if (speedY < 0)
						{
							//Yスピード変化終わり判定
							isSpeedYCangeEnd = true;
							//Yスピードを0に直す
							speedY = 0;
						}	
					}
					//XスピードとYスピードどちらの変化も終わっていたら
					if (isSpeedXCangeEnd && isSpeedYCangeEnd)
					{
						//動きの状態を左移動状態にする
						moveState = MoveState.LeftXMove;
					}
				}
				//移動
				velocity = gameObject.transform.rotation * new Vector3(speedX, speedY, 0);
				gameObject.transform.position += velocity * Time.deltaTime;

				break;

			case MoveType.LeftCueveUp_180:
				switch(speedStateCnt)
				{
					//最初に横移動している状態
					case 0:
						if (XMoveTimeCnt180 > XMoveTimeMax180)
						{
							speedStateCnt++;
						}
						XMoveTimeCnt180 += Time.deltaTime;

						break;

						//横移動速度が減少して上移動速度が上昇している状態
					case 1:
						speedX180 -= changeSpeedX_value180;
						//speedY180 += changeSpeedY_value180;
						//speedY180 *= 1.1f;
						speedY180 = defaultSpeedY180;
						//if (speedY180 > speedYMax180)
						//{
						//	speedY180 = speedYMax180;
						//}
						if (speedX180 <= 0)
						{
							speedStateCnt++;
						}
						break;

						//横移動が0になったあと最初と逆方向にスピードが上がる状態
					case 2:
						speedX180 -= changeSpeedX_value180;
						//speedY180 = speedY180 / 11 * 10;
						//speedY180 -= changeSpeedY_value180;
						speedY180 = defaultSpeedY180;
						if (speedX180 < -defaultSpeedX180)
						{
							//speedX180 = speedXMax180;
							speedY180 = 0;

							speedStateCnt++;
						}

						//if (speedY180 <= 0)
						//{
						//	speedY180 = 0;
						//	speedStateCnt++;
						//}
						break;

					case 3:
						speedX180 -= changeSpeedX_value180;
						if(speedX180 < speedXMax180)
						{
							speedX180 = speedXMax180;
							speedStateCnt++;
						}
						break;

					case 4:
						break;

				}
				velocity = gameObject.transform.rotation * new Vector3(speedX180, speedY180, 0);
				gameObject.transform.position += velocity * Time.deltaTime;

				break;

			case MoveType.LeftCurveDown_90:
				if (moveState == MoveState.DownYMove)
				{
					YMoveTimeCnt += Time.deltaTime;
					if (YMoveTimeCnt > YMoveTimeMax)
					{
						moveState = MoveState.MiddleMove;
					}
				}
				else if (moveState == MoveState.MiddleMove)
				{
					if (!isSpeedXCangeEnd)
					{
						speedX -= changeSpeedX_value;
						if (speedX < -speedXMax)
						{
							isSpeedXCangeEnd = true;
							speedX = -speedXMax;
						}
					}

					if (!isSpeedYCangeEnd)
					{
						speedY += changeSpeedX_value;
						if (speedY > 0)
						{
							isSpeedYCangeEnd = true;
							speedY = 0;
						}		
					}

					if (isSpeedXCangeEnd && isSpeedYCangeEnd)
					{
						moveState = MoveState.LeftXMove;
					}
				}
				velocity = gameObject.transform.rotation * new Vector3(speedX, speedY, 0);
				gameObject.transform.position += velocity * Time.deltaTime;

				break;

			case MoveType.LeftCueveDown_180:
				switch (speedStateCnt)
				{
					//最初に横移動している状態
					case 0:
						if (XMoveTimeCnt180 > XMoveTimeMax180)
						{
							speedStateCnt++;
						}
						XMoveTimeCnt180 += Time.deltaTime;

						break;

					//横移動速度が減少して上移動速度が上昇している状態
					case 1:
						speedX180 -= changeSpeedX_value180;
						//speedY180 += changeSpeedY_value180;
						//speedY180 *= 1.1f;
						speedY180 = -defaultSpeedY180;
						//if (speedY180 > speedYMax180)
						//{
						//	speedY180 = speedYMax180;
						//}
						if (speedX180 <= 0)
						{
							speedStateCnt++;
						}
						break;

					//横移動が0になったあと最初と逆方向にスピードが上がる状態
					case 2:
						speedX180 -= changeSpeedX_value180;
						//speedY180 = speedY180 / 11 * 10;
						//speedY180 -= changeSpeedY_value180;
						speedY180 = -defaultSpeedY180;
						if (speedX180 < -defaultSpeedX180)
						{
							//speedX180 = speedXMax180;
							speedY180 = 0;

							speedStateCnt++;
						}

						//if (speedY180 <= 0)
						//{
						//	speedY180 = 0;
						//	speedStateCnt++;
						//}
						break;

					case 3:
						speedX180 -= changeSpeedX_value180;
						if (speedX180 < speedXMax180)
						{
							speedX180 = speedXMax180;
							speedStateCnt++;
						}
						break;

					case 4:
						break;

				}
				velocity = gameObject.transform.rotation * new Vector3(speedX180, speedY180, 0);
				gameObject.transform.position += velocity * Time.deltaTime;

				break;

			case MoveType.FreeRight90:
				if (moveState == MoveState.UpYMove)
				{
					YMoveTimeCnt += Time.deltaTime;
					if (YMoveTimeCnt > YMoveTimeMax)
					{
						moveState = MoveState.MiddleMove;
					}
				}
				else if (moveState == MoveState.MiddleMove)
				{
					if (!isSpeedXCangeEnd)
					{
						speedX -= changeSpeedX_value;
						if (speedX < -speedXMax)
						{
							isSpeedXCangeEnd = true;
							speedX = -speedXMax;
						}
					}

					if (!isSpeedYCangeEnd)
					{
						speedY -= changeSpeedX_value;
						if (speedY < 0)
						{
							isSpeedYCangeEnd = true;
							speedY = 0;
						}
					}

					if (isSpeedXCangeEnd && isSpeedYCangeEnd)
					{
						moveState = MoveState.LeftXMove;
					}
				}
				velocity = gameObject.transform.rotation * new Vector3(-speedX, speedY, 0);
				gameObject.transform.position += velocity * Time.deltaTime;
				break;


			case MoveType.RightCurveUp_90:
				if (moveState == MoveState.UpYMove)
				{
					YMoveTimeCnt += Time.deltaTime;
					if (YMoveTimeCnt > YMoveTimeMax)
					{
						moveState = MoveState.MiddleMove;
					}
				}
				else if (moveState == MoveState.MiddleMove)
				{
					if (!isSpeedXCangeEnd)
					{
						speedX -= changeSpeedX_value;
						if (speedX < -speedXMax)
						{
							isSpeedXCangeEnd = true;
							speedX = -speedXMax;
						}
					}

					if (!isSpeedYCangeEnd)
					{
						speedY -= changeSpeedX_value;
						if (speedY < 0)
						{
							isSpeedYCangeEnd = true;
							speedY = 0;
						}
					}

					if (isSpeedXCangeEnd && isSpeedYCangeEnd)
					{
						moveState = MoveState.LeftXMove;
					}
				}
				velocity = gameObject.transform.rotation * new Vector3(speedX, speedY, 0);
				gameObject.transform.position += velocity * Time.deltaTime;

				//if (moveState == MoveState.UpYMove)
				//{
				//	YMoveTimeCnt += Time.deltaTime;
				//	if (YMoveTimeCnt > YMoveTimeMax)
				//	{
				//		moveState = MoveState.MiddleMove;
				//	}
				//}
				//else if (moveState == MoveState.MiddleMove)
				//{
				//	if (!isSpeedXCangeEnd)
				//	{
				//		speedX += changeSpeedX_value;
				//		if (speedX > speedXMax)
				//		{
				//			isSpeedXCangeEnd = true;
				//			speedX = speedXMax;
				//		}
				//	}

				//	if (!isSpeedYCangeEnd)
				//	{
				//		speedY -= changeSpeedX_value;
				//		if (speedY < 0)
				//		{
				//			isSpeedYCangeEnd = true;
				//			speedY = 0;
				//		}
				//	}

				//	if (isSpeedXCangeEnd && isSpeedYCangeEnd)
				//	{
				//		moveState = MoveState.LeftXMove;
				//	}
				//}

				break;

			case MoveType.RightCueveUp_180:
				switch (speedStateCnt)
				{
					//最初に横移動している状態
					case 0:
						if (XMoveTimeCnt180 > XMoveTimeMax180)
						{
							speedStateCnt++;
						}
						XMoveTimeCnt180 += Time.deltaTime;

						break;

					//横移動速度が減少して上移動速度が上昇している状態
					case 1:
						speedX180 -= changeSpeedX_value180;
						//speedY180 += changeSpeedY_value180;
						//speedY180 *= 1.1f;
						speedY180 = defaultSpeedY180;
						//if (speedY180 > speedYMax180)
						//{
						//	speedY180 = speedYMax180;
						//}
						if (speedX180 <= 0)
						{
							speedStateCnt++;
						}
						break;

					//横移動が0になったあと最初と逆方向にスピードが上がる状態
					case 2:
						speedX180 -= changeSpeedX_value180;
						//speedY180 = speedY180 / 11 * 10;
						//speedY180 -= changeSpeedY_value180;
						speedY180 = defaultSpeedY180;
						if (speedX180 < -defaultSpeedX180)
						{
							//speedX180 = speedXMax180;
							speedY180 = 0;

							speedStateCnt++;
						}

						//if (speedY180 <= 0)
						//{
						//	speedY180 = 0;
						//	speedStateCnt++;
						//}
						break;

					case 3:
						speedX180 -= changeSpeedX_value180;
						if (speedX180 < speedXMax180)
						{
							speedX180 = speedXMax180;
							speedStateCnt++;
						}
						break;

					case 4:
						break;

				}
				velocity = gameObject.transform.rotation * new Vector3(speedX180, speedY180, 0);
				gameObject.transform.position += velocity * Time.deltaTime;

				break;

			case MoveType.RightCurveDown_90:
				if (moveState == MoveState.DownYMove)
				{
					YMoveTimeCnt += Time.deltaTime;
					if (YMoveTimeCnt > YMoveTimeMax)
					{
						moveState = MoveState.MiddleMove;
					}
				}
				else if (moveState == MoveState.MiddleMove)
				{
					if (!isSpeedXCangeEnd)
					{
						speedX -= changeSpeedX_value;
						if (speedX < -speedXMax)
						{
							isSpeedXCangeEnd = true;
							speedX = -speedXMax;
						}
					}

					if (!isSpeedYCangeEnd)
					{
						speedY += changeSpeedX_value;
						if (speedY > 0)
						{
							isSpeedYCangeEnd = true;
							speedY = 0;
						}
					}

					if (isSpeedXCangeEnd && isSpeedYCangeEnd)
					{
						moveState = MoveState.LeftXMove;
					}
				}
				velocity = gameObject.transform.rotation * new Vector3(speedX, speedY, 0);
				gameObject.transform.position += velocity * Time.deltaTime;

				//if (moveState == MoveState.DownYMove)
				//{
				//	YMoveTimeCnt += Time.deltaTime;
				//	if (YMoveTimeCnt > YMoveTimeMax)
				//	{
				//		moveState = MoveState.MiddleMove;
				//	}
				//}
				//else if (moveState == MoveState.MiddleMove)
				//{
				//	if (!isSpeedXCangeEnd)
				//	{
				//		speedX += changeSpeedX_value;
				//		if (speedX > speedXMax)
				//		{
				//			isSpeedXCangeEnd = true;
				//			speedX = speedXMax;
				//		}
				//	}

				//	if (!isSpeedYCangeEnd)
				//	{
				//		speedY += changeSpeedX_value;
				//		if (speedY > 0)
				//		{
				//			isSpeedYCangeEnd = true;
				//			speedY = 0;
				//		}
				//	}

				//	if (isSpeedXCangeEnd && isSpeedYCangeEnd)
				//	{
				//		moveState = MoveState.LeftXMove;
				//	}
				//}

				break;

			case MoveType.RightCueveDown_180:
				switch (speedStateCnt)
				{
					//最初に横移動している状態
					case 0:
						if (XMoveTimeCnt180 > XMoveTimeMax180)
						{
							speedStateCnt++;
						}
						XMoveTimeCnt180 += Time.deltaTime;

						break;

					//横移動速度が減少して上移動速度が上昇している状態
					case 1:
						speedX180 -= changeSpeedX_value180;
						//speedY180 += changeSpeedY_value180;
						//speedY180 *= 1.1f;
						speedY180 = -defaultSpeedY180;
						//if (speedY180 > speedYMax180)
						//{
						//	speedY180 = speedYMax180;
						//}
						if (speedX180 <= 0)
						{
							speedStateCnt++;
						}
						break;

					//横移動が0になったあと最初と逆方向にスピードが上がる状態
					case 2:
						speedX180 -= changeSpeedX_value180;
						//speedY180 = speedY180 / 11 * 10;
						//speedY180 -= changeSpeedY_value180;
						speedY180 = -defaultSpeedY180;
						if (speedX180 < -defaultSpeedX180)
						{
							//speedX180 = speedXMax180;
							speedY180 = 0;

							speedStateCnt++;
						}

						//if (speedY180 <= 0)
						//{
						//	speedY180 = 0;
						//	speedStateCnt++;
						//}
						break;

					case 3:
						speedX180 -= changeSpeedX_value180;
						if (speedX180 < speedXMax180)
						{
							speedX180 = speedXMax180;
							speedStateCnt++;
						}
						break;

					case 4:
						break;

				}
				velocity = gameObject.transform.rotation * new Vector3(speedX180, speedY180, 0);
				gameObject.transform.position += velocity * Time.deltaTime;

				break;
		}
	}

	public void SetState(MoveType mType)
	{
		moveType = mType;
		once = true;
	}
}
