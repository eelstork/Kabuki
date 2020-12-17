using UnityEngine; using Accel; using Activ.Kabuki;

public class KaraptorAp : MonoBehaviour{

    public float  safeDistance = 15f;
    public Transform  threat, water, food;
    public Vector3? safePoint;
    public bool  dayOnly = true;

    public bool safe      => (threat == null) || transform.Dist(threat) > safeDistance;
    public bool isDayTime => dayOnly || FindObjectOfType<GameTime>().isDayTime;

    // --------------------------------------------------------------

    public void Update(){ LocateThreat(); LocateWater(); LocateFood();
                   LocateSafePoint(); }

    void LocateSafePoint(){
        if (threat == null){ safePoint = null; return; }
        var u = threat.Dir(transform);
        safePoint = threat.transform.position + u * safeDistance * 1.1f;
    }

    void LocateThreat () => threat = transform.Nearest<Actor>(
        Crit: x => !x.GetComponent<KaraptorModel>().wounded );

    void LocateFood () => food = transform.Nearest<Transform>(
        Crit: x => x.gameObject.name == "Grass" && IsReachable(x) );

    void LocateWater () => water = transform.Nearest<Transform>(
        Crit: x => x.gameObject.name == "Water" && IsReachable(x) );

    bool IsReachable(Transform target)
    => !mo.wounded || threat == null || target.Dist(threat) > safeDistance;

    KaraptorModel mo => GetComponent<KaraptorModel>();

}
