using Active.Core; using static Active.Core.status;
using UnityEngine;
using Activ.Kabuki; using static Activ.Kabuki.VectorExt;

public class Roam : Actor{

    public bool anchored = true;
    public Vector3 origin   = Vector3.zero;
    public Transform giz;

    Vector3? target;

    override public status Step(){
        if (target.HasValue) return (~ Reach(target)).due && Do( target = null );
        else {
            target = Target();
            giz.transform.position = target.Value; return cont();
        }
    }

    Vector3 Target () => RandomX_Z(15f) + (anchored ? origin : this .transform.position);

}
