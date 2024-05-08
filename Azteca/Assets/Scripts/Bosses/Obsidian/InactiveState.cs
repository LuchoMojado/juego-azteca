using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InactiveState : State
{
    Transform player;

    float _aggroRange;

    public InactiveState(ObsidianGod boss, Transform playerT, float range)
    {
        _boss = boss;
        player = playerT;
        _aggroRange = range;
    }

    public override void OnEnter()
    {
        _boss.renderer.material.color = Color.white;
        //animacion idle
    }

    public override void OnExit()
    {
        _boss.LookAtPlayer = true;
    }

    public override void OnUpdate()
    {
        if (Vector3.Distance(player.position, _boss.transform.position) <= _aggroRange)
        {
            fsm.ChangeState(ObsidianGod.ObsidianStates.Walk);
        }
    }
}
