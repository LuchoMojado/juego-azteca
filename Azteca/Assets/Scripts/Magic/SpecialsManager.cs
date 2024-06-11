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
    [SerializeField] GameObject _sunstrikeFirstRay;
    [SerializeField] GameObject _sunstrikeSecondRay;
    [SerializeField] float _sunstrikeCost, _sunstrikeDamage, _sunstrikeRadius, _sunstrikePreparation, _sunstrikeDelay, _sunstrikeLinger, _sunstrikeCooldown;

    [SerializeField] PlayerController _player;

    [HideInInspector] public Inputs inputs;

    float[] _slotsCooldowns = new float[2];

    private void Start()
    {
        _allSpecials.Add(new SpecialSunstrike(_player, inputs, _sunstrikeFirstRay, _sunstrikeSecondRay, _sunstrikeCost, _sunstrikeDamage, _sunstrikeRadius, _sunstrikePreparation, _sunstrikeDelay, _sunstrikeLinger, _sunstrikeCooldown));

        EquipSpecial(Specials.Sunstrike, 1);
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
        if (slot <= 0 || slot > _equippedSpecials.Length) return default;

        return _equippedSpecials[slot - 1].staminaCost;
    }

    public void ActivateSpecial(int slot)
    {
        if (slot <= 0 || slot > _equippedSpecials.Length) return;

        if (_slotsCooldowns[slot] > 0) return;

        _slotsCooldowns[slot] = _equippedSpecials[slot - 1].Activate();
    }

    public void EquipSpecial(Specials special, int slot)
    {
        if (slot <= 0 || slot > _equippedSpecials.Length) return;

        _equippedSpecials[slot] = _allSpecials[(int)special];
    }
}
