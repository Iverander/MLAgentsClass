using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class basicAgent : Agent
{
    [SerializeField] Transform target;
    [SerializeField] float speed;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {

        transform.localPosition = Vector3.up;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 move = new Vector3
            (
            actions.ContinuousActions[0],
            0,
            actions.ContinuousActions[1]
            );
        print(move);

        rb.transform.position += move * Time.deltaTime * speed;

        SetReward(-Vector3.Distance(transform.localPosition, target.localPosition) * Time.deltaTime);
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continousActions = actionsOut.ContinuousActions;
        continousActions[0] = Input.GetAxisRaw("Horizontal");
        continousActions[1] = Input.GetAxisRaw("Vertical");
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Reward"))
        {
            target.localPosition = new Vector3(Random.Range(-7, 7), 1, Random.Range(-4, 4));
            SetReward(+10f);
            EndEpisode();
        }
        if (other.CompareTag("Punishment"))
        {
            SetReward(-10f);
            EndEpisode();
        }
    }
}
