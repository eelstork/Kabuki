using Ex = System.Exception;
using UnityEngine; using static UnityEngine.Time; using static UnityEngine.Mathf;
using T = UnityEngine.Transform;
using Active.Core; using static Active.Core.status;

namespace Activ.Kabuki{
public static class TransformExt{

    public static Vector3 Dir(this T x, T y, bool planar = false)
        => planar ? (y.position - x.position).X_Z().normalized
                 : (y.position - x.position).normalized;

    public static float Dist(this T x, T y) => Vector3.Distance(x.position, y.position);

    public static float Radius(this T x){
        var bounds = x.GetComponentInChildren<Renderer>().bounds;
        var s = bounds.size;
        return (s.x + s.y + s.z) / 6 * 0.7f;
    }

    public static Transform Req(this Transform x, params string[] hints){
        var ㄸ = x.Find(hints);
        return ㄸ != null ? ㄸ : throw new Ex("Not found: " + hints);

    }

    public static Transform Find(this Transform x, params string[] hints){
        foreach (Transform child in x){
            var name = child.gameObject.name.ToLower();  bool match = true;
            foreach (var h in hints) if (!name.Contains(h)){ match = false; break; }
            if (match) return child;
            var ㄸ = child.Find(hints);
            if (ㄸ != null) return ㄸ;
        }
        return null;
    }

    public static bool Has(this T x, T y) => y.IsAncestor(x);

    // is y an ancestor of x?
    public static bool IsAncestor(this T x, T y){
        while (x != null){
            if (x == y) return true;
            x = x.parent;
        } return false;
    }

    public static float Look(this T x, T y, bool planar = true)
        => Vector3.Angle(x.forward, x.Dir(y, planar: planar));

    public static status RotateTowards(this Transform θ, Transform target, float speed, float μ = 0.1f){
        Vector3 u = θ.forward;
        Vector3 v = θ.Dir(target, planar: true);
        float α = Vector3.Angle(u, v);
        if (α < μ) return done();
        θ.forward = Vector3.RotateTowards(u, v, Deg2Rad * speed * δt, 1f);
        return cont();
    }

    public static status RotateTowards(this Transform θ, Vector3 dir, float speed, float μ = 0.1f){
        Vector3 u = θ.forward;
        float α = Vector3.Angle(u, dir);
        if (α < μ) return done();
        θ.forward =
            Vector3.RotateTowards(u, dir, Deg2Rad * speed * δt, 1f);
        return cont();
    }

    // a @while construct would be useful here
    public static status MoveTowards(this Transform x, Transform y, float dist, float speed)
        => x.PlanarDist(y) < dist
        || (-Do( x.position += x.PlanarDir(y) * δt * speed)).ever;

    public static float PlanarDist(this T x, T y){
        var u = y.position - x.position; u.y = 0;
        return u.magnitude;
    }

    public static Vector3 PlanarDir(this T x, T y){
        var u = y.position - x.position; u.y = 0;
        return u.normalized;
    }

    // --------------------------------------------------------------

    static float δt => Time.deltaTime;

    static action Do(object x) => @void();

}}
