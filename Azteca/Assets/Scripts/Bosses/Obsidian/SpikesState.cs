using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesState : State
{
    public SpikesState(ObsidianGod boss)
    {
        _boss = boss;
    }

    public override void OnEnter()
    {
        _boss.renderer.material.color = Color.black;

        _boss.Spikes();
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
