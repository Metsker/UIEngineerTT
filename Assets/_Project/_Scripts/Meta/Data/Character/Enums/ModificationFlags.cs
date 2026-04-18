using System;

namespace _Project._Scripts.Meta.Data.Character.Enums
{
    [Flags]
    public enum ModificationFlags
    {
        None = 0,
        Psyker = 1,
        Dot = 2,
        Attack = 4,
        Buff = 8,
    }
}