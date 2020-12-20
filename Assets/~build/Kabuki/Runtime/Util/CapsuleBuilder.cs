using UnityEngine;

namespace Active.Util{
[ExecuteInEditMode]
public class CapsuleBuilder : MonoBehaviour{

    void Update() => BuildAndResize(transform);

    public static void BuildAndResize(Transform τ){
        var capsule = τ.GetComponent<CapsuleCollider>();
        if (!capsule) capsule = τ.gameObject.AddComponent<CapsuleCollider>();
        var r = τ.GetComponentInChildren<Renderer>();
        capsule.radius = r.bounds.extents.x / τ.localScale.x;
        capsule.center = τ.InverseTransformPoint(r.bounds.center);
        capsule.height = capsule.center.y * 2f;
    }

}}
