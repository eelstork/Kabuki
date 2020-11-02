using Active.Core; using static Active.Core.status;
using UnityEngine;
using Activ.Kabuki; using static Activ.Kabuki.VectorExt;

public class Roam : Actor{

    public bool anchored = true;
    public Vector3 origin   = Vector3.zero;
    public Transform giz;

    Vector3? target;

    override public status Step() => (target.HasValue)
    ? (Reach(target) || this["Flail"]) && Do( target = null )
    : Do( target = giz.transform.position = RandomX_Z(15f) + (anchored ? origin : this .transform.position));

}
