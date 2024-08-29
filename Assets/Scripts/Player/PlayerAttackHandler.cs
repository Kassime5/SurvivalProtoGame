using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackHandler : MonoBehaviour
{
    private PlayerRayCastInfo playerRayCastInfo;

    void Awake()
    {
        playerRayCastInfo = FindObjectOfType<PlayerRayCastInfo>();
    }

    public void PunchAnimation()
    {
        playerRayCastInfo.AnimationAttack();
    }
}
