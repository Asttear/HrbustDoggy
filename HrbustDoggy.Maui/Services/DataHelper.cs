using Hrbust;
using System.Xml.Serialization;

namespace HrbustDoggy.Maui.Services;

public class DataHelper
{
    private readonly XmlSerializer _classTableSerializer = new(typeof(ClassTable));
    private readonly XmlSerializer _examsSerializer = new(typeof(Exam[]));

    public ClassTable? LoadClassTable(string path)
    {
        using FileStream stream = File.OpenRead(path);
        return _classTableSerializer.Deserialize(stream) as ClassTable;
    }

    public void SaveClassTable(string path, ClassTable? table)
    {
        using FileStream stream = File.Create(path);
        _classTableSerializer.Serialize(stream, table);
    }

    public Exam[]? LoadExams(string path)
    {
        using FileStream stream = File.OpenRead(path);
        return _examsSerializer.Deserialize(stream) as Exam[];
    }

    public void SaveExams(string path, Exam[]? exams)
    {
        using FileStream stream = File.Create(path);
        _examsSerializer.Serialize(stream, exams);
    }
}