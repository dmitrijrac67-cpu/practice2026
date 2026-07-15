using System.Collections.Generic;
using System.Linq;
using Xunit;
using task02;

namespace task02tests
{
    public class StudentServiceTests
    {
        private readonly List<Student> _testStudents;
        private readonly StudentService _service;

        public StudentServiceTests()
        {
            _testStudents = new List<Student>
            {
                new Student { Name = "Èâàí", Faculty = "ÔÈÒ", Grades = new List<int> { 5, 4, 5 } },
                new Student { Name = "Àííà", Faculty = "ÔÈÒ", Grades = new List<int> { 3, 4, 3 } },
                new Student { Name = "Ïåòð", Faculty = "Ýêîíîìèêà", Grades = new List<int> { 5, 5, 5 } }
            };
            _service = new StudentService(_testStudents);
        }

        [Fact]
        public void GetStudentsByFaculty_ReturnsCorrectStudents()
        {
            var result = _service.GetStudentsByFaculty("ÔÈÒ").ToList();
            Assert.Equal(2, result.Count); 
            Assert.True(result.All(s => s.Faculty == "ÔÈÒ")); 
        }

        [Fact]
        public void GetStudentsWithMinAverageGrade_ReturnsCorrectStudents()
        {
            var result = _service.GetStudentsWithMinAverageGrade(4.5).ToList();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, s => s.Name == "Èâàí");
            Assert.Contains(result, s => s.Name == "Ïåòð");
        }

        [Fact]
        public void GetStudentsOrderedByName_ReturnsSortedStudents()
        {
            var result = _service.GetStudentsOrderedByName().ToList();

            Assert.Equal("Àííà", result[0].Name);
            Assert.Equal("Èâàí", result[1].Name);
            Assert.Equal("Ïåòð", result[2].Name);
        }

        [Fact]
        public void GroupStudentsByFaculty_ReturnsCorrectLookup()
        {
            var result = _service.GroupStudentsByFaculty();

            Assert.Equal(2, result.Count);
            Assert.Equal(2, result["ÔÈÒ"].Count());
            Assert.Single(result["Ýêîíîìèêà"]);
        }

        [Fact]
        public void GetFacultyWithHighestAverageGrade_ReturnsCorrectFaculty()
        {
            var result = _service.GetFacultyWithHighestAverageGrade();
            Assert.Equal("Ýêîíîìèêà", result);
        }
    }
}