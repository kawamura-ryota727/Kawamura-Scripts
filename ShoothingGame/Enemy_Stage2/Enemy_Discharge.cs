//作成　川村良太
//敵排出する敵

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StorageReference;

public class Enemy_Discharge : character_status
{
	public enum MyDirection
	{
		Up,
		Down,
		Left,
		Right,
		Free,
	}

	public MyDirection myDirection;

	//public enum SetMoveType
	//{
	//	LeftCurveUp_90,
	//	LeftCueveUp_180,
	//	LeftCurveDown_90,
	//	LeftCueveDown_180,
	//	RightCurveUp_90,
	//	RightCueveUp_180,
	//	RightCurveDown_90,
	//	RightCueveDown_180,
	//}
	//public SetMoveType setMoveType;

	public Enemy_Discharged.MoveType setMoveType;

    public GameObject defaultParentObj;
    public GameObject createPosObj;
    GameObject saveObj;
	public GameObject createObj;
	public GameObject mapObj;
    Vector3 createPos;

	Enemy_Discharged discharged_Script;

	Quaternion createRotation;

	[Header("入力用　グループ排出数")]
	public int createGroupNum = 0;
	public int createGroupCnt = 0;
	[Header("入力用　グループ内排出数")]
	public int createNum = 0;
	public int createCnt = 0;
	[Header("入力用　グループ排出間隔MAX(フレーム)")]
	public int createGroupDelayMax = 0;
	public int createGroupDelayCnt = 0;
	[Header("入力用　グループ内間隔MAX(フレーム)")]
	public int createDelayMax = 0;
	public int createDelayCnt = 0;

	//死亡時処理用
	Find_Angle fd;                              //プレイヤーの方向を向くスクリプト取得用（死亡時攻撃に使う） パブリックで入れて
	public Quaternion diedAttackRota;           //死んだ時に出す弾の角度範囲
	public bool Died_Attack = false;
	[Header("死亡時の弾発射の角度範囲()")]
	public float diedAttack_RotaValue;


	bool once = true;

	new void Start()
    {
		createRotation = Quaternion.Euler(0, 0, 0);
        
		mapObj = GameObject.Find("Stage_02_Map").gameObject;

		base.Start();
	}

