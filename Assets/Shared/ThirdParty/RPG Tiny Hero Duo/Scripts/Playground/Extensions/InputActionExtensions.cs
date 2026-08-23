using System;
using UnityEngine.InputSystem;

namespace RPG_Tiny_Hero_Duo.Playground.Extensions
{
    internal static class InputActionExtensions
    {
        public static InputAction RequireValid(
            this InputAction action,
            string propertyName,
            string ownerTypeName)
        {
            if (action == null)
            {
                throw new InvalidOperationException(
                    $"{ownerTypeName} requires a configured InputAction for '{propertyName}'.");
            }

            return action;
        }
    }
}
