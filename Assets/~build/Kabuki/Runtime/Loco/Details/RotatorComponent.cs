using UnityEngine; using static UnityEngine.Mathf;

public class RotatorComponent : MonoBehaviour{

    public Transform target;
    public float speed = 10f;
    public float traction = 50f;
    public float ρ = 15f;
    [Header("Info")]
    public float scalar = 1f;

    void FixedUpdate(){
        var u = (target .transform.position - transform .transform.position).normalized; u.y = 0f;
        var α = Vector3.SignedAngle(u, transform.forward, axis: Vector3.up);
        var s = (Abs(α) < ρ) ? Abs(α/ρ) : 1f;
        var w = α < 0 ? Vector3.up : Vector3.down;
        var body = GetComponent<Rigidbody>();
        var w0 = body.angularVelocity;
        body.AddTorque((w * speed * s - w0) * traction );
    }

    void FixedUpdate2(){
        var u = (target .transform.position - transform .transform.position).normalized; u.y = 0f;
        var v = transform.forward;
        var w = Vector3.Cross(v, u);
        //torqueMagnitude = w❚;
        //Debug.DrawRay(み˙, w, Color.magenta);
        var body = GetComponent<Rigidbody>();
        var w0 = body.angularVelocity;
        //∙ m = Pow(w❚, pow);
        //w = w¹ ᐧ m;
        body.AddTorque((w * speed - w0) * traction );
    }

}
