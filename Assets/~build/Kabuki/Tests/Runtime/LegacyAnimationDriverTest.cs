using System.Collections; using ArgEx = System.ArgumentException;
using NUnit.Framework; using UnityEngine; using UnityEngine.TestTools;
using Active.Core; using static Active.Core.status; using Active.Util;

public class LegacyAnimationDriverTest : Kabuki.Test.PlayTest{

    LegacyAnimationDriver2 x;

    [SetUp] public void SetupDriver(){
        var go = Create("Raptor"); go.transform.localScale = Vector3.one * 20;
        x = new LegacyAnimationDriver2( go.GetComponent<Animation>() );
    }

    // Play ----------------------------------------------------------

    [Test] public void Play() => o( x.Play("Idle").running );

    [UnityTest] public IEnumerator PlayFully_wrapModeOnce()
    { yield return Complete( () => x.Play("Strike"), timeout: 1f); }

    [UnityTest] public IEnumerator Loop_force(){ yield return Run( () => x.Loop("Strike"), duration: 2f); }

    [UnityTest] public IEnumerator Loop_normal(){ yield return Run( () => x.Loop("Walk"), duration: 2f); }

    [UnityTest] public IEnumerator Play_loop() { yield return Run( () => x.Play("Walk"), duration: 1f); }

    [UnityTest] public IEnumerator Play_clampForever()
    { yield return Run( () => x.Play("Clamping"), duration: 1f); }

    [Test] public void Play_emptyString_throws() => Assert.Throws<ArgEx>( () => x.Play("") );

    [Test] public void Play_nullString_throws() => Assert.Throws<ArgEx>( () => x.Play(null) );

    [Test] public void Play_animationNotFound_logsError(){
        x.Play("foo");
        LogAssert.Expect(LogType.Error,
            "The animation state foo could not be played because it "
          + "couldn't be found!\nPlease attach an animation clip with"
          + " the name 'foo' or call this function only for existing "
          + "animations.");
    }

}
