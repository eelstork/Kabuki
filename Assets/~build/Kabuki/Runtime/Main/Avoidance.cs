using UnityEngine;

public static class Avoidance{

    public static Vector3? Clear(Vector3 o, Vector3 u, float maxAngle=90f, float maxDistance=5f, float δ=5f,
                           Transform ignore=null){
        for (float i = 0 ; i < maxAngle; i += δ ){
            Vector3 a = Quaternion.AngleAxis(i, Vector3.up) * u;
            if (HasClearLOS(o, a, maxDistance, ignore)) return a;
            Vector3 b = Quaternion.AngleAxis(i, Vector3.down) * u;
            if (HasClearLOS(o, b, maxDistance, ignore)) return b;
        }
        return null;
    }

    public static bool HasClearLOS(Vector3 o, Vector3 u, float distance, Transform ignore=null){
        Vector3? P = Cast(o, u, distance, ignore);
        if (!P.HasValue){
            Debug.DrawRay(o, u, Color.white);
            return true;
        }else {
            Debug.DrawLine(o, P.Value, Color.red);
            return false;
        }
    }

    static Vector3? Cast(Vector3 o, Vector3 u, float dist, Transform ignore){
        RaycastHit hit;
        if (Physics.SphereCast(o, 0.5f, u, out hit, dist)){
            if (hit.collider != null && hit.collider.transform == ignore) return null;
            Debug.Log(
                $"Avoidance did hit {hit.collider}; (ignoring {ignore}) ");
            return hit.point;
        }
        return null;
    }

}
