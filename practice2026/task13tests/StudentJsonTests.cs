using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Xunit;
using task13;

namespace task13tests;

public class StudentJsonTests
{
    private readonly JsonService _jsonService = new();

    [Fact]
    public void Serialize_ExcludesNullFieldsCorrectly()
    {
        var student = new Student
        {
            FirstName = "Александр",
            LastName = "Сидоров",
            BirthDate = new DateTime(2006, 6, 24),
            Grades = new List<Subject>
            {
                new() { Name = "Математика", Grade = 5 }
            },
            Information = null
        };

        string json = _jsonService.SerializeStudent(student);

        Assert.Contains("Александр", json);
        Assert.Contains("\"BirthDate\": \"2006-06-24\"", json);
        Assert.DoesNotContain("Information", json);
    }

    [Fact]
    public void Serialize_IncludesOptionalFieldsWhenNotNull()
    {
        var student = new Student
        {
            FirstName = "Варвара",
            LastName = "Иванова",
            BirthDate = new DateTime(2008, 2, 12),
            Grades = new List<Subject>
            {
                new() { Name = "Информатика", Grade = 5 }
            },
            Information = "Отличница"
        };

        string json = _jsonService.SerializeStudent(student);

        Assert.Contains("Отличница", json);
    }

    [Fact]
    public void Deserialize_ValidJson_ReturnsValidStudentObject()
    {
        string json = @"{
            ""FirstName"": ""Илья"",
            ""LastName"": ""Петров"",
            ""BirthDate"": ""2007-05-22"",
            ""Grades"": [
                { ""Name"": ""Физика"", ""Grade"": 4 }
            ]
        }";

        var student = _jsonService.DeserializeStudent(json);

        Assert.Equal("Илья", student.FirstName);
        Assert.Equal("Петров", student.LastName);
        Assert.Equal(new DateTime(2007, 5, 22), student.BirthDate);
    }

    [Fact]
    public void Deserialize_InvalidDateFormat_ThrowsJsonException()
    {
        string json = @"{
            ""FirstName"": ""Екатерина"",
            ""LastName"": ""Владимирова"",
            ""BirthDate"": ""25-04-2010"",
            ""Grades"": [
                { ""Name"": ""История"", ""Grade"": 5 }
            ]
        }";

        Assert.Throws<JsonException>(() => _jsonService.DeserializeStudent(json));
    }

    [Fact]
    public void Deserialize_EmptyName_ThrowsArgumentException()
    {
        string json = @"{
            ""FirstName"": """",
            ""LastName"": ""Сергеева"",
            ""BirthDate"": ""2005-01-01"",
            ""Grades"": [
                { ""Name"": ""Химия"", ""Grade"": 4 }
            ]
        }";

        Assert.Throws<ArgumentException>(() => _jsonService.DeserializeStudent(json));
    }

    [Fact]
    public void Program_MainExecution_OutputsValidConsoleData()
    {
        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        Program.Main();

        string output = consoleOutput.ToString();
        Assert.Contains("Алиса Смирнова", output);
        Assert.Contains("2007-07-07", output);
        Assert.Contains("Информация сохранена в файл", output);
    }
}