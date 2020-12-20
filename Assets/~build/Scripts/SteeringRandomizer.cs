using UnityEngine; using Active.Loco; using Activ.Kabuki;

public class SteeringRandomizer : MonoBehaviour{

    public float delay = 5f;
    public Vector3 aim = Vector3.forward;

    void Start(){
        if (delay > 0f) InvokeRepeating("Randomize", delay, delay);
    }

    void Update(){
        Debug.DrawRay(transform .transform.position + Vector3.up, aim, Color.cyan);
        GetComponent<PhysicalLocomotion>().Steer(aim, 3f);
    }

    void Randomize(){
        if (!enabled) return ;
        aim = Random.insideUnitSphere;
        aim.y = 0f;
        aim.Normalize();
    }

}
