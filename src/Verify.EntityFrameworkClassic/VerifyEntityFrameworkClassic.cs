﻿using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VerifyTests
{
    public static class VerifyEntityFrameworkClassic
    {
        public static void Enable()
        {
            VerifierSettings.RegisterFileConverter(
                QueryableToSql,
                (target, _, _) => QueryableConverter.IsQueryable(target));
            VerifierSettings.ModifySerialization(settings =>
            {
                settings.AddExtraSettings(serializer =>
                {
                    var converters = serializer.Converters;
                    converters.Add(new TrackerConverter());
                    converters.Add(new QueryableConverter());
                });
            });
        }

        static ConversionResult QueryableToSql(object arg, IReadOnlyDictionary<string, object> context)
        {
            var sql = QueryableConverter.QueryToSql(arg);
            return new ConversionResult(null, "txt", StringToMemoryStream(sql));
        }

        static MemoryStream StringToMemoryStream(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text.Replace("\r\n", "\n"));
            return new MemoryStream(bytes);
        }
    }
}