using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.Events;

public class ItemDataCollector : ItemDataChecker
{
    [Header("Collection")]
    public bool useAmountVariable = false;
    [ShowIf(nameof(useAmountVariable))]
    public IntVariableScriptable amountVariable;
    [HideIf(nameof(useAmountVariable))]
    public int collectedAmount = 0;

    [Space]
    public bool destroyOnCollect = true;

    [Space]
    public UnityEvent CollectEvent;

    public override void Check(GameObject go)
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
                collectedAmount++;

                if (useAmountVariable)
                {
                    amountVariable.Value++;
                }

                if (destroyOnCollect)
                {

                    if (go.TryGetComponent(out DestroyWithEvents script))
                    {
                        script.StartDestroy();
                    }
                    else
                    {
                        Destroy(go);
                    }

                }

                ValidDataEvent?.Invoke(value);
                CollectEvent?.Invoke();
            }

        }
    }

    public void CollectItem(ItemDataScriptable item)
    {
        collectedAmount++;
        if (useAmountVariable)
        {
            amountVariable.Value = collectedAmount;
        }
    }
}
