//作成者：川村良太
//4つのオプションの追従位置オブジェクトの親スクリプト

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPositions : MonoBehaviour
{
	GameObject playerObj;
	public GameObject firstOption;
	public GameObject secondOption;
	public GameObject thirdOption;
	public GameObject fourthOption;
	public Vector3 pos;
	public Vector3 defPos;
	public Vector3 savePos;
	public GameObject[] huntPos;		//盗まれた時の移動位置を入れる

	Player1 pl1;
	Player2 pl2;

	public string myName;

	public int resetPosCnt;
	public int stolenCnt;

	bool check = false;
	public bool isFreeze = false;
	public bool isMove;
	bool defCheck;
	public bool isFollow1P;
	public bool isFollow2P;
	public bool isResetPosEnd;
	public bool isCircle = false;
	public bool isFixed = false;

	private void Awake()
	{

	}
	void Start()
	{
		isResetPosEnd = false;
		resetPosCnt = 0;
		stolenCnt = 0;
		myName = gameObject.name;

		if (Game_Master.Number_Of_People == Game_Master.PLAYER_NUM.eONE_PLAYER)
		{
			if (myName == "Four_FollowPos_2P")
			{
				gameObject.SetActive(false);    //オブジェクトをオフにする
			}
		}
		if (myName == "Four_FollowPos_1P")
		{
			isFollow1P = true;
		}
		else if (myName == "Four_FollowPos_2P")
		{
			isFollow2P = true;
		}
		huntPos = new GameObject[4];

	}

	void Update()
	{
		if (playerObj == null)
		{
			if (isFollow1P)
			{
				if (GameObject.Find("Player"))
				{
					playerObj = Obj_Storage.Storage_Data.GetPlayer();/*GameObject.Find("Player")*/;
					pl1 = playerObj.GetComponent<Player1>();
					check = true;
					defCheck = true;
					pos = playerObj.transform.position;
					savePos = playerObj.transform.position;
				}
			}
			else if (isFollow2P)
			{
				if (GameObject.Find("Player_2"))
				{
					playerObj = Obj_Storage.Storage_Data.GetPlayer2();/*GameObject.Find("Player_2")*/;
					pl2 = playerObj.GetComponent<Player2>();
					check = true;
					defCheck = true;
					pos = playerObj.transform.position;
					savePos = playerObj.transform.position;
				}
			}
		}

		if (Input.GetButtonDown("LB"))
		{
			if (isCircle)
			{
				isCircle = false;
				isFixed = true;
			}
			else if (isFixed)
			{
				isFixed = false;
				isCircle = false;
			}
			else
			{
				isCircle = true;
				isFixed = false;
			}
		}

		if (pos == playerObj.transform.position)
		{
			isMove = false;
		}
		else
		{
			isMove = true;
			pos = playerObj.transform.position;
		}

		if (check)
		{
			if (isFollow1P)
			{
				if (ControllerDevice.GetButtonUp(pl1.InputManager.Manager.Button["Multiple"], ePadNumber.ePlayer1) || Input.GetKeyUp(KeyCode.Y))
				{
					isFreeze = false;
					//transform.parent = null;
				}
				else if (ControllerDevice.GetButton(pl1.InputManager.Manager.Button["Multiple"], ePadNumber.ePlayer1) || Input.GetKey(KeyCode.Y))
				{
					isFreeze = true;
					//transform.parent = playerObj.transform;

				}

			}
			else if (isFollow2P)
			{
				if (ControllerDevice.GetButtonUp(pl2.InputManager.Manager.Button["Multiple"], ePadNumber.ePlayer2) || Input.GetKeyUp(KeyCode.Y))
				{
					isFreeze = false;
					//transform.parent = null;
				}
				else if (ControllerDevice.GetButton(pl2.InputManager.Manager.Button["Multiple"], ePadNumber.ePlayer2) || Input.GetKey(KeyCode.Y))
				{
					isFreeze = true;
					//transform.parent = playerObj.transform;

				}

			}
			//savePos = playerObj.transform.position;
		}

		if (!isFreeze)
		{
			if (defCheck)
			{
			}

			//プレイヤーが動いていて位置配列すべてに値が入っているとき
			if (isMove)
			{
				savePos = playerObj.transform.position;
			}
		}

		else if (isFreeze && isMove)
		{
			defPos = pos - savePos;
			//defPos = playerObj.transform.position - transform.position;
			transform.position = transform.position + defPos;
			savePos = playerObj.transform.position;
		}

		if (isFollow1P)
		{
			if (pl1.Is_Resporn_End)
			{
				isResetPosEnd = false;
			}
			//pl1.Is_Resporn_End = false;
		}
		else if (isFollow2P)
		{
			if (pl2.Is_Resporn_End)
			{
				isResetPosEnd = false;
			}
			//pl2.Is_Resporn_End = false;
		}

		if (resetPosCnt == 4)
		{
			resetPosCnt = 0;
			if (isFollow1P)
			{
				pl1.Is_Resporn_End = false;
			}
			else if (isFollow2P)
			{
				pl2.Is_Resporn_End = false;
			}
			isResetPosEnd = true;
			isFreeze = false;
		}
	}
}
