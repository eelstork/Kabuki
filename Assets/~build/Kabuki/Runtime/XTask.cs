using Ex = System.Exception; using UnityEngine;
using Active.Core; using static Active.Core.status; using Active.Util;

namespace Activ.Kabuki{
public class XTask : UTask{

    AnimationDriver _animDriver;

    public pending Play(string anim) => animDriver.Play(anim);

    public status Playing(string anim,  status @while){
        if (@while.running) Play(anim);
        return @while;
    }

    public C Req<C>() where C : Component {
        var c = GetComponent<C>(); if (c != null) return c;
        return gameObject.AddComponent<C>();
    }

    public action Destroy(Transform arg){
        if (arg != null) Destroy(arg.gameObject); return @void();
    }

    // PRIVATE ------------------------------------------------------

    AnimationDriver SetupAnimDriver(){
        Animator  ator  = null;
        Animation ation = null;
        if (ator  = GetComponent<Animator>()  )
            return new MecanimDriver(ator);
        else if (ation = GetComponent<Animation>() )
            return new LegacyAnimationDriver(ation);
        else
            throw new Ex("No Animation or Animator component");
    }

    // --------------------------------------------------------------

    public Transform θ => transform;
    public AnimationDriver animDriver => _animDriver != null ? _animDriver
        : (_animDriver = SetupAnimDriver());

}}
