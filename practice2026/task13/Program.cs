using System;
using System.Collections.Generic;
using System.IO;

namespace task13;

public class Program
{
    public static void Main()
    {
        try
        {
            var jsonService = new JsonService();

            var student = new Student
            {
                FirstName = "Алиса",
                LastName = "Смирнова",
                BirthDate = new DateTime(2007, 7, 7),
                Grades = new List<Subject>
                {
                    new() { Name = "Алгоритмизация и Программирование", Grade = 5 },
                    new() { Name = "Иностранный язык", Grade = 5 },
                    new() { Name = "Математическая логика", Grade = 4 },
                    new() { Name = "Алгебра", Grade = 5 },
                    new() { Name = "Практика речевой деятельности", Grade = 4 }
                },
                Information = "Имеет повышенную стипендию за хорошую успеваемость"
            };

            string json = jsonService.SerializeStudent(student);
            Console.WriteLine(json);

            string filePath = "student.json";
            jsonService.SaveToFile(student, filePath);
            Console.WriteLine("Информация сохранена в файл 'student.json'");

            var loadedStudent = jsonService.LoadFromFile(filePath);
            Console.WriteLine($"Загружен студент: {loadedStudent.FirstName} {loadedStudent.LastName}");
            Console.WriteLine($"Дата рождения: {loadedStudent.BirthDate:yyyy-MM-dd}");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка {ex.GetType().Name}: {ex.Message}");
        }
    }
}