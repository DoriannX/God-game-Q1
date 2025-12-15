using UnityEngine;
using UnityHFSM;

public class ScareState : State
{
    private ScareComponent scareComponent;
    private GhostAI ghostAI;
    public ScareState(ScareComponent scareComponent, GhostAI ghostAI)
    {
        this.scareComponent = scareComponent;
        this.ghostAI = ghostAI;
    }

    /*public override void OnLogic()
    {
        base.OnLogic();
        Vector2 direction = scareComponent.GetScareDirection();
        if (direction != Vector2.zero)
        {
            ghostIa.GoBy(direction);
        }
    }*/
}