using System.Collections;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    [SerializeField] private ParticleSystem muzzleEffect;

    public void StartEffect()
    {
        StartCoroutine(Effect());
    }

    private IEnumerator Effect()
    {
        muzzleEffect.Emit(1);
        float effectLenght = muzzleEffect.main.duration;
        yield return new WaitForSeconds(effectLenght);
        ObjectPool.SetObjectInactive(this);
    }
}
