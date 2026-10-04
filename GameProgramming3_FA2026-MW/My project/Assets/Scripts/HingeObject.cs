using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class HingeObject : MonoBehaviour
{
    public float minAngle = 0f; //min hinge rotation
    public float maxAngle = 90f; //max hinge rotation
    public bool useSpring = true; //if true, the hinge will spring back toward the rest angle when released
    public float springTargetAngle = 0f; //the angle the spring tries to return to
    public float springForce = 50f; //how strong the spring force is
    public float springDamper = 5f; //how much the spring dampens
    //events
    public UnityEvent onReachMax; //fired when the hinge reaches or passes the max angle
    public UnityEvent onReachMin; //fired when the hinge returns to or passes the min angle
    public float eventThreshold = 5f; //how close to the limit angle before the event fires (degrees)

    HingeJoint hinge;
    bool maxEventFired = false;
    bool minEventFired = false;

    void Awake()
    {
        hinge = GetComponent<HingeJoint>();
        ConfigureHinge();
    }

    // Update is called once per frame
    void Update()
    {
        //check if we have hit the limits & should fire puzzleEvents
        float currentAngle = hinge.angle;

        if (!maxEventFired && currentAngle >= maxAngle - eventThreshold)
        {
            maxEventFired = true;
            minEventFired = false;
            onReachMax?.Invoke();
            Debug.Log(gameObject.name + " hinge reached MAX angle.");
        }
        if(!minEventFired && currentAngle <= minAngle + eventThreshold)
        {
            minEventFired = true;
            maxEventFired = false;
            onReachMin?.Invoke();
            Debug.Log(gameObject.name + "hinge reaches MIN angle.");
        }
    }

    //configure hinge
    //sets up joint limits & spring thru code
    void ConfigureHinge()
    {
        //limits
        //jointLimits is a struct we have to set all fields then assign it back
        JointLimits limits = hinge.limits;
        limits.min = minAngle;
        limits.max = maxAngle;
        limits.bounciness = 0f;
        limits.bounceMinVelocity = 0.2f;
        hinge.limits = limits;
        hinge.useLimits = true;

        if (useSpring)
        {
            JointSpring spring = hinge.spring;
            spring.targetPosition = springTargetAngle;
            spring.spring = springForce;
            spring.damper = springDamper;
            hinge.spring = spring;
            hinge.useSpring = true;
        }
        else
        {
            hinge.useSpring = false;
        }
    }
    public void DriveToMax()
    {
        SetMotorTarget(maxAngle);
    }

    public void DriveToMin()
    {
        SetMotorTarget(minAngle);
    }

    void SetMotorTarget(float targetAngle)
    {
        JointMotor motor = hinge.motor;
        //motor velocity direction determines which way it moves
        motor.targetVelocity = targetAngle > hinge.angle ? 50f : -50f; //shorthand if statement (if target angle is > hinge.angle. Set to 50 & vice versa)
        motor.force = 100f;
        motor.freeSpin = false;
        hinge.motor = motor;
        hinge.useMotor = true;
    }    
}
