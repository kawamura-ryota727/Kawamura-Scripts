//作成：川村良太
//タイムラインで関数を呼び出して敵を順に出す

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class EnemyCreate_TimeLine : MonoBehaviour
{
	#region 敵オブジェクト(プーリング前のため)
	public GameObject dischargeObj;
	public GameObject followGroundObj;
	public GameObject taihoObj;
    public GameObject taihoObj_Item;
	public GameObject OctopusObj;
    public GameObject hunterObj;
    public GameObject hitodeSpownerObj;
    public GameObject waveEnemyObj;
    public GameObject waveEnemyObj_Item;
    public GameObject walkEnemyObj;
    public GameObject mantaMoveObj;
    public GameObject mantaStopObj;
    public GameObject mantaOneMoveObj;
    public GameObject ufoGroupObj;
    public GameObject containerObj;
    public GameObject finalBossObj;
	#endregion

	public GameObject saveObj;
	public GameObject mapObj;
	public Enemy_Discharge saveDischarge_Script;
    public Enemy_Battery saveTaiho_Script;
	public OctopusType_Enemy saveOctopus_Script;
	public FollowGround3 saveFollowGrownd_Script;
    public Enemy_Walk saveWalk_Script;
    public Enemy_Wave saveWave_Script;

	//作る敵と無線
	public enum CreateEnemyType
	{
		None,
		Discharge_Free,								//自分で向き指定するもの
		Discharge_LeftCurveUp90,                    //排出の上向き左90度カープ
		Discharge_RightCurveUp90,                   //排出の上向き右90度カープ
		Discharge_LeftCurveDown90,                  //排出の下向き左90度カープ
		Discharge_RightCurveDown90,                 //排出の下向き右90度カープ
		Discharge_Up_Left180,                       //排出左向き180度カーブ上
		Discharge_Down_Left180,                     //排出左向き180度カーブ下
		Discharge_Up_Right180,                      //排出右向き180度カーブ上
		Discharge_Down_Right180,                    //排出右向き180度カーブした
		Discharge_UpAndDown_LeftCurve90,            //排出上下左カーブ
		Discharge_UpAndDown_RightCurve90,           //排出上下右カーブ
		FollowGround_Left,
		FollowGround_Right,
		Taiho_Upward,                               //上向き大砲
        Taiho_Downward,                             //下向き大砲
        Taiho_Left,                                 //左向き大砲
		Taiho_Right,                                //右向き大砲
		Taiho_Free,                                 //自分で角度を指定する大砲
		Taiho_UpAndDown,                            //大砲上下
        Octopus_UpLeft,								//上向き左移動
		Octopus_UpRight,							//上向き右移動
		Octopus_DownLeft,							//下向き左移動
		Octopus_DownRight,							//下向き右移動
		OptionHunter,                               //オプションハンター
        Hitode_Square,                              //4方向からのヒトデ
        Walk_UpLeft,                                //歩く敵上側左方向移動
        Walk_UpRight,                               //歩く敵上側右方向移動
        Walk_DownLeft,                              //歩く敵下側左方向移動
        Walk_DownRight,                             //歩く敵下側右方向移動
        Walk_UpAndDown,
        Straight,                                   //直進（闘牛）
        Wave_Up,                                    //上下移動（闘牛）
        Wave_Down,                                  //上下移動（闘牛）
        Wave_UpAndDown,                             //上下移動の2体縦に同時出し
        Wave_UpAndDown_Item,                        //上下移動の アイテム2体縦に同時出し
        Taiho_Upward_Item,                          //上向き大砲アイテム
        Taiho_Downward_Item,                        //下向き大砲アイテム
        Taiho_UpAndDown_Item,                       //大砲上下アイテム
        Manta_Move,                                 //動くマンタ
        Manta_Stop,                                 //動かないマンタ
        UFO_Group,
        Wireless,                                   //無線オン
        Taiho_UpAndDown_Left,                            //大砲上下
        Taiho_Upward_Item_Left,                          //上向き大砲アイテム
        Taiho_Downward_Item_Left,                        //下向き大砲アイテム
        Taiho_UpAndDown_Item_Left,                       //大砲上下アイテム
        MantaOne,
        Container,
        FINALBOSS,
    }

    //作る位置
    public enum CreatePos
	{
		None,
		Discharge_Top,
		Discharge_Under,
		Taiho_Top,
		Taiho_Under,
        Walk_Top,
        Walk_Under,
        Wave_Up,
        Wave_Down,
        Manta_Stop,
        Discharge_Top_Left,
        Discharge_Under_Left,
        Taiho_Top_Left,
        Taiho_Under_Left,
        Walk_Top_Left,
        Walk_Under_Left,
    }

    //生成位置変数
    [Header("Discharge_Freeにしたときに使います")]
	public Enemy_Discharged.MoveType freeMoveType;
	public Transform dischargePos_Top;
	public Transform dischargePos_Under;
	public Transform taihoPos_Top;
	public Transform taihoPos_Under;
    public Transform walkPos_Top;
    public Transform walkPos_Under;
    public Transform waveUpPos;
    public Transform waveDownPos;
    public Transform mantaStopPos;
    public Transform dischargePos_Top_Left;
    public Transform dischargePos_Under_Left;
    public Transform taihoPos_Top_Left;
    public Transform taihoPos_Under_Left;
    public Transform walkPos_Top_Left;
    public Transform walkPos_Under_Left;

    public Quaternion enemyRota;

	[System.Serializable]
	public struct EnemyInformation
	{
		public string enemyName;
		public CreateEnemyType enemyType;
		public CreatePos createPos;
		[Header("出現位置を自分で指定する時にPosをNoneにして入れる")]
		public Vector3 manualVector;                    //手打ちで出したい位置を入力できる
		[Header("出現向きを自分で指定する敵用")]
		public Vector3 enemyRota;

	}
	public EnemyInformation[] enemyInformation = new EnemyInformation[5];

	private PlayableDirector Director { get; set; }             // デバッグ用プレイアブルディレクター

	public int createNum;                   //次に出す順番の数
	public string nextGroupName;        //次に出す敵の名前

    //タイムラインを止める
    public bool Is_TimelinePause;

    void Start()
	{
		mapObj = GameObject.Find("Stage_02_Map").gameObject;
		ResouceUpload();
		CreatePosUpload();
		EnemyNameSet();
		createNum = 1;
		Director = GetComponent<PlayableDirector>();
	}


	void Update()
	{
        if (Input.GetKey(KeyCode.P) && Input.GetKey(KeyCode.B)) Director.time = 260.0;
        else if (Input.GetKey(KeyCode.P) && Input.GetKey(KeyCode.V))
        {
            Director.time = 58.0;
            createNum = 6;
        }
        else if (Input.GetKey(KeyCode.P) && Input.GetKey(KeyCode.C))
        {
            Director.time = 75.0;
            createNum = 14;

        }
        else if (Input.GetKey(KeyCode.P) && Input.GetKey(KeyCode.X))
        {
            Director.time = 125.0;
            createNum = 23;

        }
        else if (Input.GetKey(KeyCode.P) && Input.GetKey(KeyCode.Z))
        {
            Director.time = 178.0;
            createNum = 47;

        }
        else if (Input.GetKey(KeyCode.P) && Input.GetKey(KeyCode.A))
        {
            Director.time = 324.0;
            createNum = 61;

        }
        else if (Input.GetKey(KeyCode.P) && Input.GetKey(KeyCode.S))
        {
            Director.time = 369.0;
            createNum = 63;

        }
        if (Input.GetKey(KeyCode.Slash)) Director.time += 1.0;
		else if (Input.GetKey(KeyCode.Backslash)) Director.time -= 1.0;

        if (Is_TimelinePause)
        {
            Director.Pause();
            Is_TimelinePause = false;
        }


    }

    public void TimeLineStop()
    {
        Is_TimelinePause = true;
    }

    public void WirelessOn()
    {
        Wireless_sinario.Is_using_wireless = true;
    }

    //リソースのロード
    void ResouceUpload()
	{
		dischargeObj = Resources.Load("Enemy2/Enemy_Discharge") as GameObject;
		followGroundObj = Resources.Load("Enemy2/Enemy_FollowGround") as GameObject;
		taihoObj = Resources.Load("Enemy2/Enemy_Taiho") as GameObject;
        taihoObj_Item = Resources.Load("Enemy2/Enemy_Taiho_Item") as GameObject;
        OctopusObj = Resources.Load("Enemy2/OctopusType_Enemy") as GameObject;
        hunterObj = Resources.Load("Enemy2/Enemy_StagBeetle") as GameObject;
        hitodeSpownerObj = Resources.Load("Enemy2/StarFish_Spowner2") as GameObject;
        walkEnemyObj = Resources.Load("Enemy2/Enemy_Walk") as GameObject;
		waveEnemyObj = Resources.Load("Enemy/ClamChowderType_Enemy") as GameObject;
        waveEnemyObj_Item = Resources.Load("Enemy/ClamChowderType_Enemy_Item") as GameObject;
        mantaMoveObj = Resources.Load("Enemy2/Enemy_MantaGroup_move") as GameObject;
        mantaOneMoveObj = Resources.Load("Enemy2/Enemy_Manta_move") as GameObject;
        mantaStopObj = Resources.Load("Enemy2/Enemy_MantaGroup_Stop") as GameObject;
        ufoGroupObj = Resources.Load("Enemy/Enemy_UFO_Group_NoneShot") as GameObject;
        containerObj = Resources.Load("Enemy2/Container_Move") as GameObject;
        finalBossObj = Resources.Load("Boss/Final_Boss") as GameObject;
    }

    //ポジションの取得
    void CreatePosUpload()
	{
		dischargePos_Top = GameObject.Find("DischargePos_Top").transform;
		dischargePos_Under = GameObject.Find("DischargePos_Under").transform;
		taihoPos_Top = GameObject.Find("TaihoPos_Top").transform;
		taihoPos_Under = GameObject.Find("TaihoPos_Under").transform;
        walkPos_Top = GameObject.Find("WalkPos_Top").transform;
        walkPos_Under = GameObject.Find("WalkPos_Under").transform;
        waveUpPos = GameObject.Find("WaveUpPos").transform;
        waveDownPos = GameObject.Find("WaveDownPos").transform;
        mantaStopPos = GameObject.Find("MantaStopPos").transform;
        dischargePos_Top_Left = GameObject.Find("DischargePos_Top_Left").transform;
        dischargePos_Under_Left = GameObject.Find("DischargePos_Under_Left").transform;
        taihoPos_Top_Left = GameObject.Find("TaihoPos_Top_Left").transform;
        taihoPos_Under_Left = GameObject.Find("TaihoPos_Under_Left").transform;
        walkPos_Top_Left = GameObject.Find("WalkPos_Top_Left").transform;
        walkPos_Under_Left = GameObject.Find("WalkPos_Under_Left").transform;
    }

    //出す敵の名前をセット（分かりやすくするためなので敵出現に直接影響はない）
    void EnemyNameSet()
	{
		for (int i = 0; i < 5; i++)
		{
			switch (enemyInformation[i].enemyType)
			{
				//なし
				case CreateEnemyType.None:
					enemyInformation[i].enemyName = "なし";
					break;

				//上向き90度左カーブ
				case CreateEnemyType.Discharge_Free:
					enemyInformation[i].enemyName = "指定排出";

					break;

				//上向き90度左カーブ
				case CreateEnemyType.Discharge_LeftCurveUp90:
					enemyInformation[i].enemyName = "上向き90度左カーブ";

					break;

				//上向き90度右カーブ
				case CreateEnemyType.Discharge_RightCurveUp90:
					enemyInformation[i].enemyName = "上向き90度右カーブ";
					break;

				//下向き90度左カーブ
				case CreateEnemyType.Discharge_LeftCurveDown90:
					enemyInformation[i].enemyName = "下向き90度左カーブ";
					break;

				//下向き90度右カーブ
				case CreateEnemyType.Discharge_RightCurveDown90:
					enemyInformation[i].enemyName = "下向き90度右カーブ";
					break;

				//左向き180度カーブ上
				case CreateEnemyType.Discharge_Up_Left180:
					enemyInformation[i].enemyName = "左向き180度カーブ上";

					break;

				//左向き180度カーブ下
				case CreateEnemyType.Discharge_Down_Left180:
					enemyInformation[i].enemyName = "左向き180度カーブ下";
					break;

				//右向き180度カーブ上
				case CreateEnemyType.Discharge_Up_Right180:
					enemyInformation[i].enemyName = "右向き180度カーブ上";
					break;

				//右向き180度カーブ下
				case CreateEnemyType.Discharge_Down_Right180:
					enemyInformation[i].enemyName = "右向き180度カーブ下";
					break;

				//上下左カーブ
				case CreateEnemyType.Discharge_UpAndDown_LeftCurve90:
					enemyInformation[i].enemyName = "上下左カーブ";
					break;

				//上下右カーブ
				case CreateEnemyType.Discharge_UpAndDown_RightCurve90:
					enemyInformation[i].enemyName = "上下右カーブ";
					break;

				//地面に沿う敵左進み
				case CreateEnemyType.FollowGround_Left:
					enemyInformation[i].enemyName = "地面に沿う敵左進み";
					break;

				//地面に沿う敵右進み
				case CreateEnemyType.FollowGround_Right:
					enemyInformation[i].enemyName = "地面に沿う敵右進み";
					break;

				//上向き大砲
				case CreateEnemyType.Taiho_Upward:
					enemyInformation[i].enemyName = "大砲上向き";
					break;

				//下向き大砲
				case CreateEnemyType.Taiho_Downward:
					enemyInformation[i].enemyName = "大砲下向き";
					break;

				//下向き大砲
				case CreateEnemyType.Taiho_Left:
					enemyInformation[i].enemyName = "大砲左向き";
					break;

				//下向き大砲
				case CreateEnemyType.Taiho_Right:
					enemyInformation[i].enemyName = "大砲右向き";
					break;

				//大砲上下
				case CreateEnemyType.Taiho_UpAndDown:
					enemyInformation[i].enemyName = "大砲上下";
					break;

				case CreateEnemyType.Octopus_UpLeft:
					enemyInformation[i].enemyName = "タコ上向き左移動";
					break;

				case CreateEnemyType.Octopus_UpRight:
					enemyInformation[i].enemyName = "タコ上向き右移動";
					break;

				case CreateEnemyType.Octopus_DownLeft:
					enemyInformation[i].enemyName = "タコ下向き左移動";
					break;

				case CreateEnemyType.Octopus_DownRight:
					enemyInformation[i].enemyName = "タコ下向き右移動";
					break;

				default:
					enemyInformation[i].enemyName = "不明";
					break;

			}
		}
	}

	public void EnemyCreate()
	{
		Vector3 pos = Vector3.zero;

		switch (enemyInformation[createNum].createPos)
		{
			case CreatePos.None: pos = enemyInformation[createNum].manualVector; break;
			case CreatePos.Discharge_Top: pos = dischargePos_Top.position; break;
			case CreatePos.Discharge_Under: pos = dischargePos_Under.position; break;
			case CreatePos.Taiho_Top: pos = taihoPos_Top.position; break;
			case CreatePos.Taiho_Under: pos = taihoPos_Under.position; break;
		}

		switch (enemyInformation[createNum].enemyType)
		{
			//なし
			case CreateEnemyType.None:
				createNum++;
				break;


			//指定排出
			case CreateEnemyType.Discharge_Free:
				//saveObj = Instantiate(dischargeObj, pos, enemyRota);
                saveObj = Obj_Storage.Storage_Data.Discharge_Enemy.Active_Obj();
                saveDischarge_Script = saveObj.GetComponent<Enemy_Discharge>();
                saveDischarge_Script.defaultParentObj = saveObj.transform.parent.gameObject;
                saveObj.transform.position = pos;
                transform.rotation = Quaternion.Euler(0, 0, enemyInformation[createNum].enemyRota.z);
                saveObj.transform.parent = mapObj.transform;
				saveDischarge_Script.SetMyDirection(Enemy_Discharge.MyDirection.Free);
				saveDischarge_Script.setMoveType = freeMoveType;

				saveObj = null;
				saveDischarge_Script = null;
				createNum++;
				break;

			//上向き90度左カーブ
			case CreateEnemyType.Discharge_LeftCurveUp90:
				//saveObj = Instantiate(dischargeObj, pos, transform.rotation);
                saveObj = Obj_Storage.Storage_Data.Discharge_Enemy.Active_Obj();
                saveDischarge_Script = saveObj.GetComponent<Enemy_Discharge>();
                saveDischarge_Script.defaultParentObj = saveObj.transform.parent.gameObject;
                saveObj.transform.position = pos;
                saveObj.transform.parent = mapObj.transform;
				saveDischarge_Script.SetMyDirection(Enemy_Discharge.MyDirection.Up);
				saveDischarge_Script.setMoveType = Enemy_Discharged.MoveType.LeftCurveUp_90;

				saveObj = null;
				saveDischarge_Script = null;
				createNum++;
				break;

			//上向き90度右カーブ
			case CreateEnemyType.Discharge_RightCurveUp90:
				saveObj = Instantiate(dischargeObj, pos, transform.rotation);
                //saveObj = Obj_Storage.Storage_Data.Discharge_Enemy.Active_Obj();
                saveObj.transform.position = pos;
                saveObj.transform.parent = mapObj.transform;
				saveDischarge_Script = saveObj.GetComponent<Enemy_Discharge>();
				saveDischarge_Script.SetMyDirection(Enemy_Discharge.MyDirection.Up);
				saveDischarge_Script.setMoveType = Enemy_Discharged.MoveType.RightCurveUp_90;

				saveObj = null;
				saveDischarge_Script = null;
				createNum++;
				break;

			//下向き90度左カーブ
			case CreateEnemyType.Discharge_LeftCurveDown90:
                //saveObj = Instantiate(dischargeObj, pos, transform.rotation);
                saveObj = Obj_Storage.Storage_Data.Discharge_Enemy.Active_Obj();
                saveDischarge_Script = saveObj.GetComponent<Enemy_Discharge>();
                saveDischarge_Script.defaultParentObj = saveObj.transform.parent.gameObject;
                saveObj.transform.position = pos;
                saveObj.transform.parent = mapObj.transform;
				saveDischarge_Script.SetMyDirection(Enemy_Discharge.MyDirection.Down);
				saveDischarge_Script.setMoveType = Enemy_Discharged.MoveType.LeftCurveDown_90;

				saveObj = null;
				saveDischarge_Script = null;
				createNum++;
				break;

			//下向き90度右カーブ
			case CreateEnemyType.Discharge_RightCurveDown90:
			    //saveObj = Instantiate(dischargeObj, pos, transform.rotation);
                saveObj = Obj_Storage.Storage_Data.Discharge_Enemy.Active_Obj();
                saveObj.transform.position = pos;
                saveObj.transform.parent = mapObj.transform;
				saveDischarge_Script = saveObj.GetComponent<Enemy_Discharge>();
				saveDischarge_Script.SetMyDirection(Enemy_Discharge.MyDirection.Down);
				saveDischarge_Script.setMoveType = Enemy_Discharged.MoveType.RightCurveDown_90;

				saveObj = null;
				saveDischarge_Script = null;
				createNum++;
				break;

			//左向き180度カーブ上
			case CreateEnemyType.Discharge_Up_Left180:
                saveObj = Instantiate(dischargeObj, pos, transform.rotation);
                //saveObj = Obj_Storage.Storage_Data.Discharge_Enemy.Active_Obj();
                saveObj.transform.position = pos;
                saveObj.transform.parent = mapObj.transform;
				saveDischarge_Script = saveObj.GetComponent<Enemy_Discharge>();
				saveDischarge_Script.SetMyDirection(Enemy_Discharge.MyDirection.Left);
				saveDischarge_Script.setMoveType = Enemy_Discharged.MoveType.RightCueveUp_180;

				saveObj = null;
				saveDischarge_Script = null;
				createNum++;

				break;

			//左向き180度カーブ下
			case CreateEnemyType.Discharge_Down_Left180:
                saveObj = Instantiate(dischargeObj, pos, transform.rotation);
                //saveObj = Obj_Storage.Storage_Data.Discharge_Enemy.Active_Obj();
                saveObj.transform.position = pos;
                saveObj.transform.parent = mapObj.transform;
				saveDischarge_Script = saveObj.GetComponent<Enemy_Discharge>();
				saveDischarge_Script.SetMyDirection(Enemy_Discharge.MyDirection.Left);
				saveDischarge_Script.setMoveType = Enemy_Discharged.MoveType.RightCueveDown_180;

				saveObj = null;
				saveDischarge_Script = null;
				createNum++;
				break;

			//右向き180度カーブ上
			case CreateEnemyType.Discharge_Up_Right180:
                saveObj = Instantiate(dischargeObj, pos, transform.rotation);
                //saveObj = Obj_Storage.Storage_Data.Discharge_Enemy.Active_Obj();
                saveObj.transform.position = pos;
                saveObj.transform.parent = mapObj.transform;
				saveDischarge_Script = saveObj.GetComponent<Enemy_Discharge>();
				saveDischarge_Script.SetMyDirection(Enemy_Discharge.MyDirection.Right);
				saveDischarge_Script.setMoveType = Enemy_Discharged.MoveType.LeftCueveUp_180;

				saveObj = null;
				saveDischarge_Script = null;
				createNum++;
				break;

			//右向き180度カーブ下
			case CreateEnemyType.Discharge_Down_Right180:
				saveObj = Instantiate(dischargeObj, pos, transform.rotation);
                //saveObj = Obj_Storage.Storage_Data.Discharge_Enemy.Active_Obj();
                saveObj.transform.position = pos;
                saveObj.transform.parent = mapObj.transform;
				saveDischarge_Script = saveObj.GetComponent<Enemy_Discharge>();
				saveDischarge_Script.SetMyDirection(Enemy_Discharge.MyDirection.Right);
				saveDischarge_Script.setMoveType = Enemy_Discharged.MoveType.LeftCueveDown_180;

				saveObj = null;
				saveDischarge_Script = null;
				createNum++;
				break;

			//上下左カーブ
			case CreateEnemyType.Discharge_UpAndDown_LeftCurve90:
				//saveObj = Instantiate(dischargeObj, dischargePos_Under.position, transform.rotation);
                saveObj = Obj_Storage.Storage_Data.Discharge_Enemy.Active_Obj();
                saveDischarge_Script = saveObj.GetComponent<Enemy_Discharge>();
                saveDischarge_Script.defaultParentObj = saveObj.transform.parent.gameObject;
                saveObj.transform.position = dischargePos_Under.position;
                saveObj.transform.parent = mapObj.transform;
				saveDischarge_Script.SetMyDirection(Enemy_Discharge.MyDirection.Up);
				saveDischarge_Script.setMoveType = Enemy_Discharged.MoveType.LeftCurveUp_90;

                //saveObj = Instantiate(dischargeObj, dischargePos_Top.position, transform.rotation);
                saveObj = Obj_Storage.Storage_Data.Discharge_Enemy.Active_Obj();
                saveDischarge_Script = saveObj.GetComponent<Enemy_Discharge>();
                saveDischarge_Script.defaultParentObj = saveObj.transform.parent.gameObject;
                saveObj.transform.position = dischargePos_Top.position;
                saveObj.transform.parent = mapObj.transform;
				saveDischarge_Script.SetMyDirection(Enemy_Discharge.MyDirection.Down);
				saveDischarge_Script.setMoveType = Enemy_Discharged.MoveType.LeftCurveDown_90;

				saveObj = null;
				saveDischarge_Script = null;
				createNum++;
				break;

			//上下右カーブ
			case CreateEnemyType.Discharge_UpAndDown_RightCurve90:
                saveObj = Instantiate(dischargeObj, dischargePos_Under_Left.position, transform.rotation);
                //saveObj = Obj_Storage.Storage_Data.Discharge_Enemy.Active_Obj();
                //saveObj.transform.position = dischargePos_Under.position;
                saveObj.transform.parent = mapObj.transform;
				saveDischarge_Script = saveObj.GetComponent<Enemy_Discharge>();
				saveDischarge_Script.SetMyDirection(Enemy_Discharge.MyDirection.Up);
				saveDischarge_Script.setMoveType = Enemy_Discharged.MoveType.RightCurveUp_90;

                saveObj = Instantiate(dischargeObj, dischargePos_Top_Left.position, transform.rotation);
                //saveObj = Obj_Storage.Storage_Data.Discharge_Enemy.Active_Obj();
                //saveObj.transform.position = dischargePos_Top.position;
                saveObj.transform.parent = mapObj.transform;
				saveDischarge_Script = saveObj.GetComponent<Enemy_Discharge>();
				saveDischarge_Script.SetMyDirection(Enemy_Discharge.MyDirection.Down);
				saveDischarge_Script.setMoveType = Enemy_Discharged.MoveType.RightCurveDown_90;

				saveObj = null;
				saveDischarge_Script = null;
				createNum++;
				break;

			//地面沿う敵左進み
			case CreateEnemyType.FollowGround_Left:
				saveObj = Instantiate(followGroundObj, pos, transform.rotation);
				//saveObj.transform.parent = mapObj.transform;
				saveFollowGrownd_Script = saveObj.GetComponent<FollowGround3>();
				saveFollowGrownd_Script.SetDirection(FollowGround3.DirectionState.Left);

				saveObj = null;
				saveFollowGrownd_Script = null;
				createNum++;
				break;

			//地面を這う敵右進み
			case CreateEnemyType.FollowGround_Right:
				saveObj = Instantiate(followGroundObj, pos, transform.rotation);
				//saveObj.transform.parent = mapObj.transform;
				saveFollowGrownd_Script = saveObj.GetComponent<FollowGround3>();
				saveFollowGrownd_Script.SetDirection(FollowGround3.DirectionState.Right);

				saveObj = null;
				saveFollowGrownd_Script = null;
				createNum++;
				break;

			//上向き大砲
			case CreateEnemyType.Taiho_Upward:
				//saveObj = Instantiate(taihoObj, pos, transform.rotation);
                saveObj = Obj_Storage.Storage_Data.Cannon_Enemy.Active_Obj();
                saveTaiho_Script = saveObj.GetComponent<Enemy_Battery>();
                saveTaiho_Script.defaultParentObj = saveObj.transform.parent.gameObject;
                saveObj.transform.position = pos;
                saveObj.transform.rotation = Quaternion.Euler(0, 0, 0);
				saveObj.transform.parent = mapObj.transform;

                saveObj = null;
				createNum++;
				break;

			//下向き大砲
			case CreateEnemyType.Taiho_Downward:
				//saveObj = Instantiate(taihoObj, pos, Quaternion.Euler(0, 0, 180));
                saveObj = Obj_Storage.Storage_Data.Cannon_Enemy.Active_Obj();
                saveTaiho_Script = saveObj.GetComponent<Enemy_Battery>();
                saveTaiho_Script.defaultParentObj = saveObj.transform.parent.gameObject;
                saveObj.transform.position = pos;
                saveObj.transform.rotation = Quaternion.Euler(0, 0, 180);
                saveObj.transform.parent = mapObj.transform;

                saveObj = null;
				createNum++;
				break;

			//左向き大砲
			case CreateEnemyType.Taiho_Left:
                saveObj = Instantiate(taihoObj, pos, Quaternion.Euler(0, 0, 90));
               // saveObj = Obj_Storage.Storage_Data.Cannon_Enemy.Active_Obj();
                saveObj.transform.position = pos;
                saveObj.transform.rotation = Quaternion.Euler(0, 0, 90);

                saveObj.transform.parent = mapObj.transform;
				saveObj = null;
				createNum++;
				break;

			//右向き大砲
			case CreateEnemyType.Taiho_Right:
				saveObj = Instantiate(taihoObj, pos, Quaternion.Euler(0, 0, 270));
               // saveObj = Obj_Storage.Storage_Data.Cannon_Enemy.Active_Obj();
                saveObj.transform.position = pos;
                saveObj.transform.rotation = Quaternion.Euler(0, 0, 270);
                saveObj.transform.parent = mapObj.transform;

                saveObj = null;
				createNum++;
				break;

			//角度指定大砲
			case CreateEnemyType.Taiho_Free:
                //saveObj = Instantiate(taihoObj, pos, Quaternion.Euler(0, 0, enemyInformation[createNum].enemyRota.z));
                saveObj = Obj_Storage.Storage_Data.Cannon_Enemy.Active_Obj();
                saveTaiho_Script = saveObj.GetComponent<Enemy_Battery>();
                saveTaiho_Script.defaultParentObj = saveObj.transform.parent.gameObject;
                saveObj.transform.position = pos;
                saveObj.transform.rotation = Quaternion.Euler(0, 0, enemyInformation[createNum].enemyRota.z);

                saveObj.transform.parent = mapObj.transform;
				saveObj = null;
				createNum++;
				break;


			//上下大砲
			case CreateEnemyType.Taiho_UpAndDown:
                //saveObj = Instantiate(taihoObj, taihoPos_Under.position, transform.rotation);
                saveObj = Obj_Storage.Storage_Data.Cannon_Enemy.Active_Obj();
                saveTaiho_Script = saveObj.GetComponent<Enemy_Battery>();
                saveTaiho_Script.defaultParentObj = saveObj.transform.parent.gameObject;
                saveObj.transform.position = taihoPos_Under.position;
                saveObj.transform.rotation = Quaternion.Euler(0, 0, 0);
                saveObj.transform.parent = mapObj.transform;

                //saveObj = Instantiate(taihoObj, taihoPos_Top.position, Quaternion.Euler(0, 0, 180));
                saveObj = Obj_Storage.Storage_Data.Cannon_Enemy.Active_Obj();
                saveTaiho_Script = saveObj.GetComponent<Enemy_Battery>();
                saveTaiho_Script.defaultParentObj = saveObj.transform.parent.gameObject;
                saveObj.transform.position = taihoPos_Top.position;
                saveObj.transform.rotation = Quaternion.Euler(0, 0, 180);
                saveObj.transform.parent = mapObj.transform;

				saveObj = null;
				createNum++;
				break;

            //上下大砲 左側
            case CreateEnemyType.Taiho_UpAndDown_Left:
                //saveObj = Instantiate(taihoObj, taihoPos_Under_Left.position, transform.rotation);
                saveObj = Obj_Storage.Storage_Data.Cannon_Enemy.Active_Obj();
                saveTaiho_Script = saveObj.GetComponent<Enemy_Battery>();
                saveTaiho_Script.defaultParentObj = saveObj.transform.parent.gameObject;
                saveObj.transform.position = taihoPos_Under_Left.position;
                saveObj.transform.rotation = Quaternion.Euler(0, 0, 0);
                saveObj.transform.parent = mapObj.transform;

                //saveObj = Instantiate(taihoObj, taihoPos_Top_Left.position, Quaternion.Euler(0, 0, 180));
                saveObj = Obj_Storage.Storage_Data.Cannon_Enemy.Active_Obj();
                saveTaiho_Script = saveObj.GetComponent<Enemy_Battery>();
                saveTaiho_Script.defaultParentObj = saveObj.transform.parent.gameObject;
                saveObj.transform.position = taihoPos_Top_Left.position;
                saveObj.transform.rotation = Quaternion.Euler(0, 0, 180);
                saveObj.transform.parent = mapObj.transform;

                saveObj = null;
                createNum++;
                break;

            //上下大砲アイテム
            case CreateEnemyType.Taiho_UpAndDown_Item:
                //saveObj = Instantiate(taihoObj_Item, taihoPos_Under.position, transform.rotation);
                saveObj = Obj_Storage.Storage_Data.Cannon_Enemy_Item.Active_Obj();
                saveTaiho_Script = saveObj.GetComponent<Enemy_Battery>();
                saveTaiho_Script.defaultParentObj = saveObj.transform.parent.gameObject;
                saveObj.transform.position = taihoPos_Under.position;
                saveObj.transform.rotation = Quaternion.Euler(0, 0, 0);
                saveObj.transform.parent = mapObj.transform;

                //saveObj = Instantiate(taihoObj_Item, taihoPos_Top.position, Quaternion.Euler(0, 0, 180));
                saveObj = Obj_Storage.Storage_Data.Cannon_Enemy_Item.Active_Obj();
                saveTaiho_Script = saveObj.GetComponent<Enemy_Battery>();
                saveTaiho_Script.defaultParentObj = saveObj.transform.parent.gameObject;
                saveObj.transform.position = taihoPos_Top.position;
                saveObj.transform.rotation = Quaternion.Euler(0, 0, 180);
                saveObj.transform.parent = mapObj.transform;

                saveObj = null;
                createNum++;
                break;

            //上下大砲アイテム　左側
            case CreateEnemyType.Taiho_UpAndDown_Item_Left:
                //saveObj = Instantiate(taihoObj_Item, taihoPos_Under_Left.position, transform.rotation);
                saveObj = Obj_Storage.Storage_Data.Cannon_Enemy_Item.Active_Obj();
                saveTaiho_Script = saveObj.GetComponent<Enemy_Battery>();
                saveTaiho_Script.defaultParentObj = saveObj.transform.parent.gameObject;
                saveObj.transform.position = taihoPos_Under_Left.position;
                saveObj.transform.rotation = Quaternion.Euler(0, 0, 0);
                saveObj.transform.parent = mapObj.transform;

                //saveObj = Instantiate(taihoObj_Item, taihoPos_Top_Left.position, Quaternion.Euler(0, 0, 180));
                saveObj = Obj_Storage.Storage_Data.Cannon_Enemy_Item.Active_Obj();
                saveTaiho_Script = saveObj.GetComponent<Enemy_Battery>();
                saveTaiho_Script.defaultParentObj = saveObj.transform.parent.gameObject;
                saveObj.transform.position = taihoPos_Top_Left.position;
                saveObj.transform.rotation = Quaternion.Euler(0, 0, 180);
                saveObj.transform.parent = mapObj.transform;

                saveObj = null;
                createNum++;
                break;

            //タコ上向き左移動(斜めの時角度は-27度がいいかも
            case CreateEnemyType.Octopus_UpLeft:
				//saveObj = Instantiate(OctopusObj, pos, Quaternion.Euler(0, enemyRota.y, enemyRota.z));
                saveObj = Obj_Storage.Storage_Data.OctopusType_Enemy.Active_Obj();
                saveObj.transform.position = pos;
                saveObj.transform.rotation = Quaternion.Euler(0, enemyInformation[createNum].enemyRota.y, enemyInformation[createNum].enemyRota.z);
                saveObj.transform.parent = mapObj.transform;
				saveOctopus_Script = saveObj.GetComponent<OctopusType_Enemy>();
				saveOctopus_Script.bottomDirection = OctopusType_Enemy.DIRECTION.eUP;
				saveOctopus_Script.direc_Horizon = OctopusType_Enemy.DIRECTION_HORIZONTAL.eLEFT;
				saveOctopus_Script.horizontalMovementDirection = -1;

				saveObj = null;
				saveOctopus_Script = null;
				createNum++;

				break;

			//タコ上向き右移動(斜めの時角度は-27度がいいかも
			case CreateEnemyType.Octopus_UpRight:
                //saveObj = Instantiate(OctopusObj, pos, Quaternion.Euler(0, enemyRota.y, enemyRota.z));
                saveObj = Obj_Storage.Storage_Data.OctopusType_Enemy.Active_Obj();
                saveObj.transform.position = pos;
                saveObj.transform.rotation = Quaternion.Euler(0, enemyInformation[createNum].enemyRota.y, enemyInformation[createNum].enemyRota.z);
                saveObj.transform.parent = mapObj.transform;
				saveOctopus_Script = saveObj.GetComponent<OctopusType_Enemy>();
				saveOctopus_Script.bottomDirection = OctopusType_Enemy.DIRECTION.eUP;
				saveOctopus_Script.direc_Horizon = OctopusType_Enemy.DIRECTION_HORIZONTAL.eRIGHT;

				saveObj = null;
				saveOctopus_Script = null;
				createNum++;

				break;

			//タコ下向き左移動(斜めの時角度は153度がいいかも
			case CreateEnemyType.Octopus_DownLeft:
                //saveObj = Instantiate(OctopusObj, pos, Quaternion.Euler(0, enemyRota.y, enemyRota.z));
                saveObj = Obj_Storage.Storage_Data.OctopusType_Enemy.Active_Obj();
                saveObj.transform.position = pos;
                saveObj.transform.rotation = Quaternion.Euler(0, enemyInformation[createNum].enemyRota.y, enemyInformation[createNum].enemyRota.z);
                saveObj.transform.parent = mapObj.transform;
				saveOctopus_Script = saveObj.GetComponent<OctopusType_Enemy>();
				saveOctopus_Script.bottomDirection = OctopusType_Enemy.DIRECTION.eDOWN;
				saveOctopus_Script.direc_Horizon = OctopusType_Enemy.DIRECTION_HORIZONTAL.eLEFT;
				saveOctopus_Script.horizontalMovementDirection = -1;

				saveObj = null;
				saveOctopus_Script = null;
				createNum++;

				break;

			//タコ下向き右移動(斜めの時角度は153度がいいかも
			case CreateEnemyType.Octopus_DownRight:
                //saveObj = Instantiate(OctopusObj, pos, Quaternion.Euler(0, enemyRota.y, enemyRota.z));
                saveObj = Obj_Storage.Storage_Data.OctopusType_Enemy.Active_Obj();
                saveObj.transform.position = pos;
                saveObj.transform.rotation = Quaternion.Euler(0, enemyInformation[createNum].enemyRota.y, enemyInformation[createNum].enemyRota.z);
                saveObj.transform.parent = mapObj.transform;
				saveOctopus_Script = saveObj.GetComponent<OctopusType_Enemy>();
				saveOctopus_Script.bottomDirection = OctopusType_Enemy.DIRECTION.eDOWN;
				saveOctopus_Script.direc_Horizon = OctopusType_Enemy.DIRECTION_HORIZONTAL.eRIGHT;

				saveObj = null;
				saveOctopus_Script = null;
				createNum++;

				break;

            case CreateEnemyType.OptionHunter:
                saveObj = Instantiate(hunterObj, pos, transform.rotation);
                //saveObj = Obj_Storage.Storage_Data.StagBeetle_Enemy.Active_Obj();
                //saveObj.transform.position = pos;
                //saveObj.transform.parent = mapObj.transform;

                saveObj = null;
                createNum++;
                break;

            case CreateEnemyType.Hitode_Square:
                saveObj = Instantiate(hitodeSpownerObj, pos, transform.rotation);
                saveObj.transform.parent = mapObj.transform;

                saveObj = null;
                createNum++;
                break;

            case CreateEnemyType.Walk_UpLeft:
                //saveObj = Instantiate(walkEnemyObj, walkPos_Top.position, transform.rotation);
                saveObj = Obj_Storage.Storage_Data.Walk_Enemy.Active_Obj();
                saveWalk_Script = saveObj.GetComponent<Enemy_Walk>();
                saveWalk_Script.defaultParentObj = saveObj.transform.parent.gameObject;
                saveObj.transform.position = walkPos_Top.position;
                saveObj.transform.parent = mapObj.transform;
                saveWalk_Script.direcState = Enemy_Walk.DirectionState.Left;
                saveWalk_Script.direction_Vertical = Enemy_Walk.Direction_Vertical.Top;

                saveObj = null;
                saveWalk_Script = null;
                createNum++;
                break;

            case CreateEnemyType.Walk_UpRight:
                saveObj = Instantiate(walkEnemyObj, walkPos_Top_Left.position, transform.rotation);
                //saveObj = Obj_Storage.Storage_Data.Walk_Enemy.Active_Obj();
                //saveObj.transform.position = walkPos_Top.position;
                saveObj.transform.parent = mapObj.transform;
                saveWalk_Script = saveObj.GetComponent<Enemy_Walk>();
                saveWalk_Script.direcState = Enemy_Walk.DirectionState.Right;
                saveWalk_Script.direction_Vertical = Enemy_Walk.Direction_Vertical.Top;

                saveObj = null;
                saveWalk_Script = null;
                createNum++;
                break;

            case CreateEnemyType.Walk_DownLeft:
                //saveObj = Instantiate(walkEnemyObj, walkPos_Under.position, transform.rotation);
                saveObj = Obj_Storage.Storage_Data.Walk_Enemy.Active_Obj();
                saveWalk_Script = saveObj.GetComponent<Enemy_Walk>();
                saveWalk_Script.defaultParentObj = saveObj.transform.parent.gameObject;
                saveObj.transform.position = walkPos_Under.position;
                saveObj.transform.parent = mapObj.transform;
                saveWalk_Script.direcState = Enemy_Walk.DirectionState.Left;
                saveWalk_Script.direction_Vertical = Enemy_Walk.Direction_Vertical.Under;

                saveObj = null;
                saveWalk_Script = null;
                createNum++;
                break;

            case CreateEnemyType.Walk_DownRight:
                saveObj = Instantiate(walkEnemyObj, walkPos_Under_Left.position, transform.rotation);
                //saveObj = Obj_Storage.Storage_Data.Walk_Enemy.Active_Obj();
                //saveObj.transform.position = walkPos_Under.position;
                saveObj.transform.parent = mapObj.transform;
                saveWalk_Script = saveObj.GetComponent<Enemy_Walk>();
                saveWalk_Script.direcState = Enemy_Walk.DirectionState.Right;
                saveWalk_Script.direction_Vertical = Enemy_Walk.Direction_Vertical.Under;

                saveObj = null;
                saveWalk_Script = null;
                createNum++;
                break;

            case CreateEnemyType.Walk_UpAndDown:
                //saveObj = Instantiate(walkEnemyObj, walkPos_Under.position, transform.rotation);
                saveObj = Obj_Storage.Storage_Data.Walk_Enemy.Active_Obj();
                saveWalk_Script = saveObj.GetComponent<Enemy_Walk>();
                saveWalk_Script.defaultParentObj = saveObj.transform.parent.gameObject;
                saveObj.transform.position = walkPos_Under.position;
                saveObj.transform.parent = mapObj.transform;
                saveWalk_Script.direcState = Enemy_Walk.DirectionState.Left;
                saveWalk_Script.direction_Vertical = Enemy_Walk.Direction_Vertical.Under;

                //saveObj = Instantiate(walkEnemyObj, walkPos_Top.position, transform.rotation);
                saveObj = Obj_Storage.Storage_Data.Walk_Enemy.Active_Obj();
                saveWalk_Script = saveObj.GetComponent<Enemy_Walk>();
                saveWalk_Script.defaultParentObj = saveObj.transform.parent.gameObject;
                saveObj.transform.position = walkPos_Top.position;
                saveObj.transform.parent = mapObj.transform;
                saveWalk_Script.direcState = Enemy_Walk.DirectionState.Left;
                saveWalk_Script.direction_Vertical = Enemy_Walk.Direction_Vertical.Top;

                saveObj = null;
                saveWalk_Script = null;
                createNum++;
                break;


            case CreateEnemyType.Straight:
                saveObj = Instantiate(waveEnemyObj, pos, transform.rotation);
                //saveObj = Obj_Storage.Storage_Data.ClamChowderType_Enemy.Active_Obj();
                //saveObj.transform.position = pos;
                saveObj.transform.parent = mapObj.transform;
                saveWave_Script = saveObj.GetComponent<Enemy_Wave>();
                saveWave_Script.eState = Enemy_Wave.State.Straight;

                saveObj = null;
                saveWave_Script = null;
                createNum++;
                break;

            case CreateEnemyType.Wave_Up:
                //saveObj = Instantiate(waveEnemyObj, waveUpPos.position, transform.rotation);
                saveObj = Obj_Storage.Storage_Data.ClamChowderType_Enemy.Active_Obj();
                saveWave_Script = saveObj.GetComponent<Enemy_Wave>();
                saveWave_Script.defaultParentObj = saveObj.transform.parent.gameObject;
                saveObj.transform.position = waveUpPos.position;
                saveObj.transform.parent = mapObj.transform;
                saveWave_Script.eState = Enemy_Wave.State.WaveOnlyUp;

                saveObj = null;
                saveWave_Script = null;
                createNum++;

                break;

            case CreateEnemyType.Wave_Down:
                //saveObj = Instantiate(waveEnemyObj, waveDownPos.position, transform.rotation);
                saveObj = Obj_Storage.Storage_Data.ClamChowderType_Enemy.Active_Obj();
                saveWave_Script = saveObj.GetComponent<Enemy_Wave>();
                saveWave_Script.defaultParentObj = saveObj.transform.parent.gameObject;
                saveObj.transform.position = waveDownPos.position;
                saveObj.transform.parent = mapObj.transform;
                saveWave_Script.eState = Enemy_Wave.State.WaveOnlyDown;

                saveObj = null;
                saveWave_Script = null;
                createNum++;

                break;

            case CreateEnemyType.Wave_UpAndDown:
                //saveObj = Instantiate(waveEnemyObj, waveUpPos.position, transform.rotation);
                saveObj = Obj_Storage.Storage_Data.ClamChowderType_Enemy.Active_Obj();
                saveWave_Script = saveObj.GetComponent<Enemy_Wave>();
                saveWave_Script.defaultParentObj = saveObj.transform.parent.gameObject;
                saveObj.transform.position = waveUpPos.position;
                saveObj.transform.parent = mapObj.transform;
                saveWave_Script.eState = Enemy_Wave.State.WaveOnlyUp;

                //saveObj = Instantiate(waveEnemyObj, waveDownPos.position, transform.rotation);
                saveObj = Obj_Storage.Storage_Data.ClamChowderType_Enemy.Active_Obj();
                saveWave_Script = saveObj.GetComponent<Enemy_Wave>();
                saveWave_Script.defaultParentObj = saveObj.transform.parent.gameObject;
                saveObj.transform.position = waveDownPos.position;
                saveObj.transform.parent = mapObj.transform;
                saveWave_Script.eState = Enemy_Wave.State.WaveOnlyDown;

                saveObj = null;
                saveWave_Script = null;
                createNum++;
                break;

            case CreateEnemyType.Wave_UpAndDown_Item:
                //saveObj = Instantiate(waveEnemyObj_Item, waveUpPos.position, transform.rotation);
                saveObj = Obj_Storage.Storage_Data.ClamChowderType_Enemy_Item.Active_Obj();
                saveWave_Script = saveObj.GetComponent<Enemy_Wave>();
                saveWave_Script.defaultParentObj = saveObj.transform.parent.gameObject;
                saveObj.transform.position = waveUpPos.position;
                saveObj.transform.parent = mapObj.transform;
                saveWave_Script.eState = Enemy_Wave.State.WaveOnlyUp;

                //saveObj = Instantiate(waveEnemyObj_Item, waveDownPos.position, transform.rotation);
                saveObj = Obj_Storage.Storage_Data.ClamChowderType_Enemy_Item.Active_Obj();
                saveWave_Script = saveObj.GetComponent<Enemy_Wave>();
                saveWave_Script.defaultParentObj = saveObj.transform.parent.gameObject;
                saveObj.transform.position = waveDownPos.position;
                saveObj.transform.parent = mapObj.transform;
                saveWave_Script = saveObj.GetComponent<Enemy_Wave>();
                saveWave_Script.eState = Enemy_Wave.State.WaveOnlyDown;

                saveObj = null;
                saveWave_Script = null;
                createNum++;
                break;

            case CreateEnemyType.Manta_Move:
                saveObj = Instantiate(mantaMoveObj, pos, transform.rotation);
                //saveObj.transform.parent = mapObj.transform;

                saveObj = null;
                createNum++;
                break;

            case CreateEnemyType.MantaOne:
                saveObj = Instantiate(mantaOneMoveObj, pos, transform.rotation);
                //saveObj.transform.parent = mapObj.transform;

                saveObj = null;
                createNum++;
                break;

            case CreateEnemyType.Manta_Stop:
                saveObj = Instantiate(mantaStopObj, mantaStopPos.position, transform.rotation);
                saveObj.transform.parent = mapObj.transform;

                saveObj = null;
                createNum++;
                break;

            case CreateEnemyType.UFO_Group:
                //saveObj = Instantiate(ufoGroupObj, pos, transform.rotation);
                saveObj = Obj_Storage.Storage_Data.enemy_UFO_Group_NoneShot.Active_Obj();
                saveObj.transform.position = pos;
                saveObj.transform.parent = mapObj.transform;

                saveObj = null;
                createNum++;
                break;

            case CreateEnemyType.Container:
                //saveObj = Instantiate(containerObj, pos, transform.rotation);
                saveObj = Obj_Storage.Storage_Data.Container_Move.Active_Obj();
                saveObj.transform.position = pos;
                saveObj.transform.parent = mapObj.transform;

                saveObj = null;
                createNum++;
                break;

            case CreateEnemyType.Wireless:
                Wireless_sinario.Is_using_wireless = true;
                createNum++;
                break;

            case CreateEnemyType.FINALBOSS:
                saveObj = Instantiate(finalBossObj, pos, transform.rotation);

                saveObj = null;
                createNum++;
                break;

            default:
				break;
		}

		nextGroupName = enemyInformation[createNum].enemyName;
	}
}

