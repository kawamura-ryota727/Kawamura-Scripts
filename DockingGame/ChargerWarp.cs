using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerWarp : MonoBehaviour
{
    public GameObject[] warpPos;
    public GameObject chargerObj;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            chargerObj.transform.position = warpPos[0].transform.position;
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            chargerObj.transform.position = warpPos[1].transform.position;

        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            chargerObj.transform.position = warpPos[2].transform.position;

        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            chargerObj.transform.position = warpPos[3].transform.position;

        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            chargerObj.transform.position = warpPos[4].transform.position;

        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            chargerObj.transform.position = warpPos[5].transform.position;

        }

    }
}
