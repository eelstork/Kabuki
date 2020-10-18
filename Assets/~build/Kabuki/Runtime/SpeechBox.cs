using UnityEngine; using UnityEngine.UI; using Active.Core;

public class SpeechBox : MonoBehaviour{

    public float duration = 1f; const float Forever = 10000;

    static Transform canvas;  private Transform ui;  private string text = null;  private float stamp;

    public status SetText(string arg, float? lapse = null){
        if (arg == text) return status.done();
        duration = lapse ?? duration;
        text = arg;  stamp = Time.time + duration;  return status.done();
    }

    void Update(){
        if (text == null || stamp < Time.time)
        { text = null; if (ui) ui.gameObject.SetActive(false); return; }
        if (!ui) CreateUI();
        ui.GetChild(0).GetComponent<Text>().text = text;
        ui.position = Camera.main
                 .WorldToScreenPoint(transform.position + Vector3.up * 2 );
    }

    void CreateUI(){
        ui = Instantiate(Resources.Load<Transform>("SpeechBox"));
        if (!canvas){ var go = GameObject.Find("Canvas");
                     if (go) canvas = go.transform;
        }
        if (!canvas) canvas = Instantiate(Resources.Load<Transform>("Canvas"));
        ui.SetParent(canvas);
    }

    void OnDestroy(){ if (ui != null) Destroy(ui.gameObject); }

}
