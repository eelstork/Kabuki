using UnityEngine; using static UnityEngine.Debug;
using Active.Core; using static Active.Status;

namespace Active.Loco{
public class Stopping : LocoTask{

    override public status Step(PhysicalLocomotion o){
        if (o.body  == null) LogWarning($"? rb    @{Time.frameCount}");
        else if (o.motor == null) LogWarning($"? motor @{Time.frameCount}");
        else {
            o.motor.Stop(o.transform, o.body);
            o.rotor.Stop(o.transform, o.body);
            o.Play("Idle");
        } return cont();
    }

}}
