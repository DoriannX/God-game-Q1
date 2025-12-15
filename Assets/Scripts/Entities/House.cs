using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using SaveLoadSystem;
using UnityEngine;

[RequireComponent(typeof(SaveableEntity))]
public class House : WorkTask, ISaveable
{
    [Serializable]
    private struct HouseData
    {
        public float fuckProgress;
        public int ticksAlone;
        public bool isFucking;
        public List<GhostAI.GhostData> fuckingGhosts;
        
    }

    public HashSet<EntityAI> fuckingGhosts = new();
    private GrowComponent growComponent;
    private float fuckProgress = 0;
    [SerializeField] private float fuckIncrement = 0.1f;
    [SerializeField] private int tickToExitAlone = 5;
    [SerializeField] private int minBabies = 1;
    [SerializeField] private int maxBabies = 3;
    private Animator animator;
    private int ticksAlone = 0;
    private bool isFucking = false;
    public bool isFull => fuckingGhosts.Count >= 2;
    public event Action onBreedFinished;


    private void Awake()
    {
        growComponent = GetComponentInChildren<GrowComponent>();
        animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        TickSystem.ticked += OnTicked;
    }

    public override void Work()
    {
        base.Work();
        growComponent.Grow();
    }

    protected override void OnComplete()
    {
        
    }

    public void Enter(EntityAI entityAI)
    {
        fuckingGhosts.Add(entityAI);
        entityAI.gameObject.SetActive(false);
        if (fuckingGhosts.Count == 2)
            StartFucking();
    }

    private void StartFucking()
    {
        if (fuckingGhosts.Count < 2) return;
        animator.Play("HouseSexAnimation");
        isFucking = true;
    }

    private void OnTicked()
    {
        if (fuckingGhosts.Count == 1)
        {
            ticksAlone++;
            if (ticksAlone >= tickToExitAlone)
            {
                Exit(fuckingGhosts.First());
                ticksAlone = 0;
            }
        }
        else
        {
            ticksAlone = 0;
        }

        if (!isFucking)
        {
            return;
        }

        fuckProgress += fuckIncrement;
        if (fuckProgress < 1f)
        {
            return;
        }

        int babiesCount = minBabies + (int)((maxBabies - minBabies) * progress);
        for (var i = 0; i < babiesCount; i++)
        {
            EntityManager.instance.SpawnEntity(EntityType.Ghost,TilemapManager.instance.HexAxialToWorld(
                TilemapManager.instance.WorldToHexAxial(transform.position)));
        }

        fuckProgress = 0f;
        isFucking = false;

        animator.Play("Idle");
        foreach (EntityAI ghost in fuckingGhosts.ToList())
        {
            Exit(ghost);
        }
    }

    private void Exit(EntityAI entity)
    {
        fuckingGhosts.Remove(entity);
        entity.gameObject.SetActive(true);
        onBreedFinished?.Invoke();
    }
    
    private void OnDisable()
    {
        TickSystem.ticked -= OnTicked;
    }

    private void OnDestroy()
    {
        foreach (var ghost in fuckingGhosts)
        {
            if (ghost == null)
                return;
            Destroy(ghost.gameObject);
        }
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
        List<GhostAI.GhostData> ghostsData = new List<GhostAI.GhostData>();
        foreach (var ghost in fuckingGhosts)
        {
            ghostsData.Add((GhostAI.GhostData)ghost.SaveState());
            
        }
        HouseData data = new HouseData
        {
            fuckProgress = fuckProgress,
            ticksAlone = ticksAlone,
            isFucking = isFucking,
            fuckingGhosts = ghostsData
        };
        return data;
    }

    public void LoadState(object state)
    {
        HouseData data = (HouseData)state;
        fuckProgress = data.fuckProgress;
        ticksAlone = data.ticksAlone;
        isFucking = data.isFucking;
        foreach (var ghostData in data.fuckingGhosts)
        {
            GhostAI ghost = EntityManager.instance.SpawnEntity(EntityType.Ghost,TilemapManager.instance.HexAxialToWorld(
                TilemapManager.instance.WorldToHexAxial(ghostData.position.ToVector2()))).GetComponent<GhostAI>();
            ghost.LoadState(ghostData);
            ghost.gameObject.SetActive(false);
            fuckingGhosts.Add(ghost);
        }
    }

    public void PostInstantiation(object state)
    {
        HouseData data = (HouseData)state;
        if (data.isFucking && fuckingGhosts.Count == 2)
        {
            StartFucking();
        }
    }

    public void GotAddedAsChild(GameObject obj, GameObject hisParent)
    {
        
    }
}