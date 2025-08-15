using System;
using System.Collections.Generic;
using System.IO;

// ---------------- Custom Exceptions ----------------
public class InvalidScoreFormatException : Exception
{
    public InvalidScoreFormatException(string message) : base(message) { }
}

public class MissingFieldException : Exception
{
    public MissingFieldException(string message) : base(message) { }
}

// ---------------- Student Class ----------------
public class Student
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public int Score { get; set; }

    public Student(int id, string fullName, int score)
    {
        Id = id;
        FullName = fullName;
        Score = score;
    }

    public string GetGrade()
    {
        if (Score >= 80 && Score <= 100) return "A";
        if (Score >= 70) return "B";
        if (Score >= 60) return "C";
        if (Score >= 50) return "D";
        return "F";
    }
}

// ---------------- Student Result Processor ----------------
public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        var students = new List<Student>();

        using (StreamReader reader = new StreamReader(inputFilePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split(',');

                if (parts.Length != 3)
                    throw new MissingFieldException($"Missing field in record: \"{line}\"");

                if (!int.TryParse(parts[0].Trim(), out int id))
                    throw new InvalidScoreFormatException($"Invalid ID format in record: \"{line}\"");

                string fullName = parts[1].Trim();

                if (!int.TryParse(parts[2].Trim(), out int score))
                    throw new InvalidScoreFormatException($"Invalid score format in record: \"{line}\"");

                students.Add(new Student(id, fullName, score));
            }
        }

        return students;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using (StreamWriter writer = new StreamWriter(outputFilePath))
        {
            foreach (var student in students)
            {
                writer.WriteLine($"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
            }
        }
    }
}

// ---------------- Main Program ----------------
class Program
{
    static void Main(string[] args)
    {
        string inputFilePath = "students.txt";
        string outputFilePath = "report.txt";

        // Step 1: Create sample input file
        CreateSampleInputFile(inputFilePath);

        var processor = new StudentResultProcessor();

        try
        {
            // Step 2: Read and process students
            List<Student> students = processor.ReadStudentsFromFile(inputFilePath);

            // Step 3: Write the report
            processor.WriteReportToFile(students, outputFilePath);

            Console.WriteLine("✅ Report generated successfully!");
            Console.WriteLine(File.ReadAllText(outputFilePath));
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"❌ Error: Input file not found. {ex.Message}");
        }
        catch (InvalidScoreFormatException ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
        catch (MissingFieldException ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Unexpected error: {ex.Message}");
        }
    }

    // Method to create sample input file
    static void CreateSampleInputFile(string filePath)
    {
        string[] lines =
        {
            "101, Alice Smith, 84",
            "102, Bob Johnson, 75",
            "103, Charlie Brown, 92",
            "104, Diana Prince, 59",
            "105, Edward Stark, 48"
        };

        File.WriteAllLines(filePath, lines);
    }
}
