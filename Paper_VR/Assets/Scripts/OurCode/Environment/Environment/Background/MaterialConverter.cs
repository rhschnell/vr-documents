using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

/// <summary>
/// Converts materials to JSON.
/// </summary>
public class MaterialConverter : JsonConverter
{
    /// <summary>
    /// Writes the JSON representation of the material object.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="value">The material object.</param>
    /// <param name="serializer">The JSON serializer.</param>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Material material = (Material)value;
        JObject obj = new JObject
        {
            { "name", material.name },
            { "shader", material.shader.name },
        };
        obj.WriteTo(writer);
    }

    /// <summary>
    /// Reads the JSON representation and creates a Material object.
    /// </summary>
    /// <param name="reader">The JSON reader.</param>
    /// <param name="objectType">The type of the object.</param>
    /// <param name="existingValue">The existing Material object (if any).</param>
    /// <param name="serializer">The JSON serializer.</param>
    /// <returns>The deserialized Material object.</returns>
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        // Read the material from the JSON
        JObject obj = JObject.Load(reader);
        string materialName = obj["name"].ToString();
        string shaderName = obj["shader"].ToString();

        Shader shader = Shader.Find(shaderName);
        Material material = new Material(shader);
        material.name = materialName;

        return material;
    }

    /// <summary>
    /// Checks if the type is of type material.
    /// </summary>
    /// <param name="objectType">Type of the object.</param>
    /// <returns>True if the object is of type material.</returns>
    public override bool CanConvert(Type objectType)
    {
        // Return if the objectType is of type material
        return objectType == typeof(Material);
    }
}
