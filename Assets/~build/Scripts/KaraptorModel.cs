using UnityEngine;
using Active.Core; using static Active.Status;
using Store = Kabuki.Store;

public class KaraptorModel : MonoBehaviour{

    public Store nutrition = 0.5f;
    public Store hydration = 0.5f;
    public Store anger     = 0.0f;
    public Store damage    = 0.0f;
    float irritability = 0.2f;

    public bool angry   => anger >= 1f;
    public bool hungry  => nutrition.want;
    public bool thirsty => hydration.want;
    public bool wounded => damage > 0.5f;

    void Start(){
        damage.enabled = false;
    }

    void Update(){
        anger.Update(AngerStimulus());
        nutrition.Update();
        hydration.Update();
        damage.Update();
    }

    float AngerStimulus(){
        if (ap.threat == null) return 0;
        float d = Vector3.Distance(ap.threat.transform.position, transform .transform.position);
        return (d < 3f) ? 1f
         : (d < 10f) ? irritability * Time.deltaTime
         : 0f;
    }

    void OnStrike(){
        damage.amount += 0.2f;
    }

    KaraptorAp ap => GetComponent<KaraptorAp>();

}
