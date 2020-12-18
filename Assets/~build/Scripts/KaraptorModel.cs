using UnityEngine;
using Active.Core; using static Active.Status; using Store = Kabuki.Store;

public class KaraptorModel : MonoBehaviour{

    public Store nutrition = 0.5f, hydration = 0.5f, anger = 0.0f,
             damage    = 0.0f;
    public float irritability = 0.2f;

    public void OnStrike       () =>  damage.amount += 0.2f;
    public status RecoverQuickly () =>  damage.Feed(-0.2f);
    public status RecoverSlowly  () =>  damage.Feed(-0.05f);

    void Start () => damage.enabled = false;

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

    KaraptorAp ap => GetComponent<KaraptorAp>();

}
