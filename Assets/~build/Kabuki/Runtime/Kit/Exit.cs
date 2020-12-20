using UnityEngine; using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour{

    public string targetScene;
    public float input = 0f;
    public float delay = 1f;
    bool loading = false;

    void Update () => input = Mathf.Max( input - Time.deltaTime , 0f);

    void OnTriggerStay(){
        if (loading) return ;
        else if ((input += Time.deltaTime * 2f) > delay){
            SceneManager.LoadScene(targetScene);
            loading = true;
        }
    }

}
