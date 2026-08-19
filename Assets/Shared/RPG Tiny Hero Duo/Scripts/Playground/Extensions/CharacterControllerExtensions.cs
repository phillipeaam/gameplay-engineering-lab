using System;
using UnityEngine;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Extensions
{
    internal static class CharacterControllerExtensions
    {
        public static CharacterController RequireValid(this CharacterController characterController)
        {
            if (characterController == null)
            {
                throw new ArgumentNullException(nameof(characterController));
            }

            return characterController;
        }
    }
}
