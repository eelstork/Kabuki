using System.Collections;
using UnityEngine; using UnityEngine.TestTools;
using Active.Core;

namespace Kabuki.Test{
public class RaptorTest : ActorTest{

    override protected string ActorName => "Raptor"; override protected float ActorSize => 20f;

    override protected string[] skip => new string[]
    { "Give_accept", "Grab", "Push", "Tell", "Throw" };

    // Length: 0.792 + 1 = 1.792
    // With cross-fade animations complete earlier
    [UnityTest] public IEnumerator PlayChaining(){
        float fadeLength = actor.animDriver.fadeLength;
        actor.transform.forward = Vector3.right;
        //
        var t0 = Time.time ;
        status s = status.cont();
        while (!s.complete){ s = actor["Strike"]; yield return null; }
        var δ0 = Time.time - t0;
        //rint($"Strike played {δ0:0.##}s");
        //
        var t1 = Time.time ;
        s = status.cont();
        while (!s.complete){ s = actor["Greet"]; yield return null; }
        var δ1 = Time.time - t1;
        //rint($"Flail played {δ1:0.##}s");
        //
        o( s.complete );
        float D = actor.GetComponent<Animation>()["Strike"].length +
              actor.GetComponent<Animation>()["Greet"].length
              - fadeLength * 2f;
        var δ = δ0 + δ1;
        o( Mathf.Abs(D - δ) < 0.05f );
    }

    // Length: 0.792 - 0.3 ~ 0.5 (early completion for cross-fade)
    [UnityTest] public IEnumerator PlayNonLooping(){
        float fadeLength = actor.animDriver.fadeLength;
        actor.transform.forward = Vector3.right;
        pending s =  pending.cont();
        var t0 = Time.time ;
        while ( Time.time - t0 < 2f ){
            s = actor.Play("Strike");
            if (s.complete) break; yield return null;
        }
        var δ = Time.time - t0;
        Print($"Done in {δ:0.##}s");
        o( s.complete  );
        var d = 0.792 - fadeLength;
        o( (d - 0.05f) < δ && δ < (d + 0.05f) );
    }

}

public class RaptorTest_noCrossFade : RaptorTest{ override protected float fadeLength => 0f; }

}  // Kabuki.Test
