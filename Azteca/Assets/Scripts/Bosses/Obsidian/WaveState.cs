using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveState : State
{
    public WaveState(ObsidianGod boss)
    {
        _boss = boss;
    }

    public override void OnEnter()
    {
        _boss.renderer.material.color = Color.yellow;

        _boss.Wave();
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
