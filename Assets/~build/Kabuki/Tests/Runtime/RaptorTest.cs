using System.Collections;
using UnityEngine; using UnityEngine.TestTools;
using Active.Core;

namespace Kabuki.Test{
public class RaptorTest : ActorTest{

    override protected string ActorName => "Raptor";
    override protected float ActorSize => 20f;

    override protected string[] skip => new string[]{
        "Give_accept",
        "Grab",
        "Ingest",
        "Push",
        "Tell",
        "Throw"
    };

    [UnityTest] public IEnumerator PlayNonLooping(){
        actor.transform.forward = Vector3.right;
        status s =  status.cont();
        var t0 = Time.time ;
        while ( Time.time - t0 < 2f ){
            s = actor.Play("Strike");
            o( !s.failing ); if (s.complete) break; yield return null;
        }
        var δ = Time.time - t0;
        o( s.complete  );
        o( 0.75f < δ && δ < 0.85f );
        Print($"Done in {δ:0.##}s");
    }

}}
