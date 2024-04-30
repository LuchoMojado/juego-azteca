using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashState : State
{
    public DashState(ObsidianGod boss)
    {
        _boss = boss;
    }

    public override void OnEnter()
    {
        _boss.renderer.material.color = Color.blue;

        _boss.Dash();
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        if (!_boss.takingAction)
        {
            fsm.ChangeState(_boss.GetAction());
        }
    }
}