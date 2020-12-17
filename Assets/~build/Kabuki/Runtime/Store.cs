using UnityEngine;
using Active.Core; using static Active.Status;

namespace Kabuki{
[System.Serializable]
public class Store{

    public bool want;
    public float amount = 1f, cap = 1f, delta = -0.1f, fill = 1f;
    public bool enabled = true;

    public status Feed(float scalar = 1f){
        amount += scalar * fill * Time.deltaTime ;
        if (amount > cap){
            amount = cap; want = false; return done();
        }else if (amount < 0f){
            amount = 0f; want = true; return done();            
        }else return cont();
    }

    public void Update(float δ=0f){
        if (!enabled) return ;
        amount += delta * Time.deltaTime + δ;
        if (amount <  0f) { amount =  0f; want = true; }
        else if (amount > cap) { amount = cap; want = false; }
    }

    public static implicit operator Store(float that) => new Store(){ amount = that };

    public static implicit operator float(Store that) => that.amount;

}}
