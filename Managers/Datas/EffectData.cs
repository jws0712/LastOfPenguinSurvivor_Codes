namespace Lop.Survivor.Island.Effect
{
    // # System
    using System;

    // # UnityEngine
    using UnityEngine;

    //이펙트 정보
    [Serializable]
    //EffectManager에서 쓰일 이펙트 데이터 클래스
    public class EffectData
    {
        public string effectName;
        public GameObject effectPrefab;
    }
}
