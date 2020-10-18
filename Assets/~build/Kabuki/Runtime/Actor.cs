using Ex = System.Exception;
using T = UnityEngine.Transform; using UnityEngine;
using Active.Core; using static Active.Core.status; using Active.Util;

namespace Activ.Kabuki{
public class Actor : XTask{

    public float speed = 1, rotationSpeed = 180;
    public Transform leftHold, rightHold; // for holding an object in hand
    public Transform[] pushingBones;      // contact points for pushing
    public Actor other;           // for give/take interaction
    public Transform gift;

    // --------------------------------------------------------------

    public status this[string gesture, Transform that] => LookAt(that) && Play(gesture);

    public status this[string anim] => Play(anim);

    public status Face(Transform that, string anim = "Walk")
        => Playing(anim, transform.RotateTowards(that, rotationSpeed));

    public status Face(Vector3 dir, string anim = "Walk")
        => Playing(anim, transform.RotateTowards(dir, rotationSpeed));

    public status Give(Transform that, Actor recipient)
        => Reach(recipient.transform)
        && Playing("Idle", Offer(that, recipient))
        && Present(that, recipient);

    public status Grab(Transform that)
        => (Has(that) || Reach(that))
        && this["Grab"] % After(0.5f)?[ Hold(that).now ];

    public status Ingest(Transform that)
        => Hold(that, allowNull: true, hand: "L")
        && this["Eat"] % After(1.2f) ? [Destroy(that).now];

    public status Idle => this["Idle"];

    public status LookAt(Transform that, string rotationAnim = "Walk", string idleAnim = "Idle")
        => Face(that, rotationAnim) && this[idleAnim];

    public status Push(Transform that) => Sequence()[
        and ? Reach(that) && PushingSetup() :
        and ? this["Push"] && PushingTeardown() : end
    ];

    public status Strike(Transform that) => Reach(that) && this["Strike"];

    public status Take()
        => (other != null) && Face(other.transform) && this["Take"]
                      % After(0.5f)?[ Hold(gift).now ];

    public status Tell(Transform that, string msg)
        => Reach(that)
        && GetComponent<SpeechBox>().SetText(msg) && this["Tell"];

    public status Throw(Transform that, Vector3 dir) => Sequence()[
        and ? Hold(that) :
        and ? Face(dir) && this["Throw"] + After(0.5f)?
                                       [Impel(that, dir)] : end
    ];

    public status Reach(Transform that, float dist = 1f)
        => Face(that) && Playing("Walk", transform.MoveTowards(that, dist, speed));

    // ==============================================================

    public bool IsLookingAt(Actor that) => transform.Look(that.transform) < 5f;

    public bool Has(Transform that) => transform.Has(that);

    // ==============================================================

    action Hold(Transform that, bool allowNull = false, string hand = "R")
        => Hold(that, hand == "R" ? rightHold : leftHold, allowNull);

    action Hold(Transform that, Transform hold, bool allowNull){
        if (hold == null)
            throw new Ex("'hold' is null");
        if (that == null) return allowNull ? @void()
                                 : throw new Ex("that is null");
        that.gameObject.SetActive(true);
        if (that.parent == hold) return @void();
        var body = that.GetComponent<Rigidbody>();
        if (body) body.isKinematic = true;
        that.SetParent(hold);
        that.localPosition = Palm(hold) + Vector3.forward * that.Radius();
        return @void();
    }

    action Impel(Transform that, Vector3 dir, float force = 10f){
        that.SetParent(null);
        var body = that.GetComponent<Rigidbody>();
        body.isKinematic = false;
        var F = dir.normalized * force;
        body.AddForce(F, ForceMode.Impulse);
        return @void();
    }

    status Offer(Transform that, Actor recipient)
        => Do(recipient.other = this).now
        && Do(recipient.gift  = that).now
        && +(status)recipient.IsLookingAt(this);

    status Present(Transform that, Actor recipient)
        => (recipient.Has(that) || Hold(that).now) && this["Give"];

    action PushingSetup(){
        UseRootMotion(true);
        foreach (Transform x in pushingBones){
            var c = x.gameObject.AddComponent<SphereCollider>(); c.radius = 0.05f;
            var b = x.gameObject.AddComponent<Rigidbody>(); b.isKinematic = true;
        } return @void();
    }

    action PushingTeardown(){
        UseRootMotion(false);
        foreach (Transform x in pushingBones){
            Destroy(x.GetComponent<SphereCollider>());
            Destroy(x.GetComponent<Rigidbody>());
        } return @void();
    }

    action UseRootMotion(bool flag)
        => Do( GetComponent<Animator>().applyRootMotion = flag );

    // --------------------------------------------------------------

    Vector3 Palm(Transform hold){
        Vector3 sum = Vector3.zero; int count = 1;
        foreach (Transform x in hold){ sum += x.localPosition/2; count++; }
        sum /= count;
        return sum;
    }

}}
