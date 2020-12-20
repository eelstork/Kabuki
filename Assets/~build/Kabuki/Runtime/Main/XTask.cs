using Ex = System.Exception; using UnityEngine;
using Active.Core; using static Active.Core.status; using Active.Util;

namespace Activ.Kabuki{
public class XTask : UTask{

    int frame = -1;
    string currentAnimation;
    public bool ward;
    AnimationDriver _animDriver;

    public pending Play(string anim){
        FrameCheck(anim);
        return animDriver.Play(anim);
    }

    public status Playing(string anim,  status @while){
        if (@while.running){
            FrameCheck(anim);
            Play(anim);
        }
        return @while;
    }

    public C Req<C>() where C : Component {
        var c = GetComponent<C>(); if (c != null) return c;
        return gameObject.AddComponent<C>();
    }

    public action Delete(Transform arg){
        Debug.Log("Destroy thing: " + arg);
        if (arg != null) DestroyImmediate(arg.gameObject); return @void();
    }

    void FrameCheck(string anim){
        if (!ward) return ;
        int i = Time.frameCount;
        if (i == frame) throw
            new Ex($"Playing {currentAnimation} but requested {anim}");
        frame = i;
        currentAnimation = anim;
    }

    // PRIVATE ------------------------------------------------------

    void Start(){
        foreach (var s in GetComponents<XTask>()) if (s != this){
            _animDriver = s.animDriver; break;
        }
    }

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
