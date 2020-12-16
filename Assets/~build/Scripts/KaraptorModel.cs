using UnityEngine;
using Active.Core; using static Active.Status;
using Store = Kabuki.Store;

public class KaraptorModel : MonoBehaviour{

    public float anger = 0f,   irritability = 0.1f, angerDrop = 0.05f;
    public Store nutrition = new Store();
    public Store hydration = new Store();

    public bool angry   => anger  >= 1f;
    public bool hungry  => nutrition.want;
    public bool thirsty => hydration.want;

    void Update(){
        anger  += (AngerStimulus() * irritability - angerDrop) * Time.deltaTime ;
        nutrition.Update();
        hydration.Update();
    }

    float AngerStimulus(){
        if (ap.threat == null) return 0;
        float d = Vector3.Distance(ap.threat.transform.position, transform .transform.position);
        return (d < 1f) ? 1f
         : (d < 5f) ? irritability
         : 0f;
    }

    KaraptorAp ap => GetComponent<KaraptorAp>();

}
