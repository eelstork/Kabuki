using Ex = System.Exception;
using UnityEngine; using static UnityEngine.Time; using static UnityEngine.Mathf;
using Active.Core; using static Active.Status; using Active.Util;
using Activ.Kabuki;

namespace Active.Loco{
public class KinematicLocomotion : Locomotion{

    Active.Core.Details.LogString log = null;
    TVec3 direction;
    Transform transform;
    XTask actor;

    // TODO: actor is only needed for animation playback
    public KinematicLocomotion(Transform t, XTask actor){
        transform = t;
        this.actor = actor;
    }

    public status Idle() => cont();

    public status MoveTo(Vector3 target, float speed){
        if (transform .transform.position == target) return done();
        float d = transform.PlanarDist(target), δ = Time.deltaTime * speed;
        return (δ > d) ? Do(transform .transform.position = target) : DoMove(transform.PlanarDir(target), δ, d);
    }

    public status MoveTo(Transform target, float speed){
        if (transform .transform.position == target .transform.position) return done();
        float d = transform.PlanarDist(target), δ = Time.deltaTime * speed;
        return (δ > d) ? Do(transform .transform.position = target .transform.position) : DoMove(transform.PlanarDir(target), δ, d);
    }

    public status MoveTowards(Vector3 target, float dist, float speed){
        float d = transform.PlanarDist(target);
        return  (d < dist) || DoMove(transform.PlanarDir(target), Time.deltaTime * speed, d);
    }

    public status MoveTowards(Transform target, float dist, float speed){
        //ebug.Log($"Move towards {target}, dist:{dist}, speed:{speed} @{Time.frameCount}");
        float d = transform.PlanarDist(target);
        return  (d < dist) || DoMove(transform.PlanarDir(target), Time.deltaTime * speed, d, ignore: target);
    }

    public status Move(Vector3 u, float speed, float los)
        => DoMove(u.X_Z().normalized, Time.deltaTime * speed, los);

    // --------------------------------------------------------------

    status DoMove(Vector3 u, float δ, float los, Transform ignore=null){
        Vector3? v = direction;
        // Avoidance vector is valid for one second at most; this
        // avoids dithering. Here the avoidance vector expired;
        // regenerate it.
        if (v == null){
            direction = Avoidance.Clear(transform .transform.position, u, maxDistance: los,
                                                ignore: ignore);
            v = direction;
        }
        // If no valid direction was found, return fail
        if (v == null){
            actor.Play("Idle");
            return fail()[log && $"No avoidance vec @{Time.time:#.#}"];
        }
        // Since we don't always regen an avoidance vector, sometimes
        // The current vector is invalid (ie, on a collision course).
        // Then we return cont as we're just going to yield for a bit.
        if (!Avoidance.HasClearLOS(transform .transform.position, v.Value, Mathf.Min(1f, los),
                                                             ignore)){
            actor.Play("Idle");
            return fail()[log && "Collision course (avoid vec out of date)"];
        }
        // TODO: after lerp we may still be on a collision course.
        // this is not taken into account.
        transform.forward = Vector3.Lerp(transform.forward, v.Value, 0.1f);
        // If orientation is lagging behind catch-up first, no
        // location update
        if (Vector3.Angle(transform.forward, v.Value) > 10f) return cont();
        // Update position
        actor.Play("Walk");
        return Cont(transform .transform.position += v.Value * δ);
    }

    void Print(object arg) => UnityEngine.Debug.Log(arg);

}}
