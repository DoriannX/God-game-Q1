using System;
using UnityEngine;
using UnityHFSM;

public class FuckState : State
{
    private GhostAI ghostAI;
    private HouseRetriever houseRetriever;
    public event Action onFuckFinished;
    public FuckState(GhostAI ghostAI, HouseRetriever houseRetriever)
    {
        this.houseRetriever = houseRetriever;
        this.ghostAI = ghostAI;
    }
    public override void OnLogic()
    {
        base.OnLogic();
        /*House house = houseRetriever.GetNearestHouse(ghostIA.transform.position);
        if (house == null)
        {
            onFuckFinished?.Invoke();
            return;
        }
        house.onBreedFinished += () => onFuckFinished?.Invoke();
        ghostIA.GoTo(house.transform.position);
        if (Vector2.Distance(ghostIA.transform.position, house.transform.position) < 0.1f)
        {
            house.Enter(ghostIA);
        }*/
    }
}