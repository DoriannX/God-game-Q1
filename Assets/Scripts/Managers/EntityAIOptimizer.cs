using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

public class EntityAIOptimizer : MonoBehaviour {
    [SerializeField] private float tickInterval = 0.15f;
    [SerializeField] private int numberOfGroups = 3;
    
    private Dictionary<BehaviorGraphAgent, Coroutine> activeAgents = new();
    private int currentGroupID = 0;
    
    
    public void RegisterAgent(BehaviorGraphAgent agent) {
        if (agent == null || activeAgents.ContainsKey(agent))
            return;

        agent.enabled = false;
        
        float groupOffset = (currentGroupID % numberOfGroups) * (tickInterval / numberOfGroups);
        Coroutine tickCoroutine = StartCoroutine(TickAgent(agent, groupOffset));
        
        activeAgents.Add(agent, tickCoroutine);
        currentGroupID++;
    }
    
    public void UnregisterAgent(BehaviorGraphAgent agent) {
        if (agent == null || !activeAgents.ContainsKey(agent))
            return;
        
        // Arrêter la coroutine
        if (activeAgents[agent] != null) {
            StopCoroutine(activeAgents[agent]);
        }
        
        activeAgents.Remove(agent);
    }
    
    private IEnumerator TickAgent(BehaviorGraphAgent agent, float offset) {
        yield return new WaitForSeconds(offset);
        
        while (agent != null) {
            agent.enabled = true;
            yield return null;
            
            if (agent != null)
                agent.enabled = false;
            
            yield return new WaitForSeconds(tickInterval);
        }

        activeAgents.Remove(agent);
    }
    
    void OnDestroy() {
        StopAllCoroutines();
        activeAgents.Clear();
    }
}