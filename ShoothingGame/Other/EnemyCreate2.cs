using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCreate2 : MonoBehaviour
{
	public enum EnemyName
	{
		Discharge,
		Dhischarged,
		FollowGround,
		Walk,
		Taiho,
	}

	[Header("生成する敵　上から生成")]
	public EnemyName[] enemyName;

	GameObject dischargeObj;
	GameObject dischargedObj;
	GameObject followGroundObj;
	GameObject walkObj;
	GameObject taihoObj;

	//[Header("生成する敵の種類の数値だけ入れてください")]
	//public GameObject[] createObjects;

	[Header("生成する位置")]
	public Vector3[] createPositions;

	[Header("生成時のRotation")]
	public Quaternion[] createRotation;

	GameObject saveObj;		//一時保存用

	[Header("敵排出型を出す場合の排出タイプ設定")]
	public Enemy_Discharged.MoveType[] movetype;
	Enemy_Discharge discharge_Script;

	int createNum;			//出すオブジェクトの番号
	int moveTypeNum;        //敵排出の敵の動き番号

	bool isDischargeRoad = false;
	bool isDischargedRoad = false;
	bool isFollowGroundRoad = false;
	bool isWalkRoad = false;
	bool isTaihoRoad = false;


    void Start()
    {
		createNum = 0;
    }

    void Update()
    {
		//生成関数
		Create();
    }

	void Create()
	{
		//switch(createNum)
		//{
		//	case 0:

		//		break;

		//	case 1:

		//		break;

		//	case 2:

		//		break;

		//	case 3:

		//		break;

		//	case 4:

		//		break;

		//	case 5:

		//		break;

		//	case 6:

		//		break;

		//	case 7:

		//		break;
		//}

		//次に生成する敵を見て、ロードしていなかったらロードする
		switch (enemyName[createNum])
		{
			case EnemyName.Discharge:
				if (!isDischargeRoad)
				{
					dischargeObj = Resources.Load("Enemy2/Enemy_Discharge") as GameObject;
					isDischargeRoad = true;
				}
				saveObj = Instantiate(dischargeObj, createPositions[createNum], createRotation[createNum]);
				saveObj.transform.parent = transform;
				saveObj.transform.localPosition = createPositions[createNum];

				discharge_Script = saveObj.GetComponent<Enemy_Discharge>();
				discharge_Script.setMoveType = movetype[moveTypeNum];
				moveTypeNum++;

				break;

			case EnemyName.Dhischarged:
				if (!isDischargedRoad)
				{
					dischargedObj = Resources.Load("Enemy2/Enemy_Discharged") as GameObject;
					isDischargedRoad = true;
				}
				saveObj = Instantiate(dischargedObj, createPositions[createNum], createRotation[createNum]);
				saveObj.transform.parent = transform;
				saveObj.transform.localPosition = createPositions[createNum];

				break;

			case EnemyName.FollowGround:
				if (!isFollowGroundRoad)
				{
					followGroundObj = Resources.Load("Enemy2/Enemy_FollowGround") as GameObject;
					isFollowGroundRoad = true;
				}
				saveObj = Instantiate(followGroundObj, createPositions[createNum], createRotation[createNum]);
				saveObj.transform.parent = transform;
				saveObj.transform.localPosition = createPositions[createNum];

				break;

			case EnemyName.Walk:
				if (!isWalkRoad)
				{
					walkObj = Resources.Load("Enemy2/Enemy_Walk") as GameObject;
					isWalkRoad = true;
				}
				saveObj = Instantiate(walkObj, createPositions[createNum], createRotation[createNum]);
				saveObj.transform.parent = transform;
				saveObj.transform.localPosition = createPositions[createNum];

				break;

			case EnemyName.Taiho:
				if (!isTaihoRoad)
				{
					taihoObj = Resources.Load("Enemy2/Enemy_Taiho") as GameObject;
					isTaihoRoad = true;
				}
				saveObj = Instantiate(taihoObj, createPositions[createNum], createRotation[createNum]);
				saveObj.transform.parent = transform;
				saveObj.transform.localPosition = createPositions[createNum];

				break;
		}

		saveObj = null;
		createNum++;

	}
}
