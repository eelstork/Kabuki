using UnityEngine;
using Active.Core; using static Active.Core.status; using Activ.Kabuki;

public class Tracker : Actor{

    public Transform giz;

    override public status Step() => Reach(giz.transform.position);

}
