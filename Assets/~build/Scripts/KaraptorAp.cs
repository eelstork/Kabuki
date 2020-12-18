using UnityEngine; using Accel; using Activ.Kabuki;

public class KaraptorAp : MonoBehaviour{

    public float  safeDistance = 15f;
    public Transform  threat, water, food;
    public bool  dayOnly = true;

    // --------------------------------------------------------------

    public bool angry   => mo.anger > 0.66f;
    public bool annoyed => mo.anger > 0.33f;
    public bool hungry  => mo.nutrition.want;
    public bool safe    => (threat == null) || transform.Dist(threat) > safeDistance;
    public bool thirsty => mo.hydration.want;
    public bool wounded => mo.damage > 0.5f;

    public bool isDayTime => dayOnly || FindObjectOfType<GameTime>().isDayTime;

    // --------------------------------------------------------------

    void Update(){ LocateThreat(); LocateWater(); LocateFood(); }

    void LocateThreat () => threat = transform.Nearest<Actor>(
        Crit: x => !x.GetComponent<KaraptorAp>().wounded );

    void LocateFood () => food = transform.Nearest<Transform>(
        Crit: x => x.gameObject.name == "Grass" && IsReachable(x) );

    void LocateWater () => water = transform.Nearest<Transform>(
        Crit: x => x.gameObject.name == "Water" && IsReachable(x) );

    bool IsReachable(Transform target)
    => !wounded || threat == null || target.Dist(threat) > safeDistance;

    KaraptorModel mo => GetComponent<KaraptorModel>();

}
