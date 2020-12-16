using UnityEngine; using Accel; using Activ.Kabuki;

public class KaraptorAp : MonoBehaviour{

    public Transform threat, water, food;

    public bool dayOnly = true;

    public void Update(){
        LocateThreat();
        LocateWater();
        LocateFood();
    }

    void LocateThreat () => threat = transform.Nearest<Actor>();

    void LocateFood () => food = transform.Nearest<Transform>(Crit: x => x.gameObject.name == "Grass");

    void LocateWater () => water = transform.Nearest<Transform>(Crit: x => x.gameObject.name == "Water");

    public bool isDayTime => dayOnly || FindObjectOfType<GameTime>().isDayTime;

}
