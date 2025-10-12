using System;
using SaveLoadSystem;
using UnityEngine;

public class Tree : MonoBehaviour, ISaveable
{
    [Serializable]
    private struct TreeData
    {
        public SaveableEntity.Vector3Data position;
        public float waterProgress;
    }
    private GrowComponent growComponent;
    private float waterProgress = 0;
    [SerializeField] private float waterIncrement = 0.1f;
    
    private void Awake()
    {
        growComponent = GetComponentInChildren<GrowComponent>();
    }
    private void OnEnable()
    {
        TickSystem.ticked += OnTick;
    }
    
    private void OnTick()
    {
        if(MeteoManager.Instance.isRaining)
        {
            waterProgress += waterIncrement;
        }
        else
        {
            waterProgress -= waterIncrement / 2;
        }

        if (!(waterProgress >= 1))
        {
            return;
        }
        waterProgress = 0;
        growComponent.Grow();
    }
    
    private void OnDisable()
    {
        TickSystem.ticked -= OnTick;
    }

    public bool NeedsToBeSaved()
    {
        return true;
    }

    public bool NeedsReinstantiation()
    {
        return true;
    }

    public object SaveState()
    {
        var data = new TreeData
        {
            position = new SaveableEntity.Vector3Data(transform.position),
            waterProgress = waterProgress
        };
        return data;
    }

    public void LoadState(object state)
    {
        var data = (TreeData)state;
        transform.position = data.position.ToVector3();
        waterProgress = data.waterProgress;
    }

    public void PostInstantiation(object state){}

    public void GotAddedAsChild(GameObject obj, GameObject hisParent){}
}