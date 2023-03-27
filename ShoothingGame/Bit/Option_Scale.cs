﻿//作成者：川村良太
//オプションのパーティクルの見た目の大きさ変更スクリプト
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Option_Scale : MonoBehaviour
{
	Bit_Formation_3 bf;

	public int scaleDelay;
	public float scale_value;
	public float scale_Collect;

	public bool isScaleInc = false;
	public bool isScaleDec=false;
	public bool isCollectInc = true;
	public bool isStageBeginning = false;

	public bool isReset = false;
	// Start is called before the first frame update

	private void Awake()
	{
		bf = transform.parent.gameObject.GetComponent<Bit_Formation_3>();
	}
	void Start()
    {
		scale_value = 1.5f;
		scale_Collect = 0;
		isScaleInc = true;
		transform.localScale = new Vector3(scale_Collect, scale_Collect, 0);

		//isScaleInc = true;
	}

	// Update is called once per frame
	void Update()
    {
		//オプションの縮小
		scaleDelay++;
		//回収された後の大きくなる処理
		if (isCollectInc)
		{
			scale_Collect += 0.1f;
			if(scale_Collect>1.5f)
			{
				scale_Collect = 1.5f;
				scale_value = 1.5f;
				isScaleDec = true;
				isScaleInc = false;
				isCollectInc = false;
				isReset = false;
			}
			transform.localScale = new Vector3(scale_Collect, scale_Collect, 0);
		}
		else if(scaleDelay > 5)
		{
			if (!isStageBeginning)
			{
				if (isScaleInc)
				{
					scale_value += 0.2f;
					if (scale_value > 1.5f)
					{
						scale_value = 1.5f;
						isScaleInc = false;
						isScaleDec = true;
					}
				}
				else if (isScaleDec)
				{
					scale_value -= 0.2f;
					if (scale_value < 1.1f)
					{
						scale_value = 1.1f;
						isScaleDec = false;
						isScaleInc = true;
					}
				}
			}

			//scale_value = Mathf.Sin(Time.frameCount) / 12.5f + 0.42f;
			transform.localScale = new Vector3(scale_value, scale_value, 0);
			scaleDelay = 0;
		}

		if (!bf.isDead && bf.pl1.Is_Resporn)
		{
			transform.localScale = new Vector3(0, 0, 0);
			//scale_value = 0;
			scale_Collect = 0;
			isStageBeginning = true;
			isReset = true;
		}
		else
		{
			if (isReset)
			{
				isStageBeginning = false;
				//isScaleInc = true;
				//isScaleDec = false;
				isCollectInc = true;
				isReset = false;
			}
		}

		if (bf.isCollection)
		{
			bf.isCollection = false;

			scale_Collect = 0;
			transform.localScale = new Vector3(scale_Collect, scale_Collect, 0);
			isCollectInc = true;
		}
	}
}
