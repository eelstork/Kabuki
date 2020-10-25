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
        int i; for (i = 0; i < 400; i++){
            var s = actor["Take", x];
            if (s.complete && IsNext("IdleComeHere")) break;
            yield return null;
        }
        o( actor.transform.Look(x) < 1f); yield return null;
    }

    [UnityTest] public IEnumerator Give_accept(){
        if (Skip(nameof(Give_accept))) yield break;
        var history = new History();
        var ball  = Ball(0.2f, parent: actor.transform);
        var other = Other;
        other.gameObject.name = "Mimi";
        status s = status.fail(); int i;
        for (i = 0; i < 1000; i++){
            s = actor.Give(ball, other);
            history.Log(StatusFormat.CallTree(s), actor.transform);
            other.Take();
            if (other.Has(ball) && s.complete) break;
            yield return null;
        }
        Assert.That(i, Is.LessThan(1000) );
        Assert.That(other.Has(ball));
        Debug.Log(history);
    }

    [UnityTest] public IEnumerator Give_ignore(){
        var ball = Ball(0.2f, parent: actor.θ);
        var other = Other;
        status s = status.fail(); int i;
        for (i = 0; i < 300; i++){
            s = actor.Give(ball, other);
            o( s.running );
            yield return null;
        }
        o( s.running );
    }

    Actor Other
    => ActorSetup.Setup(Create(ActorName, Vector3.right * 2f), ActorSize).GetComponent<Actor>();

    [UnityTest] public IEnumerator Grab(){
        if (Skip(nameof(Grab))) yield break;
        var x = Ball(0.3f);
        x.transform.position = Vector3.right * 2 + Vector3.up * 0.15f;
        int i; for (i = 0; i < 500; i++){
            var s = actor.Grab(x);
            if (s.complete) break;
            yield return null;
        }
        o( x.transform.parent, actor.rightHold);
        o(i < 500);
        Print($"Done in {Time.time:0.##}s, {i} frames.");
    }

    [UnityTest] public IEnumerator Ingest(){
        if (Skip(nameof(Ingest))) yield break;
        actor.transform.forward = Vector3.back;
        var food = Ball( 0.15f, parent: actor.transform );
        var s = status.cont();
        while (s.running){
            s = actor.Ingest(food);
            yield return null;
        }
        o( s.complete );
        o( food == null );
    }

    [UnityTest] public IEnumerator Idle(){
        for (int i = 0; i < 5; i++){
            o( actor.Idle.running );
            o( NowPlaying("Idle") );
            yield return null;
        }
    }

    [UnityTest] public IEnumerator LookAt(){
        var x = CreateEmpty( Vector3.right * 2f );
        o( actor.transform.Look(x), 90f);
        bool didPlayRotationAnim = false;
        //
        //rint($"Start @{Time.frameCount}");
        int i; for (i = 0; i < 100; i++){
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
        status s = status.cont();
        int i; for (i = 0; i < 600; i++){
            s = actor.Play("Strike");
            if (s.complete) break;
            else o( s.running );
            yield return null;
        }
        o(s.complete);
        o(i < 600);
        o(Time.time - t0 < 6f);
        Print($"Done in {Time.time:0.##}s, {i} frames.");
    }

    [UnityTest] public IEnumerator Push(){
        if (Skip(nameof(Push))) yield break;
        var block = Box(1f, 2f, 1.5f); block.gameObject.AddComponent<Rigidbody>();
        var s = status.cont();
        for (int i = 0; i < 2000; i++ ){
            s = actor.Push(block);
            yield return null;
            if (s.complete) break;
        }
        o( s.complete );
    }

    [UnityTest] public IEnumerator Reach(){
        var x = CreateEmpty( Vector3.forward * 2f );
        for (;;){
            var s = actor.Reach(x);
            if (s.complete) break;
            yield return null;
        }
        o( actor.transform.Dist(x) < 1f);
    }

    [UnityTest] public IEnumerator Strike(){
        var other = Other.θ;
        var s = status.cont();
        while (s.running){
            s = actor.Strike(other);
            yield return null;
        }
        o( s.complete );
    }

    [UnityTest] public IEnumerator Tell(){
        if (Skip(nameof(Tell))) yield break;
        var other = Other.θ;
        var s = status.cont();
        while (s.running){
            s = actor.Tell(other, "Hello");
            yield return null;
        }
        o( s.complete );
    }

    [UnityTest] public IEnumerator Throw(){
        if (Skip(nameof(Throw))) yield break;
        var ball = Box(0f, 0f, 0.1f); ball.gameObject.AddComponent<Rigidbody>();
        var s = status.cont();
        for (int i = 0; i < 350; i++){
            s = actor.Throw(ball, Vector3.right + Vector3.forward);
            yield return null;
        }
        o( s.complete );
    }

    // --------------------------------------------------------------

    bool Skip(string test) => skip?.Contains(test) ?? false;

    bool NowPlaying(string anim)
        => actor.GetComponent<AnimationDriver>().NowPlaying(anim);

    // Likely should query several indices here
    bool IsNext(string anim)
        => actor.GetComponent<Animator>().GetNextAnimatorStateInfo(0).IsName(anim);

}}
