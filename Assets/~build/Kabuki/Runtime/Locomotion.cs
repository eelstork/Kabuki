using Ex = System.Exception;
using UnityEngine; using static UnityEngine.Time; using static UnityEngine.Mathf;
using Active.Core; using static Active.Core.status;

namespace Activ.Kabuki{ public class Locomotion{

    TVec3 direction;

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
        Vector3? v = direction;
        // Avoidance vector is valid for one second at most; this
        // avoids dithering. Here the avoidance vector expired;
        // regenerate it.
        if (v == null){
            direction = Avoidance.Clear(transform .transform.position, u, maxDistance: los);
            v = direction;
        }
        // If no valid direction was found, return fail
        if (v == null){
            Print($"No avoidance vector @{Time.time:#.#}");
            return fail();
        }
        // Since we don't always regen an avoidance vector, sometimes
        // The current vector is invalid (ie, on a collision course).
        // Then we return cont as we're just going to yield for a bit.
        if (!Avoidance.HasClearLOS(transform .transform.position, v.Value, Mathf.Min(1f, los))){
            Debug.LogWarning($"Avoidance vector is out of date");
            return fail();
        }
        // TODO: after lerp we may still be on a collision course.
        // this is not taken into account.
        transform.forward = Vector3.Lerp(transform.forward, v.Value, 0.1f);
        return Run(transform .transform.position += v.Value * δ);
    }

    static status Do(object x) { return @void(); } static status Run(object x) { return cont(); }

    void Print(object arg) => UnityEngine.Debug.Log(arg);

}}
