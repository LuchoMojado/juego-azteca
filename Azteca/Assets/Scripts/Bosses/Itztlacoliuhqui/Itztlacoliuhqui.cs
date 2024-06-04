using IA2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Personaje;

public class Itztlacoliuhqui : MonoBehaviour
{
    public enum Actions
    {
        Inactive,
        Search,
        Spikes,
        Swing,
        BreakWall,
        Shield,
        Hide,
        Gatling,
        Charge,
        ArenaSpikes
    }

    EventFSM<Actions> _myFsm;

    private void Awake()
    {
        var inactive = new State<Actions>("Inactive");
        var search = new State<Actions>("Search");
        var spikes = new State<Actions>("Spikes");
        var swing = new State<Actions>("Swing");
        var breakWall = new State<Actions>("BreakWall");
        var shield = new State<Actions>("Shield");
        var hide = new State<Actions>("Hide");
        var gatling = new State<Actions>("Gatling");
        var charge = new State<Actions>("Charge");
        var arenaSpikes = new State<Actions>("ArenaSpikes");

        StateConfigurer.Create(inactive)
            .SetTransition(Actions.Inactive, inactive)
            .Done();

        StateConfigurer.Create(search)
            .SetTransition(Actions.Inactive, inactive)
            .Done();

        StateConfigurer.Create(spikes)
            .SetTransition(Actions.Inactive, inactive)
            .Done();

        StateConfigurer.Create(swing)
            .SetTransition(Actions.Inactive, inactive)
            .Done();

        StateConfigurer.Create(breakWall)
            .SetTransition(Actions.Inactive, inactive)
            .Done();

        StateConfigurer.Create(shield)
            .SetTransition(Actions.Inactive, inactive)
            .Done();

        StateConfigurer.Create(hide)
            .SetTransition(Actions.Inactive, inactive)
            .Done();

        StateConfigurer.Create(gatling)
            .SetTransition(Actions.Inactive, inactive)
            .Done();

        StateConfigurer.Create(charge)
            .SetTransition(Actions.Inactive, inactive)
            .Done();

        StateConfigurer.Create(arenaSpikes)
            .SetTransition(Actions.Inactive, inactive)
            .Done();
    }
}
