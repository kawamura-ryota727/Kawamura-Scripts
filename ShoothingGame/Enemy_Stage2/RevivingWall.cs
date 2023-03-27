//作成者：川村良太
//壊れてからまた復活する壁


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevivingWall : character_status
{
	public enum State
	{
		Alive,
		Dead,
		Revive,
	}

	public State state;

	public Collider myCollider;

	public float hpp;
	float defaultHp;
	public float scale_Value;
	[Header("入力用　大きさを変化させる値")]
	public float scale_ChangeValue;

	[Header("入力用　死んでいる最大時間　秒")]
	public float deadTimeMax;
	public float deadTimeCnt;

	bool isDead;
	bool isRevive;

	new void Start()
    {
		defaultHp = hpp;
		scale_Value = 1;
		deadTimeCnt = 0;
		base.Start();
    }

    new void Update()
    {
		transform.localScale = new Vector3(scale_Value, scale_Value, scale_Value);

		hpp = hp;
		if (hp < 1)
		{
			state = State.Dead;
			myCollider.enabled = false;
			Reset_Status();
		}

		switch(state)
		{
			case State.Alive:
				break;
			case State.Dead:
				scale_Value -= scale_ChangeValue;
				if (scale_Value < 0)
				{
					scale_Value = 0;
					state = State.Revive;
				}
				break;
			case State.Revive:

				if (isRevive)
				{
					scale_Value += scale_ChangeValue;
					if (scale_Value > 1)
					{
						scale_Value = 1;
						isRevive = false;
						myCollider.enabled = true;
						state = State.Alive;
					}
				}
				else
				{
					deadTimeCnt += Time.deltaTime;
					if (deadTimeCnt > deadTimeMax)
					{
						isRevive = true;
						//Reset_Status();
						deadTimeCnt = 0;
					}
				}

				break;
		}

		base.Update();
    }
}
