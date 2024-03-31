
using System;
using UnityEngine;
using UnityEngine.Events;

public class ItemDataChecker : MonoBehaviour
{
    public ItemDataScriptable[] dataWhitelist = new ItemDataScriptable[0];
    public ItemDataScriptable[] dataBlacklist = new ItemDataScriptable[0];

    [Space]
    public UnityEvent<ItemDataScriptable> ValidDataEvent;
    public UnityEvent<ItemDataScriptable> InvalidDataEvent;

    [Space]
    public bool checkCollision;
    public bool checkCollisionExit;

    [Space]
    public bool checkTrigger;
    public bool checkTriggerExit;

    public virtual void Check(GameObject go)
    {
        if (go.TryGetComponent(out ItemData data))
        {
            if (data == null)
                return;

            ItemDataScriptable value = data.dataObject;

            //Array contains this value
            if (Array.IndexOf(dataBlacklist, value) != -1)
            {
                // Item is in blacklist
                InvalidDataEvent?.Invoke(value);
            }
            else if (Array.IndexOf(dataWhitelist, value) != -1)
            {
                // Item is in whitelist
                ValidDataEvent?.Invoke(value);
            }

        }
    }

    public void Check(Collider col)
    {
        Check(col.gameObject);
    }

    public void Check(Collision col)
    {
        Check(col.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Check(collision.gameObject);
    }

    private void OnCollisionExit(Collision collision)
    {
        Check(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (checkTrigger)
        {
            Check(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (checkTriggerExit)
        {
            Check(other);
        }
    }
}
