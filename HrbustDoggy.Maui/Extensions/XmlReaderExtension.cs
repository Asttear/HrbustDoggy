using Hrbust;
using System.Xml;

namespace HrbustDoggy.Maui.Extensions;

internal static class XmlReaderExtension
{
    private static readonly string[] s_days = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];

    public static List<TimeOnly> ReadElementAsStartTimes(this XmlReader reader)
    {
        reader.ReadStartElement("StartTimes");
        List<TimeOnly> list = [];
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
        List<string> description = [];
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
        return new Course(title, num, location, teacher, schedule, [.. description]);
    }

    public static ClassTable ReadElementAsClassTable(this XmlReader reader)
    {
        string? styleStr = reader.GetAttribute("Style"),
            durationStr = reader.GetAttribute("Duration"),
            dateStr = reader.GetAttribute("Date"),
            weekStr = reader.GetAttribute("Week");
        if (styleStr is null || durationStr is null || dateStr is null || weekStr is null)
        {
            throw new XmlException("Missing attributes of ClassTable!", null);
        }

        ClassTableStyle style = Enum.Parse<ClassTableStyle>(styleStr);
        TimeSpan durationOfEachClass = TimeSpan.FromMinutes(int.Parse(durationStr));
        DateOnly dateWhenObtained = DateOnly.Parse(dateStr);
        int? weekWhenObtained = string.IsNullOrEmpty(weekStr) ? null : int.Parse(weekStr);

        ClassTable table = new(style, durationOfEachClass, dateWhenObtained, weekWhenObtained);

        reader.ReadStartElement("ClassTable");
        reader.ReadElementAsStartTimes().ToArray().CopyTo(table.StartTimes, 0);
        for (int i = 0; i < 7; i++)
        {
            reader.ReadStartElement(s_days[i]);
            for (int j = 0; j < table.NumberOfClassesPerDay; j++)
            {
                if (reader.IsEmptyElement)
                {
                    reader.Read();
                    continue;
                }
                reader.ReadStartElement("Class");
                while (reader.IsStartElement("Course"))
                {
                    table[i, j].Add(reader.ReadElementAsCourse());
                }
                reader.ReadEndElement();
            }
            reader.ReadEndElement();
        }
        reader.ReadEndElement();

        return table;
    }

    public static Exam ReadElementAsExam(this XmlReader reader)
    {
        reader.ReadStartElement("Exam");
        string courseId = reader.ReadElementContentAsString();
        string courseName = reader.ReadElementContentAsString();
        reader.ReadStartElement("Time");
        DateTime start = DateTime.Parse(reader.ReadElementContentAsString());
        TimeSpan span = TimeSpan.Parse(reader.ReadElementContentAsString());
        reader.ReadEndElement();
        string location = reader.ReadElementContentAsString();
        string type = reader.ReadElementContentAsString();
        reader.ReadEndElement();

        ExamTime time = new(start, span);
        return new Exam(courseId, courseName, time, location, type);
    }
}