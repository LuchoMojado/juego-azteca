using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShardsState : State
{
    public ShardsState(ObsidianGod boss)
    {
        _boss = boss;
    }

    public override void OnEnter()
    {
        _boss.renderer.material.color = Color.grey;

        _boss.Shards(3);
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
