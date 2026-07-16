using System;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace task13;

public class JsonService
{
    private readonly JsonSerializerOptions _settings;

    public JsonService()
    {
        _settings = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
        };
        _settings.Converters.Add(new CustomDateConverter());
    }

    public string SerializeStudent(Student student)
    {
        return JsonSerializer.Serialize(student, _settings);
    }

    public Student DeserializeStudent(string json)
    {
        var student = JsonSerializer.Deserialize<Student>(json, _settings);
        if (student == null)
        {
            throw new ArgumentException("Результат десериализации равен null.");
        }

        ValidateStudent(student);
        return student;
    }

    public void SaveToFile(Student student, string filePath)
    {
        File.WriteAllText(filePath, SerializeStudent(student));
    }

    public Student LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Указанный файл не найден.");
        }

        return DeserializeStudent(File.ReadAllText(filePath));
    }

    private static void ValidateStudent(Student student)
    {
        if (string.IsNullOrWhiteSpace(student.FirstName))
        {
            throw new ArgumentException("Имя не может быть пустым.");
        }

        if (string.IsNullOrWhiteSpace(student.LastName))
        {
            throw new ArgumentException("Фамилия не может быть пустой.");
        }

        if (student.BirthDate > DateTime.Now)
        {
            throw new ArgumentException("Дата рождения не может быть в будущем.");
        }

        if (student.Grades == null || !student.Grades.Any())
        {
            throw new ArgumentException("Список оценок должен содержать хотя бы один предмет.");
        }

        foreach (var subject in student.Grades)
        {
            if (string.IsNullOrWhiteSpace(subject.Name))
            {
                throw new ArgumentException("Название предмета не может быть пустым.");
            }

            if (subject.Grade < 2 || subject.Grade > 5)
            {
                throw new ArgumentException("Оценка должна быть в диапазоне от 2 до 5.");
            }
        }
    }
}