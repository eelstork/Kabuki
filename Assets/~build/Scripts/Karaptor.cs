using UnityEngine;
using Active.Core; using static Active.Status;
using Activ.Kabuki; using static Activ.Kabuki.VectorExt;

public class Karaptor : UTask{

    override public status Step() => ε( ap.isDayTime
        ? Wake()[log && "Waking time"]
        : Sleep()[log && "Sleeping time"] );

    status Wake() => ε(
        ManageThreats()
        && Hydrate()
        && Forage()
        && Rest()
    );

    status Sleep () => ac["Sleep"];

    // --------------------------------------------------------------

    status ManageThreats() => ε(
        mo.damage > 0.50f ? Evade()  :
        mo.anger  > 0.66f ? Attack() :
        mo.anger  > 0.33f ? Ward  () : done()[log && "Not angry"]
    );

    status Ward() => ac["Flail", ap.threat];

    status Attack() => ac.Strike(ap.threat, 2f);

    status Evade(){
        if (ap.threat == null) return done()[log && "No threat"];
        else if (ap.threat.Dist(transform) > 15f) return done();
        var u = ap.threat.Dir(transform);
        var P = ap.threat.transform.position + u * 15f;
        return ac.Reach(P);
    }

    // --------------------------------------------------------------

    status Hydrate(){
        if (!mo.thirsty) return done()[log && "Not thirsty"];
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

    status Rest() => ac["Idle"];

    // --------------------------------------------------------------

    KaraptorAp    ap => GetComponent<KaraptorAp>();
    KaraptorModel mo => GetComponent<KaraptorModel>();
    Actor         ac => GetComponent<Actor>();

}
