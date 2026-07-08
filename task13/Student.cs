using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace task13
{
    public class Student
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime BirthDate { get; set; }
        public List<Subject>? Grades { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(FirstName))
                throw new ArgumentException("FirstName не может быть пустым.");

            if (BirthDate > DateTime.Now)
                throw new ArgumentException("BirthDate не может быть в будущем.");

            if (Grades != null)
                foreach (var subject in Grades)
                {
                    if (string.IsNullOrWhiteSpace(subject.Name))
                        throw new ArgumentException("Название предмета не может быть пустым.");

                    if (subject.Grade < 0 || subject.Grade > 100)
                        throw new ArgumentOutOfRangeException(nameof(subject.Grade), "Баллы должны быть от 0 до 100.");
                }
        }
    }
}
