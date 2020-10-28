using System; using System.Collections; using System.Collections.Generic;
using UnityEngine; using UnityEngine.TestTools; using NUnit.Framework;
using Active.Core; using Activ.Kabuki; using Active.Util;

namespace Kabuki.Test{ public abstract class ActorTest : PlayTest{

    protected Actor actor;

    protected abstract string ActorName { get; }
    protected abstract float ActorSize { get; }

    protected virtual float fadeLength => 0.3f;
    override protected float baseTimeScale => 12;

    [SetUp] public void SetupActor(){
        actor = ActorSetup.Setup( Create(ActorName), ActorSize )
                          .GetComponent<Actor>();
        actor.animDriver.fadeLength = fadeLength;
    }

    // --------------------------------------------------------------

    [UnityTest] public IEnumerator Gesture(){
        var x = CreateEmpty( Vector3.right * 2f );
        yield return Complete( () => actor["Greet", x], 6f);
        o( actor.transform.Look(x) < 1f);
        yield return null;
    }

    [UnityTest] public IEnumerator Give_accept(){
    if (Skip()) yield break;
        var ball  = Ball(0.2f, parent: actor.transform);
        var other = Other("Mimi");
        yield return Complete(
            () => (actor.Give(ball, other) && other.Has(ball))
               % other.Take(), 10f);
    }

    [UnityTest] public IEnumerator Give_ignore(){
        var ball = Ball(0.2f, parent: actor.transform);
        var other = Other();
        yield return Run( () => actor.Give(ball, other), 5f);
    }

    [UnityTest] public IEnumerator Grab(){
    if (Skip()) yield break;
        var x = Ball(0.3f);
        x.transform.position = Vector3.right * 2 + Vector3.up * 0.15f;
        yield return Complete( () => actor.Grab(x), 5f);
        o( x.transform.parent, actor.rightHold);
    }

    [UnityTest] public IEnumerator Ingest(){
    if (Skip()) yield break;
        actor.transform.forward = Vector3.back;
        var food = Ball( 0.15f, parent: actor.transform );
        yield return Complete( () => actor.Ingest(food), 5f);
        o( food == null );
    }

    [UnityTest] public IEnumerator Idle(){
        yield return Run( () => actor.Idle, 3f);
    }

    [UnityTest] public IEnumerator LookAt(){
        var x = CreateEmpty( Vector3.right * 2f );
        yield return Run( () => actor.LookAt(x), 2f);
        o( actor.transform.Look(x) < 1f);
    }

    [UnityTest] public IEnumerator Push(){
    if (Skip()) yield break;
        var block = Box(1f, 2f, 1.5f); block.gameObject.AddComponent<Rigidbody>();
        yield return Complete( () => actor.Push(block), 20f);
    }

    [UnityTest] public IEnumerator Reach(){
        var x = CreateEmpty( Vector3.forward * 2f );
        yield return Complete( () => actor.Reach(x), 4f);
        o( actor.transform.Dist(x) < 1f);
    }

    [UnityTest] public IEnumerator Strike(){
        var other = Other().transform;
        yield return Complete( () => actor.Strike(other), 5f);
    }

    [UnityTest] public IEnumerator Tell(){
    if (Skip()) yield break;
        var other = Other().transform;
        yield return Complete( () => actor.Tell(other, "Hello"), 4f);
    }

    [UnityTest] public IEnumerator Throw(){
    if (Skip()) yield break;
        var ball = Box(0f, 0f, 0.1f); ball.gameObject.AddComponent<Rigidbody>();
        yield return Complete( () => actor.Throw(ball, new Vector3(1, 0, 1)), 4f);
    }

    // --------------------------------------------------------------

    Actor Other(string name = "Bucks"){
        var x = ActorSetup.Setup(Create(ActorName,
                                      Vector3.right * 2f), ActorSize).GetComponent<Actor>();
        x.gameObject.name = name; return x;
    }

}}
