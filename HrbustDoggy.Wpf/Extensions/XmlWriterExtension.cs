using Hrbust;
using System;
using System.Xml;

namespace HrbustDoggy.Wpf.Extensions;

internal static class XmlWriterExtension
{
    private static readonly string[] s_days = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];

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

    public static void WriteElementClassTable(this XmlWriter writer, ClassTable table)
    {
        writer.WriteStartElement("ClassTable");
        writer.WriteAttributeString("Style", table.Style.ToString());
        writer.WriteAttributeString("Duration", table.DurationOfEachClass.TotalMinutes.ToString());
        writer.WriteAttributeString("Date", table.DateWhenObtained.ToShortDateString());
        writer.WriteAttributeString("Week", table.WeekWhenObtained.ToString());
        writer.WriteElementStartTimes(table.StartTimes);
        for (int i = 0; i < 7; i++)
        {
            writer.WriteStartElement(s_days[i]);
            for (int j = 0; j < table.NumberOfClassesPerDay; j++)
            {
                writer.WriteStartElement("Class");
                foreach (Course course in table[i, j])
                {
                    writer.WriteElementCourse(course);
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
        writer.WriteEndElement();
    }

    public static void WriteElementExam(this XmlWriter writer, Exam exam)
    {
        writer.WriteStartElement("Exam");
        writer.WriteElementString("CourseId", exam.CourseId);
        writer.WriteElementString("CourseName", exam.CourseName);
        writer.WriteStartElement("Time");
        writer.WriteElementString("Start", exam.Time.Start.ToString());
        writer.WriteElementString("Span", exam.Time.Span.ToString());
        writer.WriteEndElement();
        writer.WriteElementString("Location", exam.Location);
        writer.WriteElementString("Type", exam.Type);
        writer.WriteEndElement();
    }
}