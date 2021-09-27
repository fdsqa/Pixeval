﻿using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Toolkit.HighPerformance;
using Microsoft.Toolkit.HighPerformance.Buffers;

namespace Pixeval.Utilities
{
    [PublicAPI]
    public static class Objects
    {
        public static readonly IEqualityComparer<string> CaseIgnoredComparer = new CaseIgnoredStringComparer();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Regex ToRegex(this string str)
        {
            return new(str);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNullOrEmpty(this string? str)
        {
            return !string.IsNullOrEmpty(str);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNullOrBlank(this string? str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty(this string? str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static byte[] GetBytes(this string str, Encoding? encoding = null)
        {
            return encoding?.Let(e => e!.GetBytes(str)) ?? Encoding.UTF8.GetBytes(str);
        }

        public static string GetString(this byte[] bytes, Encoding? encoding = null)
        {
            return encoding?.Let(e => e!.GetString(bytes)) ?? Encoding.UTF8.GetString(bytes);
        }

        public static string GetString(this MemoryOwner<byte> bytes, Encoding? encoding = null)
        {
            using (bytes)
            {
                return encoding?.Let(e => e!.GetString(bytes.Span)) ?? Encoding.UTF8.GetString(bytes.Span);
            }
        }

        public static async Task<string> HashAsync<THash>(this string str) where THash : HashAlgorithm, new()
        {
            using var hasher = new THash();
            await using var memoryStream = new MemoryStream(str.GetBytes());
            var bytes = await hasher.ComputeHashAsync(memoryStream).ConfigureAwait(false);
            return bytes.Select(b => b.ToString("x2")).Aggregate(string.Concat);
        }

        public static Task<HttpResponseMessage> GetResponseHeader(this HttpClient client, string url)
        {
            return client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        }

        public static async Task<string?> ToJsonAsync<TEntity>(this TEntity? obj, Action<JsonSerializerOptions>? serializerOptionConfigure = null)
        {
            if (obj is null)
            {
                return null;
            }

            await using var memoryStream = new MemoryStream();
            await JsonSerializer.SerializeAsync(memoryStream, obj, new JsonSerializerOptions().Apply(option => serializerOptionConfigure?.Invoke(option))).ConfigureAwait(false);
            return memoryStream.ToArray().GetString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [ContractAnnotation("obj:notnull => notnull; obj:null => null")]
        public static string? ToJson(this object? obj, Action<JsonSerializerOptions>? serializerOptionConfigure = null)
        {
            return obj?.Let(o => JsonSerializer.Serialize(o, new JsonSerializerOptions().Apply(option => serializerOptionConfigure?.Invoke(option))));
        }

        public static async ValueTask<TEntity?> FromJsonAsync<TEntity>(this IMemoryOwner<byte> bytes, Action<JsonSerializerOptions>? serializerOptionConfigure = null)
        {
            using (bytes)
            {
                await using var stream = bytes.Memory.AsStream();
                return await JsonSerializer.DeserializeAsync<TEntity>(stream, new JsonSerializerOptions().Apply(option => serializerOptionConfigure?.Invoke(option))).ConfigureAwait(false);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEntity? FromJson<TEntity>(this string str, Action<JsonSerializerOptions>? serializerOptionConfigure = null)
        {
            return JsonSerializer.Deserialize<TEntity>(str, new JsonSerializerOptions().Apply(option => serializerOptionConfigure?.Invoke(option)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsIgnoreCase(this string str1, string str2)
        {
            return string.Equals(str1, str2, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     Returns <see cref="Result{T}.Failure" /> if the status code does not indicating success
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="url"></param>
        /// <param name="exceptionSelector"></param>
        /// <returns></returns>
        public static async Task<Result<string>> GetStringResultAsync(this HttpClient httpClient, string url, Func<HttpResponseMessage, Task<Exception>>? exceptionSelector = null)
        {
            var responseMessage = await httpClient.GetAsync(url).ConfigureAwait(false);
            return !responseMessage.IsSuccessStatusCode ? Result<string>.OfFailure(exceptionSelector is { } selector ? await selector.Invoke(responseMessage).ConfigureAwait(false) : null) : Result<string>.OfSuccess(await responseMessage.Content.ReadAsStringAsync());
        }

        public static Task<TResult[]> WhenAll<TResult>(this IEnumerable<Task<TResult>> tasks)
        {
            return Task.WhenAll(tasks);
        }

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

        public static string RemoveSurrounding(this string str, string prefix, string suffix)
        {
            return str[(str.StartsWith(prefix) ? prefix.Length : 0)..(str.EndsWith(suffix) ? ^suffix.Length : str.Length)];
        }

        public static async Task<string> HashAsync<T>(this T algorithm, byte[] bytes) where T : HashAlgorithm
        {
            await using var memoryStream = new MemoryStream(bytes);
            return (await algorithm.ComputeHashAsync(memoryStream)).ToHexString();
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

        public static string Format(this string str, params object?[] args)
        {
            return string.Format(str, args);
        }

        /// <summary>
        /// Start inclusive, end inclusive
        /// </summary>
        public static bool InRange(this double i, (double, double) range)
        {
            var (startInclusive, endInclusive) = range;
            return i >= startInclusive && i <= endInclusive;
        }

        public static double CoerceIn(double i, (double, double) range)
        {
            var (startInclusive, endInclusive) = range;
            return Math.Max(startInclusive, Math.Min(i, endInclusive));
        }

        public static IEnumerable<TEnum> GetEnumValues<TEnum>(this Type type)
        {
            return type.GetEnumValues().Cast<TEnum>();
        }

        public static bool Inverse(ref this bool b) => b = !b;

        [ContractAnnotation("orElse:null => null;orElse:notnull => notnull")]
        public static async Task<R?> GetOrElseAsync<R>(this Task<Result<R>> task, R? orElse)
        {
            return (await task).GetOrElse(orElse);
        }

        private class CaseIgnoredStringComparer : IEqualityComparer<string>
        {
            public bool Equals(string? x, string? y)
            {
                return x is not null && y is not null && x.EqualsIgnoreCase(y);
            }

            public int GetHashCode(string obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}