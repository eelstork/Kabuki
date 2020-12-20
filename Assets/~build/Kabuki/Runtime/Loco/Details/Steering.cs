using UnityEngine; using Active.Core; using static Active.Status;

namespace Active.Loco{
public class Steering : LocoTask{

    Vector3 direction;
    float speed;

    public bool Update(Vector3 u, float speed){
        bool changed = false;
        if (direction != u)   { direction = u;   changed = true; }
        if (this.speed != speed) { this.speed = speed; changed = true; }
        return changed;
    }

    override public status Step(PhysicalLocomotion o){
        //
        o.rotor.Update(direction, o.transform, o.body);
        // Maybe no clear LOS here since course corrections take time.
        if (Avoidance.HasClearLOS(o.transform .transform.position, o.body.velocity, 1f)){
            o.motor.Update(direction, speed, o.transform, o.body);
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
