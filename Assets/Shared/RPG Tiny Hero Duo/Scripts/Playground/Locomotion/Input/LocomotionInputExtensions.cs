using System;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Extensions;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion.Input
{
    internal static class LocomotionInputExtensions
    {
        public static ILocomotionInput RequireValid(this ILocomotionInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var ownerTypeName = input.GetType().Name;

            input.Move.RequireValid(nameof(input.Move), ownerTypeName);
            input.Look.RequireValid(nameof(input.Look), ownerTypeName);
            input.Jump.RequireValid(nameof(input.Jump), ownerTypeName);

            return input;
        }
    }
}
