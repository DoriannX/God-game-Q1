using UnityEngine;
using UnityHFSM;

public class ScareState : State
{
    private ScareComponent scareComponent;
    private GhostIa ghostIa;
    public ScareState(ScareComponent scareComponent, GhostIa ghostIa)
    {
        this.scareComponent = scareComponent;
        this.ghostIa = ghostIa;
    }

    public override void OnLogic()
    {
        base.OnLogic();
        Vector2 direction = scareComponent.GetScareDirection();
        if (direction != Vector2.zero)
        {
            ghostIa.GoBy(direction);
        }
    }
}