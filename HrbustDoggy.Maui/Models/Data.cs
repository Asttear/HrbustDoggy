using Hrbust;
using HrbustDoggy.Maui.Extensions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace HrbustDoggy.Maui.Models;

public class Data : IXmlSerializable
{
    public Exam[]? Exams { get; set; }
    public ClassTable? ClassTable { get; set; }

    public XmlSchema? GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        reader.ReadStartElement("Data");
        if (reader.IsStartElement("Exams"))
        {
            reader.ReadStartElement("Exams");
            List<Exam> exams = [];
            while (reader.IsStartElement("Exam"))
            {
                exams.Add(reader.ReadElementAsExam());
            }
            reader.ReadEndElement();
            Exams = [.. exams];
        }
        if (reader.IsStartElement("ClassTable"))
        {
            ClassTable = reader.ReadElementAsClassTable();
        }
        reader.ReadEndElement();
    }

    public void WriteXml(XmlWriter writer)
    {
        if (Exams is not null)
        {
            writer.WriteStartElement("Exams");
            foreach (var exam in Exams)
            {
                writer.WriteElementExam(exam);
            }
            writer.WriteEndElement();
        }

        if (ClassTable is not null)
        {
            writer.WriteElementClassTable(ClassTable);
        }
    }
}
