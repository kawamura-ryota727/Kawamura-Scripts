//作成者：川村良太
//作成日：2019/10/04
//スマホをランダムな位置にする処理
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSmartPhone : MonoBehaviour
{
	[Header("入力用　Xのランダム出現範囲の最大値")]
	public float randomX_ValueMax;
	[Header("入力用　Xのランダム出現範囲の最小値")]
	public float randomX_ValueMin;
	[Header("入力用　Yのランダム出現範囲の最大値")]
	public float randomY_ValueMax;
	[Header("入力用　Yのランダム出現範囲の最小値")]
	public float randomY_ValueMin;

	public int positiveORnegative;		//最小値が正か負を決める　1なら正2なら負

	public float posX;
	public float posY;

    void Start()
    {
		positiveORnegative = Random.Range(1, 3);

		//if (positiveORnegative == 1)
		//{
		//	posX = Random.Range(randomX_ValueMin, randomX_ValueMax);
		//	posY = Random.Range(randomY_ValueMin, randomY_ValueMax);
		//}
		//else if (positiveORnegative == 2)
		//{
		//	posX = Random.Range(-randomX_ValueMax, -randomX_ValueMin);
		//	posY = Random.Range(-randomY_ValueMax, -randomY_ValueMin);
		//}

		posX = Random.Range(randomX_ValueMin, randomX_ValueMax);
		posY = Random.Range(randomY_ValueMin, randomY_ValueMax);

		transform.position = new Vector3(posX, posY, 0);
    }

    void Update()
    {
        
    }
}
