using Ex = System.Exception;
using UnityEngine; using static UnityEngine.Time; using static UnityEngine.Mathf;
using Active.Core; using static Active.Core.status;

namespace Activ.Kabuki{
public static class TransformExt{

    public static Vector3 Dir(this Transform x, Transform y, bool planar = false)
        => planar ? (y.transform.position - x.transform.position).X_Z().normalized : (y.transform.position - x.transform.position).normalized;

    public static float Dist(this Transform x, Transform y) => Vector3.Distance(x.transform.position, y.transform.position);

    public static float Radius(this Transform x){
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

    public static bool Has(this Transform x, Transform y) => y.IsAncestor(x);

    // is y an ancestor of x?
    public static bool IsAncestor(this Transform x, Transform y){
        while (x != null){
            if (x == y) return true;
            x = x.parent;
        } return false;
    }

    public static float Look(this Transform x, Transform y, bool planar = true)
        => Vector3.Angle(x.forward, x.Dir(y, planar: planar));

    public static status RotateTowards(this Transform θ, Transform target, float speed, float μ = 0.1f){
        Vector3 u = θ.forward;
        Vector3 v = θ.Dir(target, planar: true);
        float α = Vector3.Angle(u, v);
        if (α < μ) return done();
        θ.forward = Vector3.RotateTowards(u, v, speed * Time.deltaTime * Mathf.Deg2Rad, 1f);
        return cont();
    }

    public static status RotateTowards(this Transform θ, Vector3 dir, float speed, float μ = 0.1f){
        Vector3 u = θ.forward;
        float α = Vector3.Angle(u, dir);
        if (α < μ) return done();
        θ.forward = Vector3.RotateTowards(u, dir, speed * Time.deltaTime * Mathf.Deg2Rad, 1f);
        return cont();
    }

    public static status MoveTowards(this Transform x, Transform y, float dist, float speed)
        => x.PlanarDist(y) < dist
        || Repeat( x.transform.position += x.PlanarDir(y) * Time.deltaTime * speed );

    public static float PlanarDist(this Transform x, Transform y) => (y.transform.position - x.transform.position).X_Z().magnitude;

    public static Vector3 PlanarDir(this Transform x, Transform y) => (y.transform.position - x.transform.position).X_Z().normalized;

    // --------------------------------------------------------------

    static action Do(object x) { return @void(); }
    static status Repeat(object x) { return cont(); }

}}
