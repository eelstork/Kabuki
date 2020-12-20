using UnityEngine;
using Active.Core; using static Active.Status;

namespace Active.Loco{
public class Avoidance2{

    // Returns cont if vector is being evaluated, fail if no valid
    // avoidance vector found, done if vector is ready
    // 'targetObj' is one we don't collide with, such as when
    // walking towards another actor or prop
    public status Correct(Transform transform, ref Vector3 u, float maxDist, Transform targetObj = null){
        Vector3? v = Avoidance.Clear(transform .transform.position + Vector3.up, u, maxDistance: maxDist,
                                             ignore: targetObj);
        if (v.HasValue){
            //ebug.Log("Avoidance vec: " + v);
            return Do(u = v.Value);
        }else {
            //ebug.LogError($"[No avoidance vec for {u}]");
            return fail();
        }
    }

}}
