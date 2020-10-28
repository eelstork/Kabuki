using System.Collections; using ArgEx = System.ArgumentException;
using NUnit.Framework; using UnityEngine; using UnityEngine.TestTools;
using Active.Core; using static Active.Core.status; using Active.Util;

public class MecanimDriverTest : Kabuki.Test.PlayTest{

    MecanimDriver2 x;
    Animator animator;

    [SetUp] public void SetupDriver(){
        animator = Create("Dummy").GetComponent<Animator>();
        x = new MecanimDriver2( animator );
    }

    // ---------------------------------------------------------------

    [Test] public void RootMotionOff () => o( animator.applyRootMotion, false );

    // Play ----------------------------------------------------------

    [Test] public void Play() => o( x.Play("Walk").running );

    [UnityTest] public IEnumerator Play_returnsContWhileTransitioning(){
        //∙ t = ⒯ ;
        yield return Run( () => x.Play("Strike"), duration: 0.3f);
        //⟳ (ᆞ i = 0; i < 30; i++ ){
        //    o( x.Play("Strike").running );
        //    ⟆
        //}
    }

    [UnityTest] public IEnumerator Play_once()
    { yield return Complete( () => x.Play("Strike"), timeout: 10f); }

    [UnityTest] public IEnumerator Play_grab()
    { yield return Complete( () => x.Play("Grab"), timeout: 8f); }

    [UnityTest] public IEnumerator Play_loop()
    { yield return Run( () => x.Play("Walk"), duration: 2f); }

    [UnityTest] public IEnumerator Loop_force(){ yield return Run( () => x.Loop("Strike"), duration: 5f); }

    [UnityTest] public IEnumerator Loop_normal(){ yield return Run( () => x.Loop("Walk"), duration: 5f); }

    // In Mecanim there is no direct equivalent to "clamp forever"
    // ⏚ Play_wrapModeClampForever()
    // { ⟾ Complete( ⎚ x.Play("KO"), timeout: 10f); }

    [Test] public void Play_emptyString_throws() => Assert.Throws<ArgEx>( () => x.Play("") );

    [Test] public void Play_nullString_throws() => Assert.Throws<ArgEx>( () => x.Play(null) );

    [Test] public void Play_animationNotFound_logsWarning(){
        x.Play("foo");
        LogAssert.Expect(LogType.Warning,
            "Animator.GotoState: State could not be found");
      }

}
