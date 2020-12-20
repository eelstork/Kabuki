using UnityEngine;

[ExecuteInEditMode]
public class TrackingCamera : MonoBehaviour{

    public Transform target;
    public Vector3 offset = new Vector3(-5f, 5f, -10f);
    [Range(0f, 1f)] public float smooth = 0.1f;

    void Update(){
        if (Time.frameCount < 25) transform.LookAt(target);
        transform .transform.position = Vector3.Lerp(transform .transform.position, target .transform.position + offset, smooth);
    }

}
