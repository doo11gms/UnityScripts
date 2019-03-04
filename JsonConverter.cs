using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// * Supported;
///     SerializableType       => string of JSON format
///     List<SerializableType> => string of JSON format
///     string of JSON format  => SerializableType
///     string of JSON format  => List<SerializableType>
/// </summary>
public class JsonConverter : MonoBehaviour
{
    /// <summary>
    /// SerializableType to string of JSON format
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="target"></param>
    /// <returns></returns>
    public string ConvertToJSON<T>(T target)
    {
        return JsonUtility.ToJson(target);
    }

    /// <summary>
    /// List<SerializableType> to string of JSON format
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="target"></param>
    /// <returns></returns>
    public string ConvertToJSON<T>(List<T> target)
    {
        return JsonUtility.ToJson(new SerializableList<T>(target));
    }

    /// <summary>
    /// string of JSON format to SerializableType
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="target"></param>
    /// <returns></returns>
    public T ConvertFromJSON<T>(string target)
    {
        return JsonUtility.FromJson<T>(target);
    }

    /// <summary>
    /// string of JSON format to List<SerializableType>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="target"></param>
    /// <returns></returns>
    public List<T> ConvertFromJSONToList<T>(string target)
    {
        return JsonUtility.FromJson<SerializableList<T>>(target).Value;
    }
}

[System.Serializable]
public class SerializableList<T>
{
    [SerializeField] List<T> m_Value;
    public List<T> Value
    {
        get { return m_Value; }
    }

    public SerializableList(List<T> value)
    {
        m_Value = value;
    }
}