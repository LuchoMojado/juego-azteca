using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObsidianGod : Entity
{
    PlayerController _player;

    float _closeDistance, _midDistance;

    public override void Die()
    {
        throw new System.NotImplementedException();
    }

    void Shards()
    {

    }

    void Spikes()
    {

    }

    void Wave()
    {

    }

    void Leap()
    {

    }

    void Swing()
    {

    }

    bool IsPlayerClose()
    {
        return GetPlayerDistance() < _closeDistance;
    }

    bool IsPlayerMidRange()
    {
        return GetPlayerDistance() < _midDistance;
    }

    float GetPlayerDistance()
    {
        return Vector3.Distance(transform.position, _player.transform.position);
    }
}
