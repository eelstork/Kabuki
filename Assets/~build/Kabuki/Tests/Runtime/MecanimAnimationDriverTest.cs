using System.Collections; using ArgEx = System.ArgumentException;
using NUnit.Framework; using UnityEngine; using UnityEngine.TestTools;
using Active.Core; using static Active.Core.status; using Active.Util;

public class MecanimDriverTest : Kabuki.Test.PlayTest{

    override protected float baseTimeScale => 12;  MecanimDriver x;  Animator animator;

    [SetUp] public void SetupDriver(){
        animator = Create("Dummy").GetComponent<Animator>();
        x = new MecanimDriver( animator );
    }

    [Test] public void RootMotionOff () => o( animator.applyRootMotion, false );

    [Test] public void Play() => o( x.Play("Walk").running );
    //
    [UnityTest] public IEnumerator Play_once()  { yield return Complete( () => x.Play("Strike").due,    8f); }
    [UnityTest] public IEnumerator Play_loop()  { yield return Run( () => x.Play("Walk").due, duration: 2f); }
    [UnityTest] public IEnumerator Play_trans() { yield return Run( () => x.Play("Strike").due, 0.3f); }
    // In Mecanim no direct equivalent to "clamp forever"
    // ⏚ Play_clamp() { ⟾ Complete( ⎚ x.Play("KO"), timeout: 10f); }

    [UnityTest] public IEnumerator Loop_force () { yield return Run( () => x.Loop("Strike").ever, duration: 5f); }
    [UnityTest] public IEnumerator Loop_normal() { yield return Run( () => x.Loop("Walk").ever,   duration: 5f); }

    [Test] public void Play_emptyString_throws() => Assert.Throws <ArgEx>( () => x.Play("") );
    [Test] public void Play_nullString_throws () => Assert.Throws <ArgEx>( () => x.Play(null ) );

    [Test] public void Play_animationNotFound_logsWarning(){
        x.Play("foo"); LogAssert.Expect(LogType.Warning,
                       "Animator.GotoState: State could not be found");
      }

}
