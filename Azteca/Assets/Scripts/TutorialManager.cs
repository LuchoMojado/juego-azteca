using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] PlayerController _player;
    [SerializeField] ObsidianGod _enemy;
    [SerializeField] string _movement, _jump, _step, _sun, _supernova, _sunstrike, _switch;
    [SerializeField] Collider[] _colliders;
    [SerializeField] float _firstWait, _firstSectionDelays, _preFightWait, _supernovaWait, _sunstrikeWait;

    int _colliderCounter = 0;

    private void Start()
    {
        StartCoroutine(FirstSection());
    }

    IEnumerator FirstSection()
    {
        yield return new WaitForSeconds(_firstWait);

        UIManager.instance.ChangeText(true, _movement);

        yield return new WaitForSeconds(_firstSectionDelays);

        UIManager.instance.ChangeText(true, _jump);

        yield return new WaitForSeconds(_firstSectionDelays);

        UIManager.instance.ChangeText(true, _step);

        yield return new WaitForSeconds(_firstSectionDelays);

        UIManager.instance.ChangeText(false);
    }

    IEnumerator PreFight()
    {
        yield return new WaitForSeconds(_preFightWait);

        UIManager.instance.ChangeText(true, _sun);
        Time.timeScale = 0;
        _player.Inputs.inputUpdate = _player.Inputs.WaitUntilAim;

        while (!_player.Inputs.trigger)
        {
            yield return null;
        }

        _player.Inputs.trigger = false;
        UIManager.instance.ChangeText(false);
        Time.timeScale = 1;
    }

    IEnumerator Specials()
    {
        _enemy.BreakWall();

        yield return new WaitForSeconds(_supernovaWait);

        UIManager.instance.ChangeText(true, _supernova);
        Time.timeScale = 0;
        _player.Inputs.inputUpdate = _player.Inputs.WaitUntilSecondSpecial;

        while (!_player.Inputs.trigger)
        {
            yield return null;
        }

        _player.Inputs.trigger = false;
        UIManager.instance.ChangeText(false);
        Time.timeScale = 1;

        yield return new WaitForSeconds(_sunstrikeWait);

        UIManager.instance.ChangeText(true, _sunstrike);

        while (_enemy != null)
        {
            yield return null;
        }

        UIManager.instance.ChangeText(false);
    }

    IEnumerator PostFight()
    {
        UIManager.instance.ChangeText(true, _switch);

        yield return new WaitForSeconds(_firstSectionDelays);

        UIManager.instance.ChangeText(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            switch (_colliderCounter)
            {
                case 0:
                    _colliders[_colliderCounter].enabled = false;
                    _colliderCounter++;
                    _enemy.gameObject.SetActive(true);
                    StartCoroutine(PreFight());
                    break;
                case 1:
                    _colliders[_colliderCounter].enabled = false;
                    _colliderCounter++;
                    StartCoroutine(Specials());
                    break;
                case 2:
                    _colliders[_colliderCounter].enabled = false;
                    StartCoroutine(PostFight());
                    break;
                default:
                    break;
            }
        }
    }
}