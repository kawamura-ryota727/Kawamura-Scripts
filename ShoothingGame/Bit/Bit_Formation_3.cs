//作成者：川村良太
//オプションの位置関係の処理のスクリプト

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bit_Formation_3 : MonoBehaviour
{
	public enum BitState
	{
		Circular,        //初期位置（円運動）
		Follow,         //追従状態
		Oblique,        //斜め撃ち状態
		Laser,          //レーザー状態
		Stay,               //停止状態
		Return,         //戻ってきている状態
		Player1,            //プレイヤー１に追従状態
		Player2,            //プレイヤー２に追従状態
	}

	[SerializeField]
	public BitState bState;                         //オプションの状態


	//[SerializeField]
	//BitState previous_state;					//オプションの前の状態（レーザーを解除したときに使う）

	public GameObject playerObj;                //プレイヤーのオブジェクト
	public GameObject player2Obj;
	//public GameObject parentObj;				//親のオブジェクト
	public GameObject followPosObj;             //プレイヤーを追従するときの位置オブジェクト
	public GameObject followPosFirstObj;        //プレイヤーに一番近い追従位置オブジェクト
	public GameObject followPosSecondObj;       //二番目
	public GameObject followPosThirdObj;        //三番目
	public GameObject followPosFourthObj;       //四番目
	public GameObject previousBitObj;			//一つ前のオプションオブジェクト
	public Bit_Formation_3 option_Script;
	public GameObject[] circlePosObjects;
	public GameObject[] fixedPosObjects;

	public GameObject target;
												//GameObject obliquePosObj;					//斜めうち状態の座標用オブジェクト
	GameObject laserPos;                        //レーザー時の座標用オブジェクト
	public GameObject particleObj;

	public ParticleSystem option_Particle;      //レーザーのパーティクルを取得するための変数

	Bit_Shot b_Shot;                            //オプションの攻撃スクリプト情報
	public Player1 pl1;							//プレイヤースクリプト情報
	public Player2 pl2;
	FollowPositions followPositions_Script;		//4つの追従位置の親のスクリプト
	FollowToPlayer_SameMotion FtoPlayer;        //プレイヤーに一番近い追従位置オブジェクトのスクリプト情報
	FollowToPreviousBit FtoPBit_Second;         //二番目の位置のスクリプト情報
	FollowToPreviousBit FtoPBit_Third;          //三番目の位置のスクリプト情報
	FollowToPreviousBit FtoPBit_Fourth;         //四番目の位置のスクリプト情報
	Option_Scale os;                            //パーティクルのスケール変更クリプト

	new Renderer renderer;                      //レンダラー　3Dオブジェクトの時使う
	Color bit_Color;                            //オプションの色　3Dオブジェクトの時使う
	Color particle_Color;                           //パーティクルのカラー
	public float scale_value = 0.5f;            //オプションのスケールの値

	float speed;                                //オプションの移動スピード（プレイヤー死亡時の処理に使う）
	public float moveSpeed;						//オプションの移動速度上とは違う 
	public float defaultSpeed;                  //プレイヤーが死んだときのオプションの初速を入れておく
	float step;                                 //スピードを計算して入れる
	int collectDelay;                           //死亡時すぐ取ってしまわないように当たり判定にディレイを持たせる

	//int state_Num;							//オプションの状態を変えるための数字		
	public int option_OrdinalNum;               //オプション自身が何番目の追従位置にいるのかの番号
	public int huntNum = 0;                     //盗まれた数
	public int huntMoveNum = 0;					//盗まれた先の位置番号

	[SerializeField]
	string myName;                              //自分の名前を入れる
	private Quaternion Direction;               //オブジェクトの向きを変更する時に使う

	Vector3 velocity;                           //ベロシティ

	//bool isCircular = false;
	//bool isFollow = false;					//プレイヤーを追従する位置に向かっているかどうか
	//bool once = true;
	//bool isScaleInc = false;
	//bool isScaleDec = false;
	//bool isPlayerDieCheck;					
	public bool isborn = true;						//オプションが出現したときupdateで一回だけ行う処理用
	public bool isDead = false;						//プレイヤーが死んで回収されるまでtrue、回収されたらfalse
	public bool isCollection = false;               //回収されたときに使う
	public bool isFirstStolen = false;
	public bool isStolen;                           //盗まれた状態
	public bool isStolenSetting = false;                  //盗まれたあとの設定をしたかどうか
	public bool isHunterHit = false;				//ハンターに当たったかどうか
	bool isCircle = false;
	public bool isFixed = false;
	public bool isMove = false;

	void Start()
	{
		isborn = true;						//出現時の処理をするように
		defaultSpeed = 20;					//死んだときの初速設定
		speed = defaultSpeed;			//初速を代入

		os = particleObj.GetComponent<Option_Scale>();
		renderer = gameObject.GetComponent<Renderer>();         //レンダラー取得

		//circlePosObjects = new GameObject[4];
		//fixedPosObjects = new GameObject[4];


		////4つの追従位置とそれぞれのスクリプト取得
		//followPosFirstObj = GameObject.Find("FollowPosFirst_1P");
		//FtoPlayer = followPosFirstObj.GetComponent<FollowToPlayer_SameMotion>();

		//followPosSecondObj = GameObject.Find("FollowPosSecond_1P");
		//FtoPBit_Second = followPosSecondObj.GetComponent<FollowToPreviousBit>();

		//followPosThirdObj = GameObject.Find("FollowPosThird_1P");
		//FtoPBit_Third=followPosThirdObj.GetComponent<FollowToPreviousBit>();

		//followPosFourthObj = GameObject.Find("FollowPosFourth_1P");
		//FtoPBit_Fourth=followPosFourthObj.GetComponent<FollowToPreviousBit>();


		//parentObj = transform.parent.gameObject;			//親のオブジェクト取得

		myName = gameObject.name;                           //自分の名前取得

		b_Shot = gameObject.GetComponent<Bit_Shot>();       //攻撃の情報取得
	}

	void Update()
	{
		//プレイヤーオブジェクトを取得していなかったら取得してプレイヤーのスクリプトも取得
		if (playerObj == null)
		{
			playerObj = Obj_Storage.Storage_Data.GetPlayer();

			pl1 = playerObj.GetComponent<Player1>();

		}

		if (player2Obj == null)
		{
			player2Obj = Obj_Storage.Storage_Data.GetPlayer2();

			pl2 = player2Obj.GetComponent<Player2>();

		}

		//生成された時の処理
		if (isborn)
		{
			//円移動🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲
			//for (int i = 0; i < 4; i++)
			//{
			//	switch (i)
			//	{
			//		case 0:
			//			circlePosObjects[i] = GameObject.Find("CirclePos_1");
			//			break;
			//		case 1:
			//			circlePosObjects[i] = GameObject.Find("CirclePos_2");
			//			break;
			//		case 2:
			//			circlePosObjects[i] = GameObject.Find("CirclePos_3");
			//			break;
			//		case 3:
			//			circlePosObjects[i] = GameObject.Find("CirclePos_4");
			//			break;

			//	}
			//}
			//円移動🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲

			//固定位置🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲
			//for (int i = 0; i < 4; i++)
			//{
			//	switch (i)
			//	{
			//		case 0:
			//			fixedPosObjects[i] = GameObject.Find("FixedPos_1");
			//			break;
			//		case 1:
			//			fixedPosObjects[i] = GameObject.Find("FixedPos_2");
			//			break;
			//		case 2:
			//			fixedPosObjects[i] = GameObject.Find("FixedPos_3");
			//			break;
			//		case 3:
			//			fixedPosObjects[i] = GameObject.Find("FixedPos_4");
			//			break;

			//	}
			//}
			//固定位置🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲

			SetFollowPos();             //追従位置設定
			option_Particle.Play();     //オプションの見た目パーティクルを起動
			isborn = false;             //この生成時処理をしないようにする
			b_Shot.isShot = true;
		}

		//フォーメーション切り替え🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲
		//if (Input.GetButtonDown("LB"))
		//{
		//これはコメントアウトしといて
		//if (followPositions_Script.isCircle)
		//{
		//	isMove = true;
		//	target = followPosObj;
		//	target = fixedPosObjects[option_OrdinalNum - 1];

		//}
		//else if (followPositions_Script.isFixed)
		//{
		//	isMove = true;
		//	target = followPosObj;
		//	transform.rotation = Quaternion.Euler(0, 0, 0);

		//}
		//else
		//{
		//	isMove = true;
		//	target = circlePosObjects[option_OrdinalNum - 1]
		//}
		//ここまでコメントアウトしといて

		//オンならオフに
		//	if (isCircle)
		//	{

		//		isCircle = false;
		//		isFixed = true;
		//		isMove = true;
		//		target = followPosObj;
		//		target = fixedPosObjects[option_OrdinalNum - 1];

		//	}
		//	else if (isFixed)
		//	{
		//		isFixed = false;
		//		isCircle = false;
		//		isMove = true;
		//		target = followPosObj;
		//		transform.rotation = Quaternion.Euler(0, 0, 0);
		//	}
		//	else
		//	{
		//		isCircle = true;
		//		isMove = true;

		//		target = circlePosObjects[option_OrdinalNum - 1];

		//	}
		//}

		//if (isMove && !isDead)
		//{
		//	float step = moveSpeed * Time.deltaTime;

		//	transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);
		//	if (transform.position == target.transform.position)
		//	{
		//		isMove = false;
		//		step = 0;
		//	}
		//}

		//if (isCircle && !isMove && !isDead)
		//{
		//	transform.position = target.transform.position;
		//}

		//if (isFixed && !isMove && !isDead)
		//{
		//	transform.position = target.transform.position;
		//}

		//フォーメーション切り替え🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲

		//追従位置を取得していtたらその位置にする
		if (followPosObj && !isCircle && !isFixed && !isMove)
		{
			//transform.position = followPosObj.transform.position;
			transform.position = target.transform.position;
		}


		//円移動🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲
		//if (Input.GetKeyDown(KeyCode.C))
		//{
		//	//オンならオフに
		//	if (isCircle)
		//	{
		//		isCircle = false;
		//		isMove = true;
		//		target = followPosObj;
		//	}
		//	else
		//	{
		//		isCircle = true;
		//		isMove = true;

		//		target = circlePosObjects[option_OrdinalNum - 1];

		//	}
		//}

		//if (isMove && !isDead)
		//{
		//	float step = moveSpeed * Time.deltaTime;

		//	transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);
		//	if (transform.position == target.transform.position)
		//	{
		//		isMove = false;
		//	}
		//}

		//円移動🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲

		//固定位置🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲
		//if (Input.GetKeyDown(KeyCode.F))
		//{
		//	//オンならオフに
		//	if (isFixed)
		//	{
		//		isFixed = false;
		//		isCircle = false;
		//		isMove = true;
		//		target = followPosObj;
		//		transform.rotation = Quaternion.Euler(0, 0, 0);
		//	}
		//	else
		//	{
		//		isFixed = true;
		//		isMove = true;

		//		target = fixedPosObjects[option_OrdinalNum - 1];

		//	}
		//}

		//固定位置🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲🔲


		//プレイヤー死亡時の処理
		if (bState == BitState.Player1)
		{
			if (Input.GetKeyDown(KeyCode.I) || pl1.Dead_Check())
			{
				//死んだ判定true
				isDead = true;
				b_Shot.isShot = false;
				b_Shot.laser_Obj.SetActive(false);

				//追従位置の参照を外す
				followPosObj = null;
				target = null;

				//追従位置番号に合った追従位置オブジェクトのオプションを持っている判定をfalseにする
				switch (option_OrdinalNum)
				{
					case 1:
						FtoPlayer.hasOption = false;
						followPositions_Script.firstOption = null;
						break;

					case 2:
						FtoPBit_Second.hasOption = false;
						followPositions_Script.secondOption = null;
						break;

					case 3:
						FtoPBit_Third.hasOption = false;
						followPositions_Script.thirdOption = null;
						break;

					case 4:
						FtoPBit_Fourth.hasOption = false;
						followPositions_Script.fourthOption = null;
						break;
				}
				//option_OrdinalNum = 0;

			}
		}
		else if (bState == BitState.Player2)
		{
			if ((Input.GetKeyDown(KeyCode.I) || pl2.Dead_Check()) && !isStolen)
			{
				//死んだ判定true
				isDead = true;
				b_Shot.isShot = false;
				b_Shot.laser_Obj.SetActive(false);

				//追従位置の参照を外す
				followPosObj = null;

				//追従位置番号に合った追従位置オブジェクトのオプションを持っている判定をfalseにする
				switch (option_OrdinalNum)
				{
					case 1:
						FtoPlayer.hasOption = false;
						followPositions_Script.firstOption = null;
						break;

					case 2:
						FtoPBit_Second.hasOption = false;
						followPositions_Script.secondOption = null;
						break;

					case 3:
						FtoPBit_Third.hasOption = false;
						followPositions_Script.thirdOption = null;
						break;

					case 4:
						FtoPBit_Fourth.hasOption = false;
						followPositions_Script.fourthOption = null;
						break;
				}
			}
		}

		if (isFirstStolen && !isStolenSetting)
		{
			target = followPositions_Script.huntPos[huntMoveNum - 1];

			isStolenSetting = true;

			//switch (option_OrdinalNum)
			//{
			//	case 1:
			//		FtoPlayer.hasOption = false;
			//		followPositions_Script.firstOption = null;
			//		break;

			//	case 2:
			//		FtoPBit_Second.hasOption = false;
			//		followPositions_Script.secondOption = null;
			//		break;

			//	case 3:
			//		FtoPBit_Third.hasOption = false;
			//		followPositions_Script.thirdOption = null;
			//		break;

			//	case 4:
			//		FtoPBit_Fourth.hasOption = false;
			//		followPositions_Script.fourthOption = null;
			//		break;
			//}
		}
		//}
		//盗まれている状態かチェック
		if (!isStolen)
		{
			StolenCheck();
		}
		//盗まれた時の処理
		else if (isStolen)
		{
			if (!isStolenSetting)
			{
				target = followPositions_Script.huntPos[huntMoveNum - 1];
				b_Shot.isShot = false;
				b_Shot.laser_Obj.SetActive(false);
				//followPosObj = null;

				switch (option_OrdinalNum)
				{
					case 1:
						FtoPlayer.hasOption = false;
						followPositions_Script.firstOption = null;
						break;

					case 2:
						FtoPBit_Second.hasOption = false;
						followPositions_Script.secondOption = null;
						break;

					case 3:
						FtoPBit_Third.hasOption = false;
						followPositions_Script.thirdOption = null;
						break;

					case 4:
						FtoPBit_Fourth.hasOption = false;
						followPositions_Script.fourthOption = null;
						break;
				}

				if (bState == BitState.Player1)
				{
					if (pl1.bitIndex == option_OrdinalNum)
					{
						pl1.bitIndex -= huntNum;
					}
				}
				else if (bState == BitState.Player2)
				{
					if (pl2.bitIndex == option_OrdinalNum)
					{
						pl2.bitIndex -= huntNum;
					}
				}
				isStolenSetting = true;
			}
		}

		if (isHunterHit)
		{
			aaaa();
			isHunterHit = false;
		}


		//死んでいたら
		if (isDead)
		{
			//未回収状態の時の移動
			velocity = gameObject.transform.rotation * new Vector3(speed, 0, -0);
			gameObject.transform.position += velocity * Time.deltaTime;

			//初速からスピードを遅くする
			speed -= 0.5f;
			//スピードは-1.5よりは遅くならない
			if (speed < -1.5f)
			{
				speed = -1.5f;
			}
			//回収の当たり判定のディレイをプラス
			collectDelay++;
		}


		//オプションの移動関数呼び出し
		//Bit_Move();
		//----------------------------------------------
		//未回収状態で画面外に出たら、オフにする
		//if (!renderer.isVisible && isDead)
		//{
		//	isDead = false;					//死んでいる判定false
		//	isborn = true;					//出現時処理できるように
		//	followPosObj = null;			//追従オブジェクト参照をなくす
		//	pl1.bitIndex--;					//ゲームに出ているオプション総数カウントを減らす
		//	gameObject.SetActive(false);	//オブジェクトをオフにする
		//}
		//------------------------------------------------
	}
	//------------------Updata終わり------------------


	//------------------ここから関数------------------

	//追従するプレイヤーをセットして、追従位置も取得する関数
	public void SetPlayer(int playerNum)
	{
		if (playerNum == 1)
		{
			bState = BitState.Player1;

			followPositions_Script = GameObject.Find("Four_FollowPos_1P").GetComponent<FollowPositions>();

			//4つの追従位置とそれぞれのスクリプト取得
			followPosFirstObj = GameObject.Find("FollowPosFirst_1P");
			FtoPlayer = followPosFirstObj.GetComponent<FollowToPlayer_SameMotion>();

			followPosSecondObj = GameObject.Find("FollowPosSecond_1P");
			FtoPBit_Second = followPosSecondObj.GetComponent<FollowToPreviousBit>();

			followPosThirdObj = GameObject.Find("FollowPosThird_1P");
			FtoPBit_Third = followPosThirdObj.GetComponent<FollowToPreviousBit>();

			followPosFourthObj = GameObject.Find("FollowPosFourth_1P");
			FtoPBit_Fourth = followPosFourthObj.GetComponent<FollowToPreviousBit>();

		}
		else if (playerNum == 2)
		{
			bState = BitState.Player2;

			followPositions_Script = GameObject.Find("Four_FollowPos_2P").GetComponent<FollowPositions>();

			//4つの追従位置とそれぞれのスクリプト取得
			followPosFirstObj = GameObject.Find("FollowPosFirst_2P");
			FtoPlayer = followPosFirstObj.GetComponent<FollowToPlayer_SameMotion>();

			followPosSecondObj = GameObject.Find("FollowPosSecond_2P");
			FtoPBit_Second = followPosSecondObj.GetComponent<FollowToPreviousBit>();

			followPosThirdObj = GameObject.Find("FollowPosThird_2P");
			FtoPBit_Third = followPosThirdObj.GetComponent<FollowToPreviousBit>();

			followPosFourthObj = GameObject.Find("FollowPosFourth_2P");
			FtoPBit_Fourth = followPosFourthObj.GetComponent<FollowToPreviousBit>();
		}
	}

	//生成(画面に表示された時のポジション設定)
	void SetFollowPos()
	{
		//プレイヤーに一番近い追従オブジェクトのオプション所持判定がなかった時
		if (!FtoPlayer.hasOption)
		{
			option_OrdinalNum = 1;
			followPositions_Script.firstOption = this.gameObject;
			//オプションを所持判定をtrue,参照する追従位置オブジェクトを入れる,位置を更新
			FtoPlayer.hasOption = true;
			followPosObj = followPosFirstObj;

			if (isCircle)
			{
				target = circlePosObjects[option_OrdinalNum - 1];
				transform.position = target.transform.position;
				transform.rotation = Quaternion.Euler(0, 0, 0);
			}
			else if (isFixed)
			{
				target = fixedPosObjects[option_OrdinalNum - 1];
				transform.position = target.transform.position;
				transform.rotation = target.transform.rotation;
			}
			else
			{
				target = followPosObj;
				transform.position = followPosObj.transform.position;
				transform.rotation = Quaternion.Euler(0, 0, 0);
			}

			//transform.parent = followPosFirstObj.transform;
			//transform.position = followPosFirstObj.transform.position;

			//スピードリセット,オプションの追従位置番号設定,回収の当たり判定ディレイリセット
			speed = defaultSpeed;
			collectDelay = 0;
		}
		//二番目の追従オブジェクトのオプション所持判定がなかった時
		else if (!FtoPBit_Second.hasOption)
		{
			//自分の番号をいれる
			option_OrdinalNum = 2;
			//追従位置達の親に自分のオブジェクトをセットする
			followPositions_Script.secondOption = this.gameObject;
			//1つ前(1つ目)のオプションとそのスクリプト取得
			previousBitObj = followPositions_Script.firstOption;
			option_Script = previousBitObj.GetComponent<Bit_Formation_3>();
			//オプションを所持判定をtrue,参照する追従位置オブジェクトを入れる,位置を更新
			FtoPBit_Second.hasOption = true;
			followPosObj = followPosSecondObj;
			if (isCircle)
			{
				target = circlePosObjects[option_OrdinalNum - 1];
				transform.position = target.transform.position;
				transform.rotation = Quaternion.Euler(0, 0, 0);
			}
			else if (isFixed)
			{
				target = fixedPosObjects[option_OrdinalNum - 1];
				transform.position = target.transform.position;
				transform.rotation = target.transform.rotation;
			}
			else
			{
				target = followPosObj;
				transform.position = followPosObj.transform.position;
				transform.rotation = Quaternion.Euler(0, 0, 0);
			}

			//transform.parent = followPosSecondObj.transform;
			//transform.position = followPosSecondObj.transform.position;

			//スピードリセット,オプションの追従位置番号設定,回収の当たり判定ディレイリセット
			speed = defaultSpeed;
			collectDelay = 0;
		}
		//三番目の追従オブジェクトのオプション所持判定がなかった時
		else if (!FtoPBit_Third.hasOption)
		{
			option_OrdinalNum = 3;
			followPositions_Script.thirdOption = this.gameObject;
			//1つ前(2つ目)のオプションとそのスクリプト取得
			previousBitObj = followPositions_Script.secondOption;
			option_Script = previousBitObj.GetComponent<Bit_Formation_3>();
			//オプションを所持判定をtrue,参照する追従位置オブジェクトを入れる,位置を更新
			FtoPBit_Third.hasOption = true;
			followPosObj = followPosThirdObj;
			if (isCircle)
			{
				target = circlePosObjects[option_OrdinalNum - 1];
				transform.position = target.transform.position;
				transform.rotation = Quaternion.Euler(0, 0, 0);
			}
			else if (isFixed)
			{
				target = fixedPosObjects[option_OrdinalNum - 1];
				transform.position = target.transform.position;
				transform.rotation = target.transform.rotation;
			}
			else
			{
				target = followPosObj;
				transform.position = followPosObj.transform.position;
				transform.rotation = Quaternion.Euler(0, 0, 0);
			}

			//transform.parent = followPosThirdObj.transform;
			//transform.position = followPosThirdObj.transform.position;

			//スピードリセット,オプションの追従位置番号設定,回収の当たり判定ディレイリセット
			speed = defaultSpeed;
			collectDelay = 0;
		}
		//四番目の追従オブジェクトのオプション所持判定がなかった時
		else if (!FtoPBit_Fourth.hasOption)
		{
			option_OrdinalNum = 4;
			followPositions_Script.fourthOption = this.gameObject;
			//1つ前(3つ目)のオプションとそのスクリプト取得
			previousBitObj = followPositions_Script.thirdOption;
			option_Script = previousBitObj.GetComponent<Bit_Formation_3>();
			//オプションを所持判定をtrue,参照する追従位置オブジェクトを入れる,位置を更新
			FtoPBit_Fourth.hasOption = true;
			followPosObj = followPosFourthObj;
			if (isCircle)
			{
				target = circlePosObjects[option_OrdinalNum - 1];
				transform.position = target.transform.position;
				transform.rotation = Quaternion.Euler(0, 0, 0);
			}
			else if (isFixed)
			{
				target = fixedPosObjects[option_OrdinalNum - 1];
				transform.position = target.transform.position;
				transform.rotation = target.transform.rotation;
			}
			else
			{
				target = followPosObj;
				transform.position = followPosObj.transform.position;
				transform.rotation = Quaternion.Euler(0, 0, 0);
			}

			//transform.parent = followPosFourthObj.transform;
			//transform.position = followPosFourthObj.transform.position;

			//スピードリセット,オプションの追従位置番号設定,回収の当たり判定ディレイリセット
			speed = defaultSpeed;
			collectDelay = 0;
		}
	}


	//オプション回収の処理
	private void OnTriggerEnter(Collider col)
	{
		//死んでいる状態で、回収の当たり判定ディレイが10fより大きかったら
		if (isDead && collectDelay > 10)
		{
			//プレイヤー１のオブジェクトに当たったら
			if (col.gameObject.name == "Player")
			{
				SE_Manager.SE_Obj.Maltiple_Catch_SE(Obj_Storage.Storage_Data.audio_se[10]);
				//Voice_Manager.VOICE_Obj.Maltiple_Active_Voice(Obj_Storage.Storage_Data.audio_voice[16]);

				int i = 0;
				while (i < pl1.Maltiple_Catch.Length)
				{
					if (!pl1.Maltiple_Catch[i].isPlaying)
					{
						pl1.Maltiple_Catch[i].Play();
						break;
					}
					i++;
				}
				//オプションパーティクルストップ
				//option_Particle.Stop();
				b_Shot.isShot = true;

				//もともとプレイヤー1に追従していたら
				if (bState == BitState.Player1)
				{
					//プレイヤーに一番近い追従位置オブジェクトがオプションを持っていなかったら
					if (!FtoPlayer.hasOption)
					{
						//自分の番号セット
						option_OrdinalNum = 1;
						//取得判定true,一番近い位置オブジェクトのオプション所持判定true,参照する追従位置オブジェクト入れる,位置を更新
						isCollection = true;
						FtoPlayer.hasOption = true;
						followPosObj = followPosFirstObj;

						followPositions_Script.firstOption = this.gameObject;
						//1つ前(1つ目)のオプション情報を消す
						previousBitObj = null;
						option_Script = null;

						if (isCircle)
						{
							//target = circlePosObjects[option_OrdinalNum - 1];
							transform.position = target.transform.position;
							transform.rotation = Quaternion.Euler(0, 0, 0);
						}
						else if (isFixed)
						{
							//target = fixedPosObjects[option_OrdinalNum - 1];
							transform.position = target.transform.position;
							transform.rotation = target.transform.rotation;
						}

						target = followPosObj;
						transform.position = followPosObj.transform.position;
						transform.rotation = Quaternion.Euler(0, 0, 0);
						//死んでる状態false,スピードを初速にリセット,オプションの追従位置判別番号設定,当たり判定のディレイリセット
						isDead = false;
						speed = defaultSpeed;
						collectDelay = 0;

						//スケール変更スクリプトの回収判定true,回収時のスケール値を０
						os.isCollectInc = true;
						os.scale_Collect = 0;
					}
					//二番目の追従位置オブジェクトがオプションを持っていなかったら
					else if (!FtoPBit_Second.hasOption)
					{
						option_OrdinalNum = 2;
						//取得判定true,二番目に近い位置オブジェクトのオプション所持判定true,参照する追従位置オブジェクト入れる,位置を更新
						isCollection = true;
						FtoPBit_Second.hasOption = true;
						followPosObj = followPosSecondObj;

						followPositions_Script.secondOption = this.gameObject;
						//1つ前(1つ目)のオプションとそのスクリプト取得
						previousBitObj = followPositions_Script.firstOption;
						option_Script = previousBitObj.GetComponent<Bit_Formation_3>();

						if (isCircle)
						{
							//target = circlePosObjects[option_OrdinalNum - 1];
							transform.position = target.transform.position;
							transform.rotation = Quaternion.Euler(0, 0, 0);
						}
						else if (isFixed)
						{
							//target = fixedPosObjects[option_OrdinalNum - 1];
							transform.position = target.transform.position;
							transform.rotation = target.transform.rotation;
						}
						else
						{
							target = followPosObj;
							transform.position = followPosObj.transform.position;
							transform.rotation = Quaternion.Euler(0, 0, 0);
						}

						//死んでる状態false,スピードを初速にリセット,オプションの追従位置判別番号設定,当たり判定のディレイリセット
						isDead = false;
						speed = defaultSpeed;
						collectDelay = 0;

						//スケール変更スクリプトの回収判定true,回収時のスケール値を０
						os.isCollectInc = true;
						os.scale_Collect = 0;
					}
					//三番目の追従位置オブジェクトがオプションを持っていなかったら
					else if (!FtoPBit_Third.hasOption)
					{
						option_OrdinalNum = 3;
						//取得判定true,三番目に近い位置オブジェクトのオプション所持判定true,参照する追従位置オブジェクト入れる,位置を更新
						isCollection = true;
						FtoPBit_Third.hasOption = true;
						followPosObj = followPosThirdObj;

						followPositions_Script.thirdOption = this.gameObject;
						//1つ前(1つ目)のオプションとそのスクリプト取得
						previousBitObj = followPositions_Script.secondOption;
						option_Script = previousBitObj.GetComponent<Bit_Formation_3>();

						if (isCircle)
						{
							target = circlePosObjects[option_OrdinalNum - 1];
							transform.position = target.transform.position;
							transform.rotation = Quaternion.Euler(0, 0, 0);
						}
						else if (isFixed)
						{
							target = fixedPosObjects[option_OrdinalNum - 1];
							transform.position = target.transform.position;
							transform.rotation = target.transform.rotation;
						}
						else
						{
							target = followPosObj;
							transform.position = followPosObj.transform.position;
							transform.rotation = Quaternion.Euler(0, 0, 0);
						}

						//死んでる状態false,スピードを初速にリセット,オプションの追従位置判別番号設定,当たり判定のディレイリセット
						isDead = false;
						speed = defaultSpeed;
						collectDelay = 0;

						//スケール変更スクリプトの回収判定true,回収時のスケール値を０
						os.isCollectInc = true;
						os.scale_Collect = 0;
					}
					//四番目の追従位置オブジェクトがオプションを持っていなかったら
					else if (!FtoPBit_Fourth.hasOption)
					{
						option_OrdinalNum = 4;
						//取得判定true,四番目に近い位置オブジェクトのオプション所持判定true,参照する追従位置オブジェクト入れる,位置を更新
						isCollection = true;
						FtoPBit_Fourth.hasOption = true;
						followPosObj = followPosFourthObj;

						followPositions_Script.fourthOption = this.gameObject;
						//1つ前(1つ目)のオプションとそのスクリプト取得
						previousBitObj = followPositions_Script.thirdOption;
						option_Script = previousBitObj.GetComponent<Bit_Formation_3>();

						if (isCircle)
						{
							//target = circlePosObjects[option_OrdinalNum - 1];
							transform.position = target.transform.position;
							transform.rotation = Quaternion.Euler(0, 0, 0);
						}
						else if (isFixed)
						{
							//target = fixedPosObjects[option_OrdinalNum - 1];
							transform.position = target.transform.position;
							transform.rotation = target.transform.rotation;
						}
						else
						{
							target = followPosObj;
							transform.position = followPosObj.transform.position;
							transform.rotation = Quaternion.Euler(0, 0, 0);
						}

						//死んでる状態false,スピードを初速にリセット,オプションの追従位置判別番号設定,当たり判定のディレイリセット
						isDead = false;
						speed = defaultSpeed;
						collectDelay = 0;

						//スケール変更スクリプトの回収判定true,回収時のスケール値を０
						os.isCollectInc = true;
						os.scale_Collect = 0;
					}
				}
				//プレイヤー2に追従していたら
				else if (bState == BitState.Player2)
				{
					//追従するプレイヤーをプレイヤー1に変更
					SetPlayer(1);
					//プレイヤーに一番近い追従位置オブジェクトがオプションを持っていなかったら
					if (!FtoPlayer.hasOption)
					{
						//取得判定true,一番近い位置オブジェクトのオプション所持判定true,参照する追従位置オブジェクト入れる,位置を更新
						isCollection = true;
						FtoPlayer.hasOption = true;
						followPosObj = followPosFirstObj;
						transform.position = followPosObj.transform.position;

						//死んでる状態false,スピードを初速にリセット,オプションの追従位置判別番号設定,当たり判定のディレイリセット
						isDead = false;
						speed = defaultSpeed;
						option_OrdinalNum = 1;
						collectDelay = 0;

						//スケール変更スクリプトの回収判定true,回収時のスケール値を０
						os.isCollectInc = true;
						os.scale_Collect = 0;
					}
					//二番目の追従位置オブジェクトがオプションを持っていなかったら
					else if (!FtoPBit_Second.hasOption)
					{
						//取得判定true,二番目に近い位置オブジェクトのオプション所持判定true,参照する追従位置オブジェクト入れる,位置を更新
						isCollection = true;
						FtoPBit_Second.hasOption = true;
						followPosObj = followPosSecondObj;
						transform.position = followPosObj.transform.position;

						//死んでる状態false,スピードを初速にリセット,オプションの追従位置判別番号設定,当たり判定のディレイリセット
						isDead = false;
						speed = defaultSpeed;
						option_OrdinalNum = 2;
						collectDelay = 0;

						//スケール変更スクリプトの回収判定true,回収時のスケール値を０
						os.isCollectInc = true;
						os.scale_Collect = 0;
					}
					//三番目の追従位置オブジェクトがオプションを持っていなかったら
					else if (!FtoPBit_Third.hasOption)
					{
						//取得判定true,三番目に近い位置オブジェクトのオプション所持判定true,参照する追従位置オブジェクト入れる,位置を更新
						isCollection = true;
						FtoPBit_Third.hasOption = true;
						followPosObj = followPosThirdObj;
						transform.position = followPosObj.transform.position;

						//死んでる状態false,スピードを初速にリセット,オプションの追従位置判別番号設定,当たり判定のディレイリセット
						isDead = false;
						speed = defaultSpeed;
						option_OrdinalNum = 3;
						collectDelay = 0;

						//スケール変更スクリプトの回収判定true,回収時のスケール値を０
						os.isCollectInc = true;
						os.scale_Collect = 0;
					}
					//四番目の追従位置オブジェクトがオプションを持っていなかったら
					else if (!FtoPBit_Fourth.hasOption)
					{
						//取得判定true,四番目に近い位置オブジェクトのオプション所持判定true,参照する追従位置オブジェクト入れる,位置を更新
						isCollection = true;
						FtoPBit_Fourth.hasOption = true;
						followPosObj = followPosFourthObj;
						transform.position = followPosObj.transform.position;

						//死んでる状態false,スピードを初速にリセット,オプションの追従位置判別番号設定,当たり判定のディレイリセット
						isDead = false;
						speed = defaultSpeed;
						option_OrdinalNum = 4;
						collectDelay = 0;

						//スケール変更スクリプトの回収判定true,回収時のスケール値を０
						os.isCollectInc = true;
						os.scale_Collect = 0;
					}

				}
			}
			//プレイヤー2に当たったら
			else if (col.gameObject.name == "Player_2")
			{
				//オプションパーティクルストップ
				//option_Particle.Stop();
				b_Shot.isShot = true;
				SE_Manager.SE_Obj.Maltiple_Catch_SE(Obj_Storage.Storage_Data.audio_se[10]);
				//Voice_Manager.VOICE_Obj.Maltiple_Active_Voice(Obj_Storage.Storage_Data.audio_voice[16]);

				int i = 0;
				while (i < pl2.Maltiple_Catch.Length)
				{
					if (!pl2.Maltiple_Catch[i].isPlaying)
					{
						pl2.Maltiple_Catch[i].Play();
						break;
					}
					i++;
				}
				//プレイヤー1を追従していたら
				if (bState == BitState.Player1)
				{
					//追従するプレイヤーをプレイヤー1に変更
					SetPlayer(2);

					//プレイヤーに一番近い追従位置オブジェクトがオプションを持っていなかったら
					if (!FtoPlayer.hasOption)
					{
						//取得判定true,一番近い位置オブジェクトのオプション所持判定true,参照する追従位置オブジェクト入れる,位置を更新
						isCollection = true;
						FtoPlayer.hasOption = true;
						followPosObj = followPosFirstObj;
						transform.position = followPosObj.transform.position;

						//死んでる状態false,スピードを初速にリセット,オプションの追従位置判別番号設定,当たり判定のディレイリセット
						isDead = false;
						speed = defaultSpeed;
						option_OrdinalNum = 1;
						collectDelay = 0;

						//スケール変更スクリプトの回収判定true,回収時のスケール値を０
						os.isCollectInc = true;
						os.scale_Collect = 0;
					}
					//二番目の追従位置オブジェクトがオプションを持っていなかったら
					else if (!FtoPBit_Second.hasOption)
					{
						//取得判定true,二番目に近い位置オブジェクトのオプション所持判定true,参照する追従位置オブジェクト入れる,位置を更新
						isCollection = true;
						FtoPBit_Second.hasOption = true;
						followPosObj = followPosSecondObj;
						transform.position = followPosObj.transform.position;

						//死んでる状態false,スピードを初速にリセット,オプションの追従位置判別番号設定,当たり判定のディレイリセット
						isDead = false;
						speed = defaultSpeed;
						option_OrdinalNum = 2;
						collectDelay = 0;

						//スケール変更スクリプトの回収判定true,回収時のスケール値を０
						os.isCollectInc = true;
						os.scale_Collect = 0;
					}
					//三番目の追従位置オブジェクトがオプションを持っていなかったら
					else if (!FtoPBit_Third.hasOption)
					{
						//取得判定true,三番目に近い位置オブジェクトのオプション所持判定true,参照する追従位置オブジェクト入れる,位置を更新
						isCollection = true;
						FtoPBit_Third.hasOption = true;
						followPosObj = followPosThirdObj;
						transform.position = followPosObj.transform.position;

						//死んでる状態false,スピードを初速にリセット,オプションの追従位置判別番号設定,当たり判定のディレイリセット
						isDead = false;
						speed = defaultSpeed;
						option_OrdinalNum = 3;
						collectDelay = 0;

						//スケール変更スクリプトの回収判定true,回収時のスケール値を０
						os.isCollectInc = true;
						os.scale_Collect = 0;
					}
					//四番目の追従位置オブジェクトがオプションを持っていなかったら
					else if (!FtoPBit_Fourth.hasOption)
					{
						//取得判定true,四番目に近い位置オブジェクトのオプション所持判定true,参照する追従位置オブジェクト入れる,位置を更新
						isCollection = true;
						FtoPBit_Fourth.hasOption = true;
						followPosObj = followPosFourthObj;
						transform.position = followPosObj.transform.position;

						//死んでる状態false,スピードを初速にリセット,オプションの追従位置判別番号設定,当たり判定のディレイリセット
						isDead = false;
						speed = defaultSpeed;
						option_OrdinalNum = 4;
						collectDelay = 0;

						//スケール変更スクリプトの回収判定true,回収時のスケール値を０
						os.isCollectInc = true;
						os.scale_Collect = 0;
					}

				}
				else if (bState == BitState.Player2)
				{
					//プレイヤーに一番近い追従位置オブジェクトがオプションを持っていなかったら
					if (!FtoPlayer.hasOption)
					{
						//取得判定true,一番近い位置オブジェクトのオプション所持判定true,参照する追従位置オブジェクト入れる,位置を更新
						isCollection = true;
						FtoPlayer.hasOption = true;
						followPosObj = followPosFirstObj;
						transform.position = followPosObj.transform.position;

						//死んでる状態false,スピードを初速にリセット,オプションの追従位置判別番号設定,当たり判定のディレイリセット
						isDead = false;
						speed = defaultSpeed;
						option_OrdinalNum = 1;
						collectDelay = 0;

						//スケール変更スクリプトの回収判定true,回収時のスケール値を０
						os.isCollectInc = true;
						os.scale_Collect = 0;
					}
					//二番目の追従位置オブジェクトがオプションを持っていなかったら
					else if (!FtoPBit_Second.hasOption)
					{
						//取得判定true,二番目に近い位置オブジェクトのオプション所持判定true,参照する追従位置オブジェクト入れる,位置を更新
						isCollection = true;
						FtoPBit_Second.hasOption = true;
						followPosObj = followPosSecondObj;
						transform.position = followPosObj.transform.position;

						//死んでる状態false,スピードを初速にリセット,オプションの追従位置判別番号設定,当たり判定のディレイリセット
						isDead = false;
						speed = defaultSpeed;
						option_OrdinalNum = 2;
						collectDelay = 0;

						//スケール変更スクリプトの回収判定true,回収時のスケール値を０
						os.isCollectInc = true;
						os.scale_Collect = 0;
					}
					//三番目の追従位置オブジェクトがオプションを持っていなかったら
					else if (!FtoPBit_Third.hasOption)
					{
						//取得判定true,三番目に近い位置オブジェクトのオプション所持判定true,参照する追従位置オブジェクト入れる,位置を更新
						isCollection = true;
						FtoPBit_Third.hasOption = true;
						followPosObj = followPosThirdObj;
						transform.position = followPosObj.transform.position;

						//死んでる状態false,スピードを初速にリセット,オプションの追従位置判別番号設定,当たり判定のディレイリセット
						isDead = false;
						speed = defaultSpeed;
						option_OrdinalNum = 3;
						collectDelay = 0;

						//スケール変更スクリプトの回収判定true,回収時のスケール値を０
						os.isCollectInc = true;
						os.scale_Collect = 0;
					}
					//四番目の追従位置オブジェクトがオプションを持っていなかったら
					else if (!FtoPBit_Fourth.hasOption)
					{
						//取得判定true,四番目に近い位置オブジェクトのオプション所持判定true,参照する追従位置オブジェクト入れる,位置を更新
						isCollection = true;
						FtoPBit_Fourth.hasOption = true;
						followPosObj = followPosFourthObj;
						transform.position = followPosObj.transform.position;

						//死んでる状態false,スピードを初速にリセット,オプションの追従位置判別番号設定,当たり判定のディレイリセット
						isDead = false;
						speed = defaultSpeed;
						option_OrdinalNum = 4;
						collectDelay = 0;

						//スケール変更スクリプトの回収判定true,回収時のスケール値を０
						os.isCollectInc = true;
						os.scale_Collect = 0;
					}

				}
			}
			//🔲🔲🔲🔲🔲🔲🔲🔲🔲　死亡処理　🔲🔲🔲🔲🔲🔲🔲🔲🔲
			else if (col.gameObject.name == "WallLeft")
			{
				isDead = false;						//死んでいる判定false
				isborn = true;						//出現時処理できるように
				followPosObj = null;				//追従オブジェクト参照をなくす
				pl1.bitIndex--;						//ゲームに出ているオプション総数カウントを減らす

				gameObject.SetActive(false);		//オブジェクトをオフにする

			}
		}

		//右の画面外壁に当たったとき（盗まれた後に当たったとき用）
		if (col.gameObject.name == "WallRight")
		{
			if (isStolen || isFirstStolen)
			{
				isborn = true;

				followPosObj = null;                //追従オブジェクト参照をなくす
				isFirstStolen = false;
				isStolen = false;
				isStolenSetting = false;
				if (option_OrdinalNum == 4)
				{

				}
				gameObject.SetActive(false);        //オブジェクトをオフにする
			}
		}

		//オプションハンターに当たった時
		//if (!isDead && col.gameObject.tag == "Hunter")
		//{
		//	if (col.gameObject.GetComponent<OptionHunter>().isHunt == false)
		//	{
		//		col.gameObject.GetComponent<OptionHunter>().isHunt = true;
		//		isFirstStolen = true;
		//		b_Shot.isShot = false;
		//		b_Shot.laser_Obj.SetActive(false);

		//		huntMoveNum = 1;

		//		//target = followPositions_Script.huntPos[huntMoveNum - 1];
				
		//		if (bState == Bit_Formation_3.BitState.Player1)
		//		{
		//			//盗まれた数＝プレイヤーのオプション所持数ー自分の番号＋1
		//			huntNum = pl1.bitIndex - option_OrdinalNum + 1;
		//		}
		//		else if (bState == Bit_Formation_3.BitState.Player2)
		//		{
		//			//盗まれた数＝プレイヤーのオプション所持数ー自分の番号＋1
		//			huntNum = pl2.bitIndex - option_OrdinalNum + 1;
		//		}

		//		switch (option_OrdinalNum)
		//		{
		//			case 1:
		//				FtoPlayer.hasOption = false;
		//				followPositions_Script.firstOption = null;
		//				break;

		//			case 2:
		//				FtoPBit_Second.hasOption = false;
		//				followPositions_Script.secondOption = null;
		//				break;

		//			case 3:
		//				FtoPBit_Third.hasOption = false;
		//				followPositions_Script.thirdOption = null;
		//				break;

		//			case 4:
		//				FtoPBit_Fourth.hasOption = false;
		//				followPositions_Script.fourthOption = null;
		//				break;
		//		}
		//	}
		//}
	}

	//盗まれている状態かチェックする
	void StolenCheck()
	{
		if (option_OrdinalNum != 1)
		{
			//一つ前のオプションが盗まれている状態だったら
			if (option_Script.isFirstStolen || option_Script.isStolen)
			{
				huntNum = option_Script.huntNum;
				if (bState == Bit_Formation_3.BitState.Player1)
				{
					//自分の移動位置＝自分の番号－プレイヤーの所持数＋盗まれた数
					huntMoveNum = option_OrdinalNum - pl1.bitIndex + huntNum;
				}
				else if (bState == Bit_Formation_3.BitState.Player2)
				{
					//自分の移動位置＝自分の番号－プレイヤーの所持数＋盗まれた数
					huntMoveNum = option_OrdinalNum - pl2.bitIndex + huntNum;

				}

				//元の追従位置のオプション所持判定をなくす
				switch (option_OrdinalNum)
				{
					case 1:
						FtoPlayer.hasOption = false;
						break;

					case 2:
						FtoPBit_Second.hasOption = false;
						break;

					case 3:
						FtoPBit_Third.hasOption = false;
						break;

					case 4:
						FtoPBit_Fourth.hasOption = false;
						break;
				}

				isStolen = true;

			}
		}
	}
	void aaaa()
	{
		isFirstStolen = true;
		b_Shot.isShot = false;
		b_Shot.laser_Obj.SetActive(false);

		huntMoveNum = 1;

		//target = followPositions_Script.huntPos[huntMoveNum - 1];

		if (bState == Bit_Formation_3.BitState.Player1)
		{
			//盗まれた数＝プレイヤーのオプション所持数ー自分の番号＋1
			huntNum = pl1.bitIndex - option_OrdinalNum + 1;
		}
		else if (bState == Bit_Formation_3.BitState.Player2)
		{
			//盗まれた数＝プレイヤーのオプション所持数ー自分の番号＋1
			huntNum = pl2.bitIndex - option_OrdinalNum + 1;
		}

		switch (option_OrdinalNum)
		{
			case 1:
				FtoPlayer.hasOption = false;
				followPositions_Script.firstOption = null;
				break;

			case 2:
				FtoPBit_Second.hasOption = false;
				followPositions_Script.secondOption = null;
				break;

			case 3:
				FtoPBit_Third.hasOption = false;
				followPositions_Script.thirdOption = null;
				break;

			case 4:
				FtoPBit_Fourth.hasOption = false;
				followPositions_Script.fourthOption = null;
				break;
		}
	}
}