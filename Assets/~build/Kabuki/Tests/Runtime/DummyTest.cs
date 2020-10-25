using System.Collections;
using UnityEngine; using UnityEngine.TestTools;
using Active.Core;

namespace Kabuki.Test{
public class DummyTest : ActorTest{

    override protected string ActorName => "Dummy";
    override protected float ActorSize => 1f;

    [UnityTest] public IEnumerator PlayNonLooping(){
        actor.transform.forward = Vector3.right;
        status s = status.cont();
        var t0 = Time.time ;
        while ( Time.time - t0 < 2f ){
            s = actor.Play("Strike");
            o( !s.failing ); if (s.complete) break; yield return null;
        }
        var δ = Time.time - t0;
        o( s.complete  );
        o( δ < 1.8f );
        Print($"Done in {δ:0.##}s");
    }

}}
