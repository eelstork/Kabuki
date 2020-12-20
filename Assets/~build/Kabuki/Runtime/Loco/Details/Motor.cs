using UnityEngine;

namespace Active.Loco{
[System.Serializable]
public class Motor{

    public float traction = 25f;
    [Range(0f, 1f)]
    public float drift = 0.5f;

    public void Update(Vector3 dir, float speed, Transform τ, Rigidbody body){
        var u = Vector3.Lerp(τ.forward, dir, drift);
        var δ = speed * u - body.velocity;
        body.AddForce(δ * body.mass * traction);
    }

    public void Stop(Transform τ, Rigidbody body){
        body.AddForce(- body.velocity * body.mass * traction);
    }

}}
