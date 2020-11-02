using UnityEngine;

public class Multicaster : MonoBehaviour{

    public float maxAngle = 45;
    public float δ = 10;
    public float distance = 10f;

    void Update(){
        var u = FindClearLOS();
        if (u.HasValue) Debug.DrawRay(this .transform.position, u.Value, Color.green);
    }

    Vector3? FindClearLOS(){
        Vector3? ㄸ = null;
        for (float i = 0 ; i < maxAngle; i += δ ){
            Vector3 a = Quaternion.AngleAxis(i, Vector3.up) * transform.forward * distance;
            Vector3 b = Quaternion.AngleAxis(i, Vector3.down) * transform.forward * distance;
            if (HasClearLOS(a)){ ㄸ = ㄸ ?? a; }
            if (HasClearLOS(b)){ ㄸ = ㄸ ?? b; }
        }
        return ㄸ;
    }

    bool HasClearLOS(Vector3 u){
        Vector3? P = Cast(u, distance);
        if (!P.HasValue){
            Debug.DrawRay(this .transform.position, u, Color.white); return true;
        }else {
            Debug.DrawLine(this .transform.position, P.Value, Color.red); return false;
        }
    }

    Vector3? Cast(Vector3 u, float dist){
        RaycastHit hit;
        if (Physics.SphereCast(this .transform.position, 0.5f, u, out hit, dist)){
            //ebug.Log("Did hit " + hit.colliderˮ);
            return hit.point;
        } return null;
    }

}
