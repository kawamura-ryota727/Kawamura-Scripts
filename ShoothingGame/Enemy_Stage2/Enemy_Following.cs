﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Following : MonoBehaviour
{
	public GameObject playerObj;   //プレイヤー（向く対象）情報を入れる変数
	public GameObject player2Obj;
	Vector3 velocity;
	Vector3 playerPos;      //プレイヤー（向く対象）の座標を入れる変数
	Vector3 player2Pos;      //プレイヤー（向く対象）の座標を入れる変数

	Vector3 myPos;     //自分の座標を入れる変数
	Vector3 dif;            //対象と自分の座標の差を入れる変数

	public float moveSpeed;
	public float rotaZ;
	public float straightTimeMax;
	float straightTimeCnt;

	public float player1_DefPosTotal;
	public float player2_DefPosTotal;

	bool once = true;
	public bool imasuyo = false;
	public bool isPlayer1Active = true;
	public bool isPlayer2Active = true;
	public bool isAimForPlayer1 = true;
	public bool isAimForPlayer2 = false;
	bool isFaceToPlayer = false;
	bool isChoosePlayer = true;
	public bool isFollow = false;
	float radian;           //ラジアン
	public float degree;    //角度

	void Start()
    {
		once = true;
		isChoosePlayer = true;
    }

    void Update()
    {
		if (once)
		{
			rotaZ = transform.rotation.z;
		}
		//プレイヤー（向く対象）情報が入っていないなら入れる
		if (playerObj == null && isPlayer1Active)
		{
			playerObj = Obj_Storage.Storage_Data.GetPlayer();
		}

		if (player2Obj == null && isPlayer2Active)
		{
			player2Obj = Obj_Storage.Storage_Data.GetPlayer2();

		}
		if (playerObj)
		{
			if (isPlayer1Active)
			{
				if (playerObj.activeInHierarchy)
				{
					playerPos = playerObj.transform.position;
					player1_DefPosTotal = Mathf.Abs(playerObj.transform.position.x - transform.position.x) + Mathf.Abs(playerObj.transform.position.y - transform.position.y);
				}
				else
				{
					playerObj = null;
					isPlayer1Active = false;
				}

			}
		}
		if (player2Obj)
		{
			if (isPlayer2Active)
			{
				if (player2Obj.activeInHierarchy)
				{
					player2Pos = player2Obj.transform.position;
					player2_DefPosTotal = Mathf.Abs(player2Obj.transform.position.x - transform.position.x) + Mathf.Abs(player2Obj.transform.position.y - transform.position.y);
					//imasuyo = true;
				}
				else
				{
					player2Obj = null;
					isPlayer2Active = false;
				}

			}
		}

		//二人プレイのとき狙いを変える
		if (Game_Master.Number_Of_People == Game_Master.PLAYER_NUM.eTWO_PLAYER)
		{
			//プレイヤー1がプレイヤー2より遠いとき
			if (player1_DefPosTotal > player2_DefPosTotal)
			{
				//近い方であるプレイヤー2を狙う
				isAimForPlayer1 = false;
				isAimForPlayer2 = true;
			}
			//プレイヤー2がプレイヤー1より遠いとき
			else if (player2_DefPosTotal > player1_DefPosTotal)
			{
				//近い方であるプレイヤー1を狙う
				isAimForPlayer1 = true;
				isAimForPlayer2 = false;
			}
		}
		//一人プレイのときはプレイヤー1を狙う
		else if (Game_Master.Number_Of_People == Game_Master.PLAYER_NUM.eONE_PLAYER)
		{
			isAimForPlayer1 = true;
		}
		else
		{
			isAimForPlayer1 = true;
		}

		//自分の座標を入れる
		myPos = this.transform.position;

		//角度計算の関数呼び出し
		DegreeCalculation();

		//if(degree>0)
		//{
		//	midBossDig = degree;
		//}
		//else if(degree<=0)
		//{
		//	midBossDig = degree;
		//}

		//自分を対象の方向へ向かせる
		if (isFollow)
		{
			this.transform.rotation = Quaternion.Euler(0, 0, degree);
		}
	}

	//----------------ここから関数----------------

	//移動関数
	void Move()
	{
		velocity = gameObject.transform.rotation * new Vector3(moveSpeed, 0, 0);
		transform.position += velocity * Time.deltaTime;

		Following();
	}

	void Following()
	{
		if (!isFaceToPlayer)
		{
			straightTimeCnt += Time.deltaTime;
			if (straightTimeCnt > straightTimeMax)
			{
				isFaceToPlayer = true;
			}
		}
		else if (isFaceToPlayer)
		{
			//プレイヤーのほうに向きを変える
			if (isChoosePlayer)
			{
				//二人プレイのとき狙いを変える
				if (Game_Master.Number_Of_People == Game_Master.PLAYER_NUM.eTWO_PLAYER)
				{
					//プレイヤー1がプレイヤー2より遠いとき
					if (player1_DefPosTotal > player2_DefPosTotal)
					{
						//近い方であるプレイヤー2を狙う
						isAimForPlayer1 = false;
						isAimForPlayer2 = true;
					}
					//プレイヤー2がプレイヤー1より遠いとき
					else if (player2_DefPosTotal > player1_DefPosTotal)
					{
						//近い方であるプレイヤー1を狙う
						isAimForPlayer1 = true;
						isAimForPlayer2 = false;
					}
				}
				//一人プレイのときはプレイヤー1を狙う
				else if (Game_Master.Number_Of_People == Game_Master.PLAYER_NUM.eONE_PLAYER)
				{
					isAimForPlayer1 = true;
				}
				else
				{
					isAimForPlayer1 = true;
				}

				isChoosePlayer = false;
			}

			//狙うプレイヤーのほうを向かせる
			if (isAimForPlayer1)
			{

			}
			else if (isAimForPlayer2)
			{

			}
		}
	}

	//角度を求める関数
	void DegreeCalculation()
	{
		if (isAimForPlayer1)
		{
			//座標の差を入れる
			dif = playerPos - myPos;
		}
		else if (isAimForPlayer2)
		{
			dif = player2Pos - myPos;
		}
		//ラジアンを求める
		radian = Mathf.Atan2(dif.y, dif.x);

		//角度を求める
		degree = radian * Mathf.Rad2Deg;
	}
}
