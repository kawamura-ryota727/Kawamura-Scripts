using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterFollow : MonoBehaviour
{
	public GameObject previousObj;			//ひとつ前の追従位置
	public GameObject parentObj;
	public GameObject hunterObj;
	public OptionHunter hunter_Script;
	public GameObject followPosFirstObj_1P;        //プレイヤー1に一番近い追従位置オブジェクト
	public GameObject followPosSecondObj_1P;       //二番目
	public GameObject followPosThirdObj_1P;        //三番目
	public GameObject followPosFourthObj_1P;       //四番目
	public GameObject followPosFirstObj_2P;        //プレイヤー2に一番近い追従位置オブジェクト
	public GameObject followPosSecondObj_2P;       //二番目
	public GameObject followPosThirdObj_2P;        //三番目
	public GameObject followPosFourthObj_2P;       //四番目
	FollowToPreviousBit FtoPBit_Second_1P;         //二番目の位置のスクリプト情報P1
	FollowToPreviousBit FtoPBit_Third_1P;          //三番目の位置のスクリプト情報P1
	FollowToPreviousBit FtoPBit_Fourth_1P;         //四番目の位置のスクリプト情報P1
	FollowToPreviousBit FtoPBit_Second_2P;         //二番目の位置のスクリプト情報P2
	FollowToPreviousBit FtoPBit_Third_2P;          //三番目の位置のスクリプト情報P2
	FollowToPreviousBit FtoPBit_Fourth_2P;         //四番目の位置のスクリプト情報P2

	public string parentName;
	public FollowPositions followParent1P_Script;    //4つの追従位置の親スクリプト
	public FollowPositions followParent2P_Script;    //4つの追従位置の親スクリプト

	public FollowToPreviousBit followBit_Script;

	public Vector3[] previousPos;
	public Vector3 pos;

	public int cnt;
	int array_Num;
	public int childCnt;

	public string myName;
	public int myNumber;

	//bool one = true;
	//bool once = true;
	public bool check = false;      //配列すべてに値が入っているかの判定
	public bool isMove = false;
	public bool hasOption = false;
	public bool isStolen = false;                       //自身がハンターに当たるとtrue
	public bool isStolen_Previous = false;
	public bool isSet = true;           //盗んだ時にポジションを入れる判定
	public bool once = true;

	void Start()
	{
		myName = gameObject.name;

		parentObj = transform.parent.gameObject;
		parentName = parentObj.name;

		//int cnt = 0;
		array_Num = 9;
		previousPos = new Vector3[array_Num];

		pos = previousObj.transform.position;

		followParent1P_Script = GameObject.Find("Four_FollowPos_1P").GetComponent<FollowPositions>();

		//追従位置取得
		followPosFirstObj_1P = GameObject.Find("FollowPosFirst_1P");

		followPosSecondObj_1P = GameObject.Find("FollowPosSecond_1P");
		FtoPBit_Second_1P = followPosSecondObj_1P.GetComponent<FollowToPreviousBit>();

		followPosThirdObj_1P = GameObject.Find("FollowPosThird_1P");
		FtoPBit_Third_1P = followPosThirdObj_1P.GetComponent<FollowToPreviousBit>();

		followPosFourthObj_1P = GameObject.Find("FollowPosFourth_1P");
		FtoPBit_Fourth_1P = followPosFourthObj_1P.GetComponent<FollowToPreviousBit>();


		if (Game_Master.Number_Of_People == Game_Master.PLAYER_NUM.eTWO_PLAYER)
		{
			followParent2P_Script = GameObject.Find("Four_FollowPos_2P").GetComponent<FollowPositions>();

			followPosFirstObj_2P = GameObject.Find("FollowPosFirst_2P");

			FtoPBit_Second_2P = followPosSecondObj_2P.GetComponent<FollowToPreviousBit>();
			followPosSecondObj_2P = GameObject.Find("FollowPosSecond_2P");

			FtoPBit_Third_2P = followPosThirdObj_2P.GetComponent<FollowToPreviousBit>();
			followPosThirdObj_2P = GameObject.Find("FollowPosThird_2P");

			FtoPBit_Fourth_2P = followPosFourthObj_2P.GetComponent<FollowToPreviousBit>();
			followPosFourthObj_2P = GameObject.Find("FollowPosFourth_2P");
		}

	}

	void Update()
	{
		if (once)
		{
			cnt = 0;
			isSet = true;
			once = false;
		}

		childCnt = this.transform.childCount;

		

		//前の追従位置が動いているか
		if (check)
		{
			if (pos == previousObj.transform.position)
			{
				isMove = false;
			}
			else
			{
				isMove = true;
				pos = previousObj.transform.position;
			}
		}

		if (!check)
		{
			for (int i = 0; i < array_Num; i++)
			{
				previousPos[i] = previousObj.transform.position;

			}
			check = true;
		}
		//前のビットが動いていて位置配列すべてに値が入っているとき
		else if (isMove && check)
		{
			//isMove = false;
			//自分の位置を配列に入っている位置に
			//transform.position = playerPos[cnt].position;
			transform.position = previousPos[cnt];

			//自分の位置を移動したのでその位置を今、前のビットのいる位置で更新
			//playerPos[cnt] = playerObj.transform;
			previousPos[cnt] = previousObj.transform.position;

			cnt++;
			if (cnt > array_Num - 1)
			{
				cnt = 0;
			}
		}

		//ハンターが盗んでいたら
		if (hunter_Script.isHunt && isSet)
		{
			//パターン2
			//盗んだ場所を見る
			switch (hunter_Script.huntOptionNum)
			{
				//1つ目を盗んでいた時
				case 1:
					//盗んだ数を見る
					switch (hunter_Script.huntNum)
					{
						//1個盗んでいたら
						case 1:
							//なにもしない
							break;

						//2個盗んでいたら
						case 2:
							if (myNumber == 2)
							{
								transform.position = followPosSecondObj_1P.transform.position;
								for (int i = 0; i < array_Num; i++)
								{
									previousPos[i] = FtoPBit_Second_1P.previousBitPos[i];
								}
								//previousPos = FtoPBit_Second_1P.previousBitPos;
								cnt = FtoPBit_Second_1P.cnt;
							}

							break;

						//3個盗んでいたら
						case 3:
							if (myNumber == 2)
							{
								transform.position = followPosSecondObj_1P.transform.position;
								for (int i = 0; i < array_Num; i++)
								{
									previousPos[i] = FtoPBit_Second_1P.previousBitPos[i];
								}

								//previousPos = FtoPBit_Second_1P.previousBitPos;
								cnt = FtoPBit_Second_1P.cnt;
							}
							else if (myNumber == 3)
							{
								transform.position = followPosThirdObj_1P.transform.position;
								for (int i = 0; i < array_Num; i++)
								{
									previousPos[i] = FtoPBit_Third_1P.previousBitPos[i];
								}
								//previousPos = FtoPBit_Third_1P.previousBitPos;
								cnt = FtoPBit_Third_1P.cnt;
							}

							break;

						//4個盗んでいたら
						case 4:
							if (myNumber == 2)
							{
								transform.position = followPosSecondObj_1P.transform.position;
								for (int i = 0; i < array_Num; i++)
								{
									previousPos[i] = FtoPBit_Second_1P.previousBitPos[i];
								}
								//previousPos = FtoPBit_Second_1P.previousBitPos;
								cnt = FtoPBit_Second_1P.cnt;
							}
							else if (myNumber == 3)
							{
								transform.position = followPosThirdObj_1P.transform.position;
								for (int i = 0; i < array_Num; i++)
								{
									previousPos[i] = FtoPBit_Third_1P.previousBitPos[i];
								}
								//previousPos = FtoPBit_Third_1P.previousBitPos;
								cnt = FtoPBit_Third_1P.cnt;
							}
							else if (myNumber == 4)
							{
								transform.position = followPosFourthObj_1P.transform.position;
								for (int i = 0; i < array_Num; i++)
								{
									previousPos[i] = FtoPBit_Fourth_1P.previousBitPos[i];
								}
								//previousPos = FtoPBit_Fourth_1P.previousBitPos;
								cnt = FtoPBit_Fourth_1P.cnt;
							}

							break;

					}
					break;

				//2つ目を盗んでいた時
				case 2:
					//盗んだ数を見る
					switch (hunter_Script.huntNum)
					{
						case 1:
							//何もしない
							break;

						//2個盗んでいたら
						case 2:
							if (myNumber == 2)
							{
								transform.position = followPosThirdObj_1P.transform.position;
								for (int i = 0; i < array_Num; i++)
								{
									previousPos[i] = FtoPBit_Third_1P.previousBitPos[i];
								}
								//previousPos = FtoPBit_Third_1P.previousBitPos;
								cnt = FtoPBit_Third_1P.cnt;
							}
							else if (myNumber == 3)
							{
								transform.position = followPosFourthObj_1P.transform.position;
								for (int i = 0; i < array_Num; i++)
								{
									previousPos[i] = FtoPBit_Fourth_1P.previousBitPos[i];
								}
								//previousPos = FtoPBit_Fourth_1P.previousBitPos;
								cnt = FtoPBit_Fourth_1P.cnt;
							}

							break;

						//3個盗んでいたら
						case 3:
							if (myNumber == 2)
							{
								transform.position = followPosThirdObj_1P.transform.position;
								for (int i = 0; i < array_Num; i++)
								{
									previousPos[i] = FtoPBit_Third_1P.previousBitPos[i];
								}
								//previousPos = FtoPBit_Third_1P.previousBitPos;
								cnt = FtoPBit_Third_1P.cnt;
							}
							else if (myNumber == 3)
							{
								transform.position = followPosFourthObj_1P.transform.position;
								for (int i = 0; i < array_Num; i++)
								{
									previousPos[i] = FtoPBit_Fourth_1P.previousBitPos[i];
								}
								//previousPos = FtoPBit_Fourth_1P.previousBitPos;
								cnt = FtoPBit_Fourth_1P.cnt;
							}
							else if (myNumber == 4)
							{
								transform.position = followPosFourthObj_1P.transform.position;
								for (int i = 0; i < array_Num; i++)
								{
									previousPos[i] = FtoPBit_Fourth_1P.previousBitPos[i];
								}
								//previousPos = FtoPBit_Fourth_1P.previousBitPos;
								cnt = FtoPBit_Fourth_1P.cnt;
							}
							break;

						//4個盗んでいたら
						case 4:
							//何もしない
							break;

					}
					break;

				//3つ目を盗んでいた時
				case 3:
					//2個盗んだ時しか処理しないのでswichじゃない
					if (hunter_Script.huntNum == 2)
					{
						transform.position = followPosFourthObj_1P.transform.position;
						for (int i = 0; i < array_Num; i++)
						{
							previousPos[i] = FtoPBit_Fourth_1P.previousBitPos[i];
						}
						//previousPos = FtoPBit_Fourth_1P.previousBitPos;
						cnt = FtoPBit_Fourth_1P.cnt;
					}
					break;

				case 4:
					//なにもしない
					break;
			}
			isSet = false;
		}
	}
}
