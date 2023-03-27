using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionHunter : MonoBehaviour
{
	public GameObject optionObj;
	public Player1 player1_Script;
	Player2 player2_Script;
	public Bit_Formation_3 option_Script;
	public FollowPositions followParent1P_Script;    //4つの追従位置の親スクリプト
	public FollowPositions followParent2P_Script;    //4つの追従位置の親スクリプト

	public Enemy_StagBeetle stagBeetle_Script;

	public GameObject[] huntPosObj;

	public int playerNum;			//盗んだプレイヤー
	public int huntOptionNum;		//盗んだオプションの番号
	public int huntNum;				//盗んだオプションの数
	public bool isHunt = false;     //盗んだ判定
	public bool once = true;


    void Start()
    {
		player1_Script = Obj_Storage.Storage_Data.GetPlayer().GetComponent<Player1>();
		player2_Script = Obj_Storage.Storage_Data.GetPlayer2().GetComponent<Player2>();
		followParent1P_Script = GameObject.Find("Four_FollowPos_1P").GetComponent<FollowPositions>();
		if (Game_Master.Number_Of_People == Game_Master.PLAYER_NUM.eTWO_PLAYER)
		{
			followParent2P_Script = GameObject.Find("Four_FollowPos_2P").GetComponent<FollowPositions>();
		}


	}


	void Update()
    {
        
    }

	private void OnTriggerEnter(Collider col)
	{
		if (once)
		{
			playerNum = 0;
			huntNum = 0;
			huntOptionNum = 0;
			isHunt = false;
			optionObj = null;
			option_Script = null;
			for (int i = 0; i < 4; i++)
			{
				followParent1P_Script.huntPos[i] = null;

			}

			once = false;
		}

		//盗んでいなくてオプションに当たった時
		if (col.gameObject.tag == "Option" && !isHunt)
		{
			//当たったオプション取得
			optionObj = col.gameObject;
			option_Script = optionObj.GetComponent<Bit_Formation_3>();
			if (option_Script.bState == Bit_Formation_3.BitState.Player1)
			{
				//盗んだプレイヤーのセット
				playerNum = 1;
				huntOptionNum = option_Script.option_OrdinalNum;
				huntNum = (player1_Script.bitIndex - option_Script.option_OrdinalNum) + 1;
				//盗み時の追従位置を渡す
				for (int i = 0; i < 4; i++)
				{
					followParent1P_Script.huntPos[i] = huntPosObj[i];

				}
			}
			else if (option_Script.bState == Bit_Formation_3.BitState.Player2)
			{
				playerNum = 2;
				huntOptionNum = option_Script.option_OrdinalNum;
				huntNum = (player2_Script.bitIndex - option_Script.option_OrdinalNum) + 1;
				//盗み時の追従位置を渡す
				for (int i = 0; i < 4; i++)
				{
					followParent2P_Script.huntPos[i] = huntPosObj[i];

				}
			}
			stagBeetle_Script.eState = Enemy_StagBeetle.State.HUNT;
			//stagBeetle_Script.speed *= -1;
			//huntNum = option_Script.option_OrdinalNum;
			isHunt = true;
			option_Script.isHunterHit = true;
		}
	}
}
