using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalWallManager : MonoBehaviour
{
    public GameMaster GM;           //ゲームマスター（ゲームクリアかどうかの判定をしたりするよう）
    [SerializeField] private SmartphoneManagement Manager;
    void OnTriggerEnter(Collider col)
    {

        if (col.tag == "Charger")
        {
            Manager.OffTriggerCollidersEnabledSet = false;
            GM.SetStageState(GameMaster.StageState.STAGEFAILURE);
        }
    }
}
