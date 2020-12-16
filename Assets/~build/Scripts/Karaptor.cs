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

    status Sleep () => actor["Sleep"];

    // --------------------------------------------------------------

    status ManageThreats() => ε(
        Ward() ||
        Attack() ||
        Evade()
    );

    status Ward() => model.angry ? actor["Flail", ap.threat]
              : done()[log && "Not angry"];

    status Attack() => actor.Strike(ap.threat);

    status Evade(){
        if (ap.threat == null) return done()[log && "No threat"];
        else if (ap.threat.Dist(transform) > 10f) return done();
        var u = ap.threat.Dir(transform);
        var P = ap.threat.transform.position + u * 10f;
        return actor.Reach(P);
    }

    // --------------------------------------------------------------

    status Hydrate(){
        if (!model.thirsty) return done()[log && "Not thirsty"];
        return ε(
            actor.Reach(ap.water)
          && actor.Ingest(ap.water, consume: false)
          && model.hydration.Feed()
        )[log && $"hydration: {model.hydration.amount}"];
    }

    status Forage(){
        if (!model.hungry) return done()[log && "Not hungry"];
        return ε(
            actor.Reach(ap.food)
          && actor.Ingest(ap.food, consume: false)
          && model.nutrition.Feed()
        )[log && $"nutrition: {model.nutrition.amount}"];
    }

    status Rest() => actor["Idle"];

    // --------------------------------------------------------------

    KaraptorAp    ap    => GetComponent<KaraptorAp>();
    KaraptorModel model => GetComponent<KaraptorModel>();
    Actor         actor => GetComponent<Actor>();

}
