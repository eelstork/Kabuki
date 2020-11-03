using Active.Core; using static Active.Core.status;
using UnityEngine;

public class OrderedPrint : UTask{

    override public status Step() => Sequence()[
        and ? Print("Step 1") :
        and ? Print("Step 2") :
        and ? Print("Step 3") : loop
    ];

    status Print(string arg){
        Debug.Log(arg); return done();
    }

}
