using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPackageTool.Core.Utils
{
    internal static class SnowflakeUtils
    {
        public static DateTime FromSnowflake(ulong value)
            => DateTimeOffset.FromUnixTimeMilliseconds((long)((value >> 22) + 1420070400000UL)).LocalDateTime;
        public static ulong ToSnowflake(DateTimeOffset value)
            => ((ulong)value.ToUnixTimeMilliseconds() - 1420070400000UL) << 22;
    }
}
