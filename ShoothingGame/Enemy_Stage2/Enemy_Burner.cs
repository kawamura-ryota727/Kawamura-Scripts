using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Burner : MonoBehaviour
{
	public GameObject createObj;
	public Vector3 createPosition;

	[Header("入力用　出す間隔秒")]
	public float create_Interval;
	float intervalCnt;


	void Start()
    {
		
		intervalCnt = 0;
    }

    
    void Update()
    {
		intervalCnt += Time.deltaTime;

		if (intervalCnt > create_Interval)
		{
			Instantiate(createObj, createPosition, transform.rotation);
			intervalCnt = 0;
		}
    }
}
