using UnityEngine; using Active.Core; using static Active.Status;

namespace Active.Loco{
public abstract class LocoTask : Gig{

    public abstract status Step(PhysicalLocomotion o);

}}
