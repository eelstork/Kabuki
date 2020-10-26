using System.Collections;
using System.Linq;
using UnityEngine; using UnityEngine.TestTools;
using NUnit.Framework;
using Active.Core; using Activ.Kabuki;
using Active.Core.Details;
using Active.Util;

namespace Kabuki.Test{
public abstract class ActorTest : PlayTest{

    protected Actor actor;
    int frame;

    protected abstract string ActorName{ get; }
    protected abstract float ActorSize{ get; }
    protected virtual string[] skip{ get; }

    [SetUp] public void _ActorSetup(){
        actor = ActorSetup.Setup( Create(ActorName), ActorSize )
                                                             .GetComponent<Actor>();
        Active.Core.Details.RoR.Enter(this, frame);
    }

    [TearDown] public void _ActorTearDown () => Active.Core.Details.RoR.Exit(this, ref frame);

    // --------------------------------------------------------------

    [UnityTest] public IEnumerator Gesture(){
        var x = CreateEmpty( Vector3.right * 2f );
        yield return Complete( () => actor["Greet", x], 6f, 4);
        o( actor.transform.Look(x) < 1f);
    }

    [UnityTest] public IEnumerator Give_accept(){
        if (Skip(nameof(Give_accept))) yield break;
        var ball  = Ball(0.2f, parent: actor.transform);
        var other = Other("Mimi");
        yield return Complete(
            () => (actor.Give(ball, other) && other.Has(ball))
               % other.Take(),
            10f, 2
        );
    }

    [UnityTest] public IEnumerator Give_ignore(){
        var ball = Ball(0.2f, parent: actor.θ);
        var other = Other();
        yield return Run( () => actor.Give(ball, other), 5f, 3);
    }

    [UnityTest] public IEnumerator Grab(){
        if (Skip(nameof(Grab))) yield break;
        var x = Ball(0.3f);
        x.transform.position = Vector3.right * 2 + Vector3.up * 0.15f;
        yield return Complete( () => actor.Grab(x), 5f, 8 );
        o( x.transform.parent, actor.rightHold);
    }

    [UnityTest] public IEnumerator Ingest(){
        if (Skip(nameof(Ingest))) yield break;
        actor.transform.forward = Vector3.back;
        var food = Ball( 0.15f, parent: actor.transform );
        yield return Complete( () => actor.Ingest(food), 5f, 4 );
        o( food == null );
    }

    [UnityTest] public IEnumerator Idle(){ yield return Run( () => actor.Idle, 3, 3 ); }

    [UnityTest] public IEnumerator LookAt(){
        var x = CreateEmpty( Vector3.right * 2f );
        //o( actor.transform.Look(x), 90f);
        bool didPlayRotationAnim = false;
        //
        Print($"Start @{Time.frameCount}");
        int i; for (i = 0; i < 500; i++){
            var s = actor.LookAt(x);
            didPlayRotationAnim |= NowPlaying("Walk");
            o ( s.running );
            if ( didPlayRotationAnim && NowPlaying("Idle") ){
                //rint("Exit loop because did play rot, now idle");
                break;
            }
            //rint($"It {i}");
            yield return null;
        }
        //rint($"Frame {Time.frameCount}");
        //rint($"Did play rotation anim? {didPlayRotationAnim}");
        //rint($"Angle {actor.transform.Look(x)}");
        o( didPlayRotationAnim );
        o( actor.transform.Look(x) < 1f);
        //rint($"Done in {Time.time:0.##}s, {i} frames.");
        yield return null;
    }

    [UnityTest] public IEnumerator Play(){
        var t0 = Time.time ;
        actor.transform.forward = Vector3.right;
        yield return Complete( () => actor["Strike"], 5f, 2);
    }

    [UnityTest] public IEnumerator Push(){
        if (Skip(nameof(Push))) yield break;
        var block = Box(1f, 2f, 1.5f); block.gameObject.AddComponent<Rigidbody>();
        yield return Complete( () => actor.Push(block), 20f, 2);
    }

    [UnityTest] public IEnumerator Reach(){
        var x = CreateEmpty( Vector3.forward * 2f );
        yield return Complete( () => actor.Reach(x), 4f, 2);
        o( actor.transform.Dist(x) < 1f);
    }

    [UnityTest] public IEnumerator Strike(){
        var other = Other().transform;
        yield return Complete( () => actor.Strike(other), 5f, 2);
    }

    [UnityTest] public IEnumerator Tell(){
        if (Skip(nameof(Tell))) yield break;
        var other = Other().transform;
        yield return Complete( () => actor.Tell(other, "Hello"), 4f, 2);
    }

    [UnityTest] public IEnumerator Throw(){
        if (Skip(nameof(Throw))) yield break;
        var ball = Box(0f, 0f, 0.1f); ball.gameObject.AddComponent<Rigidbody>();
        yield return Complete( () => actor.Throw(ball, Vector3.right + Vector3.forward),
                           4f, 2);
    }

    // --------------------------------------------------------------

    Actor Other(string name = "Bucks"){
        var x = ActorSetup.Setup(
                      Create(ActorName, Vector3.right * 2f), ActorSize).GetComponent<Actor>();
        x.name = name;
        return x;
    }

    bool Skip(string test) => skip?.Contains(test) ?? false;

    bool NowPlaying(string anim) => actor.GetComponent<AnimationDriver>().NowPlaying(anim);

    // Query several indices?
    bool IsNext(string anim)
        => actor.GetComponent<Animator>().GetNextAnimatorStateInfo(0).IsName(anim);

}}
