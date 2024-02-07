using System.Xml.Serialization;
using Hrbust;
using HrbustDoggy.Maui.Models;

namespace HrbustDoggy.Maui.Services;

public class DataHelper
{
    public const string DefaultFileName = "Data.xml";

    private readonly XmlSerializer _serializer = new(typeof(Data));
    private readonly string _defaultPath;

    private Data? _data;

    public DataHelper()
    {
        _defaultPath = Path.Combine(FileSystem.CacheDirectory, DefaultFileName);
    }

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

    public void Load(string? path = null)
    {
        path ??= _defaultPath;
        using FileStream file = File.OpenRead(path);
        _data = _serializer.Deserialize(file) as Data;
    }

    public void Save(string? path = null)
    {
        path ??= _defaultPath;
        using FileStream file = File.Create(path);
        _serializer.Serialize(file, _data);
    }

    public bool FileExist() => File.Exists(_defaultPath);
}
