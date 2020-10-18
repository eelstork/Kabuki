using UnityEngine;

namespace Activ.Kabuki{
public static class VectorExt{

    const float PRECISION = 0.0000001f;

    public static Vector3 Abs(this Vector3 u){
        var v = u;
        for (int i = 0; i < 3; i++) v[i] = Mathf.Abs(v[i]);
        return v;
    }

    public static Vector3 Drop(this Vector3 P, float maxDist=10f){
        RaycastHit hit;
        bool didHit = Physics.Raycast(P, Vector3.down, out hit, maxDist);
        if (!didHit) throw new System.Exception("No hit");
        return hit.point;
    }

    public static bool Eq(this Vector3 u, Vector3 v)
    => (u - v).magnitude <= PRECISION;

    public static float Max(this Vector3 u) => Mathf.Max(u.x, u.y, u.z);

    public static Vector3 Ortho(this Vector3 u){
        Vector3 v = Abs(u);
        float w = v.Max();
        Vector3 θ = u;
        for (int i = 0; i < 3; i++){
            θ[i] = v[i] == w ? u[i] : 0;
        }
        return θ;
    }

    public static Vector3 Shift(this Vector3 u, int c){
        for (int i = 0; i < c; i++)
            u = new Vector3(u.z, u.x, u.y);
        return u;
    }

    public static Vector3[] Shift(this Vector3[] S, int c){
        if (c == 0) return S;
        for (int i = 0; i < S.Length; i++){
            S[i] = S[i].Shift(c);
        } return S;
    }

    public static Vector3 ZYX(this Vector3 u) => new Vector3(u.z, u.y, u.x);

    public static Vector2 XZ(this Vector3 u) => new Vector3(u.x, u.z);

    public static Vector3 X_Z(this Vector3 u) => new Vector3(u.x, 0f, u.z);

}}
