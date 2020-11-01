using Active.Core; using static Active.Core.status;
using UnityEngine;
using Activ.Kabuki; using static Activ.Kabuki.VectorExt;

public class Roam : Actor{

    Vector3 target; public Transform giz;

    override public status Step() => Reach(target) && Do( target = this .transform.position + RandomX_Z(5f) );

}
