using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : State
{
    float _duration, _timer;

    public WalkState(ObsidianGod boss, float duration)
    {
        _boss = boss;
        _duration = duration;
    }

    public override void OnEnter()
    {
        _boss.renderer.material.color = Color.white;
        _timer = 0;

        _boss.Walk();
    }

    public override void OnExit()
    {
        _boss.StopWalking();
    }

    public override void OnUpdate()
    {
        if (_timer < _duration)
        {
            _timer += Time.deltaTime;
        }
        else
        {
            var dist = _boss.GetPlayerDistance();

            if (dist > 20)
            {
                fsm.ChangeState(ObsidianGod.ObsidianStates.Wave);
            }
            else if (dist > 10)
            {
                var rnd = Random.Range(0, 3);

                switch (rnd)
                {
                    case 0:
                        fsm.ChangeState(ObsidianGod.ObsidianStates.Wave);
                        break;
                    case 1:
                        fsm.ChangeState(ObsidianGod.ObsidianStates.Shards);
                        break;
                    case 2:
                        fsm.ChangeState(ObsidianGod.ObsidianStates.Spikes);
                        break;
                }
            }
            else
            {
                var rnd = Random.Range(0, 2);

                switch (rnd)
                {
                    case 0:
                        fsm.ChangeState(ObsidianGod.ObsidianStates.Swing);
                        break;
                    case 1:
                        fsm.ChangeState(ObsidianGod.ObsidianStates.Spikes);
                        break;
                }
            }
        }
    }
}
