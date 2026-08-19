using UnityEngine;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Extensions
{
    internal static class Vector3Extensions
    {
        public static bool IsFinite(this Vector3 value)
        {
            return value.x.IsFinite()
                && value.y.IsFinite()
                && value.z.IsFinite();
        }
    }
}
