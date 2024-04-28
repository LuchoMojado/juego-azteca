using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : State
{
    float _duration, _extraDuration, _timer;

    public WalkState(ObsidianGod boss, float duration, float durationPerCombo)
    {
        _boss = boss;
        _duration = duration;
        _extraDuration = durationPerCombo;
    }

    public override void OnEnter()
    {
        _boss.renderer.material.color = Color.white;
        _timer = _duration + _extraDuration * _boss.comboCount;
        _boss.comboCount = 0;

        _boss.ToggleWalk(true);
    }

    public override void OnExit()
    {
        _boss.ToggleWalk(false);
    }

    public override void OnUpdate()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
        }
        else
        {
            fsm.ChangeState(_boss.GetAction());
        }
    }
}
