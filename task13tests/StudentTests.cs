using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using task13;

namespace task13tests
{
    public class StudentTests
    {
        [Fact]
        public void Deserialize_ValidJson_ShouldReturnCorrectObject()
        {
            string json = "{\n  \"FirstName\": \"Дмитрий\",\n  \"BirthDate\": \"2006-01-15\"\n}";

            Student student = StudentService.DeserializeStudent(json);

            Assert.Equal("Дмитрий", student.FirstName);
            Assert.Equal(new DateTime(2006, 1, 15), student.BirthDate.Date);
            Assert.Null(student.LastName);
        }

        [Fact]
        public void Deserialize_EmptyName_ShouldThrowArgumentException()
        {
            string invalidJson = "{\n  \"FirstName\": \"   \",\n  \"BirthDate\": \"2004-05-10\"\n}";

            Assert.Throws<ArgumentException>(() => StudentService.DeserializeStudent(invalidJson));
        }

        [Fact]
        public void Deserialize_InvalidGrade_ShouldThrowArgumentOutOfRangeException()
        {
            string invalidJson = "{\n  \"FirstName\": \"Игорь\",\n  \"BirthDate\": \"2005-03-22\",\n  \"Grades\": [\n    { \"Name\": \"Физика\", \"Grade\": 120 }\n  ]\n}";

            Assert.Throws<ArgumentOutOfRangeException>(() => StudentService.DeserializeStudent(invalidJson));
        }

        [Fact]
        public void FileIO_ShouldSaveAndLoadCorrectly()
        {
            string tempFilePath = Path.GetTempFileName();
            var student = new Student
            {
                FirstName = "Ольга",
                BirthDate = new DateTime(2004, 11, 30)
            };

            try
            {
                StudentService.SaveToFile(tempFilePath, student);
                Student loadedStudent = StudentService.LoadFromFile(tempFilePath);

                Assert.Equal("Ольга", loadedStudent.FirstName);
                Assert.Equal(student.BirthDate, loadedStudent.BirthDate);
            }
            finally
            {

                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }
    }
}
