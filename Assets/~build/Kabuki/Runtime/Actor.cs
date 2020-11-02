using Ex = System.Exception;
using T = UnityEngine.Transform; using UnityEngine;
using Active.Core; using static Active.Core.status; using Active.Util;

namespace Activ.Kabuki{
public class Actor : Activ.Kabuki.XTask{

    public float speed = 1, rotationSpeed = 180;
    public Transform leftHold, rightHold; // for holding an object in hand
    public Transform[] pushingBones;      // contact points for pushing
    public Actor other;           // for give/take interaction
    public Transform gift;

    // --------------------------------------------------------------

    public status this[string gesture, Transform that] => Face(that) && this[gesture];

    public status this[string anim] => Play(anim).due;

    public status Face(Transform that, string anim = "Walk")
    => Playing(anim, transform.RotateTowards(that, rotationSpeed));

    //    → ⸨ み.RotateTowards(⧕, rotationSpeed) ≫ ⦿[anim] ⸩;

    public status Face(Vector3 pos, string anim = "Walk")
        => Playing(anim, transform.RotateTowards(pos, rotationSpeed));

    public status Give(Transform that, Actor recipient)
        => Reach(recipient.transform)
        && While(Offer(that, recipient))?[ this["Idle"] ]
        && Present(that, recipient);

    public status Grab(Transform that)
        => (Has(that) || Reach(that)) && this["Grab"] % After(0.5f)?[ Hold(that) ];

    public status Ingest(Transform that)
        => Hold(that, allowNull: true, hand: "L")
        & this["Eat"] % ( Wait(1.2f) && Destroy(that) );

    public status Idle => this["Idle"];

    public impending LookAt(Transform that, string rotationAnim = "Walk", string idleAnim = "Idle"){
        var s = Face(that, rotationAnim) && this[idleAnim];
        return impending.cont();
    }

    public status Push(Transform that)
        => Once()?[Reach(that) && PushingSetup() ]
        && this["Push"] && PushingTeardown() ;

    public status Reach(Transform that, float dist = 1f)
    => Face(that) && Playing("Walk", transform.MoveTowards(that, dist, speed));

    public status Reach(Vector3? that) => that.HasValue
    ? Once()?[Face(that .Value)] && Playing("Walk", transform.MoveToWithAvoidance(that .Value, speed))
    : done();

    // TODO: flakiness in "Reach" requires the Once node here.
    // if the memory node is removed the strike action will sometimes
    // stop half-way and repeat. Solution may be fix the "Reach"
    // action.
    public status Strike(Transform that) => Once()?[Reach(that)] && this["Strike"];

    public status Take()
    => (other != null) && Face(other.transform) && this["Take"]
                  % After(0.5f)?[ Hold(gift) ];

    public status Tell(Transform that, string msg)
    => Once()?[Reach(that)] && GetComponent<SpeechBox>().SetText(msg) && this["Tell"];

    public status Throw(Transform that, Vector3 dir)
    => Once()?[Hold(that)]
    && Face(dir) && this["Throw"] * After(0.5f)? [ Impel(that, dir) ];

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
        body.AddForce(dir.normalized * force, ForceMode.Impulse);
        return @void();
    }

    status Offer(Transform that, Actor recipient)
        => Do(recipient.other = this)
        & Do(recipient.gift  = that)
        & (+(status)recipient.IsLookingAt(this)).due;

    status Present(Transform that, Actor recipient)
        => (recipient.Has(that) || Hold(that)) && this["Give"];

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

    action UseRootMotion(bool flag) => Do( GetComponent<Animator>().applyRootMotion = flag );

    // --------------------------------------------------------------

    Vector3 Palm(Transform hold){
        Vector3 sum = Vector3.zero; int count = 1;
        foreach (Transform x in hold){ sum += x.localPosition/2; count++; }
        sum /= count;
        return sum;
    }

}}
