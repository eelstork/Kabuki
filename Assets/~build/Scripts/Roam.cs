using Active.Core; using static Active.Core.status;
using UnityEngine;
using Activ.Kabuki; using static Activ.Kabuki.VectorExt;

public class Roam : Actor{

    public bool anchored = true;
    public Vector3 origin   = Vector3.zero;
    public Transform giz;

    Vector3? target;

    void Start () => loco = new Locomotion();

    override public status Step() => (Eat() || this["Flail"]) && Retarget();

    status Eat() => (target != null) ? Reach(target) && this["*Munch*"] : done();

    action Retarget()
    => Do( target = giz.transform.position = RandomX_Z(15f) + (anchored ? origin : this .transform.position) );

}
