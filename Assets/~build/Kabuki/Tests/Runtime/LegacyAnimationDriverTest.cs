using System.Collections; using ArgEx = System.ArgumentException;
using NUnit.Framework; using UnityEngine; using UnityEngine.TestTools;
using Active.Core; using static Active.Core.status; using Active.Util;

public class LegacyAnimationDriverTest : Kabuki.Test.PlayTest{

    override protected float baseTimeScale => 12;  LegacyAnimationDriver x;

    [SetUp] public void SetupDriver(){
        var go = Create("Raptor"); go.transform.localScale = Vector3.one * 20;
        x = new LegacyAnimationDriver( go.GetComponent<Animation>() );
    }

    [Test] public void Play () => o( x.Play("Idle").running );
    //
    [UnityTest] public IEnumerator Play_once (){ yield return Complete( () => x.Play("Strike").due,        1f); }
    [UnityTest] public IEnumerator Play_loop (){ yield return Run( () => x.Play("Walk").due    , duration: 1f); }
    [UnityTest] public IEnumerator Play_clamp(){ yield return Run( () => x.Play("Clamping").due, duration: 1f); }
    //
    [Test] public void Play_emptyString_throws () => Assert.Throws<ArgEx>( () => x.Play("") );
    [Test] public void Play_nullString_throws  () => Assert.Throws<ArgEx>( () => x.Play(null ) );

    [UnityTest] public IEnumerator Loop_force () { yield return Run( () => x.Loop("Strike").ever, duration: 2f); }
    [UnityTest] public IEnumerator Loop_normal() { yield return Run( () => x.Loop("Walk").ever  , duration: 2f); }

    [Test] public void Play_animationNotFound_logsError(){
        x.Play("foo");
        LogAssert.Expect(LogType.Error,
            "The animation state foo could not be played because it "
          + "couldn't be found!\nPlease attach an animation clip with"
          + " the name 'foo' or call this function only for existing "
          + "animations.");
    }

}
