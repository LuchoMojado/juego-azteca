using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialsManager : MonoBehaviour
{
    public enum Specials
    {
        Sunstrike,
        Supernova,
        ObsidianTrap,
        RockToss
    }

    Dictionary<Specials, SpecialMagic> _allSpecials = new();
    SpecialMagic[] _equippedSpecials = new SpecialMagic[2];

    [Header("Sunstrike")]
    [SerializeField] GameObject _sunstrikeFirstRay;
    [SerializeField] GameObject _sunstrikeSecondRay;
    [SerializeField] float _sunstrikeCost, _sunstrikeDamage, _sunstrikeRadius, _sunstrikePreparation, _sunstrikeDelay, _sunstrikeLinger, _sunstrikeCooldown;

    [Header("Supernova")]
    [SerializeField] GameObject _supernova;
    [SerializeField] float _supernovaCost, _supernovaRadius, _supernovaDamage, _supernovaPreparation, _supernovaDuration, _supernovaRecovery, _supernovaCooldown;

    [Header("Obsidian Trap")]
    [SerializeField] ObsidianTrap _obsidianTrap;
    [SerializeField] float _obsidianTrapCost, _obsidianTrapShardDamage, _obsidianTrapShardSpeed, _obsidianTrapPreparation, _obsidianTrapRecovery, _obsidianTrapCooldown;

    [Header("Rock Toss")]
    [SerializeField] Rock _rock;
    [SerializeField] Transform _rockTossPos;
    [SerializeField] float _rockTossCost, _rockTossDamage, _rockTossStrength, _rockTossAngle, _rockTossPreparation, _rockTossRecovery, _rockTossCooldown;

    PlayerController _player;
    Inputs _inputs;

    float[] _slotsCooldowns = new float[2];

    private void Start()
    {
        _player = GetComponent<PlayerController>();
        _inputs = _player.Inputs;

        _allSpecials.Add(Specials.Sunstrike, new SpecialSunstrike(_player, _inputs, _sunstrikeFirstRay, _sunstrikeSecondRay, _sunstrikeCost, _sunstrikeDamage, _sunstrikeRadius, _sunstrikePreparation, _sunstrikeDelay, _sunstrikeLinger, _sunstrikeCooldown));
        _allSpecials.Add(Specials.Supernova, new SpecialSupernova(_player, _inputs, _supernova, _supernovaCost, _supernovaRadius, _supernovaDamage, _supernovaPreparation, _supernovaDuration, _supernovaRecovery, _supernovaCooldown));
        _allSpecials.Add(Specials.ObsidianTrap, new SpecialObsidianTrap(_player, _inputs, _obsidianTrap, _obsidianTrapCost, _obsidianTrapShardDamage, _obsidianTrapShardSpeed, _obsidianTrapPreparation, _obsidianTrapRecovery, _obsidianTrapCooldown));
        _allSpecials.Add(Specials.RockToss, new SpecialRockToss(_player, _inputs, _rock, _rockTossPos, _rockTossCost, _rockTossDamage, _rockTossStrength, _rockTossAngle, _rockTossPreparation, _rockTossRecovery, _rockTossCooldown));

        EquipSpecial(Specials.ObsidianTrap, 1);
        EquipSpecial(Specials.RockToss, 2);
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

        _equippedSpecials[slot - 1] = _allSpecials[special];
    }
}
