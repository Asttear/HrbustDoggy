using Hrbust.Extensions;
using System.Collections;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Hrbust;

/// <summary>
/// 表示课程表。
/// </summary>
public class ClassTable : IEnumerable<Class>, IXmlSerializable
{
    private static readonly string[] s_days = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
    private Class[,] _classes;

    private ClassTable()
    {
        _classes = null!;
        StartTimes = null!;
    }

    /// <summary>
    /// 构造课程表。
    /// </summary>
    /// <param name="style">课表样式。</param>
    /// <param name="classDuration">每节课时长。</param>
    /// <param name="date">日期。</param>
    /// <param name="week">学周。</param>
    public ClassTable(ClassTableStyle style, TimeSpan classDuration, DateOnly date, int? week)
    {
        Style = style;
        DurationOfEachClass = classDuration;
        DateWhenObtained = date;
        WeekWhenObtained = week;
        int length = NumberOfClassesPerDay;
        StartTimes = new TimeOnly[length];
        _classes = new Class[7, length];
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < length; j++)
            {
                _classes[i, j] = new();
            }
        }
    }

    /// <summary>
    /// 获取该课程表时的日期。
    /// </summary>
    public DateOnly DateWhenObtained { get; private set; }

    /// <summary>
    /// 获取该课程表时的学周。
    /// </summary>
    public int? WeekWhenObtained { get; private set; }

    /// <summary>
    /// 课程开始时间。
    /// </summary>
    public TimeOnly[] StartTimes { get; private set; }

    /// <summary>
    /// 每节课时长。
    /// </summary>
    public TimeSpan DurationOfEachClass { get; private set; }

    /// <summary>
    /// 课程表样式（大节课表或小节课表）。
    /// </summary>
    public ClassTableStyle Style { get; private set; }

    /// <summary>
    /// 一天课程数。
    /// </summary>
    public int NumberOfClassesPerDay => (int)Style;

    /// <summary>
    /// 获取或设置该节课可能教学的课程。
    /// </summary>
    /// <param name="i">一周中的一天，<c>0</c> 代表周一。</param>
    /// <param name="j">一天中的课程节数，<c>0</c> 代表第一节。</param>
    /// <returns>该节课可能教学的课程。</returns>
    public Class this[int i, int j]
    {
        get => _classes[i, j];
        private set => _classes[i, j] = value;
    }

    /// <summary>
    /// 获取指定日期的学周。
    /// </summary>
    /// <param name="date">日期。</param>
    /// <returns>学周。如果课表获取时无学周信息，返回 <see langword="null"/>。</returns>
    public int? GetWeek(DateOnly date)
    {
        if (WeekWhenObtained is null)
        {
            return null;
        }
        int dayOfWeek = DateWhenObtained.DayOfWeek switch
        {
            DayOfWeek.Sunday => 6,
            _ => (int)DateWhenObtained.DayOfWeek - 1
        };
        return WeekWhenObtained + ((date.DayNumber - DateWhenObtained.DayNumber + dayOfWeek) / 7);
    }

    /// <summary>
    /// 获取当天课程。
    /// </summary>
    /// <param name="date">要获取的日期。</param>
    /// <returns>包含当天每节课对象的数组。如有撞课亦放入同一节课对象中。如果课表获取时无学周信息，返回 <see langword="null" />。</returns>
    public Class[]? GetClasses(DateOnly date)
    {
        int? week = GetWeek(date);
        if (week is null)
        {
            return null;
        }
        int day = date.DayOfWeek switch
        {
            DayOfWeek.Sunday => 6,
            _ => (int)date.DayOfWeek - 1
        };
        int length = NumberOfClassesPerDay;
        Class[] classes = Enumerable.Range(0, length)
                                    .Select(i => new Class(this[day, i].Where(c => c.CheckIfScheduled(week.Value))))
                                    .ToArray();
        return classes;
    }

    public IEnumerator<Class> GetEnumerator() => _classes.Cast<Class>().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _classes.GetEnumerator();

    public XmlSchema? GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        string? style = reader.GetAttribute("Style"),
            duration = reader.GetAttribute("Duration"),
            date = reader.GetAttribute("Date"),
            week = reader.GetAttribute("Week");
        if (style is null || duration is null || date is null || week is null)
        {
            throw new XmlException("Missing attributes of ClassTable!", null);
        }

        Style = Enum.Parse<ClassTableStyle>(style);
        DurationOfEachClass = TimeSpan.FromMinutes(int.Parse(duration));
        DateWhenObtained = DateOnly.Parse(date);
        WeekWhenObtained = string.IsNullOrEmpty(week) ? null : int.Parse(week);
        _classes = new Class[7, NumberOfClassesPerDay];

        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < NumberOfClassesPerDay; j++)
            {
                this[i, j] = new();
            }
        }

        reader.ReadStartElement("ClassTable");
        StartTimes = reader.ReadElementAsStartTimes().ToArray();
        for (int i = 0; i < 7; i++)
        {
            reader.ReadStartElement(s_days[i]);
            for (int j = 0; j < NumberOfClassesPerDay; j++)
            {
                if (reader.IsEmptyElement)
                {
                    reader.Read();
                    continue;
                }
                reader.ReadStartElement("Class");
                while (reader.IsStartElement("Course"))
                {
                    this[i, j].Add(reader.ReadElementAsCourse());
                }
                reader.ReadEndElement();
            }
            reader.ReadEndElement();
        }
        reader.ReadEndElement();
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("Style", Style.ToString());
        writer.WriteAttributeString("Duration", DurationOfEachClass.TotalMinutes.ToString());
        writer.WriteAttributeString("Date", DateWhenObtained.ToShortDateString());
        writer.WriteAttributeString("Week", WeekWhenObtained.ToString());
        writer.WriteElementStartTimes(StartTimes);
        for (int i = 0; i < 7; i++)
        {
            writer.WriteStartElement(s_days[i]);
            for (int j = 0; j < NumberOfClassesPerDay; j++)
            {
                writer.WriteStartElement("Class");
                foreach (Course course in this[i, j])
                {
                    writer.WriteElementCourse(course);
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }
}