using UnityEngine; using Active.Loco; using Activ.Kabuki;
using Active.Input;

public class HomingRandomizer : MonoBehaviour{

    public float delay = 5f;
    public float radius = 10f;
    public Vector3 target = Vector3.zero;

    void Start(){
        if (delay > 0f) InvokeRepeating("Randomize", delay, delay);
    }

    void Update(){
        GetTap();
        var λ = GetComponent<PhysicalLocomotion>();
        Debug.DrawLine(transform .transform.position + Vector3.up, target, Color.red);
        var s = λ.MoveTo(target, 3f) && λ.Playing("Idle", λ.Idle());
    }

    void GetTap(){
        var taps = FindObjectOfType<Tap>();
        if (!taps) return ;
        var P = taps.target;
        if (P.HasValue){ target = P.Value; taps.target = null; }
    }

    void Randomize(){
        if (!enabled) return ;
        target = Random.insideUnitSphere * radius;
        target.y = 0f;
    }

}
