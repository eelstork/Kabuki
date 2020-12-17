using UnityEngine;
using Active.Core; using static Active.Status;
using Activ.Kabuki; using static Activ.Kabuki.VectorExt;

public class Karaptor : UTask{

    void Start () => ac.loco = new Locomotion();

    override public status Step() => ε( ap.isDayTime
        ? Wake()[log && "Waking time"]
        : Sleep()[log && "Sleeping time"] );

    status Wake() => ε(
        ~ (ManageThreats() && Hydrate() && Forage())
        && Rest()
    );

    status Sleep () => ε(
        (ap.safe || Evade()) && ac["Sleep"] % mo.damage.Feed(-0.2f)
    );

    // --------------------------------------------------------------

    status ManageThreats() => ε(
        ap.safe ? done()[log && "Safe"]
        : mo.wounded       ? Evade(2f)
        : mo.anger > 0.66f ? Attack()
        : mo.anger > 0.33f ? Ward  ()
        : done()[log && "Not angry"]
    );

    status Ward() => ac["Flail", ap.threat];

    status Attack() => ac.Strike(ap.threat, 2f);

    status Evade(float scalar = 1f) => ac.Evade(ap.threat, scalar);

    // --------------------------------------------------------------

    status Hydrate(){
        if (!mo.thirsty) return done()[log && "Not thirsty"];
        if (!ap.water) return fail()[log && "No water or not reachable"];
        return ε(
            ac.Reach(ap.water)
          && ac.Ingest(ap.water, consume: false)
          && mo.hydration.Feed()
        )[log && $"hydration: {mo.hydration.amount}"];
    }

    status Forage(){
        if (!mo.hungry) return done()[log && "Not hungry"];
        return ε(
            ac.Reach(ap.food)
          && ac.Ingest(ap.food, consume: false)
          && mo.nutrition.Feed()
        )[log && $"nutrition: {mo.nutrition.amount}"];
    }

    status Rest() => ε(ac["Idle"] % mo.damage.Feed(-0.05f));

    // --------------------------------------------------------------

    KaraptorAp    ap => GetComponent<KaraptorAp>();
    KaraptorModel mo => GetComponent<KaraptorModel>();
    Actor         ac => GetComponent<Actor>();

}
