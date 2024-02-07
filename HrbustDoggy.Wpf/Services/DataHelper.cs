using System.IO;
using System.Xml.Serialization;
using Hrbust;
using HrbustDoggy.Wpf.Models;

namespace HrbustDoggy.Wpf.Services;

public class DataHelper
{
    public const string DefaultFileName = "Data.xml";

    private readonly XmlSerializer _serializer = new(typeof(Data));
    private Data? _data;

    public ClassTable? ClassTable
    {
        get => _data?.ClassTable;
        set
        {
            _data ??= new();
            _data.ClassTable = value;
        }
    }

    public Exam[]? Exams
    {
        get => _data?.Exams;
        set
        {
            _data ??= new();
            _data.Exams = value;
        }
    }

    public void Load(string path = DefaultFileName)
    {
        using FileStream file = File.OpenRead(path);
        _data = _serializer.Deserialize(file) as Data;
    }

    public void Save(string path = DefaultFileName)
    {
        using FileStream file = File.Create(path);
        _serializer.Serialize(file, _data);
    }

    public static bool FileExist() => File.Exists(DefaultFileName);
}
