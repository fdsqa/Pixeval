﻿using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Pixeval.Util
{
    public static class Objects
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNull(this object? obj)
        {
            return obj is null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Negate(this bool b)
        {
            return !b;
        }

        public static async Task<string> HashAsync<T>(this byte[] bytes) where T : HashAlgorithm, new()
        {
            using var hasher = new T();
            await using var memoryStream = new MemoryStream(bytes);
            return (await hasher.ComputeHashAsync(memoryStream)).ToHexString();
        }

        public static string ToHexString(this byte[] bytes)
        {
            return bytes.Select(b => b.ToString("X2")).Aggregate((s1, s2) => s1 + s2);
        }

        public static T CastOrThrow<T>(this object? obj)
        {
            // Debugger compliant: NullReferenceException will cause debugger to break, meanwhile the NRE is not supposed to be thrown by developer
            return (T) (obj ?? throw new InvalidCastException());
        }
    }
}