using UnityEngine;
using Active.Core; using static Active.Status;

namespace Kabuki{
public class Store{

    public bool want;
    public float amount = 1f, cap = 1f, usage = -0.1f, fill = 1f;

    public status Feed(){
        amount += fill * Time.deltaTime ;
        if (amount > cap){ amount = cap; want = false; return done(); } else return cont();
    }

    public void Update(){
        amount += usage * Time.deltaTime ;
        if (amount < 0f){
            amount = 0f;
            want = true;
        }
        else if (amount > cap){
            amount = cap;
            want = false;
        }
    }

}}
