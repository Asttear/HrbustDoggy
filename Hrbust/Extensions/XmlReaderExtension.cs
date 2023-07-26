using System.Xml;

namespace Hrbust.Extensions;

internal static class XmlReaderExtension
{
    public static List<TimeOnly> ReadElementAsStartTimes(this XmlReader reader)
    {
        reader.ReadStartElement("StartTimes");
        List<TimeOnly> list = new();
        while (reader.IsStartElement("Time"))
        {
            TimeOnly time = TimeOnly.Parse(reader.ReadElementContentAsString());
            list.Add(time);
        }
        reader.ReadEndElement();
        return list;
    }

    public static Course ReadElementAsCourse(this XmlReader reader)
    {
        reader.ReadStartElement("Course");
        string title = "";
        int num = 0;
        string location = "";
        string teacher = "";
        Schedule schedule = default;
        List<string> description = new();
        while (reader.IsStartElement())
        {
            switch (reader.Name)
            {
                case "Title":
                    title = reader.ReadElementContentAsString();
                    break;

                case "Num":
                    num = reader.ReadElementContentAsInt();
                    break;

                case "Location":
                    location = reader.ReadElementContentAsString();
                    break;

                case "Teacher":
                    teacher = reader.ReadElementContentAsString();
                    break;

                case "Schedule":
                    schedule = Schedule.Parse(reader.ReadElementContentAsString());
                    break;

                case "Description":
                    reader.ReadStartElement("Description");
                    while (reader.IsStartElement("Text"))
                    {
                        description.Add(reader.ReadElementContentAsString());
                    }
                    reader.ReadEndElement();
                    break;

                default:
                    break;
            }
        }
        reader.ReadEndElement();
        return new Course(title, num, location, teacher, schedule, description.ToArray());
    }
}