using UnityEngine; using UnityEngine.UI; using Active.Core;
using static Active.Core.status;

public class SpeechBox : MonoBehaviour{

    public float duration = 1f; const float Forever = 10000;

    static Transform canvas;  private Transform ui;  private string text = null;  private float stamp;

    public status SetText(string arg, float? lapse = null){
        duration = lapse ?? duration;
        if (arg != text){
            text = arg;  stamp = Time.time + duration;
        } return done();
    }

    void Update(){
        if (!ui) CreateUI();
        if ( Time.time > stamp) text = null;
        if (ui.gameObject.activeInHierarchy && text == null)
        {
            text = null;
            if (ui) ui.gameObject.SetActive(false);
            return ;
        }
        if (text != null){
            ui.gameObject.SetActive(true);
            ui.GetChild(0).GetComponent<Text>().text = text;
            ui.position = Camera.main
                 .WorldToScreenPoint(transform.position + Vector3.up * 2 );
        }
    }

    void Print(string arg) => Debug.Log($"{arg} @{Time.frameCount}");

    void CreateUI(){
        ui = Instantiate(Resources.Load<Transform>("SpeechBox"));
        if (!canvas){
            var go = GameObject.Find("Canvas");
            if (go) canvas = go.transform;
        }
        if (!canvas) canvas = Instantiate(Resources.Load<Transform>("Canvas"));
        ui.SetParent(canvas);
    }

    void OnDestroy(){ if (ui != null) Destroy(ui.gameObject); }

}
