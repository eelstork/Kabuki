using System; using UnityEngine; using Active.Core; using static Active.Status;
using Activ.Kabuki; using Active.Util;

namespace Active.Loco{
public class PhysicalLocomotion : Activ.Kabuki.XTask, Locomotion{

    public Motor motor = new Motor();
    public Rotor rotor = new Rotor();
    public bool setupCapsule = true;
    LocoTask @delegate;
    int cframe;

    Avoidance2 avoidance = new Avoidance2();
    status state;

    void Awake(){
        if (setupCapsule)CapsuleBuilder.BuildAndResize(transform);
        var rb = GetComponent<Rigidbody>();
        if (!rb){
            rb =gameObject.AddComponent<Rigidbody>();
            rb.mass = 50;
            rb.constraints = RigidbodyConstraints.FreezeRotationX
                           | RigidbodyConstraints.FreezeRotationZ;

        }
    }

    public status Idle(){
        cframe = Time.frameCount;
        @delegate = new Stopping(); return cont();
    }

    // <Locomotion>
    public status Move(Vector3 u, float speed, float los) => Steer(u, speed, los);

    public status Steer(Vector3 target, float speed, float maxDist = 3f){
        cframe = Time.frameCount;
        Vector3 direction = target;
        status av = avoidance.Correct(transform, ref direction, maxDist);
        if (av.failing){
            direction = Vector3.zero;
            if (!(@delegate is Stopping)) @delegate = new Stopping();
            return fail()[log && "No clear course"];
        }else{
            bool changed = false;
            if (!(@delegate is Steering)){
                @delegate = new Steering(); changed = true;
            }
            changed |= (@delegate as Steering).Update(direction, speed);
            return changed ? cont() : state;
        }
    }

    // <Locomotion>
    // NOTE: do not modify state when done (no idle stance/animation)
    public status MoveTowards(Vector3 target, float dist, float speed)
        => (transform.Dist(target) <= dist) || MoveTo(target, speed);

    public status MoveTowards(Transform target, float dist, float speed){
        //ebug.Log($"(Physics) Move towards {target}, dist:{dist}, speed:{speed} @{Time.frameCount}");
        return (transform.Dist(target) <= dist) || MoveTo(target .transform.position, speed, target);
    }

    // <Locomotion>
    public status MoveTo(Vector3 target, float speed) => MoveTo(target, speed, null);

    public status MoveTo(Vector3 target, float speed, Transform ignore){
        // We're going to check first whether we can move towards
        // the target.
        cframe = Time.frameCount;
        Vector3 u0 = transform.Dir(target, planar: true);
        Vector3 u = u0;
        status av = avoidance.Correct(transform, ref u, transform.Dist(target), ignore);
        if (av.failing){
            if (!(@delegate is Stopping)) @delegate = new Stopping();
            return fail()[log && "No clear course"];
        }
        bool changed = false;
        // Now uh... this is a little tricky. If we have an
        // avoidance vector then we probably should not want the
        // homing delegate. Otherwise we use that.
        if (u == u0){
            if (!(@delegate is Homing)){
                @delegate = new Homing(); changed = true;
            }
            changed |= (@delegate as Homing).Update(target, speed);
            return changed ? cont() : state;
        }else {
            if (!(@delegate is Steering)){
                @delegate = new Steering(); changed = true;
            }
            changed |= (@delegate as Steering).Update(u, speed);
            return changed ? cont() : state;
        }
    }

    override public status Step(){
        if (Time.frameCount > cframe + 1) return done();
        if (!grounded){
            Play("Flail");
            return state = fail()[log && "Not grounded"]; }
        Play("Walk");
        return ε(state = @delegate != null ? @delegate.Step(this)
                                   : fail()[log && "No delegate"]);
    }

    bool grounded{ get{
        var P = shape.bounds.center;
        var h = shape.bounds.extents.y * 1.1f;
        RaycastHit hit;
        bool didHit = Physics.Raycast(P, Vector3.down, out hit, h);
        return didHit;
    }}

    public Collider  shape => GetComponent<Collider>();
    public Rigidbody body  => GetComponent<Rigidbody>();

}}
