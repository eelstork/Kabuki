using UnityEngine;
using Active.Core; using static Active.Status;
using Activ.Kabuki; using static Activ.Kabuki.VectorExt;

public class Karaptor : UTask{

    void Start () => ac.loco = new Locomotion();

    override public status Step() => ε( ap.isDayTime ? Wake() : Sleep() );

    status Wake() => ε( ~ (ManageThreats() && Hydrate() && Forage())
                  && Rest() );

    status Sleep () => ε( (ap.safe || Evade())
                  && ac["Sleep"] % mo.RecoverQuickly() );

    // --------------------------------------------------------------

    status ManageThreats() => ε(
        ap.safe ? done()[log && "Safe"]
        : ap.wounded ? Evade(2f)
        : ap.angry   ? Attack()
        : ap.annoyed ? Ward  ()
        : done()[log && "Not angry"]
    );

    status Ward() => ac["Flail", ap.threat];

    status Attack() => ac.Strike(ap.threat, 2f);

    status Evade(float scalar = 1f) => ac.Evade(ap.threat, scalar);

    // --------------------------------------------------------------

    status Hydrate(){
        if (!ap.thirsty ) return done()[log && "Not thirsty"];
        if (!ap.water   ) return fail()[log && "No water or not reachable"];
        return ε(   ac.Reach(ap.water)
              && ac.Ingest(ap.water, consume: false)
              && mo.hydration.Feed()
        )[log && $"hydration: {mo.hydration.amount}"];
    }

    status Forage(){
        if (!ap.hungry ) return done()[log && "Not hungry"];
        if (!ap.food   ) return fail()[log && "No food or not reachable"];
        return ε(   ac.Reach(ap.food)
              && ac.Ingest(ap.food, consume: false)
              && mo.nutrition.Feed()
        )[log && $"nutrition: {mo.nutrition.amount}"];
    }

    status Rest() => ε(ac["Idle"] % mo.RecoverSlowly());

    // --------------------------------------------------------------

    KaraptorAp    ap => GetComponent<KaraptorAp>();
    KaraptorModel mo => GetComponent<KaraptorModel>();
    Actor         ac => GetComponent<Actor>();

}
