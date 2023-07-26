using System.Xml;

namespace Hrbust.Extensions;

internal static class XmlWriterExtension
{
    public static void WriteElementCourse(this XmlWriter writer, Course course)
    {
        writer.WriteStartElement("Course");
        writer.WriteElementString("Title", course.Title);
        writer.WriteElementString("Num", course.Num.ToString());
        writer.WriteElementString("Location", course.Location);
        writer.WriteElementString("Teacher", course.Teacher);
        writer.WriteElementString("Schedule", course.Schedule.RawText);
        writer.WriteStartElement("Description");
        foreach (string text in course.Description)
        {
            writer.WriteElementString("Text", text);
        }
        writer.WriteEndElement();
        writer.WriteEndElement();
    }

    public static void WriteElementStartTimes(this XmlWriter writer, TimeOnly[] startTimes)
    {
        writer.WriteStartElement("StartTimes");
        foreach (TimeOnly time in startTimes)
        {
            writer.WriteElementString("Time", time.ToString("HH:mm"));
        }
        writer.WriteEndElement();
    }
}