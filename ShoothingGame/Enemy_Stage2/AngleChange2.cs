using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleChange2 : MonoBehaviour
{
	public GameObject parentObj;
	public BoxCollider hitCol;

	public FollowGround3 followGround_Script;
	public float angleZ;
	public float angleZ_ChangeValue;


	//
	RaycastHit hit; //ヒットしたオブジェクト情報
	RaycastHit hit2;
	public Vector3 angleTest;
	public Vector3 rayDirection;
	public Vector3 checkNomal;
	//

	Vector3 raypos; //レイの位置がずれるから調整用
	public Vector3 kakuninVector;
	Vector3 underRayPos;
	Vector3 sideRayPos;
	public Vector3 sideRayVector;
	public float groundAngle;
	public float angle_Sin;
	public float angle_Cos;

	public int raydasitayo = 0;

	//
	public int colDelayMax = 0;
	public int colDelayCnt = 0;
	public bool colHit = false;

	//

	public string tagname;

	public bool aaa = false;
	public bool bbb = false;
	public bool ccc = false;
	public bool ddd = false;
	public bool eee = false;
	public bool fff = false;
	public bool ggg = false;
	public bool hhh = false;

	public bool hithit = false;
	public bool kakuninnnnnnnnnnnnnnnnnnnnn = false;
	void Start()
	{
		if (followGround_Script.direcState == FollowGround3.DirectionState.Left)
		{
			angleZ = 0;
			hitCol.center = new Vector3(-0.28f, 0.119f, 0);
		}
		if (followGround_Script.direcState == FollowGround3.DirectionState.Right)
		{
			angleZ = 180;
			hitCol.center = new Vector3(-0.28f, -0.119f, 0);
		}


	}

	void Update()
	{
		//raypos = new Vector3(transform.position.x - 0.0333f, transform.position.y, transform.position.z);
		kakuninVector = -transform.right;
		sideRayVector = new Vector3(-transform.right.x, -transform.right.y, 0);

		if (followGround_Script.direcState == FollowGround3.DirectionState.Left)
		{
			if (followGround_Script.isHitP && !colHit)
			//if (hithit)
			{
				//angleZ = -followGround_Script.groundAngle;
				//angleZ = -followGround_Script.angle_sin;

				if (followGround_Script.normalVector.y > 0 && followGround_Script.normalVector.x > 0)
				{
					//angleZ = -followGround_Script.angle_sin;
					angleZ = -followGround_Script.angle_cos;

					aaa = true;
				}
				else if (followGround_Script.normalVector.y < 0 && followGround_Script.normalVector.x > 0)
				{
					bbb = true;
					angleZ = -followGround_Script.groundAngle;
				}
				else if (followGround_Script.normalVector.y < 0 && followGround_Script.normalVector.x < 0)
				{
					ccc = true;
					//angleZ = followGround_Script.angle_cos;
					angleZ = followGround_Script.groundAngle;
				}
				else if (followGround_Script.normalVector.y > 0 && followGround_Script.normalVector.x < 0)
				{
					ddd = true;
					angleZ = -followGround_Script.angle_cos;
				}
				else if (followGround_Script.normalVector.y == 0 && followGround_Script.normalVector.x > 0)
				{
					eee = true;
					angleZ = -followGround_Script.angle_cos;
				}
				else if (followGround_Script.normalVector.y == 0 && followGround_Script.normalVector.x < 0)
				{
					fff = true;
					//angleZ = -followGround_Script.angle_cos;
					angleZ = followGround_Script.groundAngle;
				}
				else if (followGround_Script.normalVector.y > 0 && followGround_Script.normalVector.x == 0)
				{
					ggg = true;
					angleZ = 0;
				}
				else if (followGround_Script.normalVector.y < 0 && followGround_Script.normalVector.x == 0)
				{
					hhh = true;
					angleZ = 180f;
				}
			}
			else
			{
				if (followGround_Script.notHitCnt > followGround_Script.NotHitMax)
				{
					angleZ += angleZ_ChangeValue;
					//followGround_Script.notHitCnt = 0;
				}
				//followGround_Script.groundAngle = angleZ;
			}
		}
		else if (followGround_Script.direcState == FollowGround3.DirectionState.Right)
		{
			if (followGround_Script.isHitP && !colHit)
			//if (hithit)
			{
				//angleZ = -followGround_Script.groundAngle;
				//angleZ = -followGround_Script.angle_sin;

				if (followGround_Script.normalVector.y > 0 && followGround_Script.normalVector.x > 0)
				{
					//angleZ = -followGround_Script.angle_sin;
					angleZ = 180 - followGround_Script.angle_cos;

					//aaa = true;
				}
				else if (followGround_Script.normalVector.y < 0 && followGround_Script.normalVector.x > 0)
				{
					bbb = true;
					angleZ = followGround_Script.angle_cos;
				}
				else if (followGround_Script.normalVector.y < 0 && followGround_Script.normalVector.x < 0)
				{
					ccc = true;
					//angleZ = followGround_Script.angle_cos;
					angleZ = followGround_Script.angle_cos;
				}
				else if (followGround_Script.normalVector.y > 0 && followGround_Script.normalVector.x < 0)
				{
					ddd = true;
					angleZ = 180.0f - followGround_Script.angle_cos;
				} 
				else if (followGround_Script.normalVector.y == 0 && followGround_Script.normalVector.x > 0)
				{
					eee = true;
					angleZ = 90.0f;
				}
				else if (followGround_Script.normalVector.y == 0 && followGround_Script.normalVector.x < 0)
				{
					fff = true;
					//angleZ = -followGround_Script.angle_cos;
					angleZ = -90.0f;
				}
				else if (followGround_Script.normalVector.y > 0 && followGround_Script.normalVector.x == 0)
				{
					ggg = true;
					angleZ = 180;
				}
				else if (followGround_Script.normalVector.y < 0 && followGround_Script.normalVector.x == 0)
				{
					hhh = true;
					angleZ = 0;
				}

			}
			else
			{
				//if (colHit)
				//{
				//	colDelayCnt++;
				//	if (colDelayCnt > colDelayMax)
				//	{
				//		colHit = false;
				//		colDelayCnt = 0;
				//	}
				//}
				if (followGround_Script.notHitCnt > followGround_Script.NotHitMax)
				{
					angleZ -= angleZ_ChangeValue;
					//followGround_Script.notHitCnt = 0;
				}
				//followGround_Script.groundAngle = angleZ;
			}
		}
		transform.rotation = Quaternion.Euler(0, 0, angleZ);

		RaySide();
		RayUnder();
	}

	//下方向にRayを出す
	void RayUnder()
	{
		int layerMask = 1 << 8;

		layerMask = ~layerMask;

		if (followGround_Script.direcState == FollowGround3.DirectionState.Left)
		{
			underRayPos = transform.position - new Vector3(0, 0.01f, 0);
			if (Physics.Raycast(underRayPos, -transform.up, out hit, 0.5f))
			{
				//rayDelayCnt++;
				if (hit.collider.tag == "Wall")
				{

					//if (rayDelayCnt > rayDelayMax)
					angleTest = Quaternion.FromToRotation(-transform.up, hit.normal).eulerAngles;

					Debug.DrawRay(underRayPos, -transform.up * hit.distance, Color.red);
				}
				//Wall以外のものに当たっていた時の確認用
				else
				{
					Debug.DrawRay(underRayPos, -transform.up * 0.5F, Color.yellow);
					tagname = hit.collider.tag;
				}
			}
			else
			{
				raydasitayo++;
				Debug.DrawRay(underRayPos, -transform.up * 0.5F, Color.white);
				if (followGround_Script.hitDelayCnt > followGround_Script.hitDelayMax)
				{
					followGround_Script.isHitP = false;
					followGround_Script.hitDelayCnt = 0;
				}
			}
		}
		else if (followGround_Script.direcState == FollowGround3.DirectionState.Right)
		{
			underRayPos = transform.position - new Vector3(0, 0.01f, 0);
			if (Physics.Raycast(underRayPos, transform.up, out hit, 0.5f))
			{
				//rayDelayCnt++;
				if (hit.collider.tag == "Wall")
				{

					//if (rayDelayCnt > rayDelayMax)
					angleTest = Quaternion.FromToRotation(transform.up, hit.normal).eulerAngles;

					Debug.DrawRay(underRayPos, transform.up * hit.distance, Color.red);
				}
				//Wall以外のものに当たっていた時の確認用
				else
				{
					Debug.DrawRay(underRayPos, transform.up * 0.5F, Color.yellow);
					tagname = hit.collider.tag;
				}
			}
			else
			{
				raydasitayo++;
				Debug.DrawRay(underRayPos, transform.up * 0.5F, Color.white);
				if (followGround_Script.hitDelayCnt > followGround_Script.hitDelayMax)
				{
					followGround_Script.isHitP = false;
					followGround_Script.hitDelayCnt = 0;
				}
			}

		}

	}

	//横方向のRay(進行方向で左右どちらに出すか変わる)
	void RaySide()
	{
		if (followGround_Script.direcState == FollowGround3.DirectionState.Left)
		{
			sideRayPos = transform.position - new Vector3(0.01f, 0, 0);
		}
		if (followGround_Script.direcState == FollowGround3.DirectionState.Right)
		{
			sideRayPos = transform.position - new Vector3(-0.01f, 0, 0);
		}

		if (Physics.Raycast(sideRayPos, -transform.right, out hit2, 0.75f))
		{
			//rayDelayCnt++;
			if (hit2.collider.tag == "Wall")
			{

				//if (rayDelayCnt > rayDelayMax)
				//angleTest = Quaternion.FromToRotation(-transform.right, hit2.normal).eulerAngles;
				angle_Sin = (Mathf.Asin(hit2.normal.y) * Mathf.Rad2Deg);
				angle_Cos = (Mathf.Asin(hit2.normal.x) * Mathf.Rad2Deg);
				groundAngle= Vector3.Angle(hit2.normal, Vector3.up);
				groundAngle = Mathf.Round(groundAngle * 10);
				groundAngle /= 10;

				//kakuninVector = hit2.normal;
				sideRayVector = hit2.normal;
				if (sideRayVector.x > -0.0001f && sideRayVector.x < 0.0001f)
				{
					sideRayVector.x = 0;
				}
				if (sideRayVector.y > -0.0001f && sideRayVector.y < 0.0001f)
				{
					sideRayVector.y = 0;
				}

				Debug.DrawRay(sideRayPos, -transform.right * hit2.distance, Color.red);
			}
		}
		else
		{
			Debug.DrawRay(sideRayPos, -transform.right * 0.75F, Color.white);
		}
	}

	//
	private void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == "Wall" && followGround_Script.isHitP)
		{
			if (followGround_Script.direcState == FollowGround3.DirectionState.Left)
			{
				if (sideRayVector.y > 0 && sideRayVector.x > 0)
				{
					//angleZ = -followGround_Script.angle_sin;
					angleZ = -angle_Cos;
					followGround_Script.normalVector = sideRayVector;
					aaa = true;
				}
				else if (sideRayVector.y < 0 && sideRayVector.x > 0)
				{
					bbb = true;
					angleZ = -groundAngle;
					kakuninnnnnnnnnnnnnnnnnnnnn = true;
					followGround_Script.isHitP = false;
				}
				else if (sideRayVector.y < 0 && sideRayVector.x < 0)
				{
					ccc = true;
					//angleZ = followGround_Script.angle_cos;
					//angleZ = followGround_Script.groundAngle;
				}
				else if (sideRayVector.y > 0 && sideRayVector.x < 0)
				{
					ddd = true;
					angleZ = groundAngle;
					followGround_Script.isHitP = false;
				}
				else if (sideRayVector.y == 0 && sideRayVector.x > 0)
				{
					eee = true;
					angleZ = -90.0f;
				}
				else if (sideRayVector.y == 0 && sideRayVector.x < 0)
				{
					fff = true;
					//angleZ = -followGround_Script.angle_cos;
					//angleZ = followGround_Script.groundAngle;
				}
				else if (sideRayVector.y > 0 && sideRayVector.x == 0)
				{
					ggg = true;
					angleZ = 0;
				}
				else if (sideRayVector.y < 0 && sideRayVector.x == 0)
				{
					hhh = true;
					angleZ = 180f;
				}
			}
			else if (followGround_Script.direcState == FollowGround3.DirectionState.Right)
			{
				colHit = true;
				if (sideRayVector.y > 0 && sideRayVector.x > 0)
				{
					//angleZ = -followGround_Script.angle_sin;
					//angleZ = -angle_Cos;
					angleZ = 180f - angle_Cos;
					followGround_Script.normalVector = sideRayVector;
					transform.rotation = Quaternion.Euler(0, 0, angleZ);

					aaa = true;
				}
				else if (sideRayVector.y < 0 && sideRayVector.x > 0)
				{
					bbb = true;
					angleZ = -groundAngle;
					followGround_Script.isHitP = false;
				}
				else if (sideRayVector.y < 0 && sideRayVector.x < 0)
				{
					ccc = true;
					kakuninnnnnnnnnnnnnnnnnnnnn = true;

					angleZ = angle_Cos;
				}
				else if (sideRayVector.y > 0 && sideRayVector.x < 0)
				{
					ddd = true;
					angleZ = groundAngle;
					followGround_Script.isHitP = false;
				}
				else if (sideRayVector.y == 0 && sideRayVector.x > 0)
				{
					eee = true;
					angleZ = 90.0f;
				}
				else if (sideRayVector.y == 0 && sideRayVector.x < 0)
				{
					fff = true;
					angleZ = -90;
				}
				else if (sideRayVector.y > 0 && sideRayVector.x == 0)
				{
					ggg = true;
					angleZ = 180;
				}
				else if (sideRayVector.y < 0 && sideRayVector.x == 0)
				{
					hhh = true;
					angleZ = 0;
				}

			}

		}
	}
	private void OnTriggerStay(Collider col)
	{
		if (col.gameObject.tag == "Wall")
		{
			colHit = true;
		}
	}

	private void OnTriggerExit(Collider col)
	{
		if (col.gameObject.tag == "Wall")
		{
			colHit = false;
		}
	}
}
