using System;
using UnityEngine.InputSystem;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Test.Extensions
{
    internal static class InputActionReferenceExtensions
    {
        public static InputAction RequireAction(
            this InputActionReference reference,
            string fieldName,
            string ownerTypeName)
        {
            if (reference == null || reference.action == null)
            {
                throw new InvalidOperationException(
                    $"{ownerTypeName} requires a configured InputActionReference for '{fieldName}'.");
            }

            return reference.action;
        }
    }
}
