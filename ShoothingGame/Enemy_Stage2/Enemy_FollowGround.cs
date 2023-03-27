using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_FollowGround : MonoBehaviour
{
	public enum MoveState
	{
		Up,					//上移動
		Dowx,				//下移動
		Left,					//左移動
		Right,				//右移動
		DefaultLeft,		//最初左に移動する
		DefaultRight,		//最初右に移動する
	}

	public MoveState defaultState;		//最初の移動向き
	public MoveState moveState;			//移動向き
	public MoveState saveState;			//ひとつ前のを見るために入れておく用

	Vector3 velocity;
	Vector3 velocity_norm;

	Collider coll;

	public ColCheck TopCheck;			//上側のコライダーチェック用
	public ColCheck UnderCheck;		//下側のコライダーチェック用
	public ColCheck LeftCheck;			//左側のコライダーチェック用
	public ColCheck RightCheck;     //右側のコライダーチェック用

	public float speedX_Max;        //Xスピードマックス
	public float speedX;			//Xスピード
	public float speedY_Max;        //Yスピードマックス
	public float speedY;			//Yスピード

	float changeDelayCnt;					//移動向きが変わったときに連続で切り替わらないようにするためのディレイカウント
	public float chamgeDelayMax;		//ディレイの最大

	public int hitTotal;						//いくつのオブジェクトに当たっているかの合計

	bool isTop;			//上の判定
	bool isUnder;		//下の判定
	bool isLeft;		//左の判定
	bool isRight;		//右の判定



	void Start()
    {
		//移動向きの設定
		if (defaultState == MoveState.DefaultRight)
		{
			moveState = MoveState.Right;
			saveState = moveState;
		}
		else if (defaultState == MoveState.DefaultLeft)
		{
			moveState = MoveState.Left;
			saveState = moveState;
		}
		changeDelayCnt = 0;
		hitTotal = 0;
	}


	void Update()
    {
		//どれが当たっているか判定
		isTop = TopCheck.isCheck;
		isUnder = UnderCheck.isCheck;
		isLeft = LeftCheck.isCheck;
		isRight = RightCheck.isCheck;

		//当たっている数を入れる
		hitTotal = TopCheck.hitCnt + UnderCheck.hitCnt + LeftCheck.hitCnt + RightCheck.hitCnt;


		if (changeDelayCnt > chamgeDelayMax)
		{
			ChangeDirection();
		}
		else
		{
			changeDelayCnt += Time.deltaTime;
		}

		Move();
		if (changeDelayCnt > chamgeDelayMax)
		{
			ChangeDirection();
		}
		Move();
	}

	//動く関数
	void Move()
	{
		switch(moveState)
		{
			case MoveState.Up:
				velocity = gameObject.transform.rotation * new Vector3(0, speedY, 0);
				velocity_norm = velocity.normalized;
				gameObject.transform.position += velocity_norm * Time.deltaTime;
				//transform.position += new Vector3(0, 0.025f, 0);
				break;

			case MoveState.Dowx:
				velocity = gameObject.transform.rotation * new Vector3(0, -speedY, 0);
				velocity_norm = velocity.normalized;
				gameObject.transform.position += velocity_norm * Time.deltaTime;
				//transform.position += new Vector3(0, -0.025f, 0);
				break;

			case MoveState.Left:
				velocity = gameObject.transform.rotation * new Vector3(-speedX, 0, 0);
				velocity_norm = velocity.normalized;
				gameObject.transform.position += velocity_norm * Time.deltaTime;
				//transform.position += new Vector3(-0.025f, 0, 0);
				break;

			case MoveState.Right:
				velocity = gameObject.transform.rotation * new Vector3(speedX, 0, 0);
				velocity_norm = velocity.normalized;
				gameObject.transform.position += velocity_norm * Time.deltaTime;
				//transform.position += new Vector3(0.025f, 0, 0);
				break;
		}
	}

	//移動方向を変える関数
	void ChangeDirection()
	{
		if (defaultState == MoveState.DefaultRight)
		{
			if (isUnder && isRight)
			{
				moveState = MoveState.Up;
				saveState = moveState;
				changeDelayCnt = 0;
			}
			else if (isRight && isTop)
			{
				moveState = MoveState.Left;
				saveState = moveState;
				changeDelayCnt = 0;
			}
			else if (isTop && isLeft)
			{
				moveState = MoveState.Dowx;
				saveState = moveState;
				changeDelayCnt = 0;
			}
			else if (isLeft && isUnder)
			{
				moveState = MoveState.Right;
				saveState = moveState;
				changeDelayCnt = 0;
			}
		}
		else if (defaultState == MoveState.DefaultLeft)
		{
			if (isUnder && isRight)
			{
				moveState = MoveState.Left;
				saveState = moveState;
				changeDelayCnt = 0;
			}
			else if (isRight && isTop)
			{
				moveState = MoveState.Dowx;
				saveState = moveState;
				changeDelayCnt = 0;
			}
			else if (isTop && isLeft)
			{
				moveState = MoveState.Right;
				saveState = moveState;
				changeDelayCnt = 0;
			}
			else if (isLeft && isUnder)
			{
				moveState = MoveState.Up;
				saveState = moveState;
				changeDelayCnt = 0;
			}
		}

		//地面の角に来た時
		if (!isTop && !isUnder && !isLeft && !isRight && hitTotal == 0)
		{
			if (defaultState == MoveState.DefaultRight)
			{
				switch (saveState)
				{
					case MoveState.Up:
						moveState = MoveState.Right;
						saveState = moveState;
						changeDelayCnt = 0;
						break;

					case MoveState.Dowx:
						moveState = MoveState.Left;
						saveState = moveState;
						changeDelayCnt = 0;
						break;

					case MoveState.Left:
						moveState = MoveState.Up;
						saveState = moveState;
						changeDelayCnt = 0;
						break;

					case MoveState.Right:
						moveState = MoveState.Dowx;
						saveState = moveState;
						changeDelayCnt = 0;
						break;
				}
			}
			else if (defaultState == MoveState.DefaultLeft)
			{
				switch (saveState)
				{
					case MoveState.Up:
						moveState = MoveState.Left;
						saveState = moveState;
						changeDelayCnt = 0;
						break;

					case MoveState.Dowx:
						moveState = MoveState.Right;
						saveState = moveState;
						changeDelayCnt = 0;
						break;

					case MoveState.Left:
						moveState = MoveState.Dowx;
						saveState = moveState;
						changeDelayCnt = 0;
						break;

					case MoveState.Right:
						moveState = MoveState.Up;
						saveState = moveState;
						changeDelayCnt = 0;
						break;
				}
			}
		}
	}
}
