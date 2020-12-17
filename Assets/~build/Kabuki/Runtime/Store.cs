using UnityEngine;
using Active.Core; using static Active.Status;

namespace Kabuki{
[System.Serializable]
public class Store{

    public bool want;
    public float amount = 1f, cap = 1f, usage = -0.1f, fill = 1f;

    public status Feed(){
        amount += fill * Time.deltaTime ;
        if (amount > cap){
            amount = cap; want = false; return done();
        } else return cont();
    }

    public void Update(float δ=0f){
        amount += usage * Time.deltaTime + δ;
        if (amount <  0f) { amount =  0f; want = true; }
        else if (amount > cap) { amount = cap; want = false; }
    }

    public static implicit operator Store(float that) => new Store(){ amount = that };

    public static implicit operator float(Store that) => that.amount;

    //‒̥ ㅇ ⨕ > (Store x, ㅅ y) → x.amount > y;
    //‒̥ ㅇ ⨕ < (Store x, ㅅ y) → x.amount < y;

}}