	new void Update()
    {
		if (once)
		{
			createGroupCnt = 0;
			createGroupDelayCnt = 0;
			createDelayCnt = 0;

			once = false;
		}

		if (transform.position.x < 16)
		{
			//ひとまとまりを出すディレイカウントが最大を超えたら
			if (createGroupDelayCnt > createGroupDelayMax)
			{
				createDelayCnt++;
				//ひとまとまりの排出がまだ残っていたら
				if (createGroupNum > createGroupCnt)
				{
					//ひとまとまり内の排出数が残っていたら
					if (createNum > createCnt)
					{
						//一体を出すディレイカウントが最大を超えたら
						if (createDelayCnt > createDelayMax)
						{
                            ////
                            //if (Random.Range(1, 5) % 4 == 0)
                            //{
                            //    //オブジェクト生成（のちにプーリング）
                            //    saveObj = Instantiate(createObj, transform.position, createRotation);

                            //}
                            //else
                            //{
                            //    //オブジェクト生成（のちにプーリング）
                            //    saveObj = Instantiate(createObj, transform.position, createRotation);

                            //}
                            //オブジェクト生成（のちにプーリング）
                            //saveObj = Instantiate(createObj, transform.position, createRotation);
                            saveObj = Obj_Storage.Storage_Data.Discharged_Enemy.Active_Obj();
                            if (myDirection == MyDirection.Up)
                            {
                                saveObj.transform.position = new Vector3(transform.position.x, transform.position.y + 0.6f, 0);

                            }
                            else if (myDirection == MyDirection.Down)
                            {
                                saveObj.transform.position = new Vector3(transform.position.x, transform.position.y - 0.6f, 0);

                            }

                            saveObj.transform.rotation = Quaternion.Euler(createRotation.x, createRotation.y, createRotation.z);
							//スクリプト取得
							discharged_Script = saveObj.GetComponent<Enemy_Discharged>();
                            discharged_Script.defaultParentObj = saveObj.transform.parent.gameObject;

							if (myDirection==Enemy_Discharge.MyDirection.Free)
							{
								discharged_Script.isRotaReset = false;
								saveObj.transform.rotation = transform.rotation;

							}
							else
							{
								discharged_Script.isRotaReset = true;
							}
							//子供にする
							saveObj.transform.parent = mapObj.transform;
							//動きの種類を設定
							discharged_Script.moveType = setMoveType;
							//SetState(setMoveType);

							//一体を出すディレイカウントリセット
							createDelayCnt = 0;
                            //出した数カウント加算
                            createCnt++;
                            saveObj = null;
                            discharged_Script = null;
    

                        }
					}
					//出した数カウントが出す数と同じになったら
					if (createCnt == createNum)
					{
						//出したひとまとまりカウント加算
						createGroupCnt++;
						//ひとまとまりを出すディレイカウントリセット
						createGroupDelayCnt = 0;
						//一体を出すカウントリセット
						createCnt = 0;
					}
				}
			}
			//ひとまとまりを出すディレイカウントが最大以下のとき
			else
			{
				createGroupDelayCnt++;
			}

		}

		if (hp < 1)
		{
			if (Died_Attack)
			{
				//死亡時攻撃の処理
				int bulletSpread = 180;      //角度を広げるための変数

				//複数発出すよう
				for (int i = 0; i < 7; i++)
				{
					//diedAttack_RotaZ = Random.Range(fd.degree - diedAttack_RotaValue, fd.degree + diedAttack_RotaValue);
					//diedAttack_Transform.rotation = Quaternion.Euler(0, 0, diedAttack_RotaZ);
					//diedAttackRota = Quaternion.Euler(0, 0, Random.Range(fd.degree - diedAttack_RotaValue, fd.degree + diedAttack_RotaValue));

					diedAttackRota = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + bulletSpread);

					Object_Instantiation.Object_Reboot(Game_Master.OBJECT_NAME.eENEMY_BULLET, createPosObj.transform.position, diedAttackRota);
					bulletSpread -= 30;     //広げる角度を変える
				}

				//diedAttackRota = Quaternion.Euler(0, 0, Random.Range(fd.degree - diedAttack_RotaValue, fd.degree + diedAttack_RotaValue));
				//Object_Instantiation.Object_Reboot(Game_Master.OBJECT_NAME.eENEMY_BULLET, transform.position, diedAttackRota);

			}
            Object_Instantiation.Object_Reboot(Game_Master.OBJECT_NAME.ePOWERUP_ITEM, createPosObj.transform.position, Quaternion.identity);

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
		//if (createDelayCnt > createDelayMax)
		//{
		//	Instantiate(createObj, transform.position, transform.rotation);
		//	createDelayCnt = 0;
		//}
	}

	public void SetMyDirection(MyDirection direc)
	{
		switch(direc)
		{
			case MyDirection.Up: transform.rotation = Quaternion.Euler(0, 0, 0); break;
			case MyDirection.Down: transform.rotation = Quaternion.Euler(0, 0, 180); break;
			case MyDirection.Left: transform.rotation = Quaternion.Euler(0, 0, 90); break;
			case MyDirection.Right: transform.rotation = Quaternion.Euler(0, 0, 270); break;
			case MyDirection.Free: break;
		}
		myDirection = direc;
	}

	//void SetState(SetMoveType setType)
	//{
	//	switch(setType)
	//	{
	//		case SetMoveType.LeftCurveUp_90:
	//			discharged_Script.SetState(Enemy_Discharged.MoveType.LeftCurveUp_90);
	//			break;

	//		case SetMoveType.LeftCueveUp_180:
	//			discharged_Script.SetState(Enemy_Discharged.MoveType.LeftCueveUp_180);
	//			break;

	//		case SetMoveType.LeftCurveDown_90:
	//			discharged_Script.SetState(Enemy_Discharged.MoveType.LeftCurveDown_90);
	//			break;

	//		case SetMoveType.LeftCueveDown_180:
	//			discharged_Script.SetState(Enemy_Discharged.MoveType.LeftCueveDown_180);
	//			break;

	//		case SetMoveType.RightCurveUp_90:
	//			discharged_Script.SetState(Enemy_Discharged.MoveType.RightCurveUp_90);
	//			break;

	//		case SetMoveType.RightCueveUp_180:
	//			discharged_Script.SetState(Enemy_Discharged.MoveType.RightCueveUp_180);
	//			break;

	//		case SetMoveType.RightCurveDown_90:
	//			discharged_Script.SetState(Enemy_Discharged.MoveType.RightCurveDown_90);
	//			break;

	//		case SetMoveType.RightCueveDown_180:
	//			discharged_Script.SetState(Enemy_Discharged.MoveType.RightCueveDown_180);
	//			break;
	//	}
	//}
}
