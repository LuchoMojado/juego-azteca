using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InactiveState : State
{
    float _aggroRange;

    public InactiveState(ObsidianGod boss, float range)
    {
        _boss = boss;
        _aggroRange = range;
    }

    public override void OnEnter()
    {
        _boss.renderer.material.color = Color.white;
        //animacion idle
    }

    public override void OnExit()
    {
        _boss.StartFight();
    }

    public override void OnUpdate()
    {
        if (_boss.GetPlayerDistance() <= _aggroRange)
        {
            fsm.ChangeState(ObsidianGod.ObsidianStates.Walk);
        }
    }
}
