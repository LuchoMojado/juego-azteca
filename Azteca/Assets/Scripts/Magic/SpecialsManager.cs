using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialsManager : MonoBehaviour
{
    public enum Specials
    {
        Sunstrike,

    }

    List<SpecialMagic> _allSpecials = new List<SpecialMagic>();
    SpecialMagic[] _equippedSpecials = new SpecialMagic[2];

    [Header("Sunstrike")]
    [SerializeField] float _sunstrikeCost;
    [SerializeField] float _sunstrikeDamage, _sunstrikePreparation, _sunstrikeDelay, _sunstrikeRecovery, _sunstrikeCooldown;

    [SerializeField] PlayerController _player;

    [HideInInspector] public Inputs inputs;

    private void Start()
    {
        _allSpecials.Add(new SpecialSunstrike(_player, inputs, _sunstrikeCost, _sunstrikeDamage, _sunstrikePreparation, _sunstrikeDelay, _sunstrikeRecovery));
    }

    public float GetCost(int slot)
    {
        if (slot <= 0 || slot > _equippedSpecials.Length) return default;

        return _equippedSpecials[slot - 1].staminaCost;
    }

    public void ActivateSpecial(int slot)
    {
        if (slot <= 0 || slot > _equippedSpecials.Length) return;

        _equippedSpecials[slot - 1].Activate();
    }

    public void EquipSpecial(Specials special, int slot)
    {
        if (slot <= 0 || slot > _equippedSpecials.Length) return;

        
    }
}
