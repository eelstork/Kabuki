using UnityEngine; using Active.Core; using static Active.Status;

namespace Active.Loco{
public class Homing : LocoTask{

    Vector3 target;
    float speed;

    public bool Update(Vector3 target, float speed){
        bool changed = false;
        if (this.target != target) { this.target = target; changed = true; }
        if (this.speed  != speed) { this.speed = speed; changed = true; }
        //⤴ (changed) Debug.LogError("target has changed");
        return changed;
    }

    override public status Step(PhysicalLocomotion o){
        var dist = Vector3.Distance(o.transform .transform.position, target);
        if (dist < 0.1f){
            // TODO: do not ever mutate on 'done'
            // o.Play("Idle");
            // o.motor.Stop(o.み, o.body);
            // o.rotor.Stop(o.み, o.body);
            return done()[log && "On target"];
        }
        var direction = (target - o.transform .transform.position).normalized;
        o.rotor.Update(direction, o.transform, o.body);
        // Maybe no clear LOS here since course corrections take time.
        // was o.み˙
        if (Avoidance.HasClearLOS(o.shape.bounds.center,
                                  o.body.velocity, 1f)){
            var s = speed;
            if (dist <= 1f) s *= dist;
            o.motor.Update(direction, s, o.transform, o.body);
            o.Play("Walk");
            return cont()[log && "Move"];
        }else {
            // So, if not, stop moving.
            o.motor.Stop(o.transform, o.body);
            o.Play("Idle");
            return cont()[log && "Stall"];
        }
    }

}}
