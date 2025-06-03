namespace Lop.Survivor.Island.Effect
{

    // # System
    using System.Collections;
    using System.Collections.Generic;

    // # UnityEngine
    using UnityEngine;
    using UnityEngine.Pool;

    public class Effect : MonoBehaviour
    {
        [Header("ParticleInfo")]
        [SerializeField] private string sfxName = default;
        [SerializeField] private float destroyTime = default;
        

        private IObjectPool<Effect> managedPool;

        //이펙트가 생성될때 실행
        private void OnEnable()
        {
            Invoke("DestroyObject", destroyTime);
        }

        //이펙트 객체가 속할 오브젝트 풀을 설정
        public void SetManagedPool(IObjectPool<Effect> pool)
        {
            managedPool = pool;
        }

        //이펙트 오브젝트 파괴
        public void DestroyObject()
        {
            managedPool.Release(this);
        }
    }
}
