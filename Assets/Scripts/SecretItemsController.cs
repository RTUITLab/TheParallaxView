using System.Collections.Generic;
using UnityEngine;

public class SecretItemsController : Singleton<SecretItemsController>
{
    [field: SerializeField] public Transform Camera { get; private set; }
    [field: SerializeField, Min(.1f)] public float SearchDistance { get; private set; }
    [field: SerializeField] public LayerMask SearchRayMask { get; private set; }
    [field: SerializeField, Min(0f)] public float SearchTime { get; private set; }
    [field: SerializeField, Range(30f, 180f)] public float SearchAngle { get; private set; }

    private List<SecretItem> _items = new();

    public void AddSecretItem(SecretItem secretItem)
    {
        if(!_items.Contains(secretItem))
        {
            _items.Add(secretItem);
        }
    }

    public void ResetAllItems()
    {
        foreach(var item in _items)
        {
            item.Activate();
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            ResetAllItems();
        }
    }
}
