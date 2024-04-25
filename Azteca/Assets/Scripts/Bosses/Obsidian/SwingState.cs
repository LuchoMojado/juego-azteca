using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingState : State
{
    public SwingState(ObsidianGod boss)
    {
        _boss = boss;
    }

    public override void OnEnter()
    {
        _boss.renderer.material.color = Color.red;

        _boss.Swing();
    }

    public override void OnExit()
    {
        
    }

    public override void OnUpdate()
    {
        if (!_boss.takingAction)
        {
            fsm.ChangeState(ObsidianGod.ObsidianStates.Walk);
        }
    }
}
