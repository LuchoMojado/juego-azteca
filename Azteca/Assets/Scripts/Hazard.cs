using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    [HideInInspector] public float damage, duration;

    [SerializeField] Animation _animation;

    private void Update()
    {
        if (duration <= 0)
        {
            StartCoroutine(Die());
        }
        else
        {
            duration -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            player.TakeDamage(damage);
        }
    }

    IEnumerator Die()
    {
        _animation.Play("SpikesLower");

        yield return new WaitForSeconds(0.1f);

        Destroy(gameObject);
    }
}
