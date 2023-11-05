﻿using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FDex.Application.Extensions
{
    public class DoubleInfinityConverter : JsonConverter<double>
    {
        public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.GetDouble();

        public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                writer.WriteStringValue(default(double).ToString());
                return;
            }
            writer.WriteStringValue(value.ToString());
        }
    }
}

