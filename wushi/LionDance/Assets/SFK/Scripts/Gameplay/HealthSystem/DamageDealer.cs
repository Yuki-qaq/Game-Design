using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public string damageType = "Default";
    public int damageAmount = 10;
    public bool randomDamage = false;
    [ShowIf(nameof(randomDamage))]
    [Min(0)] public int damageRandomVariance = 1;

    [Space]
    public bool repeat = false;

    [ShowIf(nameof(repeat)), Min(0)] public int repeatTimes = 1;
    [ShowIf(nameof(repeat)), Min(0)] public int repeatDamage = 10;
    [ShowIf(nameof(repeat)), Min(0)] public float repeatInterval = 0.5f;
    [ShowIf(nameof(repeat)), Min(0)] public float repeatDelay = 1;

    public void DealDamange(Collider collider) => Deal(collider.gameObject);
    public void DealDamange(Collision collision) => Deal(collision.collider.gameObject);
    public void DealDamange(GameObject go) => Deal(go);

    public bool Deal(GameObject target)
    {
        if (!target.TryGetComponent(out DamageReciever receiver))
            receiver = target.GetComponentInChildren<DamageReciever>();

        if (receiver == null)
            return false;

        int finalDamage = damageAmount;
        if (randomDamage)
            finalDamage += Random.Range(-damageRandomVariance, 1 + damageRandomVariance);

        Debug.Log($"Dealing {finalDamage} to {target.name} with {(repeat ? $"{repeatTimes}repeats of {repeatDamage} damage" : "no repeats")}");
        receiver.Receive(finalDamage, damageType);

        if (repeat && repeatTimes > 0)
            StartCoroutine(RepeatDamage(receiver));

        return true; 
    }

    private IEnumerator RepeatDamage(DamageReciever reciever)
    {
        int counter = 0;
        yield return new WaitForSeconds(repeatDelay);

        while (counter < repeatTimes && reciever)
        {
            reciever.Receive(repeatDamage, damageType);
            counter++;
            yield return new WaitForSeconds(repeatInterval);
        } 
    }
}
