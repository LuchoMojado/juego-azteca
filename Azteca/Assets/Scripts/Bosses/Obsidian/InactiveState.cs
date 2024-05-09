using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InactiveState : State
{
    float _aggroRange;

    public InactiveState(ObsidianGod boss, float range, Animator anim)
    {
        _boss = boss;
        _aggroRange = range;
        _anim = anim;
    }

    public override void OnEnter()
    {
        _boss.renderer.material.color = Color.white;
        //animacion idle
        _anim.SetBool("IsIdle", true);
    }

    public override void OnExit()
    {
        _anim.SetBool("IsIdle", false);
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
