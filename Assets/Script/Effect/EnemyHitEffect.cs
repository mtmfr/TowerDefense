using System.Collections;
using UnityEngine;

public class EnemyHitEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem damagedEffect;

    public void StartEffect()
    {
        Effect();
    }

    private IEnumerator Effect()
    {
        damagedEffect.Emit(1);
        float effectLenght = damagedEffect.main.duration;
        yield return new WaitForSeconds(effectLenght);
        ObjectPool.SetObjectInactive(this);
    }
}
