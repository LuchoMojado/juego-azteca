using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected ObsidianGod _boss;

    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit();

    public FiniteStateMachine fsm;
}
