using UnityEngine; using static UnityEngine.Mathf;

namespace Active.Loco{
[System.Serializable] public class Rotor{

    public float speed = 10f;
    public float traction = 25f;
    public float ρ = 30f;

    public void UpdateWithTarget(Vector3 u, Transform τ, Rigidbody body){}

    public void Update(Vector3 u, Transform τ, Rigidbody body){
        var α = Vector3.SignedAngle(u, τ.forward, axis: Vector3.up);
        var s = (Abs(α) < ρ) ? Abs(α/ρ) : 1f;
        var w = α < 0 ? Vector3.up : Vector3.down;
        body.AddTorque((w * speed * s - body.angularVelocity) * traction );
    }

    public void Stop(Transform τ, Rigidbody body)
        => body.AddTorque( - body.angularVelocity * traction );

}}
