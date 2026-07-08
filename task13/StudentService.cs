using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace task13
{
    public static class StudentService
    {

        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public static string SerializeStudent(Student student)
        {
            if (student == null) throw new ArgumentNullException(nameof(student));
            return JsonSerializer.Serialize(student, Options);
        }

        public static Student DeserializeStudent(string json)
        {
            var student = JsonSerializer.Deserialize<Student>(json, Options);

            student?.Validate();

            return student ?? throw new JsonException("Ошибка десериализации.");
        }

        public static void SaveToFile(string filePath, Student student)
        {
            string json = SerializeStudent(student); 
            File.WriteAllText(filePath, json);
        }
        public static Student LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл не найден.", filePath);

            string json = File.ReadAllText(filePath); 
            return DeserializeStudent(json);       
        }
    }
}
