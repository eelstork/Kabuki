using UnityEngine;

namespace Active.Util{
public class TVec3{

    Vector3? value;
    float stamp;
    float duration;

    public TVec3(){
        value = null;
    }

    public TVec3(Vector3 val, float duration = 1f){
        this.value    = val;
        this.stamp    = Time.time ;
        this.duration = duration;
    }

    public static implicit operator TVec3(Vector3 that) => new TVec3(that);

    public static implicit operator Vector3?(TVec3 that) => (that?.Expire() ?? true) ? (Vector3?)null : that.value.Value;

    bool Expire(){
        if ( value == null ) return true;
        if ( Time.time < stamp + duration) return false;
        value = null;
        return true;
    }

}}
