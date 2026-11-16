using System.Runtime.CompilerServices;
using UnityEngine;

namespace Game.Interactions
{
    abstract class GrabServiceBase
    {
        //todo: place this in GrabConfig
        //
        const float CAST_WIDTH_PERCENT = 0.3f;
        const float DISTANCE_MULTIPLIER = 3.3f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected float GetCastRadius(Bounds bounds)
            => bounds.size.x * CAST_WIDTH_PERCENT;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected float GetCastDistance(float castWidth)
            => castWidth * DISTANCE_MULTIPLIER;
    }
}
