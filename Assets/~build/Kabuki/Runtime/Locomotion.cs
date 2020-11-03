using Ex = System.Exception;
using UnityEngine; using static UnityEngine.Time; using static UnityEngine.Mathf;
using Active.Core; using static Active.Core.status;

namespace Activ.Kabuki{ public class Locomotion{

    public status MoveTo(Transform x, Vector3 y, float speed){
        if (x.transform.position == y) return done();
        float d = x.PlanarDist(y), δ = Time.deltaTime * speed;
        return (δ > d) ? Do( x.transform.position = y) : Move(x, x.PlanarDir(y), δ, d);
    }

    public status MoveTowards(Transform x, Vector3 y, float dist, float speed){
        float d = x.PlanarDist(y);
        return  (d < dist) || Move(x, x.PlanarDir(y), Time.deltaTime * speed, d);
    }

    public status MoveTowards(Transform x, Transform y, float dist, float speed){
        float d = x.PlanarDist(y);
        return  (d < dist) || Move(x, x.PlanarDir(y), Time.deltaTime * speed, d);
    }

    // --------------------------------------------------------------

    status Move(Transform transform, Vector3 u, float δ, float los){
        Vector3? v = Avoidance.Clear(transform .transform.position, u, maxDistance: los);
        if (v == null) return fail();
        transform.forward = Vector3.Lerp(transform.forward, v.Value, 0.1f);
        return Run(transform .transform.position += v.Value * δ);
    }

    static status Do(object x) { return @void(); } static status Run(object x) { return cont(); }

}}
