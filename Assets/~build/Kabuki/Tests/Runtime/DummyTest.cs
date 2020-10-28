using System.Collections;
using UnityEngine; using UnityEngine.TestTools;
using Active.Core;

namespace Kabuki.Test{
public class DummyTest : ActorTest{

    override protected string ActorName => "Dummy";
    override protected float ActorSize => 1f;

}

public class DummyTest_noCrossFade : DummyTest{
    override protected float fadeLength => 0f;
}

}
