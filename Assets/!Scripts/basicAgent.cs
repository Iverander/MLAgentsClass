using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.UIElements;

public class basicAgent : Agent
{
    [SerializeField] Transform target;
    [SerializeField] float speed;
    [SerializeField] float rotSpeed;
    [SerializeField] float jumpForce;

    Vector3 startPos;

    float jumpCooldown;

    Rigidbody rb;

    [SerializeField]LayerMask groundLayer;

    Vector3 groundedCheckPosition { get { return transform.position + Vector3.down; } }
    bool grounded
    {
        get { return Physics.CheckSphere(groundedCheckPosition, .2f, groundLayer); }
    }

    RaycastHit lookingAt;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.localPosition;
    }
    private void Update()
    {
        Physics.Raycast(transform.position, transform.forward, out lookingAt, 10);
        Debug.DrawLine(transform.position, lookingAt.point);

        if (jumpCooldown > 0)
            jumpCooldown -= Time.deltaTime;
    }
    public override void OnEpisodeBegin()
    {

        transform.localPosition = startPos;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.eulerAngles);
        sensor.AddObservation(lookingAt.point);
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

        Vector3 rotate = new Vector3
        (
        0,
        actions.ContinuousActions[3],
        0
        );

        if (grounded && jumpCooldown <= 0)
        {
            //rb.velocity = Vector3.zero;
            rb.velocity = Vector3.up * actions.DiscreteActions[2] * jumpForce;

            jumpCooldown = .1f;
        }

        transform.position += move * Time.deltaTime * speed;
        transform.eulerAngles += rotate * Time.deltaTime * rotSpeed;

        SetReward(-Vector3.Distance(transform.localPosition, target.localPosition) * 10 * Time.deltaTime);
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continousActions = actionsOut.ContinuousActions;
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        continousActions[0] = Input.GetAxisRaw("Horizontal");
        continousActions[1] = Input.GetAxisRaw("Vertical");
        discreteActions[2] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Reward"))
        {
            //target.localPosition = new Vector3(Random.Range(-7, 7), Random.Range(0, 2.5f), Random.Range(-4, 4));
            SetReward(+100f);
            EndEpisode();
        }
        if (other.CompareTag("Punishment"))
        {
            SetReward(-10f);
            EndEpisode();
        }
    }
}
