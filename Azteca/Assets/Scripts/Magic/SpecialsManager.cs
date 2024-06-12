using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialsManager : MonoBehaviour
{
    public enum Specials
    {
        Sunstrike,
        Supernova
    }

    List<SpecialMagic> _allSpecials = new List<SpecialMagic>();
    SpecialMagic[] _equippedSpecials = new SpecialMagic[2];

    [Header("Sunstrike")]
    [SerializeField] GameObject _sunstrikeFirstRay;
    [SerializeField] GameObject _sunstrikeSecondRay;
    [SerializeField] float _sunstrikeCost, _sunstrikeDamage, _sunstrikeRadius, _sunstrikePreparation, _sunstrikeDelay, _sunstrikeLinger, _sunstrikeCooldown;

    [Header("Supernova")]
    [SerializeField] GameObject _supernova;
    [SerializeField] float _supernovaCost, _supernovaRadius, _supernovaDamage, _supernovaPreparation, _supernovaRecovery, _supernovaCooldown;

    PlayerController _player;
    Inputs _inputs;

    float[] _slotsCooldowns = new float[2];

    private void Start()
    {
        _player = GetComponent<PlayerController>();
        _inputs = _player.Inputs;

        _allSpecials.Add(new SpecialSunstrike(_player, _inputs, _sunstrikeFirstRay, _sunstrikeSecondRay, _sunstrikeCost, _sunstrikeDamage, _sunstrikeRadius, _sunstrikePreparation, _sunstrikeDelay, _sunstrikeLinger, _sunstrikeCooldown));
        _allSpecials.Add(new SpecialSupernova(_player, _inputs, _supernova, _supernovaCost, _supernovaRadius, _supernovaDamage, _supernovaPreparation, _supernovaRecovery, _supernovaCooldown));

        EquipSpecial(Specials.Sunstrike, 1);
        EquipSpecial(Specials.Supernova, 2);
    }

    private void Update()
    {
        if (_slotsCooldowns[0] > 0)
        {
            _slotsCooldowns[0] -= Time.deltaTime;
        }
        if (_slotsCooldowns[1] > 0)
        {
            _slotsCooldowns[1] -= Time.deltaTime;
        }
    }

    public float GetCost(int slot)
    {
        return _equippedSpecials[slot - 1].staminaCost;
    }

    public bool IsOffCooldown(int slot)
    {
        return _slotsCooldowns[slot - 1] <= 0;
    }

    public void ActivateSpecial(int slot)
    {
        _slotsCooldowns[slot - 1] = _equippedSpecials[slot - 1].Activate();
    }

    public void EquipSpecial(Specials special, int slot)
    {
        if (slot <= 0 || slot > _equippedSpecials.Length) return;

        _equippedSpecials[slot - 1] = _allSpecials[(int)special];
    }
}
