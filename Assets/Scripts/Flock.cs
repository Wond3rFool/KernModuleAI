using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public FlockAgent flockPrefab;
    private List<FlockAgent> agents = new List<FlockAgent>();
    public FlockBehavior behavior;

    [Range(10, 500)]
    public int startingCount = 250;
    const float AgentDensity = 0.08f;

    [Range(1f, 100f)]
    public float driveFactor = 10f;

    [Range(1f, 100f)]
    public float maxSpeed = 5f;

    [Range(1f, 10f)]
    public float neighbourRadius = 1.5f;

    [Range(0f, 1f)]
    public float avoidanceRadiusMultiplier = 0.5f;

    private float squareMaxSpeed;
    private float squareNeighborRadius;
    private float squareAvoidanceRadius;
    public float squareAvoidanceRadiusGET { get { return squareAvoidanceRadius; } }

    void Start()
    {
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighborRadius = neighbourRadius * neighbourRadius;
        squareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;


        for (int i = 0; i < startingCount; i++) 
        {
            FlockAgent newAgent = Instantiate(
                flockPrefab, 
                Random.insideUnitCircle * startingCount * AgentDensity,
                Quaternion.Euler(Vector3.forward * Random.Range(0f,360f)),
                transform
                );
            newAgent.name = "Agent # " + i;
            agents.Add(newAgent);
        }
    }

    void Update()
    {
        foreach (FlockAgent agent in agents) 
        {
            List<Transform> context = GetNearbyObjects(agent);
            agent.GetComponentInChildren<SpriteRenderer>().color = Color.Lerp(Color.white, Color.green, context.Count /6f);


            Vector2 move = behavior.CalculateMove(agent, context, this);
            move *= driveFactor;
            if (move.sqrMagnitude > squareMaxSpeed)
            {
                move = move.normalized * maxSpeed;
            }
            agent.Move(move);
        }
    }

    List<Transform> GetNearbyObjects(FlockAgent agent) 
    {
        List<Transform> context = new List<Transform>();
        Collider2D[] contextColliders = Physics2D.OverlapCircleAll(agent.transform.position, neighbourRadius);
        foreach (Collider2D c in contextColliders) 
        {
            if (c != agent.agentColliderGET) 
            {
                context.Add(c.transform);
            }
        
        }
        return context;
    }
}
