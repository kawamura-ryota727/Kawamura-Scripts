using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleWall : MonoBehaviour
{
	Vector3 velocity;
	Vector3 defaultPos;

	public float upSpeed;			//上昇速度
	public float upSpeed_Max;		//最大上昇速度
	public float downSpeed;			//下降速度
	public float downSpeed_Max;		//最大下降速度

	float speed;					//上下のスピード
	
	float defaultPosY;				//初期ポジションY

	float delayCnt;					//攻撃が当たっていない間のカウント
	public float delayMax;			//攻撃が当たってなかったときにまた上昇し始める時間

	bool isHit = false;
	bool isUpEnd = false;
    void Start()
    {
		//初期位置代入
		defaultPos = transform.position;
		defaultPosY = transform.position.y;

		delayCnt = 0;
    }

    void Update()
    {
		//弾に当たったら
		if (isHit)
		{
			//velocity = gameObject.transform.rotation * new Vector3(0, downSpeed, 0);
			//gameObject.transform.position += velocity * Time.deltaTime;
			velocity = gameObject.transform.rotation * new Vector3(0, speed, 0);
			gameObject.transform.position += velocity * Time.deltaTime;

			delayCnt += Time.deltaTime;
		}
		else if (!isUpEnd && !isHit)
		{
			speed += 0.2f;
			//upSpeed += 0.2f;
			//if (upSpeed > upSpeed_Max)
			//{
			//	upSpeed = upSpeed_Max;
			//}
			if (speed > upSpeed_Max)
			{
				speed = upSpeed_Max;
			}

			//velocity = gameObject.transform.rotation * new Vector3(0, upSpeed, 0);
			//gameObject.transform.position += velocity * Time.deltaTime;
			velocity = gameObject.transform.rotation * new Vector3(0, speed, 0);
			gameObject.transform.position += velocity * Time.deltaTime;

			if (transform.position.y > defaultPosY)
			{
				transform.position = defaultPos;
				isUpEnd = true;
			}
		}
		else
		{
			//なにもしない
		}

		if (delayCnt > delayMax)
		{
			isHit = false;
		}

	}

	private void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == "Player_Bullet")
		{
			isHit = true;
			isUpEnd = false;
			speed = downSpeed_Max;
			//upSpeed = 0;
			delayCnt = 0;
		}
	}
}
