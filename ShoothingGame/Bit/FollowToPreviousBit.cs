//作成者：川村良太
//1つ前のオプションと同じ動きで追従する位置用オブジェクトのスクリプト

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowToPreviousBit : MonoBehaviour
{
	public GameObject playerObj;
	Player1 pl1;
	Player2 pl2;
	GameObject previousBitObj;
	public GameObject parentObj;
	public GameObject hunterObj;
	public string parentName;
	FollowPositions followParent_Script;    //4つの追従位置の親スクリプト
	FollowToPlayer_SameMotion firstPos_Script;
	public FollowToPreviousBit followBit_Script;

	public Vector3[] previousBitPos;
	public Vector3 pos;
    Vector3 savePos;                //フリーズ開始時の座標を入れる
    Vector3 defPos;                 //フリーズ解除時、開始時と今の座標との差を入れる

	Vector3 freesePos;

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
	bool defCheck = false;
	public bool isFreeze = false;
	public bool isFollow1P;
	public bool isFollow2P;
	public bool isPlayerLive;							//プレイヤーを取得したらtrue
	public bool isResetPos;							//リスポーン終了時に位置をリセットしたかどうか
	public bool isStolen = false;						//自身がハンターに当たるとtrue
	public bool isStolen_Previous = false;		

	void Start()
	{
		isPlayerLive = false;
		myName = gameObject.name;

		parentObj = transform.parent.gameObject;
		parentName = parentObj.name;
		followParent_Script = parentObj.GetComponent<FollowPositions>();

		if (parentName == "Four_FollowPos_1P")
		{
			isFollow1P = true;
			isFollow2P = false;
		}
		else if (parentName == "Four_FollowPos_2P")
		{
			isFollow2P = true;
			isFollow1P = false;
		}

		if (isFollow1P)
		{
			if (myName == "FollowPosSecond_1P")
			{
				previousBitObj = GameObject.Find("FollowPosFirst_1P");
				firstPos_Script = previousBitObj.GetComponent<FollowToPlayer_SameMotion>();
				myNumber = 2;
			}
			else if (myName == "FollowPosThird_1P")
			{
				previousBitObj = GameObject.Find("FollowPosSecond_1P");
				followBit_Script = previousBitObj.GetComponent<FollowToPreviousBit>();
				myNumber = 3;
			}
			else if (myName == "FollowPosFourth_1P")
			{
				previousBitObj = GameObject.Find("FollowPosThird_1P");
				followBit_Script = previousBitObj.GetComponent<FollowToPreviousBit>();
				myNumber = 4;
			}
		}
		else if (isFollow2P)
		{
			if (myName == "FollowPosSecond_2P")
			{
				previousBitObj = GameObject.Find("FollowPosFirst_2P");
				firstPos_Script = previousBitObj.GetComponent<FollowToPlayer_SameMotion>();
				myNumber = 2;
			}
			else if (myName == "FollowPosThird_2P")
			{
				previousBitObj = GameObject.Find("FollowPosSecond_2P");
				followBit_Script = previousBitObj.GetComponent<FollowToPreviousBit>();
				myNumber = 3;
			}
			else if (myName == "FollowPosFourth_2P")
			{
				previousBitObj = GameObject.Find("FollowPosThird_2P");
				followBit_Script = previousBitObj.GetComponent<FollowToPreviousBit>();
				myNumber = 4;
			}
		}

		//int cnt = 0;
		array_Num = 9;
		previousBitPos = new Vector3[array_Num];

		defCheck = true;
		pos = previousBitObj.transform.position;

	}

	void Update()
	{
		childCnt = this.transform.childCount;

		//プレイヤー格納がnullなら入れる
		if (playerObj == null)
		{
			if (isFollow1P)
			{
				//プレイヤーがいたら入れる
				playerObj = Obj_Storage.Storage_Data.GetPlayer();
                pl1 = playerObj.GetComponent<Player1>();
				//playerPos[cnt] = playerObj.transform;
				transform.position = playerObj.transform.position;
				defCheck = true;
				check = true;
				//pos = playerObj.transform.position;

			}
			else if (isFollow2P)
			{
				//プレイヤーがいたら入れる
				playerObj = Obj_Storage.Storage_Data.GetPlayer2();
                pl2 = playerObj.GetComponent<Player2>();
				//playerPos[cnt] = playerObj.transform;
				transform.position = playerObj.transform.position;
				defCheck = true;
				check = true;
				//isPlayerLive = true;
				//pos = playerObj.transform.position;

			}
		}
		else
		{
			isPlayerLive = true;
		}

		if (isFollow1P)
		{
			if (isPlayerLive)
			{
				if (!followParent_Script.isResetPosEnd && !isResetPos)
				{
					//pl1.Is_Resporn_End = false;
					transform.position = playerObj.transform.position;
					pos = playerObj.transform.position;
					savePos = playerObj.transform.position;
					for (int i = 0; i < array_Num; i++)
					{
						previousBitPos[i] = playerObj.transform.position;
						previousBitPos[i] = new Vector3(previousBitPos[i].x, previousBitPos[i].y, 0);
					}

					//transform.position = playerObj.transform.position;
					//transform.position = new Vector3(transform.position.x, transform.position.y, 0);
					followParent_Script.resetPosCnt++;
					isResetPos = true;
					isFreeze = false;
				}
				if (!pl1.Is_Resporn)
				{
					if (ControllerDevice.GetButtonUp(pl1.InputManager.Manager.Button["Multiple"], ePadNumber.ePlayer1) || Input.GetKeyUp(KeyCode.Y))
					{
						isFreeze = false;
						defPos = transform.position - savePos;

						for (int i = 0; i < array_Num; i++)
						{
							previousBitPos[i] += defPos;
						}
						savePos = transform.position;
						pos = previousBitObj.transform.position;
					}
					else if (ControllerDevice.GetButton(pl1.InputManager.Manager.Button["Multiple"], ePadNumber.ePlayer1) || Input.GetKey(KeyCode.Y))
					{
						isFreeze = true;
					}
				}
			}
		}
		else if (isFollow2P)
		{
			if (isPlayerLive)
			{
				if (!followParent_Script.isResetPosEnd && !isResetPos)
				{
					//pl2.Is_Resporn_End = false;
					transform.position = playerObj.transform.position;
					pos = playerObj.transform.position;
					savePos = playerObj.transform.position;
					for (int i = 0; i < array_Num; i++)
					{
						previousBitPos[i] = playerObj.transform.position;
						previousBitPos[i] = new Vector3(previousBitPos[i].x, previousBitPos[i].y, 0);
					}

					//transform.position = playerObj.transform.position;
					//transform.position = new Vector3(transform.position.x, transform.position.y, 0);
					followParent_Script.resetPosCnt++;
					isResetPos = true;
				}

				if (!pl2.Is_Resporn)
				{
					if (ControllerDevice.GetButtonUp(pl2.InputManager.Manager.Button["Multiple"], ePadNumber.ePlayer2) || Input.GetKeyUp(KeyCode.Y))
					{
						isFreeze = false;
						defPos = transform.position - savePos;

						for (int i = 0; i < array_Num; i++)
						{
							previousBitPos[i] += defPos;
						}
						savePos = transform.position;
						pos = previousBitObj.transform.position;
					}
					else if (ControllerDevice.GetButton(pl2.InputManager.Manager.Button["Multiple"], ePadNumber.ePlayer2) || Input.GetKey(KeyCode.Y))
					{
						isFreeze = true;
					}
				}
			}
		}

		if (isStolen_Previous)
		{
			if (defCheck)
			{
				if (isFollow1P)
				{
					if (pos != previousBitObj.transform.position)
					{
						isMove = true;
						pos = previousBitObj.transform.position;
					}
					else
					{
						isMove = false;
					}
					//前のビットの座標が動いていないとき
					//if (pos == previousBitObj.transform.position)
					////if ((Input.GetAxis("Horizontal") == 0) && (Input.GetAxis("Vertical") == 0))
					//{
					//	isMove = false;
					//	if ((Input.GetAxis("Horizontal") != 0) || (Input.GetAxis("Vertical") != 0))
					//	{
					//		isMove = true;
					//	}
					//}
					////前のビットの座標が動いていたとき
					//else
					//{
					//	//isMove = true;
					//	//前のビットのtransform保存
					//	pos = previousBitObj.transform.position;
					//}
				}
				else if (isFollow2P)
				{
					//前のビットの座標が動いていないとき
					if (pos == previousBitObj.transform.position)
					//if ((Input.GetAxis("Horizontal") == 0) && (Input.GetAxis("Vertical") == 0))
					{
						isMove = false;
						if ((ControllerDevice.GetAxis("P2_Horizontal", ePadNumber.ePlayer2) != 0) || (ControllerDevice.GetAxis("P2_Vertical", ePadNumber.ePlayer2) != 0))
						{
							isMove = true;
						}
					}
					//前のビットの座標が動いていたとき
					else
					{
						isMove = true;
						//前のビットのtransform保存
						pos = previousBitObj.transform.position;
					}
				}
			}
			//前のビットが動いていて位置配列すべてに値が入っているとき
			if (isMove && check)
			{
				//isMove = false;
				//自分の位置を配列に入っている位置に
				//transform.position = playerPos[cnt].position;
				transform.position = previousBitPos[cnt];

				//自分の位置を移動したのでその位置を今、前のビットのいる位置で更新
				//playerPos[cnt] = playerObj.transform;
				previousBitPos[cnt] = previousBitObj.transform.position;

				cnt++;
				if (cnt > array_Num - 1)
				{
					cnt = 0;
				}
				savePos = transform.position;
			}

		}
		//盗まれ状態でフリーズで止まっちゃうのがダメ
		else if (!isFreeze && !isStolen)
		{
			if (defCheck)
			{
				if (isFollow1P)
				{
					if (pos == previousBitObj.transform.position)
					{
						isMove = false;
						if (((ControllerDevice.GetAxis("Horizontal", ePadNumber.ePlayer1) != 0) || (ControllerDevice.GetAxis("Vertical", ePadNumber.ePlayer1) != 0)) && !isStolen_Previous)
						{
							isMove = true;
							//pos = previousBitObj.transform.position;
						}
					}
					else
					{
						isMove = true;
						pos = previousBitObj.transform.position;
					}
					//前のビットの座標が動いていないとき
					//if (pos == previousBitObj.transform.position)
					////if ((Input.GetAxis("Horizontal") == 0) && (Input.GetAxis("Vertical") == 0))
					//{
					//	isMove = false;
					//	if ((Input.GetAxis("Horizontal") != 0) || (Input.GetAxis("Vertical") != 0))
					//	{
					//		isMove = true;
					//	}
					//}
					////前のビットの座標が動いていたとき
					//else
					//{
					//	//isMove = true;
					//	//前のビットのtransform保存
					//	pos = previousBitObj.transform.position;
					//}
				}
				else if (isFollow2P)
				{
					//前のビットの座標が動いていないとき
					if (pos == previousBitObj.transform.position)
					//if ((Input.GetAxis("Horizontal") == 0) && (Input.GetAxis("Vertical") == 0))
					{
						isMove = false;
						if ((ControllerDevice.GetAxis("P2_Horizontal", ePadNumber.ePlayer2) != 0) || (ControllerDevice.GetAxis("P2_Vertical", ePadNumber.ePlayer2) != 0))
						{
							isMove = true;
						}
					}
					//前のビットの座標が動いていたとき
					else
					{
						isMove = true;
						//前のビットのtransform保存
						pos = previousBitObj.transform.position;
					}
				}
			}

			//前のビットが動いていて位置配列すべてに値が入っているとき
			if (isMove && check)
			{
				//isMove = false;
				//自分の位置を配列に入っている位置に
				//transform.position = playerPos[cnt].position;
				transform.position = previousBitPos[cnt];

				//自分の位置を移動したのでその位置を今、前のビットのいる位置で更新
				//playerPos[cnt] = playerObj.transform;
				previousBitPos[cnt] = previousBitObj.transform.position;

				cnt++;
				if (cnt > array_Num - 1)
				{
					cnt = 0;
				}
				savePos = transform.position;
			}
		}
		//else if (isStolen)
		//{
		//	transform.parent = null;
		//	if (isFollow1P)
		//	{
		//		if (pos == previousBitObj.transform.position)
		//		{
		//			isMove = false;
		//		}
		//		else if (pos != previousBitObj.transform.position)
		//		{
		//			isMove = true;
		//			pos = previousBitObj.transform.position;
		//		}
		//	}
		//	else if (isFollow2P)
		//	{

		//	}
		//	transform.position = hunterObj.transform.position;
		//}

		//リスポーン時の位置リセット
		if (isFollow1P)
		{
			if (pl1.Is_Resporn_End)
			{
				for (int i = 0; i < previousBitPos.Length; i++)
				{
					previousBitPos[i] = playerObj.transform.position;

				}
			}
		}
		//リスポーン時の位置リセット
		else if (isFollow2P)
		{
			if (pl2.Is_Resporn_End)
			{
				for (int i = 0; i < previousBitPos.Length; i++)
				{
					previousBitPos[i] = playerObj.transform.position;

				}
			}
		}

		if (followParent_Script.isResetPosEnd)
		{
			isResetPos = false;
		}

		//if (myNumber == 2)
		//{
		//	if (firstPos_Script.isStolen)
		//	{
		//		isStolen_Previous = true;
		//		transform.parent = null;

		//	}
		//}
		//else if (myNumber == 3 || myNumber == 4)
		//{
		//	if (followBit_Script.isStolen || followBit_Script.isStolen_Previous)
		//	{
		//		isStolen_Previous = true;
		//		transform.parent = null;
		//	}
		//}
	}
}
