using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdoll : MonoBehaviour
{

    [SerializeField] private Transform ragdollRootBones;

    public void SetUp(Transform originalRootBone)
    {
        MatchAllChildTransforms(originalRootBone, ragdollRootBones);

        Vector3 randomDirection = new Vector3(Random.Range(-1f, +1f), 0, Random.Range(-1f, +1f));
        ApplyExploasionToRagdoll(ragdollRootBones, 300f, transform.position + randomDirection, 10f);
    }

    private void MatchAllChildTransforms(Transform root, Transform clone)
    {
        foreach (Transform child in root)
        {
            Transform cloneChild = clone.Find(child.name);
            if (cloneChild != null)
            {
                cloneChild.position = child.position;
                cloneChild.rotation = child.rotation;

                MatchAllChildTransforms(child, cloneChild);
            }
        }
    }

    private void ApplyExploasionToRagdoll(Transform root, float exploasionForce, Vector3 exploaionposition, float exploaionRange)
    {
        foreach (Transform child in root)
        {
            if(child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(exploasionForce, exploaionposition, exploaionRange);
            }

            ApplyExploasionToRagdoll(child, exploasionForce, exploaionposition, exploaionRange);
        }
    }
}
