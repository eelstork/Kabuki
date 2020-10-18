using UnityEngine;
using Active.Core; using static Active.Core.status; using Active.Util;

namespace Activ.Kabuki{
public class XTask : UTask{

    public status Play(string anim) => Req<MecanimDriver>().Play(anim);

    public status Playing(string anim, status @while){
        if (@while.running) Req<MecanimDriver>().Play(anim);
        return @while;
    }

    public C Req<C>() where C : Component {
        var c = GetComponent<C>(); if (c != null) return c;
        return gameObject.AddComponent<C>();
    }

    public action Destroy(Transform arg){
        if (arg != null) Destroy(arg.gameObject); return @void();
    }

    public Transform θ => transform;

}}
