using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlanetScript : MonoBehaviour
{

    [Header("Base values")]
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 startVelocity;
    [SerializeField] private Vector3 startAcceleration;

    [Header("Physics attributes")]
    [SerializeField] private float mass;
    public float Mass { get { return mass; } }
    private Vector3 position;
    public Vector3 Position { get { return position; } }
    private Vector3 velocity;

    private const float G = 4 * Mathf.PI * Mathf.PI; // Converted to AU^3/yr^2*M

    void Start()
    {
        position = startPosition;
        velocity = startVelocity;
        transform.position = position;
    }

    void Update()
    {
        (position, velocity) = RungeKutta4(position, velocity);
        transform.position = position;
    }

    private (Vector3, Vector3) differentialFunction(Vector3 _position, Vector3 _velocity) => (_velocity, Acceleration(_position));

    private (Vector3, Vector3) RungeKutta4(Vector3 _position, Vector3 _velocity)
    {
        Vector3 K1position, K1velocity, K2position, K2velocity, K3position, K3velocity, K4position, K4velocity;
        float dt = PlanetManager.Instance.StepTime;

        (K1position, K1velocity) = differentialFunction(_position, _velocity);
        (K2position, K2velocity) = differentialFunction(_position + dt * 0.5f * K1position, _velocity + dt * 0.5f * K1velocity);
        (K3position, K3velocity) = differentialFunction(_position + dt * 0.5f * K2position, _velocity + dt * 0.5f * K2velocity);
        (K4position, K4velocity) = differentialFunction(_position + dt * K3position, _velocity + dt * K3velocity);

        Vector3 newPosition = _position + dt / 6 * (K1position + 2 * K2position + 2 * K3position + K4position);
        Vector3 newVelocity = _velocity + dt / 6 * (K1velocity + 2 * K2velocity + 2 * K3velocity + K4velocity);

        return (newPosition, newVelocity);
    }

    private Vector3 Acceleration(Vector3 _position)
    {
        Vector3 newAcceleration = Vector3.zero;
        foreach (PlanetScript p in PlanetManager.Instance.Planets)
            if (p != this)
                newAcceleration +=  (G * p.Mass / Mathf.Pow(Vector3.Distance(_position, p.Position), 2)) * (p.Position - _position).normalized;
        return newAcceleration;
    }

    public void Reset()
    {
        position = startPosition;
        velocity = startVelocity;
        transform.position = position;
        this.GetComponent<TrailRenderer>().Clear();
    }
}
