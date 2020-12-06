/*
	CSharp-GradeBookApplication
	
	Description: Project to add features to an existing C Sharp Grade Book Application.
	Website: http:\\www.pluralsight.com
	         https://github.com/pmash2/CSharp-GradeBookApplication
*/


using System;
using GradeBook.UserInterfaces;

namespace GradeBook
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("#=======================#");
            Console.WriteLine("# Welcome to GradeBook! #");
            Console.WriteLine("#=======================#");
            Console.WriteLine();

            StartingUserInterface.CommandLoop();
            
            Console.WriteLine("Thank you for using GradeBook!");
            Console.WriteLine("Have a nice day!");
            Console.Read();
        }
    }
}

namespace GradeBook.Enums
{
    public enum EnrollmentType
    {
        Campus,
        State,
        National,
        International
    }
}

namespace GradeBook.Enums
{
  public enum GradeBookType
  {
    Standard,
    Ranked,
    ESNU,
    OneToFour,
    SixPoint
  }
}

namespace GradeBook.Enums
{
    public enum StudentType
    {
        Standard,
        Honors,
        DualEnrolled
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

using GradeBook.Enums;

namespace GradeBook
{
    public class Student
    {
        public string Name { get; set; }
        public StudentType Type { get; set; }
        public EnrollmentType Enrollment { get; set; }
        public List<double> Grades { get; set; }
        [JsonIgnore]
        public double AverageGrade
        {
            get
            {
                return Grades.Average();
            }
        }
        [JsonIgnore]
        public char LetterGrade { get; set; }
        [JsonIgnore]
        public double GPA { get; set; }

        public Student(string name, StudentType studentType, EnrollmentType enrollment)
        {
            Name = name;
            Type = studentType;
            Enrollment = enrollment;
            Grades = new List<double>();
        }

        public void AddGrade(double grade)
        {
            if (grade < 0 || grade > 100)
                throw new ArgumentException("Grades must be between 0 and 100.");
            Grades.Add(grade);
        }

        public void RemoveGrade(double grade)
        {
            Grades.Remove(grade);
        }
    }
}

using GradeBook.GradeBooks;
using System;

namespace GradeBook.UserInterfaces
{
    public static class StartingUserInterface
    {
        public static bool Quit = false;
        public static void CommandLoop()
        {
            while (!Quit)
            {
                Console.WriteLine("What would you like to do?");
                var command = Console.ReadLine().ToLower();
                CommandRoute(command);
            }
        }

        public static void CommandRoute(string command)
        {
            if (command.StartsWith("create"))
                CreateCommand(command);
            else if (command.StartsWith("load"))
                LoadCommand(command);
            else if (command == "help")
                HelpCommand();
            else if (command == "quit")
                Quit = true;
            else
                Console.WriteLine("{0} was not recognized, please try again.", command);
        }

        public static void CreateCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Command not valid, Create requires a name, type of gradebook, if it's weighted (true / false).");
                return;
            }
            var name = parts[1];
            var type = parts[2].ToLower();
            var weighted = bool.Parse(parts[3]);
            BaseGradeBook gradeBook;

            if (type == "standard")
                gradeBook = new StandardGradeBook(name, weighted);
            else if (type == "ranked")
                gradeBook = new RankedGradeBook(name, weighted);
            else
            {
                System.Console.WriteLine("{0} is not a supported type of gradebook, please try again", type);
                return;
            }

            Console.WriteLine("Created gradebook {0}.", name);
            GradeBookUserInterface.CommandLoop(gradeBook);
        }

        public static void LoadCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Load requires a name.");
                return;
            }
            var name = parts[1];
            var gradeBook = BaseGradeBook.Load(name);

            if (gradeBook == null)
                return;

            GradeBookUserInterface.CommandLoop(gradeBook);
        }

        public static void HelpCommand()
        {
            Console.WriteLine("GradeBook accepts the following commands:");
            Console.WriteLine();
            Console.WriteLine("Create 'Name' 'Type' 'Weighted' - Creates a new gradebook where 'Name' is the name of the gradebook, 'Type' is what type of grading it should use, and 'Weighted' is whether or not grades should be weighted (true or false).");
            Console.WriteLine();
            Console.WriteLine("Load 'Name' - Loads the gradebook with the provided 'Name'.");
            Console.WriteLine();
            Console.WriteLine("Help - Displays all accepted commands.");
            Console.WriteLine();
            Console.WriteLine("Quit - Exits the application");
        }
    }
}


using GradeBook.Enums;
using GradeBook.GradeBooks;
using System;

namespace GradeBook.UserInterfaces
{
    public static class GradeBookUserInterface
    {
        public static BaseGradeBook GradeBook;
        public static bool Quit = false;
        public static void CommandLoop(BaseGradeBook gradeBook)
        {
            GradeBook = gradeBook;
            Quit = false;

            Console.WriteLine("#=======================#");
            Console.WriteLine(GradeBook.Name + " : " + GradeBook.GetType().Name);
            Console.WriteLine("#=======================#");
            Console.WriteLine(string.Empty);

            while(!Quit)
            {
                Console.WriteLine("What would you like to do?");
                var command = Console.ReadLine().ToLower();
                CommandRoute(command);
            }

            Console.WriteLine(GradeBook.Name + " has been closed.");
        }

        public static void CommandRoute(string command)
        {
            if (command == "save")
                SaveCommand();
            else if (command.StartsWith("addgrade"))
                AddGradeCommand(command);
            else if (command.StartsWith("removegrade"))
                RemoveGradeCommand(command);
            else if (command.StartsWith("add"))
                AddStudentCommand(command);
            else if (command.StartsWith("remove"))
                RemoveStudentCommand(command);
            else if (command == "list")
                ListCommand();
            else if (command == "statistics all")
                StatisticsCommand();
            else if (command.StartsWith("statistics"))
                StudentStatisticsCommand(command);
            else if (command == "help")
                HelpCommand();
            else if (command == "close")
                Quit = true;
            else
                Console.WriteLine("{0} was not recognized, please try again.", command);
        }

        public static void SaveCommand()
        {
            GradeBook.Save();
            Console.WriteLine("{0} has been saved.", GradeBook.Name);
        }
        
        public static void AddGradeCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Command not valid, AddGrade requires a name and score.");
                return;
            }
            var name = parts[1];
            var score = Double.Parse(parts[2]);
            GradeBook.AddGrade(name, score);
            Console.WriteLine("Added a score of {0} to {1}'s grades", score, name);
        }

        public static void RemoveGradeCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Command not valid, RemoveGrade requires a name and score.");
                return;
            }
            var name = parts[1];
            var score = Double.Parse(parts[2]);
            GradeBook.RemoveGrade(name, score);
            Console.WriteLine("Removed a score of {0} from {1}'s grades", score, name);
        }

        public static void AddStudentCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Command not valid, Add requires a name, student type, enrollment type.");
                return;
            }
            var name = parts[1];

            StudentType studentType;
            if (!Enum.TryParse(parts[2], true, out studentType))
            {
                Console.WriteLine("{0} is not a valid student type, try again.", parts[2]);
                return;
            }

            EnrollmentType enrollmentType;
            if (!Enum.TryParse(parts[3], true, out enrollmentType))
            {
                Console.WriteLine("{0} is not a volid enrollment type, try again.", parts[3]);
                return;
            }

            var student = new Student(name, studentType, enrollmentType);
            GradeBook.AddStudent(student);
            Console.WriteLine("Added {0} to the gradebook.", name);
        }
        
        public static void RemoveStudentCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Remove requires a name.");
                return;
            }
            var name = parts[1];
            GradeBook.RemoveStudent(name);
            Console.WriteLine("Removed {0} from the gradebook.", name);
        }

        public static void ListCommand()
        {
            GradeBook.ListStudents();
        }
        
        public static void StatisticsCommand()
        {
            GradeBook.CalculateStatistics();
        }

        public static void StudentStatisticsCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Requires Name or All.");
                return;
            }
            var name = parts[1];
            GradeBook.CalculateStudentStatistics(name);
        }

        public static void HelpCommand()
        {
            Console.WriteLine("While a gradebook is open you can use the following commands:");
            Console.WriteLine();
            Console.WriteLine("Add 'Name' 'Student Type' 'Enrollment Type' - Adds a new student to the gradebook with the provided name, type of student, and type of enrollment.");
            Console.WriteLine();
            Console.WriteLine("Accepted Student Types:");
            Console.WriteLine("Standard - Student not enrolled in Honors classes or Dual Enrolled.");
            Console.WriteLine("Honors - Students enrolled in Honors classes and not Dual Enrolled.");
            Console.WriteLine("DualEnrolled - Students who are Duel Enrolled.");
            Console.WriteLine();
            Console.WriteLine("Accepted Enrollement Types:");
            Console.WriteLine("Campus - Students who are in the same disctrict as the school.");
            Console.WriteLine("State - Students who's legal residence is outside the school's district, but is in the same state as the school.");
            Console.WriteLine("National - Students who's legal residence is not in the same state as the school, but is in the same country as the school.");
            Console.WriteLine("International - Students who's legal residence is not in the same country as the school.");
            Console.WriteLine();
            Console.WriteLine("List - Lists all students.");
            Console.WriteLine();
            Console.WriteLine("AddGrade 'Name' 'Score' - Adds a new grade to a student with the matching name of the provided score.");
            Console.WriteLine();
            Console.WriteLine("RemoveGrade 'Name' 'Score' - Removes a grade to a student with the matching name and score.");
            Console.WriteLine();
            Console.WriteLine("Remove 'Name' - Removes the student with the provided name.");
            Console.WriteLine();
            Console.WriteLine("Statistics 'Name' - Gets statistics for the specified student.");
            Console.WriteLine();
            Console.WriteLine("Statistics All - Gets general statistics for the entire gradebook.");
            Console.WriteLine();
            Console.WriteLine("Close - closes the gradebook and takes you back to the starting command options.");
            Console.WriteLine();
            Console.WriteLine("Save - saves the gradebook to the hard drive for later use.");
        }
    }
}

using System;
using System.Linq;

using GradeBook.Enums;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GradeBook.GradeBooks
{
    public abstract class BaseGradeBook
    {
        public string Name { get; set; }
        public List<Student> Students { get; set; }
        public GradeBookType Type { get; set; }
        public bool IsWeighted { get; set; }

        public BaseGradeBook(string name, bool isWeighted)
        {
            Name = name;
            Students = new List<Student>();
            IsWeighted = isWeighted;
        }

        public void AddStudent(Student student)
        {
            if (string.IsNullOrEmpty(student.Name))
                throw new ArgumentException("A Name is required to add a student to a gradebook.");
            Students.Add(student);
        }

        public void RemoveStudent(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to remove a student from a gradebook.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            Students.Remove(student);
        }

        public void AddGrade(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to add a grade to a student.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            student.AddGrade(score);
        }

        public void RemoveGrade(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to remove a grade from a student.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            student.RemoveGrade(score);
        }

        public void ListStudents()
        {
            foreach (var student in Students)
            {
                Console.WriteLine("{0} : {1} : {2}", student.Name, student.Type, student.Enrollment);
            }
        }

        public static BaseGradeBook Load(string name)
        {
            if (!File.Exists(name + ".gdbk"))
            {
                Console.WriteLine("Gradebook could not be found.");
                return null;
            }

            using (var file = new FileStream(name + ".gdbk", FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(file))
                {
                    var json = reader.ReadToEnd();
                    return ConvertToGradeBook(json);
                }
            }
        }

        public void Save()
        {
            using (var file = new FileStream(Name + ".gdbk", FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(file))
                {
                    var json = JsonConvert.SerializeObject(this);
                    writer.Write(json);
                }
            }
        }

        public virtual double GetGPA(char letterGrade, StudentType studentType)
        {
            var gpa = 0;
            
            switch (letterGrade)
            {
                case 'A':
                    gpa = 4;
                    break;
                case 'B':
                    gpa = 3;
                    break;
                case 'C':
                    gpa = 2;
                    break;
                case 'D':
                    gpa = 1;
                    break;
            }
            
            if (IsWeighted && (studentType == StudentType.Honors || studentType == StudentType.DualEnrolled))
                gpa++;
            
            return gpa;
        }

        public virtual void CalculateStatistics()
        {
            var allStudentsPoints = 0d;
            var campusPoints = 0d;
            var statePoints = 0d;
            var nationalPoints = 0d;
            var internationalPoints = 0d;
            var standardPoints = 0d;
            var honorPoints = 0d;
            var dualEnrolledPoints = 0d;

            foreach (var student in Students)
            {
                student.LetterGrade = GetLetterGrade(student.AverageGrade);
                student.GPA = GetGPA(student.LetterGrade, student.Type);

                Console.WriteLine("{0} ({1}:{2}) GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
                allStudentsPoints += student.AverageGrade;

                switch (student.Enrollment)
                {
                    case EnrollmentType.Campus:
                        campusPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.State:
                        statePoints += student.AverageGrade;
                        break;
                    case EnrollmentType.National:
                        nationalPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.International:
                        internationalPoints += student.AverageGrade;
                        break;
                }

                switch (student.Type)
                {
                    case StudentType.Standard:
                        standardPoints += student.AverageGrade;
                        break;
                    case StudentType.Honors:
                        honorPoints += student.AverageGrade;
                        break;
                    case StudentType.DualEnrolled:
                        dualEnrolledPoints += student.AverageGrade;
                        break;
                }
            }

            //#todo refactor into it's own method with calculations performed here
            Console.WriteLine("Average Grade of all students is " + (allStudentsPoints / Students.Count));
            if (campusPoints != 0)
                Console.WriteLine("Average for only local students is " + (campusPoints / Students.Where(e => e.Enrollment == EnrollmentType.Campus).Count()));
            if (statePoints != 0)
                Console.WriteLine("Average for only state students (excluding local) is " + (statePoints / Students.Where(e => e.Enrollment == EnrollmentType.State).Count()));
            if (nationalPoints != 0)
                Console.WriteLine("Average for only national students (excluding state and local) is " + (nationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.National).Count()));
            if (internationalPoints != 0)
                Console.WriteLine("Average for only international students is " + (internationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.International).Count()));
            if (standardPoints != 0)
                Console.WriteLine("Average for students excluding honors and duel enrollment is " + (standardPoints / Students.Where(e => e.Type == StudentType.Standard).Count()));
            if (honorPoints != 0)
                Console.WriteLine("Average for only honors students is " + (honorPoints / Students.Where(e => e.Type == StudentType.Honors).Count()));
            if (dualEnrolledPoints != 0)
                Console.WriteLine("Average for only duel enrolled students is " + (dualEnrolledPoints / Students.Where(e => e.Type == StudentType.DualEnrolled).Count()));
        }

        public virtual void CalculateStudentStatistics(string name)
        {
            var student = Students.FirstOrDefault(e => e.Name == name);
            student.LetterGrade = GetLetterGrade(student.AverageGrade);
            student.GPA = GetGPA(student.LetterGrade, student.Type);

            Console.WriteLine("{0} ({1}:{2}) GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
            Console.WriteLine();
            Console.WriteLine("Grades:");
            foreach (var grade in student.Grades)
            {
                Console.WriteLine(grade);
            }
        }

        public virtual char GetLetterGrade(double averageGrade)
        {
            if (averageGrade >= 90)
                return 'A';
            else if (averageGrade >= 80)
                return 'B';
            else if (averageGrade >= 70)
                return 'C';
            else if (averageGrade >= 60)
                return 'D';
            else
                return 'F';
        }

        /// <summary>
        ///     Converts json to the appropriate grade book type.
        ///     Note: This method contains code that is not recommended practice.
        ///     This has been used as a compromise to avoid adding additional complexity to the learner.
        /// </summary>
        /// <returns>The to grade book.</returns>
        /// <param name="json">Json.</param>
        public static dynamic ConvertToGradeBook(string json)
        {
            // Get GradeBookType from the GradeBook.Enums namespace
            var gradebookEnum = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 from type in assembly.GetTypes()
                                 where type.FullName == "GradeBook.Enums.GradeBookType"
                                 select type).FirstOrDefault();

            var jobject = JsonConvert.DeserializeObject<JObject>(json);
            var gradeBookType = jobject.Property("Type")?.Value?.ToString();

            // Check if StandardGradeBook exists
            if ((from assembly in AppDomain.CurrentDomain.GetAssemblies()
                 from type in assembly.GetTypes()
                 where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                 select type).FirstOrDefault() == null)
                gradeBookType = "Base";
            else
            {
                if (string.IsNullOrEmpty(gradeBookType))
                    gradeBookType = "Standard";
                else
                    gradeBookType = Enum.GetName(gradebookEnum, int.Parse(gradeBookType));
            }

            // Get GradeBook from the GradeBook.GradeBooks namespace
            var gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks." + gradeBookType + "GradeBook"
                             select type).FirstOrDefault();


            //protection code
            if (gradebook == null)
                gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                             select type).FirstOrDefault();
            
            return JsonConvert.DeserializeObject(json, gradebook);
        }
    }
}

using GradeBook.Enums;
using System;
using System.Linq;

namespace GradeBook.GradeBooks
{
  public class RankedGradeBook : BaseGradeBook
  {
    public RankedGradeBook(string name, bool isWeighted) : base(name, isWeighted)
    {
      Type = GradeBookType.Ranked;
    }

    public override void CalculateStudentStatistics(string name)
    {
      if (Students.Count < 5)
      {
        System.Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
        return;
      }
      base.CalculateStudentStatistics(name);
    }

    public override void CalculateStatistics()
    {
      if (Students.Count < 5)
      {
        System.Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
        return;
      }
      base.CalculateStatistics();
    }

    public override char GetLetterGrade(double averageGrade)
    {
      if (Students.Count < 5)
      {
        throw new InvalidOperationException("Ranked grading requires at least 5 students");
      }

      // Number of students that should receive each letter grade
      var threshold = (int)Math.Ceiling(Students.Count * 0.2);

      //                Order students by       their avg grade   and take their avg grade val  to a list
      var grades = Students.OrderByDescending(e => e.AverageGrade).Select(e => e.AverageGrade).ToList();
      
      if (grades[threshold - 1] <= averageGrade)
        return 'A';
      else if (grades[(threshold * 2) - 1] <= averageGrade)
        return 'B';
      else if (grades[(threshold * 3) - 1] <= averageGrade)
        return 'C';
      else if (grades[(threshold * 4) - 1] <= averageGrade)
        return 'D';
      else
        return 'F';
    }
  }
}

using GradeBook.Enums;

namespace GradeBook.GradeBooks
{
  public class StandardGradeBook : BaseGradeBook
  {

    public StandardGradeBook(string name, bool isWeighted) : base(name, isWeighted)
    {
      Type = GradeBookType.Standard;
    }
  }
}

/*
	CSharp-GradeBookApplication
	
	Description: Project to add features to an existing C Sharp Grade Book Application.
	Website: http:\\www.pluralsight.com
	         https://github.com/pmash2/CSharp-GradeBookApplication
*/


using System;
using GradeBook.UserInterfaces;

namespace GradeBook
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("#=======================#");
            Console.WriteLine("# Welcome to GradeBook! #");
            Console.WriteLine("#=======================#");
            Console.WriteLine();

            StartingUserInterface.CommandLoop();
            
            Console.WriteLine("Thank you for using GradeBook!");
            Console.WriteLine("Have a nice day!");
            Console.Read();
        }
    }
}

namespace GradeBook.Enums
{
    public enum EnrollmentType
    {
        Campus,
        State,
        National,
        International
    }
}

namespace GradeBook.Enums
{
  public enum GradeBookType
  {
    Standard,
    Ranked,
    ESNU,
    OneToFour,
    SixPoint
  }
}

namespace GradeBook.Enums
{
    public enum StudentType
    {
        Standard,
        Honors,
        DualEnrolled
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

using GradeBook.Enums;

namespace GradeBook
{
    public class Student
    {
        public string Name { get; set; }
        public StudentType Type { get; set; }
        public EnrollmentType Enrollment { get; set; }
        public List<double> Grades { get; set; }
        [JsonIgnore]
        public double AverageGrade
        {
            get
            {
                return Grades.Average();
            }
        }
        [JsonIgnore]
        public char LetterGrade { get; set; }
        [JsonIgnore]
        public double GPA { get; set; }

        public Student(string name, StudentType studentType, EnrollmentType enrollment)
        {
            Name = name;
            Type = studentType;
            Enrollment = enrollment;
            Grades = new List<double>();
        }

        public void AddGrade(double grade)
        {
            if (grade < 0 || grade > 100)
                throw new ArgumentException("Grades must be between 0 and 100.");
            Grades.Add(grade);
        }

        public void RemoveGrade(double grade)
        {
            Grades.Remove(grade);
        }
    }
}

using GradeBook.GradeBooks;
using System;

namespace GradeBook.UserInterfaces
{
    public static class StartingUserInterface
    {
        public static bool Quit = false;
        public static void CommandLoop()
        {
            while (!Quit)
            {
                Console.WriteLine("What would you like to do?");
                var command = Console.ReadLine().ToLower();
                CommandRoute(command);
            }
        }

        public static void CommandRoute(string command)
        {
            if (command.StartsWith("create"))
                CreateCommand(command);
            else if (command.StartsWith("load"))
                LoadCommand(command);
            else if (command == "help")
                HelpCommand();
            else if (command == "quit")
                Quit = true;
            else
                Console.WriteLine("{0} was not recognized, please try again.", command);
        }

        public static void CreateCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Command not valid, Create requires a name, type of gradebook, if it's weighted (true / false).");
                return;
            }
            var name = parts[1];
            var type = parts[2].ToLower();
            var weighted = bool.Parse(parts[3]);
            BaseGradeBook gradeBook;

            if (type == "standard")
                gradeBook = new StandardGradeBook(name, weighted);
            else if (type == "ranked")
                gradeBook = new RankedGradeBook(name, weighted);
            else
            {
                System.Console.WriteLine("{0} is not a supported type of gradebook, please try again", type);
                return;
            }

            Console.WriteLine("Created gradebook {0}.", name);
            GradeBookUserInterface.CommandLoop(gradeBook);
        }

        public static void LoadCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Load requires a name.");
                return;
            }
            var name = parts[1];
            var gradeBook = BaseGradeBook.Load(name);

            if (gradeBook == null)
                return;

            GradeBookUserInterface.CommandLoop(gradeBook);
        }

        public static void HelpCommand()
        {
            Console.WriteLine("GradeBook accepts the following commands:");
            Console.WriteLine();
            Console.WriteLine("Create 'Name' 'Type' 'Weighted' - Creates a new gradebook where 'Name' is the name of the gradebook, 'Type' is what type of grading it should use, and 'Weighted' is whether or not grades should be weighted (true or false).");
            Console.WriteLine();
            Console.WriteLine("Load 'Name' - Loads the gradebook with the provided 'Name'.");
            Console.WriteLine();
            Console.WriteLine("Help - Displays all accepted commands.");
            Console.WriteLine();
            Console.WriteLine("Quit - Exits the application");
        }
    }
}


using GradeBook.Enums;
using GradeBook.GradeBooks;
using System;

namespace GradeBook.UserInterfaces
{
    public static class GradeBookUserInterface
    {
        public static BaseGradeBook GradeBook;
        public static bool Quit = false;
        public static void CommandLoop(BaseGradeBook gradeBook)
        {
            GradeBook = gradeBook;
            Quit = false;

            Console.WriteLine("#=======================#");
            Console.WriteLine(GradeBook.Name + " : " + GradeBook.GetType().Name);
            Console.WriteLine("#=======================#");
            Console.WriteLine(string.Empty);

            while(!Quit)
            {
                Console.WriteLine("What would you like to do?");
                var command = Console.ReadLine().ToLower();
                CommandRoute(command);
            }

            Console.WriteLine(GradeBook.Name + " has been closed.");
        }

        public static void CommandRoute(string command)
        {
            if (command == "save")
                SaveCommand();
            else if (command.StartsWith("addgrade"))
                AddGradeCommand(command);
            else if (command.StartsWith("removegrade"))
                RemoveGradeCommand(command);
            else if (command.StartsWith("add"))
                AddStudentCommand(command);
            else if (command.StartsWith("remove"))
                RemoveStudentCommand(command);
            else if (command == "list")
                ListCommand();
            else if (command == "statistics all")
                StatisticsCommand();
            else if (command.StartsWith("statistics"))
                StudentStatisticsCommand(command);
            else if (command == "help")
                HelpCommand();
            else if (command == "close")
                Quit = true;
            else
                Console.WriteLine("{0} was not recognized, please try again.", command);
        }

        public static void SaveCommand()
        {
            GradeBook.Save();
            Console.WriteLine("{0} has been saved.", GradeBook.Name);
        }
        
        public static void AddGradeCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Command not valid, AddGrade requires a name and score.");
                return;
            }
            var name = parts[1];
            var score = Double.Parse(parts[2]);
            GradeBook.AddGrade(name, score);
            Console.WriteLine("Added a score of {0} to {1}'s grades", score, name);
        }

        public static void RemoveGradeCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Command not valid, RemoveGrade requires a name and score.");
                return;
            }
            var name = parts[1];
            var score = Double.Parse(parts[2]);
            GradeBook.RemoveGrade(name, score);
            Console.WriteLine("Removed a score of {0} from {1}'s grades", score, name);
        }

        public static void AddStudentCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Command not valid, Add requires a name, student type, enrollment type.");
                return;
            }
            var name = parts[1];

            StudentType studentType;
            if (!Enum.TryParse(parts[2], true, out studentType))
            {
                Console.WriteLine("{0} is not a valid student type, try again.", parts[2]);
                return;
            }

            EnrollmentType enrollmentType;
            if (!Enum.TryParse(parts[3], true, out enrollmentType))
            {
                Console.WriteLine("{0} is not a volid enrollment type, try again.", parts[3]);
                return;
            }

            var student = new Student(name, studentType, enrollmentType);
            GradeBook.AddStudent(student);
            Console.WriteLine("Added {0} to the gradebook.", name);
        }
        
        public static void RemoveStudentCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Remove requires a name.");
                return;
            }
            var name = parts[1];
            GradeBook.RemoveStudent(name);
            Console.WriteLine("Removed {0} from the gradebook.", name);
        }

        public static void ListCommand()
        {
            GradeBook.ListStudents();
        }
        
        public static void StatisticsCommand()
        {
            GradeBook.CalculateStatistics();
        }

        public static void StudentStatisticsCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Requires Name or All.");
                return;
            }
            var name = parts[1];
            GradeBook.CalculateStudentStatistics(name);
        }

        public static void HelpCommand()
        {
            Console.WriteLine("While a gradebook is open you can use the following commands:");
            Console.WriteLine();
            Console.WriteLine("Add 'Name' 'Student Type' 'Enrollment Type' - Adds a new student to the gradebook with the provided name, type of student, and type of enrollment.");
            Console.WriteLine();
            Console.WriteLine("Accepted Student Types:");
            Console.WriteLine("Standard - Student not enrolled in Honors classes or Dual Enrolled.");
            Console.WriteLine("Honors - Students enrolled in Honors classes and not Dual Enrolled.");
            Console.WriteLine("DualEnrolled - Students who are Duel Enrolled.");
            Console.WriteLine();
            Console.WriteLine("Accepted Enrollement Types:");
            Console.WriteLine("Campus - Students who are in the same disctrict as the school.");
            Console.WriteLine("State - Students who's legal residence is outside the school's district, but is in the same state as the school.");
            Console.WriteLine("National - Students who's legal residence is not in the same state as the school, but is in the same country as the school.");
            Console.WriteLine("International - Students who's legal residence is not in the same country as the school.");
            Console.WriteLine();
            Console.WriteLine("List - Lists all students.");
            Console.WriteLine();
            Console.WriteLine("AddGrade 'Name' 'Score' - Adds a new grade to a student with the matching name of the provided score.");
            Console.WriteLine();
            Console.WriteLine("RemoveGrade 'Name' 'Score' - Removes a grade to a student with the matching name and score.");
            Console.WriteLine();
            Console.WriteLine("Remove 'Name' - Removes the student with the provided name.");
            Console.WriteLine();
            Console.WriteLine("Statistics 'Name' - Gets statistics for the specified student.");
            Console.WriteLine();
            Console.WriteLine("Statistics All - Gets general statistics for the entire gradebook.");
            Console.WriteLine();
            Console.WriteLine("Close - closes the gradebook and takes you back to the starting command options.");
            Console.WriteLine();
            Console.WriteLine("Save - saves the gradebook to the hard drive for later use.");
        }
    }
}

using System;
using System.Linq;

using GradeBook.Enums;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GradeBook.GradeBooks
{
    public abstract class BaseGradeBook
    {
        public string Name { get; set; }
        public List<Student> Students { get; set; }
        public GradeBookType Type { get; set; }
        public bool IsWeighted { get; set; }

        public BaseGradeBook(string name, bool isWeighted)
        {
            Name = name;
            Students = new List<Student>();
            IsWeighted = isWeighted;
        }

        public void AddStudent(Student student)
        {
            if (string.IsNullOrEmpty(student.Name))
                throw new ArgumentException("A Name is required to add a student to a gradebook.");
            Students.Add(student);
        }

        public void RemoveStudent(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to remove a student from a gradebook.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            Students.Remove(student);
        }

        public void AddGrade(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to add a grade to a student.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            student.AddGrade(score);
        }

        public void RemoveGrade(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to remove a grade from a student.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            student.RemoveGrade(score);
        }

        public void ListStudents()
        {
            foreach (var student in Students)
            {
                Console.WriteLine("{0} : {1} : {2}", student.Name, student.Type, student.Enrollment);
            }
        }

        public static BaseGradeBook Load(string name)
        {
            if (!File.Exists(name + ".gdbk"))
            {
                Console.WriteLine("Gradebook could not be found.");
                return null;
            }

            using (var file = new FileStream(name + ".gdbk", FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(file))
                {
                    var json = reader.ReadToEnd();
                    return ConvertToGradeBook(json);
                }
            }
        }

        public void Save()
        {
            using (var file = new FileStream(Name + ".gdbk", FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(file))
                {
                    var json = JsonConvert.SerializeObject(this);
                    writer.Write(json);
                }
            }
        }

        public virtual double GetGPA(char letterGrade, StudentType studentType)
        {
            var gpa = 0;
            
            switch (letterGrade)
            {
                case 'A':
                    gpa = 4;
                    break;
                case 'B':
                    gpa = 3;
                    break;
                case 'C':
                    gpa = 2;
                    break;
                case 'D':
                    gpa = 1;
                    break;
            }
            
            if (IsWeighted && (studentType == StudentType.Honors || studentType == StudentType.DualEnrolled))
                gpa++;
            
            return gpa;
        }

        public virtual void CalculateStatistics()
        {
            var allStudentsPoints = 0d;
            var campusPoints = 0d;
            var statePoints = 0d;
            var nationalPoints = 0d;
            var internationalPoints = 0d;
            var standardPoints = 0d;
            var honorPoints = 0d;
            var dualEnrolledPoints = 0d;

            foreach (var student in Students)
            {
                student.LetterGrade = GetLetterGrade(student.AverageGrade);
                student.GPA = GetGPA(student.LetterGrade, student.Type);

                Console.WriteLine("{0} ({1}:{2}) GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
                allStudentsPoints += student.AverageGrade;

                switch (student.Enrollment)
                {
                    case EnrollmentType.Campus:
                        campusPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.State:
                        statePoints += student.AverageGrade;
                        break;
                    case EnrollmentType.National:
                        nationalPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.International:
                        internationalPoints += student.AverageGrade;
                        break;
                }

                switch (student.Type)
                {
                    case StudentType.Standard:
                        standardPoints += student.AverageGrade;
                        break;
                    case StudentType.Honors:
                        honorPoints += student.AverageGrade;
                        break;
                    case StudentType.DualEnrolled:
                        dualEnrolledPoints += student.AverageGrade;
                        break;
                }
            }

            //#todo refactor into it's own method with calculations performed here
            Console.WriteLine("Average Grade of all students is " + (allStudentsPoints / Students.Count));
            if (campusPoints != 0)
                Console.WriteLine("Average for only local students is " + (campusPoints / Students.Where(e => e.Enrollment == EnrollmentType.Campus).Count()));
            if (statePoints != 0)
                Console.WriteLine("Average for only state students (excluding local) is " + (statePoints / Students.Where(e => e.Enrollment == EnrollmentType.State).Count()));
            if (nationalPoints != 0)
                Console.WriteLine("Average for only national students (excluding state and local) is " + (nationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.National).Count()));
            if (internationalPoints != 0)
                Console.WriteLine("Average for only international students is " + (internationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.International).Count()));
            if (standardPoints != 0)
                Console.WriteLine("Average for students excluding honors and duel enrollment is " + (standardPoints / Students.Where(e => e.Type == StudentType.Standard).Count()));
            if (honorPoints != 0)
                Console.WriteLine("Average for only honors students is " + (honorPoints / Students.Where(e => e.Type == StudentType.Honors).Count()));
            if (dualEnrolledPoints != 0)
                Console.WriteLine("Average for only duel enrolled students is " + (dualEnrolledPoints / Students.Where(e => e.Type == StudentType.DualEnrolled).Count()));
        }

        public virtual void CalculateStudentStatistics(string name)
        {
            var student = Students.FirstOrDefault(e => e.Name == name);
            student.LetterGrade = GetLetterGrade(student.AverageGrade);
            student.GPA = GetGPA(student.LetterGrade, student.Type);

            Console.WriteLine("{0} ({1}:{2}) GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
            Console.WriteLine();
            Console.WriteLine("Grades:");
            foreach (var grade in student.Grades)
            {
                Console.WriteLine(grade);
            }
        }

        public virtual char GetLetterGrade(double averageGrade)
        {
            if (averageGrade >= 90)
                return 'A';
            else if (averageGrade >= 80)
                return 'B';
            else if (averageGrade >= 70)
                return 'C';
            else if (averageGrade >= 60)
                return 'D';
            else
                return 'F';
        }

        /// <summary>
        ///     Converts json to the appropriate grade book type.
        ///     Note: This method contains code that is not recommended practice.
        ///     This has been used as a compromise to avoid adding additional complexity to the learner.
        /// </summary>
        /// <returns>The to grade book.</returns>
        /// <param name="json">Json.</param>
        public static dynamic ConvertToGradeBook(string json)
        {
            // Get GradeBookType from the GradeBook.Enums namespace
            var gradebookEnum = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 from type in assembly.GetTypes()
                                 where type.FullName == "GradeBook.Enums.GradeBookType"
                                 select type).FirstOrDefault();

            var jobject = JsonConvert.DeserializeObject<JObject>(json);
            var gradeBookType = jobject.Property("Type")?.Value?.ToString();

            // Check if StandardGradeBook exists
            if ((from assembly in AppDomain.CurrentDomain.GetAssemblies()
                 from type in assembly.GetTypes()
                 where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                 select type).FirstOrDefault() == null)
                gradeBookType = "Base";
            else
            {
                if (string.IsNullOrEmpty(gradeBookType))
                    gradeBookType = "Standard";
                else
                    gradeBookType = Enum.GetName(gradebookEnum, int.Parse(gradeBookType));
            }

            // Get GradeBook from the GradeBook.GradeBooks namespace
            var gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks." + gradeBookType + "GradeBook"
                             select type).FirstOrDefault();


            //protection code
            if (gradebook == null)
                gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                             select type).FirstOrDefault();
            
            return JsonConvert.DeserializeObject(json, gradebook);
        }
    }
}

using GradeBook.Enums;
using System;
using System.Linq;

namespace GradeBook.GradeBooks
{
  public class RankedGradeBook : BaseGradeBook
  {
    public RankedGradeBook(string name, bool isWeighted) : base(name, isWeighted)
    {
      Type = GradeBookType.Ranked;
    }

    public override void CalculateStudentStatistics(string name)
    {
      if (Students.Count < 5)
      {
        System.Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
        return;
      }
      base.CalculateStudentStatistics(name);
    }

    public override void CalculateStatistics()
    {
      if (Students.Count < 5)
      {
        System.Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
        return;
      }
      base.CalculateStatistics();
    }

    public override char GetLetterGrade(double averageGrade)
    {
      if (Students.Count < 5)
      {
        throw new InvalidOperationException("Ranked grading requires at least 5 students");
      }

      // Number of students that should receive each letter grade
      var threshold = (int)Math.Ceiling(Students.Count * 0.2);

      //                Order students by       their avg grade   and take their avg grade val  to a list
      var grades = Students.OrderByDescending(e => e.AverageGrade).Select(e => e.AverageGrade).ToList();
      
      if (grades[threshold - 1] <= averageGrade)
        return 'A';
      else if (grades[(threshold * 2) - 1] <= averageGrade)
        return 'B';
      else if (grades[(threshold * 3) - 1] <= averageGrade)
        return 'C';
      else if (grades[(threshold * 4) - 1] <= averageGrade)
        return 'D';
      else
        return 'F';
    }
  }
}

using GradeBook.Enums;

namespace GradeBook.GradeBooks
{
  public class StandardGradeBook : BaseGradeBook
  {

    public StandardGradeBook(string name, bool isWeighted) : base(name, isWeighted)
    {
      Type = GradeBookType.Standard;
    }
  }
}

/*
	CSharp-GradeBookApplication
	
	Description: Project to add features to an existing C Sharp Grade Book Application.
	Website: http:\\www.pluralsight.com
	         https://github.com/pmash2/CSharp-GradeBookApplication
*/


using System;
using GradeBook.UserInterfaces;

namespace GradeBook
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("#=======================#");
            Console.WriteLine("# Welcome to GradeBook! #");
            Console.WriteLine("#=======================#");
            Console.WriteLine();

            StartingUserInterface.CommandLoop();
            
            Console.WriteLine("Thank you for using GradeBook!");
            Console.WriteLine("Have a nice day!");
            Console.Read();
        }
    }
}

namespace GradeBook.Enums
{
    public enum EnrollmentType
    {
        Campus,
        State,
        National,
        International
    }
}

namespace GradeBook.Enums
{
  public enum GradeBookType
  {
    Standard,
    Ranked,
    ESNU,
    OneToFour,
    SixPoint
  }
}

namespace GradeBook.Enums
{
    public enum StudentType
    {
        Standard,
        Honors,
        DualEnrolled
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

using GradeBook.Enums;

namespace GradeBook
{
    public class Student
    {
        public string Name { get; set; }
        public StudentType Type { get; set; }
        public EnrollmentType Enrollment { get; set; }
        public List<double> Grades { get; set; }
        [JsonIgnore]
        public double AverageGrade
        {
            get
            {
                return Grades.Average();
            }
        }
        [JsonIgnore]
        public char LetterGrade { get; set; }
        [JsonIgnore]
        public double GPA { get; set; }

        public Student(string name, StudentType studentType, EnrollmentType enrollment)
        {
            Name = name;
            Type = studentType;
            Enrollment = enrollment;
            Grades = new List<double>();
        }

        public void AddGrade(double grade)
        {
            if (grade < 0 || grade > 100)
                throw new ArgumentException("Grades must be between 0 and 100.");
            Grades.Add(grade);
        }

        public void RemoveGrade(double grade)
        {
            Grades.Remove(grade);
        }
    }
}

using GradeBook.GradeBooks;
using System;

namespace GradeBook.UserInterfaces
{
    public static class StartingUserInterface
    {
        public static bool Quit = false;
        public static void CommandLoop()
        {
            while (!Quit)
            {
                Console.WriteLine("What would you like to do?");
                var command = Console.ReadLine().ToLower();
                CommandRoute(command);
            }
        }

        public static void CommandRoute(string command)
        {
            if (command.StartsWith("create"))
                CreateCommand(command);
            else if (command.StartsWith("load"))
                LoadCommand(command);
            else if (command == "help")
                HelpCommand();
            else if (command == "quit")
                Quit = true;
            else
                Console.WriteLine("{0} was not recognized, please try again.", command);
        }

        public static void CreateCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Command not valid, Create requires a name, type of gradebook, if it's weighted (true / false).");
                return;
            }
            var name = parts[1];
            var type = parts[2].ToLower();
            var weighted = bool.Parse(parts[3]);
            BaseGradeBook gradeBook;

            if (type == "standard")
                gradeBook = new StandardGradeBook(name, weighted);
            else if (type == "ranked")
                gradeBook = new RankedGradeBook(name, weighted);
            else
            {
                System.Console.WriteLine("{0} is not a supported type of gradebook, please try again", type);
                return;
            }

            Console.WriteLine("Created gradebook {0}.", name);
            GradeBookUserInterface.CommandLoop(gradeBook);
        }

        public static void LoadCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Load requires a name.");
                return;
            }
            var name = parts[1];
            var gradeBook = BaseGradeBook.Load(name);

            if (gradeBook == null)
                return;

            GradeBookUserInterface.CommandLoop(gradeBook);
        }

        public static void HelpCommand()
        {
            Console.WriteLine("GradeBook accepts the following commands:");
            Console.WriteLine();
            Console.WriteLine("Create 'Name' 'Type' 'Weighted' - Creates a new gradebook where 'Name' is the name of the gradebook, 'Type' is what type of grading it should use, and 'Weighted' is whether or not grades should be weighted (true or false).");
            Console.WriteLine();
            Console.WriteLine("Load 'Name' - Loads the gradebook with the provided 'Name'.");
            Console.WriteLine();
            Console.WriteLine("Help - Displays all accepted commands.");
            Console.WriteLine();
            Console.WriteLine("Quit - Exits the application");
        }
    }
}


using GradeBook.Enums;
using GradeBook.GradeBooks;
using System;

namespace GradeBook.UserInterfaces
{
    public static class GradeBookUserInterface
    {
        public static BaseGradeBook GradeBook;
        public static bool Quit = false;
        public static void CommandLoop(BaseGradeBook gradeBook)
        {
            GradeBook = gradeBook;
            Quit = false;

            Console.WriteLine("#=======================#");
            Console.WriteLine(GradeBook.Name + " : " + GradeBook.GetType().Name);
            Console.WriteLine("#=======================#");
            Console.WriteLine(string.Empty);

            while(!Quit)
            {
                Console.WriteLine("What would you like to do?");
                var command = Console.ReadLine().ToLower();
                CommandRoute(command);
            }

            Console.WriteLine(GradeBook.Name + " has been closed.");
        }

        public static void CommandRoute(string command)
        {
            if (command == "save")
                SaveCommand();
            else if (command.StartsWith("addgrade"))
                AddGradeCommand(command);
            else if (command.StartsWith("removegrade"))
                RemoveGradeCommand(command);
            else if (command.StartsWith("add"))
                AddStudentCommand(command);
            else if (command.StartsWith("remove"))
                RemoveStudentCommand(command);
            else if (command == "list")
                ListCommand();
            else if (command == "statistics all")
                StatisticsCommand();
            else if (command.StartsWith("statistics"))
                StudentStatisticsCommand(command);
            else if (command == "help")
                HelpCommand();
            else if (command == "close")
                Quit = true;
            else
                Console.WriteLine("{0} was not recognized, please try again.", command);
        }

        public static void SaveCommand()
        {
            GradeBook.Save();
            Console.WriteLine("{0} has been saved.", GradeBook.Name);
        }
        
        public static void AddGradeCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Command not valid, AddGrade requires a name and score.");
                return;
            }
            var name = parts[1];
            var score = Double.Parse(parts[2]);
            GradeBook.AddGrade(name, score);
            Console.WriteLine("Added a score of {0} to {1}'s grades", score, name);
        }

        public static void RemoveGradeCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Command not valid, RemoveGrade requires a name and score.");
                return;
            }
            var name = parts[1];
            var score = Double.Parse(parts[2]);
            GradeBook.RemoveGrade(name, score);
            Console.WriteLine("Removed a score of {0} from {1}'s grades", score, name);
        }

        public static void AddStudentCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Command not valid, Add requires a name, student type, enrollment type.");
                return;
            }
            var name = parts[1];

            StudentType studentType;
            if (!Enum.TryParse(parts[2], true, out studentType))
            {
                Console.WriteLine("{0} is not a valid student type, try again.", parts[2]);
                return;
            }

            EnrollmentType enrollmentType;
            if (!Enum.TryParse(parts[3], true, out enrollmentType))
            {
                Console.WriteLine("{0} is not a volid enrollment type, try again.", parts[3]);
                return;
            }

            var student = new Student(name, studentType, enrollmentType);
            GradeBook.AddStudent(student);
            Console.WriteLine("Added {0} to the gradebook.", name);
        }
        
        public static void RemoveStudentCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Remove requires a name.");
                return;
            }
            var name = parts[1];
            GradeBook.RemoveStudent(name);
            Console.WriteLine("Removed {0} from the gradebook.", name);
        }

        public static void ListCommand()
        {
            GradeBook.ListStudents();
        }
        
        public static void StatisticsCommand()
        {
            GradeBook.CalculateStatistics();
        }

        public static void StudentStatisticsCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Requires Name or All.");
                return;
            }
            var name = parts[1];
            GradeBook.CalculateStudentStatistics(name);
        }

        public static void HelpCommand()
        {
            Console.WriteLine("While a gradebook is open you can use the following commands:");
            Console.WriteLine();
            Console.WriteLine("Add 'Name' 'Student Type' 'Enrollment Type' - Adds a new student to the gradebook with the provided name, type of student, and type of enrollment.");
            Console.WriteLine();
            Console.WriteLine("Accepted Student Types:");
            Console.WriteLine("Standard - Student not enrolled in Honors classes or Dual Enrolled.");
            Console.WriteLine("Honors - Students enrolled in Honors classes and not Dual Enrolled.");
            Console.WriteLine("DualEnrolled - Students who are Duel Enrolled.");
            Console.WriteLine();
            Console.WriteLine("Accepted Enrollement Types:");
            Console.WriteLine("Campus - Students who are in the same disctrict as the school.");
            Console.WriteLine("State - Students who's legal residence is outside the school's district, but is in the same state as the school.");
            Console.WriteLine("National - Students who's legal residence is not in the same state as the school, but is in the same country as the school.");
            Console.WriteLine("International - Students who's legal residence is not in the same country as the school.");
            Console.WriteLine();
            Console.WriteLine("List - Lists all students.");
            Console.WriteLine();
            Console.WriteLine("AddGrade 'Name' 'Score' - Adds a new grade to a student with the matching name of the provided score.");
            Console.WriteLine();
            Console.WriteLine("RemoveGrade 'Name' 'Score' - Removes a grade to a student with the matching name and score.");
            Console.WriteLine();
            Console.WriteLine("Remove 'Name' - Removes the student with the provided name.");
            Console.WriteLine();
            Console.WriteLine("Statistics 'Name' - Gets statistics for the specified student.");
            Console.WriteLine();
            Console.WriteLine("Statistics All - Gets general statistics for the entire gradebook.");
            Console.WriteLine();
            Console.WriteLine("Close - closes the gradebook and takes you back to the starting command options.");
            Console.WriteLine();
            Console.WriteLine("Save - saves the gradebook to the hard drive for later use.");
        }
    }
}

using System;
using System.Linq;

using GradeBook.Enums;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GradeBook.GradeBooks
{
    public abstract class BaseGradeBook
    {
        public string Name { get; set; }
        public List<Student> Students { get; set; }
        public GradeBookType Type { get; set; }
        public bool IsWeighted { get; set; }

        public BaseGradeBook(string name, bool isWeighted)
        {
            Name = name;
            Students = new List<Student>();
            IsWeighted = isWeighted;
        }

        public void AddStudent(Student student)
        {
            if (string.IsNullOrEmpty(student.Name))
                throw new ArgumentException("A Name is required to add a student to a gradebook.");
            Students.Add(student);
        }

        public void RemoveStudent(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to remove a student from a gradebook.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            Students.Remove(student);
        }

        public void AddGrade(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to add a grade to a student.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            student.AddGrade(score);
        }

        public void RemoveGrade(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to remove a grade from a student.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            student.RemoveGrade(score);
        }

        public void ListStudents()
        {
            foreach (var student in Students)
            {
                Console.WriteLine("{0} : {1} : {2}", student.Name, student.Type, student.Enrollment);
            }
        }

        public static BaseGradeBook Load(string name)
        {
            if (!File.Exists(name + ".gdbk"))
            {
                Console.WriteLine("Gradebook could not be found.");
                return null;
            }

            using (var file = new FileStream(name + ".gdbk", FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(file))
                {
                    var json = reader.ReadToEnd();
                    return ConvertToGradeBook(json);
                }
            }
        }

        public void Save()
        {
            using (var file = new FileStream(Name + ".gdbk", FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(file))
                {
                    var json = JsonConvert.SerializeObject(this);
                    writer.Write(json);
                }
            }
        }

        public virtual double GetGPA(char letterGrade, StudentType studentType)
        {
            var gpa = 0;
            
            switch (letterGrade)
            {
                case 'A':
                    gpa = 4;
                    break;
                case 'B':
                    gpa = 3;
                    break;
                case 'C':
                    gpa = 2;
                    break;
                case 'D':
                    gpa = 1;
                    break;
            }
            
            if (IsWeighted && (studentType == StudentType.Honors || studentType == StudentType.DualEnrolled))
                gpa++;
            
            return gpa;
        }

        public virtual void CalculateStatistics()
        {
            var allStudentsPoints = 0d;
            var campusPoints = 0d;
            var statePoints = 0d;
            var nationalPoints = 0d;
            var internationalPoints = 0d;
            var standardPoints = 0d;
            var honorPoints = 0d;
            var dualEnrolledPoints = 0d;

            foreach (var student in Students)
            {
                student.LetterGrade = GetLetterGrade(student.AverageGrade);
                student.GPA = GetGPA(student.LetterGrade, student.Type);

                Console.WriteLine("{0} ({1}:{2}) GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
                allStudentsPoints += student.AverageGrade;

                switch (student.Enrollment)
                {
                    case EnrollmentType.Campus:
                        campusPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.State:
                        statePoints += student.AverageGrade;
                        break;
                    case EnrollmentType.National:
                        nationalPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.International:
                        internationalPoints += student.AverageGrade;
                        break;
                }

                switch (student.Type)
                {
                    case StudentType.Standard:
                        standardPoints += student.AverageGrade;
                        break;
                    case StudentType.Honors:
                        honorPoints += student.AverageGrade;
                        break;
                    case StudentType.DualEnrolled:
                        dualEnrolledPoints += student.AverageGrade;
                        break;
                }
            }

            //#todo refactor into it's own method with calculations performed here
            Console.WriteLine("Average Grade of all students is " + (allStudentsPoints / Students.Count));
            if (campusPoints != 0)
                Console.WriteLine("Average for only local students is " + (campusPoints / Students.Where(e => e.Enrollment == EnrollmentType.Campus).Count()));
            if (statePoints != 0)
                Console.WriteLine("Average for only state students (excluding local) is " + (statePoints / Students.Where(e => e.Enrollment == EnrollmentType.State).Count()));
            if (nationalPoints != 0)
                Console.WriteLine("Average for only national students (excluding state and local) is " + (nationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.National).Count()));
            if (internationalPoints != 0)
                Console.WriteLine("Average for only international students is " + (internationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.International).Count()));
            if (standardPoints != 0)
                Console.WriteLine("Average for students excluding honors and duel enrollment is " + (standardPoints / Students.Where(e => e.Type == StudentType.Standard).Count()));
            if (honorPoints != 0)
                Console.WriteLine("Average for only honors students is " + (honorPoints / Students.Where(e => e.Type == StudentType.Honors).Count()));
            if (dualEnrolledPoints != 0)
                Console.WriteLine("Average for only duel enrolled students is " + (dualEnrolledPoints / Students.Where(e => e.Type == StudentType.DualEnrolled).Count()));
        }

        public virtual void CalculateStudentStatistics(string name)
        {
            var student = Students.FirstOrDefault(e => e.Name == name);
            student.LetterGrade = GetLetterGrade(student.AverageGrade);
            student.GPA = GetGPA(student.LetterGrade, student.Type);

            Console.WriteLine("{0} ({1}:{2}) GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
            Console.WriteLine();
            Console.WriteLine("Grades:");
            foreach (var grade in student.Grades)
            {
                Console.WriteLine(grade);
            }
        }

        public virtual char GetLetterGrade(double averageGrade)
        {
            if (averageGrade >= 90)
                return 'A';
            else if (averageGrade >= 80)
                return 'B';
            else if (averageGrade >= 70)
                return 'C';
            else if (averageGrade >= 60)
                return 'D';
            else
                return 'F';
        }

        /// <summary>
        ///     Converts json to the appropriate grade book type.
        ///     Note: This method contains code that is not recommended practice.
        ///     This has been used as a compromise to avoid adding additional complexity to the learner.
        /// </summary>
        /// <returns>The to grade book.</returns>
        /// <param name="json">Json.</param>
        public static dynamic ConvertToGradeBook(string json)
        {
            // Get GradeBookType from the GradeBook.Enums namespace
            var gradebookEnum = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 from type in assembly.GetTypes()
                                 where type.FullName == "GradeBook.Enums.GradeBookType"
                                 select type).FirstOrDefault();

            var jobject = JsonConvert.DeserializeObject<JObject>(json);
            var gradeBookType = jobject.Property("Type")?.Value?.ToString();

            // Check if StandardGradeBook exists
            if ((from assembly in AppDomain.CurrentDomain.GetAssemblies()
                 from type in assembly.GetTypes()
                 where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                 select type).FirstOrDefault() == null)
                gradeBookType = "Base";
            else
            {
                if (string.IsNullOrEmpty(gradeBookType))
                    gradeBookType = "Standard";
                else
                    gradeBookType = Enum.GetName(gradebookEnum, int.Parse(gradeBookType));
            }

            // Get GradeBook from the GradeBook.GradeBooks namespace
            var gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks." + gradeBookType + "GradeBook"
                             select type).FirstOrDefault();


            //protection code
            if (gradebook == null)
                gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                             select type).FirstOrDefault();
            
            return JsonConvert.DeserializeObject(json, gradebook);
        }
    }
}

using GradeBook.Enums;
using System;
using System.Linq;

namespace GradeBook.GradeBooks
{
  public class RankedGradeBook : BaseGradeBook
  {
    public RankedGradeBook(string name, bool isWeighted) : base(name, isWeighted)
    {
      Type = GradeBookType.Ranked;
    }

    public override void CalculateStudentStatistics(string name)
    {
      if (Students.Count < 5)
      {
        System.Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
        return;
      }
      base.CalculateStudentStatistics(name);
    }

    public override void CalculateStatistics()
    {
      if (Students.Count < 5)
      {
        System.Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
        return;
      }
      base.CalculateStatistics();
    }

    public override char GetLetterGrade(double averageGrade)
    {
      if (Students.Count < 5)
      {
        throw new InvalidOperationException("Ranked grading requires at least 5 students");
      }

      // Number of students that should receive each letter grade
      var threshold = (int)Math.Ceiling(Students.Count * 0.2);

      //                Order students by       their avg grade   and take their avg grade val  to a list
      var grades = Students.OrderByDescending(e => e.AverageGrade).Select(e => e.AverageGrade).ToList();
      
      if (grades[threshold - 1] <= averageGrade)
        return 'A';
      else if (grades[(threshold * 2) - 1] <= averageGrade)
        return 'B';
      else if (grades[(threshold * 3) - 1] <= averageGrade)
        return 'C';
      else if (grades[(threshold * 4) - 1] <= averageGrade)
        return 'D';
      else
        return 'F';
    }
  }
}

using GradeBook.Enums;

namespace GradeBook.GradeBooks
{
  public class StandardGradeBook : BaseGradeBook
  {

    public StandardGradeBook(string name, bool isWeighted) : base(name, isWeighted)
    {
      Type = GradeBookType.Standard;
    }
  }
}

/*
	CSharp-GradeBookApplication
	
	Description: Project to add features to an existing C Sharp Grade Book Application.
	Website: http:\\www.pluralsight.com
	         https://github.com/pmash2/CSharp-GradeBookApplication
*/


using System;
using GradeBook.UserInterfaces;

namespace GradeBook
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("#=======================#");
            Console.WriteLine("# Welcome to GradeBook! #");
            Console.WriteLine("#=======================#");
            Console.WriteLine();

            StartingUserInterface.CommandLoop();
            
            Console.WriteLine("Thank you for using GradeBook!");
            Console.WriteLine("Have a nice day!");
            Console.Read();
        }
    }
}

namespace GradeBook.Enums
{
    public enum EnrollmentType
    {
        Campus,
        State,
        National,
        International
    }
}

namespace GradeBook.Enums
{
  public enum GradeBookType
  {
    Standard,
    Ranked,
    ESNU,
    OneToFour,
    SixPoint
  }
}

namespace GradeBook.Enums
{
    public enum StudentType
    {
        Standard,
        Honors,
        DualEnrolled
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

using GradeBook.Enums;

namespace GradeBook
{
    public class Student
    {
        public string Name { get; set; }
        public StudentType Type { get; set; }
        public EnrollmentType Enrollment { get; set; }
        public List<double> Grades { get; set; }
        [JsonIgnore]
        public double AverageGrade
        {
            get
            {
                return Grades.Average();
            }
        }
        [JsonIgnore]
        public char LetterGrade { get; set; }
        [JsonIgnore]
        public double GPA { get; set; }

        public Student(string name, StudentType studentType, EnrollmentType enrollment)
        {
            Name = name;
            Type = studentType;
            Enrollment = enrollment;
            Grades = new List<double>();
        }

        public void AddGrade(double grade)
        {
            if (grade < 0 || grade > 100)
                throw new ArgumentException("Grades must be between 0 and 100.");
            Grades.Add(grade);
        }

        public void RemoveGrade(double grade)
        {
            Grades.Remove(grade);
        }
    }
}

using GradeBook.GradeBooks;
using System;

namespace GradeBook.UserInterfaces
{
    public static class StartingUserInterface
    {
        public static bool Quit = false;
        public static void CommandLoop()
        {
            while (!Quit)
            {
                Console.WriteLine("What would you like to do?");
                var command = Console.ReadLine().ToLower();
                CommandRoute(command);
            }
        }

        public static void CommandRoute(string command)
        {
            if (command.StartsWith("create"))
                CreateCommand(command);
            else if (command.StartsWith("load"))
                LoadCommand(command);
            else if (command == "help")
                HelpCommand();
            else if (command == "quit")
                Quit = true;
            else
                Console.WriteLine("{0} was not recognized, please try again.", command);
        }

        public static void CreateCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Command not valid, Create requires a name, type of gradebook, if it's weighted (true / false).");
                return;
            }
            var name = parts[1];
            var type = parts[2].ToLower();
            var weighted = bool.Parse(parts[3]);
            BaseGradeBook gradeBook;

            if (type == "standard")
                gradeBook = new StandardGradeBook(name, weighted);
            else if (type == "ranked")
                gradeBook = new RankedGradeBook(name, weighted);
            else
            {
                System.Console.WriteLine("{0} is not a supported type of gradebook, please try again", type);
                return;
            }

            Console.WriteLine("Created gradebook {0}.", name);
            GradeBookUserInterface.CommandLoop(gradeBook);
        }

        public static void LoadCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Load requires a name.");
                return;
            }
            var name = parts[1];
            var gradeBook = BaseGradeBook.Load(name);

            if (gradeBook == null)
                return;

            GradeBookUserInterface.CommandLoop(gradeBook);
        }

        public static void HelpCommand()
        {
            Console.WriteLine("GradeBook accepts the following commands:");
            Console.WriteLine();
            Console.WriteLine("Create 'Name' 'Type' 'Weighted' - Creates a new gradebook where 'Name' is the name of the gradebook, 'Type' is what type of grading it should use, and 'Weighted' is whether or not grades should be weighted (true or false).");
            Console.WriteLine();
            Console.WriteLine("Load 'Name' - Loads the gradebook with the provided 'Name'.");
            Console.WriteLine();
            Console.WriteLine("Help - Displays all accepted commands.");
            Console.WriteLine();
            Console.WriteLine("Quit - Exits the application");
        }
    }
}


using GradeBook.Enums;
using GradeBook.GradeBooks;
using System;

namespace GradeBook.UserInterfaces
{
    public static class GradeBookUserInterface
    {
        public static BaseGradeBook GradeBook;
        public static bool Quit = false;
        public static void CommandLoop(BaseGradeBook gradeBook)
        {
            GradeBook = gradeBook;
            Quit = false;

            Console.WriteLine("#=======================#");
            Console.WriteLine(GradeBook.Name + " : " + GradeBook.GetType().Name);
            Console.WriteLine("#=======================#");
            Console.WriteLine(string.Empty);

            while(!Quit)
            {
                Console.WriteLine("What would you like to do?");
                var command = Console.ReadLine().ToLower();
                CommandRoute(command);
            }

            Console.WriteLine(GradeBook.Name + " has been closed.");
        }

        public static void CommandRoute(string command)
        {
            if (command == "save")
                SaveCommand();
            else if (command.StartsWith("addgrade"))
                AddGradeCommand(command);
            else if (command.StartsWith("removegrade"))
                RemoveGradeCommand(command);
            else if (command.StartsWith("add"))
                AddStudentCommand(command);
            else if (command.StartsWith("remove"))
                RemoveStudentCommand(command);
            else if (command == "list")
                ListCommand();
            else if (command == "statistics all")
                StatisticsCommand();
            else if (command.StartsWith("statistics"))
                StudentStatisticsCommand(command);
            else if (command == "help")
                HelpCommand();
            else if (command == "close")
                Quit = true;
            else
                Console.WriteLine("{0} was not recognized, please try again.", command);
        }

        public static void SaveCommand()
        {
            GradeBook.Save();
            Console.WriteLine("{0} has been saved.", GradeBook.Name);
        }
        
        public static void AddGradeCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Command not valid, AddGrade requires a name and score.");
                return;
            }
            var name = parts[1];
            var score = Double.Parse(parts[2]);
            GradeBook.AddGrade(name, score);
            Console.WriteLine("Added a score of {0} to {1}'s grades", score, name);
        }

        public static void RemoveGradeCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Command not valid, RemoveGrade requires a name and score.");
                return;
            }
            var name = parts[1];
            var score = Double.Parse(parts[2]);
            GradeBook.RemoveGrade(name, score);
            Console.WriteLine("Removed a score of {0} from {1}'s grades", score, name);
        }

        public static void AddStudentCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Command not valid, Add requires a name, student type, enrollment type.");
                return;
            }
            var name = parts[1];

            StudentType studentType;
            if (!Enum.TryParse(parts[2], true, out studentType))
            {
                Console.WriteLine("{0} is not a valid student type, try again.", parts[2]);
                return;
            }

            EnrollmentType enrollmentType;
            if (!Enum.TryParse(parts[3], true, out enrollmentType))
            {
                Console.WriteLine("{0} is not a volid enrollment type, try again.", parts[3]);
                return;
            }

            var student = new Student(name, studentType, enrollmentType);
            GradeBook.AddStudent(student);
            Console.WriteLine("Added {0} to the gradebook.", name);
        }
        
        public static void RemoveStudentCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Remove requires a name.");
                return;
            }
            var name = parts[1];
            GradeBook.RemoveStudent(name);
            Console.WriteLine("Removed {0} from the gradebook.", name);
        }

        public static void ListCommand()
        {
            GradeBook.ListStudents();
        }
        
        public static void StatisticsCommand()
        {
            GradeBook.CalculateStatistics();
        }

        public static void StudentStatisticsCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Requires Name or All.");
                return;
            }
            var name = parts[1];
            GradeBook.CalculateStudentStatistics(name);
        }

        public static void HelpCommand()
        {
            Console.WriteLine("While a gradebook is open you can use the following commands:");
            Console.WriteLine();
            Console.WriteLine("Add 'Name' 'Student Type' 'Enrollment Type' - Adds a new student to the gradebook with the provided name, type of student, and type of enrollment.");
            Console.WriteLine();
            Console.WriteLine("Accepted Student Types:");
            Console.WriteLine("Standard - Student not enrolled in Honors classes or Dual Enrolled.");
            Console.WriteLine("Honors - Students enrolled in Honors classes and not Dual Enrolled.");
            Console.WriteLine("DualEnrolled - Students who are Duel Enrolled.");
            Console.WriteLine();
            Console.WriteLine("Accepted Enrollement Types:");
            Console.WriteLine("Campus - Students who are in the same disctrict as the school.");
            Console.WriteLine("State - Students who's legal residence is outside the school's district, but is in the same state as the school.");
            Console.WriteLine("National - Students who's legal residence is not in the same state as the school, but is in the same country as the school.");
            Console.WriteLine("International - Students who's legal residence is not in the same country as the school.");
            Console.WriteLine();
            Console.WriteLine("List - Lists all students.");
            Console.WriteLine();
            Console.WriteLine("AddGrade 'Name' 'Score' - Adds a new grade to a student with the matching name of the provided score.");
            Console.WriteLine();
            Console.WriteLine("RemoveGrade 'Name' 'Score' - Removes a grade to a student with the matching name and score.");
            Console.WriteLine();
            Console.WriteLine("Remove 'Name' - Removes the student with the provided name.");
            Console.WriteLine();
            Console.WriteLine("Statistics 'Name' - Gets statistics for the specified student.");
            Console.WriteLine();
            Console.WriteLine("Statistics All - Gets general statistics for the entire gradebook.");
            Console.WriteLine();
            Console.WriteLine("Close - closes the gradebook and takes you back to the starting command options.");
            Console.WriteLine();
            Console.WriteLine("Save - saves the gradebook to the hard drive for later use.");
        }
    }
}

using System;
using System.Linq;

using GradeBook.Enums;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GradeBook.GradeBooks
{
    public abstract class BaseGradeBook
    {
        public string Name { get; set; }
        public List<Student> Students { get; set; }
        public GradeBookType Type { get; set; }
        public bool IsWeighted { get; set; }

        public BaseGradeBook(string name, bool isWeighted)
        {
            Name = name;
            Students = new List<Student>();
            IsWeighted = isWeighted;
        }

        public void AddStudent(Student student)
        {
            if (string.IsNullOrEmpty(student.Name))
                throw new ArgumentException("A Name is required to add a student to a gradebook.");
            Students.Add(student);
        }

        public void RemoveStudent(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to remove a student from a gradebook.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            Students.Remove(student);
        }

        public void AddGrade(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to add a grade to a student.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            student.AddGrade(score);
        }

        public void RemoveGrade(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to remove a grade from a student.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            student.RemoveGrade(score);
        }

        public void ListStudents()
        {
            foreach (var student in Students)
            {
                Console.WriteLine("{0} : {1} : {2}", student.Name, student.Type, student.Enrollment);
            }
        }

        public static BaseGradeBook Load(string name)
        {
            if (!File.Exists(name + ".gdbk"))
            {
                Console.WriteLine("Gradebook could not be found.");
                return null;
            }

            using (var file = new FileStream(name + ".gdbk", FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(file))
                {
                    var json = reader.ReadToEnd();
                    return ConvertToGradeBook(json);
                }
            }
        }

        public void Save()
        {
            using (var file = new FileStream(Name + ".gdbk", FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(file))
                {
                    var json = JsonConvert.SerializeObject(this);
                    writer.Write(json);
                }
            }
        }

        public virtual double GetGPA(char letterGrade, StudentType studentType)
        {
            var gpa = 0;
            
            switch (letterGrade)
            {
                case 'A':
                    gpa = 4;
                    break;
                case 'B':
                    gpa = 3;
                    break;
                case 'C':
                    gpa = 2;
                    break;
                case 'D':
                    gpa = 1;
                    break;
            }
            
            if (IsWeighted && (studentType == StudentType.Honors || studentType == StudentType.DualEnrolled))
                gpa++;
            
            return gpa;
        }

        public virtual void CalculateStatistics()
        {
            var allStudentsPoints = 0d;
            var campusPoints = 0d;
            var statePoints = 0d;
            var nationalPoints = 0d;
            var internationalPoints = 0d;
            var standardPoints = 0d;
            var honorPoints = 0d;
            var dualEnrolledPoints = 0d;

            foreach (var student in Students)
            {
                student.LetterGrade = GetLetterGrade(student.AverageGrade);
                student.GPA = GetGPA(student.LetterGrade, student.Type);

                Console.WriteLine("{0} ({1}:{2}) GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
                allStudentsPoints += student.AverageGrade;

                switch (student.Enrollment)
                {
                    case EnrollmentType.Campus:
                        campusPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.State:
                        statePoints += student.AverageGrade;
                        break;
                    case EnrollmentType.National:
                        nationalPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.International:
                        internationalPoints += student.AverageGrade;
                        break;
                }

                switch (student.Type)
                {
                    case StudentType.Standard:
                        standardPoints += student.AverageGrade;
                        break;
                    case StudentType.Honors:
                        honorPoints += student.AverageGrade;
                        break;
                    case StudentType.DualEnrolled:
                        dualEnrolledPoints += student.AverageGrade;
                        break;
                }
            }

            //#todo refactor into it's own method with calculations performed here
            Console.WriteLine("Average Grade of all students is " + (allStudentsPoints / Students.Count));
            if (campusPoints != 0)
                Console.WriteLine("Average for only local students is " + (campusPoints / Students.Where(e => e.Enrollment == EnrollmentType.Campus).Count()));
            if (statePoints != 0)
                Console.WriteLine("Average for only state students (excluding local) is " + (statePoints / Students.Where(e => e.Enrollment == EnrollmentType.State).Count()));
            if (nationalPoints != 0)
                Console.WriteLine("Average for only national students (excluding state and local) is " + (nationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.National).Count()));
            if (internationalPoints != 0)
                Console.WriteLine("Average for only international students is " + (internationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.International).Count()));
            if (standardPoints != 0)
                Console.WriteLine("Average for students excluding honors and duel enrollment is " + (standardPoints / Students.Where(e => e.Type == StudentType.Standard).Count()));
            if (honorPoints != 0)
                Console.WriteLine("Average for only honors students is " + (honorPoints / Students.Where(e => e.Type == StudentType.Honors).Count()));
            if (dualEnrolledPoints != 0)
                Console.WriteLine("Average for only duel enrolled students is " + (dualEnrolledPoints / Students.Where(e => e.Type == StudentType.DualEnrolled).Count()));
        }

        public virtual void CalculateStudentStatistics(string name)
        {
            var student = Students.FirstOrDefault(e => e.Name == name);
            student.LetterGrade = GetLetterGrade(student.AverageGrade);
            student.GPA = GetGPA(student.LetterGrade, student.Type);

            Console.WriteLine("{0} ({1}:{2}) GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
            Console.WriteLine();
            Console.WriteLine("Grades:");
            foreach (var grade in student.Grades)
            {
                Console.WriteLine(grade);
            }
        }

        public virtual char GetLetterGrade(double averageGrade)
        {
            if (averageGrade >= 90)
                return 'A';
            else if (averageGrade >= 80)
                return 'B';
            else if (averageGrade >= 70)
                return 'C';
            else if (averageGrade >= 60)
                return 'D';
            else
                return 'F';
        }

        /// <summary>
        ///     Converts json to the appropriate grade book type.
        ///     Note: This method contains code that is not recommended practice.
        ///     This has been used as a compromise to avoid adding additional complexity to the learner.
        /// </summary>
        /// <returns>The to grade book.</returns>
        /// <param name="json">Json.</param>
        public static dynamic ConvertToGradeBook(string json)
        {
            // Get GradeBookType from the GradeBook.Enums namespace
            var gradebookEnum = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 from type in assembly.GetTypes()
                                 where type.FullName == "GradeBook.Enums.GradeBookType"
                                 select type).FirstOrDefault();

            var jobject = JsonConvert.DeserializeObject<JObject>(json);
            var gradeBookType = jobject.Property("Type")?.Value?.ToString();

            // Check if StandardGradeBook exists
            if ((from assembly in AppDomain.CurrentDomain.GetAssemblies()
                 from type in assembly.GetTypes()
                 where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                 select type).FirstOrDefault() == null)
                gradeBookType = "Base";
            else
            {
                if (string.IsNullOrEmpty(gradeBookType))
                    gradeBookType = "Standard";
                else
                    gradeBookType = Enum.GetName(gradebookEnum, int.Parse(gradeBookType));
            }

            // Get GradeBook from the GradeBook.GradeBooks namespace
            var gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks." + gradeBookType + "GradeBook"
                             select type).FirstOrDefault();


            //protection code
            if (gradebook == null)
                gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                             select type).FirstOrDefault();
            
            return JsonConvert.DeserializeObject(json, gradebook);
        }
    }
}

using GradeBook.Enums;
using System;
using System.Linq;

namespace GradeBook.GradeBooks
{
  public class RankedGradeBook : BaseGradeBook
  {
    public RankedGradeBook(string name, bool isWeighted) : base(name, isWeighted)
    {
      Type = GradeBookType.Ranked;
    }

    public override void CalculateStudentStatistics(string name)
    {
      if (Students.Count < 5)
      {
        System.Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
        return;
      }
      base.CalculateStudentStatistics(name);
    }

    public override void CalculateStatistics()
    {
      if (Students.Count < 5)
      {
        System.Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
        return;
      }
      base.CalculateStatistics();
    }

    public override char GetLetterGrade(double averageGrade)
    {
      if (Students.Count < 5)
      {
        throw new InvalidOperationException("Ranked grading requires at least 5 students");
      }

      // Number of students that should receive each letter grade
      var threshold = (int)Math.Ceiling(Students.Count * 0.2);

      //                Order students by       their avg grade   and take their avg grade val  to a list
      var grades = Students.OrderByDescending(e => e.AverageGrade).Select(e => e.AverageGrade).ToList();
      
      if (grades[threshold - 1] <= averageGrade)
        return 'A';
      else if (grades[(threshold * 2) - 1] <= averageGrade)
        return 'B';
      else if (grades[(threshold * 3) - 1] <= averageGrade)
        return 'C';
      else if (grades[(threshold * 4) - 1] <= averageGrade)
        return 'D';
      else
        return 'F';
    }
  }
}

using GradeBook.Enums;

namespace GradeBook.GradeBooks
{
  public class StandardGradeBook : BaseGradeBook
  {

    public StandardGradeBook(string name, bool isWeighted) : base(name, isWeighted)
    {
      Type = GradeBookType.Standard;
    }
  }
}

/*
	CSharp-GradeBookApplication
	
	Description: Project to add features to an existing C Sharp Grade Book Application.
	Website: http:\\www.pluralsight.com
	         https://github.com/pmash2/CSharp-GradeBookApplication
*/


using System;
using GradeBook.UserInterfaces;

namespace GradeBook
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("#=======================#");
            Console.WriteLine("# Welcome to GradeBook! #");
            Console.WriteLine("#=======================#");
            Console.WriteLine();

            StartingUserInterface.CommandLoop();
            
            Console.WriteLine("Thank you for using GradeBook!");
            Console.WriteLine("Have a nice day!");
            Console.Read();
        }
    }
}

namespace GradeBook.Enums
{
    public enum EnrollmentType
    {
        Campus,
        State,
        National,
        International
    }
}

namespace GradeBook.Enums
{
  public enum GradeBookType
  {
    Standard,
    Ranked,
    ESNU,
    OneToFour,
    SixPoint
  }
}

namespace GradeBook.Enums
{
    public enum StudentType
    {
        Standard,
        Honors,
        DualEnrolled
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

using GradeBook.Enums;

namespace GradeBook
{
    public class Student
    {
        public string Name { get; set; }
        public StudentType Type { get; set; }
        public EnrollmentType Enrollment { get; set; }
        public List<double> Grades { get; set; }
        [JsonIgnore]
        public double AverageGrade
        {
            get
            {
                return Grades.Average();
            }
        }
        [JsonIgnore]
        public char LetterGrade { get; set; }
        [JsonIgnore]
        public double GPA { get; set; }

        public Student(string name, StudentType studentType, EnrollmentType enrollment)
        {
            Name = name;
            Type = studentType;
            Enrollment = enrollment;
            Grades = new List<double>();
        }

        public void AddGrade(double grade)
        {
            if (grade < 0 || grade > 100)
                throw new ArgumentException("Grades must be between 0 and 100.");
            Grades.Add(grade);
        }

        public void RemoveGrade(double grade)
        {
            Grades.Remove(grade);
        }
    }
}

using GradeBook.GradeBooks;
using System;

namespace GradeBook.UserInterfaces
{
    public static class StartingUserInterface
    {
        public static bool Quit = false;
        public static void CommandLoop()
        {
            while (!Quit)
            {
                Console.WriteLine("What would you like to do?");
                var command = Console.ReadLine().ToLower();
                CommandRoute(command);
            }
        }

        public static void CommandRoute(string command)
        {
            if (command.StartsWith("create"))
                CreateCommand(command);
            else if (command.StartsWith("load"))
                LoadCommand(command);
            else if (command == "help")
                HelpCommand();
            else if (command == "quit")
                Quit = true;
            else
                Console.WriteLine("{0} was not recognized, please try again.", command);
        }

        public static void CreateCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Command not valid, Create requires a name, type of gradebook, if it's weighted (true / false).");
                return;
            }
            var name = parts[1];
            var type = parts[2].ToLower();
            var weighted = bool.Parse(parts[3]);
            BaseGradeBook gradeBook;

            if (type == "standard")
                gradeBook = new StandardGradeBook(name, weighted);
            else if (type == "ranked")
                gradeBook = new RankedGradeBook(name, weighted);
            else
            {
                System.Console.WriteLine("{0} is not a supported type of gradebook, please try again", type);
                return;
            }

            Console.WriteLine("Created gradebook {0}.", name);
            GradeBookUserInterface.CommandLoop(gradeBook);
        }

        public static void LoadCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Load requires a name.");
                return;
            }
            var name = parts[1];
            var gradeBook = BaseGradeBook.Load(name);

            if (gradeBook == null)
                return;

            GradeBookUserInterface.CommandLoop(gradeBook);
        }

        public static void HelpCommand()
        {
            Console.WriteLine("GradeBook accepts the following commands:");
            Console.WriteLine();
            Console.WriteLine("Create 'Name' 'Type' 'Weighted' - Creates a new gradebook where 'Name' is the name of the gradebook, 'Type' is what type of grading it should use, and 'Weighted' is whether or not grades should be weighted (true or false).");
            Console.WriteLine();
            Console.WriteLine("Load 'Name' - Loads the gradebook with the provided 'Name'.");
            Console.WriteLine();
            Console.WriteLine("Help - Displays all accepted commands.");
            Console.WriteLine();
            Console.WriteLine("Quit - Exits the application");
        }
    }
}


using GradeBook.Enums;
using GradeBook.GradeBooks;
using System;

namespace GradeBook.UserInterfaces
{
    public static class GradeBookUserInterface
    {
        public static BaseGradeBook GradeBook;
        public static bool Quit = false;
        public static void CommandLoop(BaseGradeBook gradeBook)
        {
            GradeBook = gradeBook;
            Quit = false;

            Console.WriteLine("#=======================#");
            Console.WriteLine(GradeBook.Name + " : " + GradeBook.GetType().Name);
            Console.WriteLine("#=======================#");
            Console.WriteLine(string.Empty);

            while(!Quit)
            {
                Console.WriteLine("What would you like to do?");
                var command = Console.ReadLine().ToLower();
                CommandRoute(command);
            }

            Console.WriteLine(GradeBook.Name + " has been closed.");
        }

        public static void CommandRoute(string command)
        {
            if (command == "save")
                SaveCommand();
            else if (command.StartsWith("addgrade"))
                AddGradeCommand(command);
            else if (command.StartsWith("removegrade"))
                RemoveGradeCommand(command);
            else if (command.StartsWith("add"))
                AddStudentCommand(command);
            else if (command.StartsWith("remove"))
                RemoveStudentCommand(command);
            else if (command == "list")
                ListCommand();
            else if (command == "statistics all")
                StatisticsCommand();
            else if (command.StartsWith("statistics"))
                StudentStatisticsCommand(command);
            else if (command == "help")
                HelpCommand();
            else if (command == "close")
                Quit = true;
            else
                Console.WriteLine("{0} was not recognized, please try again.", command);
        }

        public static void SaveCommand()
        {
            GradeBook.Save();
            Console.WriteLine("{0} has been saved.", GradeBook.Name);
        }
        
        public static void AddGradeCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Command not valid, AddGrade requires a name and score.");
                return;
            }
            var name = parts[1];
            var score = Double.Parse(parts[2]);
            GradeBook.AddGrade(name, score);
            Console.WriteLine("Added a score of {0} to {1}'s grades", score, name);
        }

        public static void RemoveGradeCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Command not valid, RemoveGrade requires a name and score.");
                return;
            }
            var name = parts[1];
            var score = Double.Parse(parts[2]);
            GradeBook.RemoveGrade(name, score);
            Console.WriteLine("Removed a score of {0} from {1}'s grades", score, name);
        }

        public static void AddStudentCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Command not valid, Add requires a name, student type, enrollment type.");
                return;
            }
            var name = parts[1];

            StudentType studentType;
            if (!Enum.TryParse(parts[2], true, out studentType))
            {
                Console.WriteLine("{0} is not a valid student type, try again.", parts[2]);
                return;
            }

            EnrollmentType enrollmentType;
            if (!Enum.TryParse(parts[3], true, out enrollmentType))
            {
                Console.WriteLine("{0} is not a volid enrollment type, try again.", parts[3]);
                return;
            }

            var student = new Student(name, studentType, enrollmentType);
            GradeBook.AddStudent(student);
            Console.WriteLine("Added {0} to the gradebook.", name);
        }
        
        public static void RemoveStudentCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Remove requires a name.");
                return;
            }
            var name = parts[1];
            GradeBook.RemoveStudent(name);
            Console.WriteLine("Removed {0} from the gradebook.", name);
        }

        public static void ListCommand()
        {
            GradeBook.ListStudents();
        }
        
        public static void StatisticsCommand()
        {
            GradeBook.CalculateStatistics();
        }

        public static void StudentStatisticsCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Requires Name or All.");
                return;
            }
            var name = parts[1];
            GradeBook.CalculateStudentStatistics(name);
        }

        public static void HelpCommand()
        {
            Console.WriteLine("While a gradebook is open you can use the following commands:");
            Console.WriteLine();
            Console.WriteLine("Add 'Name' 'Student Type' 'Enrollment Type' - Adds a new student to the gradebook with the provided name, type of student, and type of enrollment.");
            Console.WriteLine();
            Console.WriteLine("Accepted Student Types:");
            Console.WriteLine("Standard - Student not enrolled in Honors classes or Dual Enrolled.");
            Console.WriteLine("Honors - Students enrolled in Honors classes and not Dual Enrolled.");
            Console.WriteLine("DualEnrolled - Students who are Duel Enrolled.");
            Console.WriteLine();
            Console.WriteLine("Accepted Enrollement Types:");
            Console.WriteLine("Campus - Students who are in the same disctrict as the school.");
            Console.WriteLine("State - Students who's legal residence is outside the school's district, but is in the same state as the school.");
            Console.WriteLine("National - Students who's legal residence is not in the same state as the school, but is in the same country as the school.");
            Console.WriteLine("International - Students who's legal residence is not in the same country as the school.");
            Console.WriteLine();
            Console.WriteLine("List - Lists all students.");
            Console.WriteLine();
            Console.WriteLine("AddGrade 'Name' 'Score' - Adds a new grade to a student with the matching name of the provided score.");
            Console.WriteLine();
            Console.WriteLine("RemoveGrade 'Name' 'Score' - Removes a grade to a student with the matching name and score.");
            Console.WriteLine();
            Console.WriteLine("Remove 'Name' - Removes the student with the provided name.");
            Console.WriteLine();
            Console.WriteLine("Statistics 'Name' - Gets statistics for the specified student.");
            Console.WriteLine();
            Console.WriteLine("Statistics All - Gets general statistics for the entire gradebook.");
            Console.WriteLine();
            Console.WriteLine("Close - closes the gradebook and takes you back to the starting command options.");
            Console.WriteLine();
            Console.WriteLine("Save - saves the gradebook to the hard drive for later use.");
        }
    }
}

using System;
using System.Linq;

using GradeBook.Enums;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GradeBook.GradeBooks
{
    public abstract class BaseGradeBook
    {
        public string Name { get; set; }
        public List<Student> Students { get; set; }
        public GradeBookType Type { get; set; }
        public bool IsWeighted { get; set; }

        public BaseGradeBook(string name, bool isWeighted)
        {
            Name = name;
            Students = new List<Student>();
            IsWeighted = isWeighted;
        }

        public void AddStudent(Student student)
        {
            if (string.IsNullOrEmpty(student.Name))
                throw new ArgumentException("A Name is required to add a student to a gradebook.");
            Students.Add(student);
        }

        public void RemoveStudent(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to remove a student from a gradebook.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            Students.Remove(student);
        }

        public void AddGrade(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to add a grade to a student.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            student.AddGrade(score);
        }

        public void RemoveGrade(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to remove a grade from a student.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            student.RemoveGrade(score);
        }

        public void ListStudents()
        {
            foreach (var student in Students)
            {
                Console.WriteLine("{0} : {1} : {2}", student.Name, student.Type, student.Enrollment);
            }
        }

        public static BaseGradeBook Load(string name)
        {
            if (!File.Exists(name + ".gdbk"))
            {
                Console.WriteLine("Gradebook could not be found.");
                return null;
            }

            using (var file = new FileStream(name + ".gdbk", FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(file))
                {
                    var json = reader.ReadToEnd();
                    return ConvertToGradeBook(json);
                }
            }
        }

        public void Save()
        {
            using (var file = new FileStream(Name + ".gdbk", FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(file))
                {
                    var json = JsonConvert.SerializeObject(this);
                    writer.Write(json);
                }
            }
        }

        public virtual double GetGPA(char letterGrade, StudentType studentType)
        {
            var gpa = 0;
            
            switch (letterGrade)
            {
                case 'A':
                    gpa = 4;
                    break;
                case 'B':
                    gpa = 3;
                    break;
                case 'C':
                    gpa = 2;
                    break;
                case 'D':
                    gpa = 1;
                    break;
            }
            
            if (IsWeighted && (studentType == StudentType.Honors || studentType == StudentType.DualEnrolled))
                gpa++;
            
            return gpa;
        }

        public virtual void CalculateStatistics()
        {
            var allStudentsPoints = 0d;
            var campusPoints = 0d;
            var statePoints = 0d;
            var nationalPoints = 0d;
            var internationalPoints = 0d;
            var standardPoints = 0d;
            var honorPoints = 0d;
            var dualEnrolledPoints = 0d;

            foreach (var student in Students)
            {
                student.LetterGrade = GetLetterGrade(student.AverageGrade);
                student.GPA = GetGPA(student.LetterGrade, student.Type);

                Console.WriteLine("{0} ({1}:{2}) GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
                allStudentsPoints += student.AverageGrade;

                switch (student.Enrollment)
                {
                    case EnrollmentType.Campus:
                        campusPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.State:
                        statePoints += student.AverageGrade;
                        break;
                    case EnrollmentType.National:
                        nationalPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.International:
                        internationalPoints += student.AverageGrade;
                        break;
                }

                switch (student.Type)
                {
                    case StudentType.Standard:
                        standardPoints += student.AverageGrade;
                        break;
                    case StudentType.Honors:
                        honorPoints += student.AverageGrade;
                        break;
                    case StudentType.DualEnrolled:
                        dualEnrolledPoints += student.AverageGrade;
                        break;
                }
            }

            //#todo refactor into it's own method with calculations performed here
            Console.WriteLine("Average Grade of all students is " + (allStudentsPoints / Students.Count));
            if (campusPoints != 0)
                Console.WriteLine("Average for only local students is " + (campusPoints / Students.Where(e => e.Enrollment == EnrollmentType.Campus).Count()));
            if (statePoints != 0)
                Console.WriteLine("Average for only state students (excluding local) is " + (statePoints / Students.Where(e => e.Enrollment == EnrollmentType.State).Count()));
            if (nationalPoints != 0)
                Console.WriteLine("Average for only national students (excluding state and local) is " + (nationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.National).Count()));
            if (internationalPoints != 0)
                Console.WriteLine("Average for only international students is " + (internationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.International).Count()));
            if (standardPoints != 0)
                Console.WriteLine("Average for students excluding honors and duel enrollment is " + (standardPoints / Students.Where(e => e.Type == StudentType.Standard).Count()));
            if (honorPoints != 0)
                Console.WriteLine("Average for only honors students is " + (honorPoints / Students.Where(e => e.Type == StudentType.Honors).Count()));
            if (dualEnrolledPoints != 0)
                Console.WriteLine("Average for only duel enrolled students is " + (dualEnrolledPoints / Students.Where(e => e.Type == StudentType.DualEnrolled).Count()));
        }

        public virtual void CalculateStudentStatistics(string name)
        {
            var student = Students.FirstOrDefault(e => e.Name == name);
            student.LetterGrade = GetLetterGrade(student.AverageGrade);
            student.GPA = GetGPA(student.LetterGrade, student.Type);

            Console.WriteLine("{0} ({1}:{2}) GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
            Console.WriteLine();
            Console.WriteLine("Grades:");
            foreach (var grade in student.Grades)
            {
                Console.WriteLine(grade);
            }
        }

        public virtual char GetLetterGrade(double averageGrade)
        {
            if (averageGrade >= 90)
                return 'A';
            else if (averageGrade >= 80)
                return 'B';
            else if (averageGrade >= 70)
                return 'C';
            else if (averageGrade >= 60)
                return 'D';
            else
                return 'F';
        }

        /// <summary>
        ///     Converts json to the appropriate grade book type.
        ///     Note: This method contains code that is not recommended practice.
        ///     This has been used as a compromise to avoid adding additional complexity to the learner.
        /// </summary>
        /// <returns>The to grade book.</returns>
        /// <param name="json">Json.</param>
        public static dynamic ConvertToGradeBook(string json)
        {
            // Get GradeBookType from the GradeBook.Enums namespace
            var gradebookEnum = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 from type in assembly.GetTypes()
                                 where type.FullName == "GradeBook.Enums.GradeBookType"
                                 select type).FirstOrDefault();

            var jobject = JsonConvert.DeserializeObject<JObject>(json);
            var gradeBookType = jobject.Property("Type")?.Value?.ToString();

            // Check if StandardGradeBook exists
            if ((from assembly in AppDomain.CurrentDomain.GetAssemblies()
                 from type in assembly.GetTypes()
                 where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                 select type).FirstOrDefault() == null)
                gradeBookType = "Base";
            else
            {
                if (string.IsNullOrEmpty(gradeBookType))
                    gradeBookType = "Standard";
                else
                    gradeBookType = Enum.GetName(gradebookEnum, int.Parse(gradeBookType));
            }

            // Get GradeBook from the GradeBook.GradeBooks namespace
            var gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks." + gradeBookType + "GradeBook"
                             select type).FirstOrDefault();


            //protection code
            if (gradebook == null)
                gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                             select type).FirstOrDefault();
            
            return JsonConvert.DeserializeObject(json, gradebook);
        }
    }
}

using GradeBook.Enums;
using System;
using System.Linq;

namespace GradeBook.GradeBooks
{
  public class RankedGradeBook : BaseGradeBook
  {
    public RankedGradeBook(string name, bool isWeighted) : base(name, isWeighted)
    {
      Type = GradeBookType.Ranked;
    }

    public override void CalculateStudentStatistics(string name)
    {
      if (Students.Count < 5)
      {
        System.Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
        return;
      }
      base.CalculateStudentStatistics(name);
    }

    public override void CalculateStatistics()
    {
      if (Students.Count < 5)
      {
        System.Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
        return;
      }
      base.CalculateStatistics();
    }

    public override char GetLetterGrade(double averageGrade)
    {
      if (Students.Count < 5)
      {
        throw new InvalidOperationException("Ranked grading requires at least 5 students");
      }

      // Number of students that should receive each letter grade
      var threshold = (int)Math.Ceiling(Students.Count * 0.2);

      //                Order students by       their avg grade   and take their avg grade val  to a list
      var grades = Students.OrderByDescending(e => e.AverageGrade).Select(e => e.AverageGrade).ToList();
      
      if (grades[threshold - 1] <= averageGrade)
        return 'A';
      else if (grades[(threshold * 2) - 1] <= averageGrade)
        return 'B';
      else if (grades[(threshold * 3) - 1] <= averageGrade)
        return 'C';
      else if (grades[(threshold * 4) - 1] <= averageGrade)
        return 'D';
      else
        return 'F';
    }
  }
}

using GradeBook.Enums;

namespace GradeBook.GradeBooks
{
  public class StandardGradeBook : BaseGradeBook
  {

    public StandardGradeBook(string name, bool isWeighted) : base(name, isWeighted)
    {
      Type = GradeBookType.Standard;
    }
  }
}

/*
	CSharp-GradeBookApplication
	
	Description: Project to add features to an existing C Sharp Grade Book Application.
	Website: http:\\www.pluralsight.com
	         https://github.com/pmash2/CSharp-GradeBookApplication
*/


using System;
using GradeBook.UserInterfaces;

namespace GradeBook
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("#=======================#");
            Console.WriteLine("# Welcome to GradeBook! #");
            Console.WriteLine("#=======================#");
            Console.WriteLine();

            StartingUserInterface.CommandLoop();
            
            Console.WriteLine("Thank you for using GradeBook!");
            Console.WriteLine("Have a nice day!");
            Console.Read();
        }
    }
}

namespace GradeBook.Enums
{
    public enum EnrollmentType
    {
        Campus,
        State,
        National,
        International
    }
}

namespace GradeBook.Enums
{
  public enum GradeBookType
  {
    Standard,
    Ranked,
    ESNU,
    OneToFour,
    SixPoint
  }
}

namespace GradeBook.Enums
{
    public enum StudentType
    {
        Standard,
        Honors,
        DualEnrolled
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

using GradeBook.Enums;

namespace GradeBook
{
    public class Student
    {
        public string Name { get; set; }
        public StudentType Type { get; set; }
        public EnrollmentType Enrollment { get; set; }
        public List<double> Grades { get; set; }
        [JsonIgnore]
        public double AverageGrade
        {
            get
            {
                return Grades.Average();
            }
        }
        [JsonIgnore]
        public char LetterGrade { get; set; }
        [JsonIgnore]
        public double GPA { get; set; }

        public Student(string name, StudentType studentType, EnrollmentType enrollment)
        {
            Name = name;
            Type = studentType;
            Enrollment = enrollment;
            Grades = new List<double>();
        }

        public void AddGrade(double grade)
        {
            if (grade < 0 || grade > 100)
                throw new ArgumentException("Grades must be between 0 and 100.");
            Grades.Add(grade);
        }

        public void RemoveGrade(double grade)
        {
            Grades.Remove(grade);
        }
    }
}

using GradeBook.GradeBooks;
using System;

namespace GradeBook.UserInterfaces
{
    public static class StartingUserInterface
    {
        public static bool Quit = false;
        public static void CommandLoop()
        {
            while (!Quit)
            {
                Console.WriteLine("What would you like to do?");
                var command = Console.ReadLine().ToLower();
                CommandRoute(command);
            }
        }

        public static void CommandRoute(string command)
        {
            if (command.StartsWith("create"))
                CreateCommand(command);
            else if (command.StartsWith("load"))
                LoadCommand(command);
            else if (command == "help")
                HelpCommand();
            else if (command == "quit")
                Quit = true;
            else
                Console.WriteLine("{0} was not recognized, please try again.", command);
        }

        public static void CreateCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Command not valid, Create requires a name, type of gradebook, if it's weighted (true / false).");
                return;
            }
            var name = parts[1];
            var type = parts[2].ToLower();
            var weighted = bool.Parse(parts[3]);
            BaseGradeBook gradeBook;

            if (type == "standard")
                gradeBook = new StandardGradeBook(name, weighted);
            else if (type == "ranked")
                gradeBook = new RankedGradeBook(name, weighted);
            else
            {
                System.Console.WriteLine("{0} is not a supported type of gradebook, please try again", type);
                return;
            }

            Console.WriteLine("Created gradebook {0}.", name);
            GradeBookUserInterface.CommandLoop(gradeBook);
        }

        public static void LoadCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Load requires a name.");
                return;
            }
            var name = parts[1];
            var gradeBook = BaseGradeBook.Load(name);

            if (gradeBook == null)
                return;

            GradeBookUserInterface.CommandLoop(gradeBook);
        }

        public static void HelpCommand()
        {
            Console.WriteLine("GradeBook accepts the following commands:");
            Console.WriteLine();
            Console.WriteLine("Create 'Name' 'Type' 'Weighted' - Creates a new gradebook where 'Name' is the name of the gradebook, 'Type' is what type of grading it should use, and 'Weighted' is whether or not grades should be weighted (true or false).");
            Console.WriteLine();
            Console.WriteLine("Load 'Name' - Loads the gradebook with the provided 'Name'.");
            Console.WriteLine();
            Console.WriteLine("Help - Displays all accepted commands.");
            Console.WriteLine();
            Console.WriteLine("Quit - Exits the application");
        }
    }
}


using GradeBook.Enums;
using GradeBook.GradeBooks;
using System;

namespace GradeBook.UserInterfaces
{
    public static class GradeBookUserInterface
    {
        public static BaseGradeBook GradeBook;
        public static bool Quit = false;
        public static void CommandLoop(BaseGradeBook gradeBook)
        {
            GradeBook = gradeBook;
            Quit = false;

            Console.WriteLine("#=======================#");
            Console.WriteLine(GradeBook.Name + " : " + GradeBook.GetType().Name);
            Console.WriteLine("#=======================#");
            Console.WriteLine(string.Empty);

            while(!Quit)
            {
                Console.WriteLine("What would you like to do?");
                var command = Console.ReadLine().ToLower();
                CommandRoute(command);
            }

            Console.WriteLine(GradeBook.Name + " has been closed.");
        }

        public static void CommandRoute(string command)
        {
            if (command == "save")
                SaveCommand();
            else if (command.StartsWith("addgrade"))
                AddGradeCommand(command);
            else if (command.StartsWith("removegrade"))
                RemoveGradeCommand(command);
            else if (command.StartsWith("add"))
                AddStudentCommand(command);
            else if (command.StartsWith("remove"))
                RemoveStudentCommand(command);
            else if (command == "list")
                ListCommand();
            else if (command == "statistics all")
                StatisticsCommand();
            else if (command.StartsWith("statistics"))
                StudentStatisticsCommand(command);
            else if (command == "help")
                HelpCommand();
            else if (command == "close")
                Quit = true;
            else
                Console.WriteLine("{0} was not recognized, please try again.", command);
        }

        public static void SaveCommand()
        {
            GradeBook.Save();
            Console.WriteLine("{0} has been saved.", GradeBook.Name);
        }
        
        public static void AddGradeCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Command not valid, AddGrade requires a name and score.");
                return;
            }
            var name = parts[1];
            var score = Double.Parse(parts[2]);
            GradeBook.AddGrade(name, score);
            Console.WriteLine("Added a score of {0} to {1}'s grades", score, name);
        }

        public static void RemoveGradeCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Command not valid, RemoveGrade requires a name and score.");
                return;
            }
            var name = parts[1];
            var score = Double.Parse(parts[2]);
            GradeBook.RemoveGrade(name, score);
            Console.WriteLine("Removed a score of {0} from {1}'s grades", score, name);
        }

        public static void AddStudentCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Command not valid, Add requires a name, student type, enrollment type.");
                return;
            }
            var name = parts[1];

            StudentType studentType;
            if (!Enum.TryParse(parts[2], true, out studentType))
            {
                Console.WriteLine("{0} is not a valid student type, try again.", parts[2]);
                return;
            }

            EnrollmentType enrollmentType;
            if (!Enum.TryParse(parts[3], true, out enrollmentType))
            {
                Console.WriteLine("{0} is not a volid enrollment type, try again.", parts[3]);
                return;
            }

            var student = new Student(name, studentType, enrollmentType);
            GradeBook.AddStudent(student);
            Console.WriteLine("Added {0} to the gradebook.", name);
        }
        
        public static void RemoveStudentCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Remove requires a name.");
                return;
            }
            var name = parts[1];
            GradeBook.RemoveStudent(name);
            Console.WriteLine("Removed {0} from the gradebook.", name);
        }

        public static void ListCommand()
        {
            GradeBook.ListStudents();
        }
        
        public static void StatisticsCommand()
        {
            GradeBook.CalculateStatistics();
        }

        public static void StudentStatisticsCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Requires Name or All.");
                return;
            }
            var name = parts[1];
            GradeBook.CalculateStudentStatistics(name);
        }

        public static void HelpCommand()
        {
            Console.WriteLine("While a gradebook is open you can use the following commands:");
            Console.WriteLine();
            Console.WriteLine("Add 'Name' 'Student Type' 'Enrollment Type' - Adds a new student to the gradebook with the provided name, type of student, and type of enrollment.");
            Console.WriteLine();
            Console.WriteLine("Accepted Student Types:");
            Console.WriteLine("Standard - Student not enrolled in Honors classes or Dual Enrolled.");
            Console.WriteLine("Honors - Students enrolled in Honors classes and not Dual Enrolled.");
            Console.WriteLine("DualEnrolled - Students who are Duel Enrolled.");
            Console.WriteLine();
            Console.WriteLine("Accepted Enrollement Types:");
            Console.WriteLine("Campus - Students who are in the same disctrict as the school.");
            Console.WriteLine("State - Students who's legal residence is outside the school's district, but is in the same state as the school.");
            Console.WriteLine("National - Students who's legal residence is not in the same state as the school, but is in the same country as the school.");
            Console.WriteLine("International - Students who's legal residence is not in the same country as the school.");
            Console.WriteLine();
            Console.WriteLine("List - Lists all students.");
            Console.WriteLine();
            Console.WriteLine("AddGrade 'Name' 'Score' - Adds a new grade to a student with the matching name of the provided score.");
            Console.WriteLine();
            Console.WriteLine("RemoveGrade 'Name' 'Score' - Removes a grade to a student with the matching name and score.");
            Console.WriteLine();
            Console.WriteLine("Remove 'Name' - Removes the student with the provided name.");
            Console.WriteLine();
            Console.WriteLine("Statistics 'Name' - Gets statistics for the specified student.");
            Console.WriteLine();
            Console.WriteLine("Statistics All - Gets general statistics for the entire gradebook.");
            Console.WriteLine();
            Console.WriteLine("Close - closes the gradebook and takes you back to the starting command options.");
            Console.WriteLine();
            Console.WriteLine("Save - saves the gradebook to the hard drive for later use.");
        }
    }
}

using System;
using System.Linq;

using GradeBook.Enums;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GradeBook.GradeBooks
{
    public abstract class BaseGradeBook
    {
        public string Name { get; set; }
        public List<Student> Students { get; set; }
        public GradeBookType Type { get; set; }
        public bool IsWeighted { get; set; }

        public BaseGradeBook(string name, bool isWeighted)
        {
            Name = name;
            Students = new List<Student>();
            IsWeighted = isWeighted;
        }

        public void AddStudent(Student student)
        {
            if (string.IsNullOrEmpty(student.Name))
                throw new ArgumentException("A Name is required to add a student to a gradebook.");
            Students.Add(student);
        }

        public void RemoveStudent(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to remove a student from a gradebook.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            Students.Remove(student);
        }

        public void AddGrade(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to add a grade to a student.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            student.AddGrade(score);
        }

        public void RemoveGrade(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to remove a grade from a student.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            student.RemoveGrade(score);
        }

        public void ListStudents()
        {
            foreach (var student in Students)
            {
                Console.WriteLine("{0} : {1} : {2}", student.Name, student.Type, student.Enrollment);
            }
        }

        public static BaseGradeBook Load(string name)
        {
            if (!File.Exists(name + ".gdbk"))
            {
                Console.WriteLine("Gradebook could not be found.");
                return null;
            }

            using (var file = new FileStream(name + ".gdbk", FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(file))
                {
                    var json = reader.ReadToEnd();
                    return ConvertToGradeBook(json);
                }
            }
        }

        public void Save()
        {
            using (var file = new FileStream(Name + ".gdbk", FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(file))
                {
                    var json = JsonConvert.SerializeObject(this);
                    writer.Write(json);
                }
            }
        }

        public virtual double GetGPA(char letterGrade, StudentType studentType)
        {
            var gpa = 0;
            
            switch (letterGrade)
            {
                case 'A':
                    gpa = 4;
                    break;
                case 'B':
                    gpa = 3;
                    break;
                case 'C':
                    gpa = 2;
                    break;
                case 'D':
                    gpa = 1;
                    break;
            }
            
            if (IsWeighted && (studentType == StudentType.Honors || studentType == StudentType.DualEnrolled))
                gpa++;
            
            return gpa;
        }

        public virtual void CalculateStatistics()
        {
            var allStudentsPoints = 0d;
            var campusPoints = 0d;
            var statePoints = 0d;
            var nationalPoints = 0d;
            var internationalPoints = 0d;
            var standardPoints = 0d;
            var honorPoints = 0d;
            var dualEnrolledPoints = 0d;

            foreach (var student in Students)
            {
                student.LetterGrade = GetLetterGrade(student.AverageGrade);
                student.GPA = GetGPA(student.LetterGrade, student.Type);

                Console.WriteLine("{0} ({1}:{2}) GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
                allStudentsPoints += student.AverageGrade;

                switch (student.Enrollment)
                {
                    case EnrollmentType.Campus:
                        campusPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.State:
                        statePoints += student.AverageGrade;
                        break;
                    case EnrollmentType.National:
                        nationalPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.International:
                        internationalPoints += student.AverageGrade;
                        break;
                }

                switch (student.Type)
                {
                    case StudentType.Standard:
                        standardPoints += student.AverageGrade;
                        break;
                    case StudentType.Honors:
                        honorPoints += student.AverageGrade;
                        break;
                    case StudentType.DualEnrolled:
                        dualEnrolledPoints += student.AverageGrade;
                        break;
                }
            }

            //#todo refactor into it's own method with calculations performed here
            Console.WriteLine("Average Grade of all students is " + (allStudentsPoints / Students.Count));
            if (campusPoints != 0)
                Console.WriteLine("Average for only local students is " + (campusPoints / Students.Where(e => e.Enrollment == EnrollmentType.Campus).Count()));
            if (statePoints != 0)
                Console.WriteLine("Average for only state students (excluding local) is " + (statePoints / Students.Where(e => e.Enrollment == EnrollmentType.State).Count()));
            if (nationalPoints != 0)
                Console.WriteLine("Average for only national students (excluding state and local) is " + (nationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.National).Count()));
            if (internationalPoints != 0)
                Console.WriteLine("Average for only international students is " + (internationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.International).Count()));
            if (standardPoints != 0)
                Console.WriteLine("Average for students excluding honors and duel enrollment is " + (standardPoints / Students.Where(e => e.Type == StudentType.Standard).Count()));
            if (honorPoints != 0)
                Console.WriteLine("Average for only honors students is " + (honorPoints / Students.Where(e => e.Type == StudentType.Honors).Count()));
            if (dualEnrolledPoints != 0)
                Console.WriteLine("Average for only duel enrolled students is " + (dualEnrolledPoints / Students.Where(e => e.Type == StudentType.DualEnrolled).Count()));
        }

        public virtual void CalculateStudentStatistics(string name)
        {
            var student = Students.FirstOrDefault(e => e.Name == name);
            student.LetterGrade = GetLetterGrade(student.AverageGrade);
            student.GPA = GetGPA(student.LetterGrade, student.Type);

            Console.WriteLine("{0} ({1}:{2}) GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
            Console.WriteLine();
            Console.WriteLine("Grades:");
            foreach (var grade in student.Grades)
            {
                Console.WriteLine(grade);
            }
        }

        public virtual char GetLetterGrade(double averageGrade)
        {
            if (averageGrade >= 90)
                return 'A';
            else if (averageGrade >= 80)
                return 'B';
            else if (averageGrade >= 70)
                return 'C';
            else if (averageGrade >= 60)
                return 'D';
            else
                return 'F';
        }

        /// <summary>
        ///     Converts json to the appropriate grade book type.
        ///     Note: This method contains code that is not recommended practice.
        ///     This has been used as a compromise to avoid adding additional complexity to the learner.
        /// </summary>
        /// <returns>The to grade book.</returns>
        /// <param name="json">Json.</param>
        public static dynamic ConvertToGradeBook(string json)
        {
            // Get GradeBookType from the GradeBook.Enums namespace
            var gradebookEnum = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 from type in assembly.GetTypes()
                                 where type.FullName == "GradeBook.Enums.GradeBookType"
                                 select type).FirstOrDefault();

            var jobject = JsonConvert.DeserializeObject<JObject>(json);
            var gradeBookType = jobject.Property("Type")?.Value?.ToString();

            // Check if StandardGradeBook exists
            if ((from assembly in AppDomain.CurrentDomain.GetAssemblies()
                 from type in assembly.GetTypes()
                 where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                 select type).FirstOrDefault() == null)
                gradeBookType = "Base";
            else
            {
                if (string.IsNullOrEmpty(gradeBookType))
                    gradeBookType = "Standard";
                else
                    gradeBookType = Enum.GetName(gradebookEnum, int.Parse(gradeBookType));
            }

            // Get GradeBook from the GradeBook.GradeBooks namespace
            var gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks." + gradeBookType + "GradeBook"
                             select type).FirstOrDefault();


            //protection code
            if (gradebook == null)
                gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                             select type).FirstOrDefault();
            
            return JsonConvert.DeserializeObject(json, gradebook);
        }
    }
}

using GradeBook.Enums;
using System;
using System.Linq;

namespace GradeBook.GradeBooks
{
  public class RankedGradeBook : BaseGradeBook
  {
    public RankedGradeBook(string name, bool isWeighted) : base(name, isWeighted)
    {
      Type = GradeBookType.Ranked;
    }

    public override void CalculateStudentStatistics(string name)
    {
      if (Students.Count < 5)
      {
        System.Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
        return;
      }
      base.CalculateStudentStatistics(name);
    }

    public override void CalculateStatistics()
    {
      if (Students.Count < 5)
      {
        System.Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
        return;
      }
      base.CalculateStatistics();
    }

    public override char GetLetterGrade(double averageGrade)
    {
      if (Students.Count < 5)
      {
        throw new InvalidOperationException("Ranked grading requires at least 5 students");
      }

      // Number of students that should receive each letter grade
      var threshold = (int)Math.Ceiling(Students.Count * 0.2);

      //                Order students by       their avg grade   and take their avg grade val  to a list
      var grades = Students.OrderByDescending(e => e.AverageGrade).Select(e => e.AverageGrade).ToList();
      
      if (grades[threshold - 1] <= averageGrade)
        return 'A';
      else if (grades[(threshold * 2) - 1] <= averageGrade)
        return 'B';
      else if (grades[(threshold * 3) - 1] <= averageGrade)
        return 'C';
      else if (grades[(threshold * 4) - 1] <= averageGrade)
        return 'D';
      else
        return 'F';
    }
  }
}

using GradeBook.Enums;

namespace GradeBook.GradeBooks
{
  public class StandardGradeBook : BaseGradeBook
  {

    public StandardGradeBook(string name, bool isWeighted) : base(name, isWeighted)
    {
      Type = GradeBookType.Standard;
    }
  }
}

/*
	CSharp-GradeBookApplication
	
	Description: Project to add features to an existing C Sharp Grade Book Application.
	Website: http:\\www.pluralsight.com
	         https://github.com/pmash2/CSharp-GradeBookApplication
*/


using System;
using GradeBook.UserInterfaces;

namespace GradeBook
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("#=======================#");
            Console.WriteLine("# Welcome to GradeBook! #");
            Console.WriteLine("#=======================#");
            Console.WriteLine();

            StartingUserInterface.CommandLoop();
            
            Console.WriteLine("Thank you for using GradeBook!");
            Console.WriteLine("Have a nice day!");
            Console.Read();
        }
    }
}

namespace GradeBook.Enums
{
    public enum EnrollmentType
    {
        Campus,
        State,
        National,
        International
    }
}

namespace GradeBook.Enums
{
  public enum GradeBookType
  {
    Standard,
    Ranked,
    ESNU,
    OneToFour,
    SixPoint
  }
}

namespace GradeBook.Enums
{
    public enum StudentType
    {
        Standard,
        Honors,
        DualEnrolled
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

using GradeBook.Enums;

namespace GradeBook
{
    public class Student
    {
        public string Name { get; set; }
        public StudentType Type { get; set; }
        public EnrollmentType Enrollment { get; set; }
        public List<double> Grades { get; set; }
        [JsonIgnore]
        public double AverageGrade
        {
            get
            {
                return Grades.Average();
            }
        }
        [JsonIgnore]
        public char LetterGrade { get; set; }
        [JsonIgnore]
        public double GPA { get; set; }

        public Student(string name, StudentType studentType, EnrollmentType enrollment)
        {
            Name = name;
            Type = studentType;
            Enrollment = enrollment;
            Grades = new List<double>();
        }

        public void AddGrade(double grade)
        {
            if (grade < 0 || grade > 100)
                throw new ArgumentException("Grades must be between 0 and 100.");
            Grades.Add(grade);
        }

        public void RemoveGrade(double grade)
        {
            Grades.Remove(grade);
        }
    }
}

using GradeBook.GradeBooks;
using System;

namespace GradeBook.UserInterfaces
{
    public static class StartingUserInterface
    {
        public static bool Quit = false;
        public static void CommandLoop()
        {
            while (!Quit)
            {
                Console.WriteLine("What would you like to do?");
                var command = Console.ReadLine().ToLower();
                CommandRoute(command);
            }
        }

        public static void CommandRoute(string command)
        {
            if (command.StartsWith("create"))
                CreateCommand(command);
            else if (command.StartsWith("load"))
                LoadCommand(command);
            else if (command == "help")
                HelpCommand();
            else if (command == "quit")
                Quit = true;
            else
                Console.WriteLine("{0} was not recognized, please try again.", command);
        }

        public static void CreateCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Command not valid, Create requires a name, type of gradebook, if it's weighted (true / false).");
                return;
            }
            var name = parts[1];
            var type = parts[2].ToLower();
            var weighted = bool.Parse(parts[3]);
            BaseGradeBook gradeBook;

            if (type == "standard")
                gradeBook = new StandardGradeBook(name, weighted);
            else if (type == "ranked")
                gradeBook = new RankedGradeBook(name, weighted);
            else
            {
                System.Console.WriteLine("{0} is not a supported type of gradebook, please try again", type);
                return;
            }

            Console.WriteLine("Created gradebook {0}.", name);
            GradeBookUserInterface.CommandLoop(gradeBook);
        }

        public static void LoadCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Load requires a name.");
                return;
            }
            var name = parts[1];
            var gradeBook = BaseGradeBook.Load(name);

            if (gradeBook == null)
                return;

            GradeBookUserInterface.CommandLoop(gradeBook);
        }

        public static void HelpCommand()
        {
            Console.WriteLine("GradeBook accepts the following commands:");
            Console.WriteLine();
            Console.WriteLine("Create 'Name' 'Type' 'Weighted' - Creates a new gradebook where 'Name' is the name of the gradebook, 'Type' is what type of grading it should use, and 'Weighted' is whether or not grades should be weighted (true or false).");
            Console.WriteLine();
            Console.WriteLine("Load 'Name' - Loads the gradebook with the provided 'Name'.");
            Console.WriteLine();
            Console.WriteLine("Help - Displays all accepted commands.");
            Console.WriteLine();
            Console.WriteLine("Quit - Exits the application");
        }
    }
}


using GradeBook.Enums;
using GradeBook.GradeBooks;
using System;

namespace GradeBook.UserInterfaces
{
    public static class GradeBookUserInterface
    {
        public static BaseGradeBook GradeBook;
        public static bool Quit = false;
        public static void CommandLoop(BaseGradeBook gradeBook)
        {
            GradeBook = gradeBook;
            Quit = false;

            Console.WriteLine("#=======================#");
            Console.WriteLine(GradeBook.Name + " : " + GradeBook.GetType().Name);
            Console.WriteLine("#=======================#");
            Console.WriteLine(string.Empty);

            while(!Quit)
            {
                Console.WriteLine("What would you like to do?");
                var command = Console.ReadLine().ToLower();
                CommandRoute(command);
            }

            Console.WriteLine(GradeBook.Name + " has been closed.");
        }

        public static void CommandRoute(string command)
        {
            if (command == "save")
                SaveCommand();
            else if (command.StartsWith("addgrade"))
                AddGradeCommand(command);
            else if (command.StartsWith("removegrade"))
                RemoveGradeCommand(command);
            else if (command.StartsWith("add"))
                AddStudentCommand(command);
            else if (command.StartsWith("remove"))
                RemoveStudentCommand(command);
            else if (command == "list")
                ListCommand();
            else if (command == "statistics all")
                StatisticsCommand();
            else if (command.StartsWith("statistics"))
                StudentStatisticsCommand(command);
            else if (command == "help")
                HelpCommand();
            else if (command == "close")
                Quit = true;
            else
                Console.WriteLine("{0} was not recognized, please try again.", command);
        }

        public static void SaveCommand()
        {
            GradeBook.Save();
            Console.WriteLine("{0} has been saved.", GradeBook.Name);
        }
        
        public static void AddGradeCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Command not valid, AddGrade requires a name and score.");
                return;
            }
            var name = parts[1];
            var score = Double.Parse(parts[2]);
            GradeBook.AddGrade(name, score);
            Console.WriteLine("Added a score of {0} to {1}'s grades", score, name);
        }

        public static void RemoveGradeCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Command not valid, RemoveGrade requires a name and score.");
                return;
            }
            var name = parts[1];
            var score = Double.Parse(parts[2]);
            GradeBook.RemoveGrade(name, score);
            Console.WriteLine("Removed a score of {0} from {1}'s grades", score, name);
        }

        public static void AddStudentCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Command not valid, Add requires a name, student type, enrollment type.");
                return;
            }
            var name = parts[1];

            StudentType studentType;
            if (!Enum.TryParse(parts[2], true, out studentType))
            {
                Console.WriteLine("{0} is not a valid student type, try again.", parts[2]);
                return;
            }

            EnrollmentType enrollmentType;
            if (!Enum.TryParse(parts[3], true, out enrollmentType))
            {
                Console.WriteLine("{0} is not a volid enrollment type, try again.", parts[3]);
                return;
            }

            var student = new Student(name, studentType, enrollmentType);
            GradeBook.AddStudent(student);
            Console.WriteLine("Added {0} to the gradebook.", name);
        }
        
        public static void RemoveStudentCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Remove requires a name.");
                return;
            }
            var name = parts[1];
            GradeBook.RemoveStudent(name);
            Console.WriteLine("Removed {0} from the gradebook.", name);
        }

        public static void ListCommand()
        {
            GradeBook.ListStudents();
        }
        
        public static void StatisticsCommand()
        {
            GradeBook.CalculateStatistics();
        }

        public static void StudentStatisticsCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Requires Name or All.");
                return;
            }
            var name = parts[1];
            GradeBook.CalculateStudentStatistics(name);
        }

        public static void HelpCommand()
        {
            Console.WriteLine("While a gradebook is open you can use the following commands:");
            Console.WriteLine();
            Console.WriteLine("Add 'Name' 'Student Type' 'Enrollment Type' - Adds a new student to the gradebook with the provided name, type of student, and type of enrollment.");
            Console.WriteLine();
            Console.WriteLine("Accepted Student Types:");
            Console.WriteLine("Standard - Student not enrolled in Honors classes or Dual Enrolled.");
            Console.WriteLine("Honors - Students enrolled in Honors classes and not Dual Enrolled.");
            Console.WriteLine("DualEnrolled - Students who are Duel Enrolled.");
            Console.WriteLine();
            Console.WriteLine("Accepted Enrollement Types:");
            Console.WriteLine("Campus - Students who are in the same disctrict as the school.");
            Console.WriteLine("State - Students who's legal residence is outside the school's district, but is in the same state as the school.");
            Console.WriteLine("National - Students who's legal residence is not in the same state as the school, but is in the same country as the school.");
            Console.WriteLine("International - Students who's legal residence is not in the same country as the school.");
            Console.WriteLine();
            Console.WriteLine("List - Lists all students.");
            Console.WriteLine();
            Console.WriteLine("AddGrade 'Name' 'Score' - Adds a new grade to a student with the matching name of the provided score.");
            Console.WriteLine();
            Console.WriteLine("RemoveGrade 'Name' 'Score' - Removes a grade to a student with the matching name and score.");
            Console.WriteLine();
            Console.WriteLine("Remove 'Name' - Removes the student with the provided name.");
            Console.WriteLine();
            Console.WriteLine("Statistics 'Name' - Gets statistics for the specified student.");
            Console.WriteLine();
            Console.WriteLine("Statistics All - Gets general statistics for the entire gradebook.");
            Console.WriteLine();
            Console.WriteLine("Close - closes the gradebook and takes you back to the starting command options.");
            Console.WriteLine();
            Console.WriteLine("Save - saves the gradebook to the hard drive for later use.");
        }
    }
}

using System;
using System.Linq;

using GradeBook.Enums;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GradeBook.GradeBooks
{
    public abstract class BaseGradeBook
    {
        public string Name { get; set; }
        public List<Student> Students { get; set; }
        public GradeBookType Type { get; set; }
        public bool IsWeighted { get; set; }

        public BaseGradeBook(string name, bool isWeighted)
        {
            Name = name;
            Students = new List<Student>();
            IsWeighted = isWeighted;
        }

        public void AddStudent(Student student)
        {
            if (string.IsNullOrEmpty(student.Name))
                throw new ArgumentException("A Name is required to add a student to a gradebook.");
            Students.Add(student);
        }

        public void RemoveStudent(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to remove a student from a gradebook.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            Students.Remove(student);
        }

        public void AddGrade(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to add a grade to a student.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            student.AddGrade(score);
        }

        public void RemoveGrade(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to remove a grade from a student.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            student.RemoveGrade(score);
        }

        public void ListStudents()
        {
            foreach (var student in Students)
            {
                Console.WriteLine("{0} : {1} : {2}", student.Name, student.Type, student.Enrollment);
            }
        }

        public static BaseGradeBook Load(string name)
        {
            if (!File.Exists(name + ".gdbk"))
            {
                Console.WriteLine("Gradebook could not be found.");
                return null;
            }

            using (var file = new FileStream(name + ".gdbk", FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(file))
                {
                    var json = reader.ReadToEnd();
                    return ConvertToGradeBook(json);
                }
            }
        }

        public void Save()
        {
            using (var file = new FileStream(Name + ".gdbk", FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(file))
                {
                    var json = JsonConvert.SerializeObject(this);
                    writer.Write(json);
                }
            }
        }

        public virtual double GetGPA(char letterGrade, StudentType studentType)
        {
            var gpa = 0;
            
            switch (letterGrade)
            {
                case 'A':
                    gpa = 4;
                    break;
                case 'B':
                    gpa = 3;
                    break;
                case 'C':
                    gpa = 2;
                    break;
                case 'D':
                    gpa = 1;
                    break;
            }
            
            if (IsWeighted && (studentType == StudentType.Honors || studentType == StudentType.DualEnrolled))
                gpa++;
            
            return gpa;
        }

        public virtual void CalculateStatistics()
        {
            var allStudentsPoints = 0d;
            var campusPoints = 0d;
            var statePoints = 0d;
            var nationalPoints = 0d;
            var internationalPoints = 0d;
            var standardPoints = 0d;
            var honorPoints = 0d;
            var dualEnrolledPoints = 0d;

            foreach (var student in Students)
            {
                student.LetterGrade = GetLetterGrade(student.AverageGrade);
                student.GPA = GetGPA(student.LetterGrade, student.Type);

                Console.WriteLine("{0} ({1}:{2}) GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
                allStudentsPoints += student.AverageGrade;

                switch (student.Enrollment)
                {
                    case EnrollmentType.Campus:
                        campusPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.State:
                        statePoints += student.AverageGrade;
                        break;
                    case EnrollmentType.National:
                        nationalPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.International:
                        internationalPoints += student.AverageGrade;
                        break;
                }

                switch (student.Type)
                {
                    case StudentType.Standard:
                        standardPoints += student.AverageGrade;
                        break;
                    case StudentType.Honors:
                        honorPoints += student.AverageGrade;
                        break;
                    case StudentType.DualEnrolled:
                        dualEnrolledPoints += student.AverageGrade;
                        break;
                }
            }

            //#todo refactor into it's own method with calculations performed here
            Console.WriteLine("Average Grade of all students is " + (allStudentsPoints / Students.Count));
            if (campusPoints != 0)
                Console.WriteLine("Average for only local students is " + (campusPoints / Students.Where(e => e.Enrollment == EnrollmentType.Campus).Count()));
            if (statePoints != 0)
                Console.WriteLine("Average for only state students (excluding local) is " + (statePoints / Students.Where(e => e.Enrollment == EnrollmentType.State).Count()));
            if (nationalPoints != 0)
                Console.WriteLine("Average for only national students (excluding state and local) is " + (nationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.National).Count()));
            if (internationalPoints != 0)
                Console.WriteLine("Average for only international students is " + (internationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.International).Count()));
            if (standardPoints != 0)
                Console.WriteLine("Average for students excluding honors and duel enrollment is " + (standardPoints / Students.Where(e => e.Type == StudentType.Standard).Count()));
            if (honorPoints != 0)
                Console.WriteLine("Average for only honors students is " + (honorPoints / Students.Where(e => e.Type == StudentType.Honors).Count()));
            if (dualEnrolledPoints != 0)
                Console.WriteLine("Average for only duel enrolled students is " + (dualEnrolledPoints / Students.Where(e => e.Type == StudentType.DualEnrolled).Count()));
        }

        public virtual void CalculateStudentStatistics(string name)
        {
            var student = Students.FirstOrDefault(e => e.Name == name);
            student.LetterGrade = GetLetterGrade(student.AverageGrade);
            student.GPA = GetGPA(student.LetterGrade, student.Type);

            Console.WriteLine("{0} ({1}:{2}) GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
            Console.WriteLine();
            Console.WriteLine("Grades:");
            foreach (var grade in student.Grades)
            {
                Console.WriteLine(grade);
            }
        }

        public virtual char GetLetterGrade(double averageGrade)
        {
            if (averageGrade >= 90)
                return 'A';
            else if (averageGrade >= 80)
                return 'B';
            else if (averageGrade >= 70)
                return 'C';
            else if (averageGrade >= 60)
                return 'D';
            else
                return 'F';
        }

        /// <summary>
        ///     Converts json to the appropriate grade book type.
        ///     Note: This method contains code that is not recommended practice.
        ///     This has been used as a compromise to avoid adding additional complexity to the learner.
        /// </summary>
        /// <returns>The to grade book.</returns>
        /// <param name="json">Json.</param>
        public static dynamic ConvertToGradeBook(string json)
        {
            // Get GradeBookType from the GradeBook.Enums namespace
            var gradebookEnum = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 from type in assembly.GetTypes()
                                 where type.FullName == "GradeBook.Enums.GradeBookType"
                                 select type).FirstOrDefault();

            var jobject = JsonConvert.DeserializeObject<JObject>(json);
            var gradeBookType = jobject.Property("Type")?.Value?.ToString();

            // Check if StandardGradeBook exists
            if ((from assembly in AppDomain.CurrentDomain.GetAssemblies()
                 from type in assembly.GetTypes()
                 where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                 select type).FirstOrDefault() == null)
                gradeBookType = "Base";
            else
            {
                if (string.IsNullOrEmpty(gradeBookType))
                    gradeBookType = "Standard";
                else
                    gradeBookType = Enum.GetName(gradebookEnum, int.Parse(gradeBookType));
            }

            // Get GradeBook from the GradeBook.GradeBooks namespace
            var gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks." + gradeBookType + "GradeBook"
                             select type).FirstOrDefault();


            //protection code
            if (gradebook == null)
                gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                             select type).FirstOrDefault();
            
            return JsonConvert.DeserializeObject(json, gradebook);
        }
    }
}

using GradeBook.Enums;
using System;
using System.Linq;

namespace GradeBook.GradeBooks
{
  public class RankedGradeBook : BaseGradeBook
  {
    public RankedGradeBook(string name, bool isWeighted) : base(name, isWeighted)
    {
      Type = GradeBookType.Ranked;
    }

    public override void CalculateStudentStatistics(string name)
    {
      if (Students.Count < 5)
      {
        System.Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
        return;
      }
      base.CalculateStudentStatistics(name);
    }

    public override void CalculateStatistics()
    {
      if (Students.Count < 5)
      {
        System.Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
        return;
      }
      base.CalculateStatistics();
    }

    public override char GetLetterGrade(double averageGrade)
    {
      if (Students.Count < 5)
      {
        throw new InvalidOperationException("Ranked grading requires at least 5 students");
      }

      // Number of students that should receive each letter grade
      var threshold = (int)Math.Ceiling(Students.Count * 0.2);

      //                Order students by       their avg grade   and take their avg grade val  to a list
      var grades = Students.OrderByDescending(e => e.AverageGrade).Select(e => e.AverageGrade).ToList();
      
      if (grades[threshold - 1] <= averageGrade)
        return 'A';
      else if (grades[(threshold * 2) - 1] <= averageGrade)
        return 'B';
      else if (grades[(threshold * 3) - 1] <= averageGrade)
        return 'C';
      else if (grades[(threshold * 4) - 1] <= averageGrade)
        return 'D';
      else
        return 'F';
    }
  }
}

using GradeBook.Enums;

namespace GradeBook.GradeBooks
{
  public class StandardGradeBook : BaseGradeBook
  {

    public StandardGradeBook(string name, bool isWeighted) : base(name, isWeighted)
    {
      Type = GradeBookType.Standard;
    }
  }
}

/*
	CSharp-GradeBookApplication
	
	Description: Project to add features to an existing C Sharp Grade Book Application.
	Website: http:\\www.pluralsight.com
	         https://github.com/pmash2/CSharp-GradeBookApplication
*/


using System;
using GradeBook.UserInterfaces;

namespace GradeBook
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("#=======================#");
            Console.WriteLine("# Welcome to GradeBook! #");
            Console.WriteLine("#=======================#");
            Console.WriteLine();

            StartingUserInterface.CommandLoop();
            
            Console.WriteLine("Thank you for using GradeBook!");
            Console.WriteLine("Have a nice day!");
            Console.Read();
        }
    }
}

namespace GradeBook.Enums
{
    public enum EnrollmentType
    {
        Campus,
        State,
        National,
        International
    }
}

namespace GradeBook.Enums
{
  public enum GradeBookType
  {
    Standard,
    Ranked,
    ESNU,
    OneToFour,
    SixPoint
  }
}

namespace GradeBook.Enums
{
    public enum StudentType
    {
        Standard,
        Honors,
        DualEnrolled
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

using GradeBook.Enums;

namespace GradeBook
{
    public class Student
    {
        public string Name { get; set; }
        public StudentType Type { get; set; }
        public EnrollmentType Enrollment { get; set; }
        public List<double> Grades { get; set; }
        [JsonIgnore]
        public double AverageGrade
        {
            get
            {
                return Grades.Average();
            }
        }
        [JsonIgnore]
        public char LetterGrade { get; set; }
        [JsonIgnore]
        public double GPA { get; set; }

        public Student(string name, StudentType studentType, EnrollmentType enrollment)
        {
            Name = name;
            Type = studentType;
            Enrollment = enrollment;
            Grades = new List<double>();
        }

        public void AddGrade(double grade)
        {
            if (grade < 0 || grade > 100)
                throw new ArgumentException("Grades must be between 0 and 100.");
            Grades.Add(grade);
        }

        public void RemoveGrade(double grade)
        {
            Grades.Remove(grade);
        }
    }
}

using GradeBook.GradeBooks;
using System;

namespace GradeBook.UserInterfaces
{
    public static class StartingUserInterface
    {
        public static bool Quit = false;
        public static void CommandLoop()
        {
            while (!Quit)
            {
                Console.WriteLine("What would you like to do?");
                var command = Console.ReadLine().ToLower();
                CommandRoute(command);
            }
        }

        public static void CommandRoute(string command)
        {
            if (command.StartsWith("create"))
                CreateCommand(command);
            else if (command.StartsWith("load"))
                LoadCommand(command);
            else if (command == "help")
                HelpCommand();
            else if (command == "quit")
                Quit = true;
            else
                Console.WriteLine("{0} was not recognized, please try again.", command);
        }

        public static void CreateCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Command not valid, Create requires a name, type of gradebook, if it's weighted (true / false).");
                return;
            }
            var name = parts[1];
            var type = parts[2].ToLower();
            var weighted = bool.Parse(parts[3]);
            BaseGradeBook gradeBook;

            if (type == "standard")
                gradeBook = new StandardGradeBook(name, weighted);
            else if (type == "ranked")
                gradeBook = new RankedGradeBook(name, weighted);
            else
            {
                System.Console.WriteLine("{0} is not a supported type of gradebook, please try again", type);
                return;
            }

            Console.WriteLine("Created gradebook {0}.", name);
            GradeBookUserInterface.CommandLoop(gradeBook);
        }

        public static void LoadCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Load requires a name.");
                return;
            }
            var name = parts[1];
            var gradeBook = BaseGradeBook.Load(name);

            if (gradeBook == null)
                return;

            GradeBookUserInterface.CommandLoop(gradeBook);
        }

        public static void HelpCommand()
        {
            Console.WriteLine("GradeBook accepts the following commands:");
            Console.WriteLine();
            Console.WriteLine("Create 'Name' 'Type' 'Weighted' - Creates a new gradebook where 'Name' is the name of the gradebook, 'Type' is what type of grading it should use, and 'Weighted' is whether or not grades should be weighted (true or false).");
            Console.WriteLine();
            Console.WriteLine("Load 'Name' - Loads the gradebook with the provided 'Name'.");
            Console.WriteLine();
            Console.WriteLine("Help - Displays all accepted commands.");
            Console.WriteLine();
            Console.WriteLine("Quit - Exits the application");
        }
    }
}


using GradeBook.Enums;
using GradeBook.GradeBooks;
using System;

namespace GradeBook.UserInterfaces
{
    public static class GradeBookUserInterface
    {
        public static BaseGradeBook GradeBook;
        public static bool Quit = false;
        public static void CommandLoop(BaseGradeBook gradeBook)
        {
            GradeBook = gradeBook;
            Quit = false;

            Console.WriteLine("#=======================#");
            Console.WriteLine(GradeBook.Name + " : " + GradeBook.GetType().Name);
            Console.WriteLine("#=======================#");
            Console.WriteLine(string.Empty);

            while(!Quit)
            {
                Console.WriteLine("What would you like to do?");
                var command = Console.ReadLine().ToLower();
                CommandRoute(command);
            }

            Console.WriteLine(GradeBook.Name + " has been closed.");
        }

        public static void CommandRoute(string command)
        {
            if (command == "save")
                SaveCommand();
            else if (command.StartsWith("addgrade"))
                AddGradeCommand(command);
            else if (command.StartsWith("removegrade"))
                RemoveGradeCommand(command);
            else if (command.StartsWith("add"))
                AddStudentCommand(command);
            else if (command.StartsWith("remove"))
                RemoveStudentCommand(command);
            else if (command == "list")
                ListCommand();
            else if (command == "statistics all")
                StatisticsCommand();
            else if (command.StartsWith("statistics"))
                StudentStatisticsCommand(command);
            else if (command == "help")
                HelpCommand();
            else if (command == "close")
                Quit = true;
            else
                Console.WriteLine("{0} was not recognized, please try again.", command);
        }

        public static void SaveCommand()
        {
            GradeBook.Save();
            Console.WriteLine("{0} has been saved.", GradeBook.Name);
        }
        
        public static void AddGradeCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Command not valid, AddGrade requires a name and score.");
                return;
            }
            var name = parts[1];
            var score = Double.Parse(parts[2]);
            GradeBook.AddGrade(name, score);
            Console.WriteLine("Added a score of {0} to {1}'s grades", score, name);
        }

        public static void RemoveGradeCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Command not valid, RemoveGrade requires a name and score.");
                return;
            }
            var name = parts[1];
            var score = Double.Parse(parts[2]);
            GradeBook.RemoveGrade(name, score);
            Console.WriteLine("Removed a score of {0} from {1}'s grades", score, name);
        }

        public static void AddStudentCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Command not valid, Add requires a name, student type, enrollment type.");
                return;
            }
            var name = parts[1];

            StudentType studentType;
            if (!Enum.TryParse(parts[2], true, out studentType))
            {
                Console.WriteLine("{0} is not a valid student type, try again.", parts[2]);
                return;
            }

            EnrollmentType enrollmentType;
            if (!Enum.TryParse(parts[3], true, out enrollmentType))
            {
                Console.WriteLine("{0} is not a volid enrollment type, try again.", parts[3]);
                return;
            }

            var student = new Student(name, studentType, enrollmentType);
            GradeBook.AddStudent(student);
            Console.WriteLine("Added {0} to the gradebook.", name);
        }
        
        public static void RemoveStudentCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Remove requires a name.");
                return;
            }
            var name = parts[1];
            GradeBook.RemoveStudent(name);
            Console.WriteLine("Removed {0} from the gradebook.", name);
        }

        public static void ListCommand()
        {
            GradeBook.ListStudents();
        }
        
        public static void StatisticsCommand()
        {
            GradeBook.CalculateStatistics();
        }

        public static void StudentStatisticsCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Requires Name or All.");
                return;
            }
            var name = parts[1];
            GradeBook.CalculateStudentStatistics(name);
        }

        public static void HelpCommand()
        {
            Console.WriteLine("While a gradebook is open you can use the following commands:");
            Console.WriteLine();
            Console.WriteLine("Add 'Name' 'Student Type' 'Enrollment Type' - Adds a new student to the gradebook with the provided name, type of student, and type of enrollment.");
            Console.WriteLine();
            Console.WriteLine("Accepted Student Types:");
            Console.WriteLine("Standard - Student not enrolled in Honors classes or Dual Enrolled.");
            Console.WriteLine("Honors - Students enrolled in Honors classes and not Dual Enrolled.");
            Console.WriteLine("DualEnrolled - Students who are Duel Enrolled.");
            Console.WriteLine();
            Console.WriteLine("Accepted Enrollement Types:");
            Console.WriteLine("Campus - Students who are in the same disctrict as the school.");
            Console.WriteLine("State - Students who's legal residence is outside the school's district, but is in the same state as the school.");
            Console.WriteLine("National - Students who's legal residence is not in the same state as the school, but is in the same country as the school.");
            Console.WriteLine("International - Students who's legal residence is not in the same country as the school.");
            Console.WriteLine();
            Console.WriteLine("List - Lists all students.");
            Console.WriteLine();
            Console.WriteLine("AddGrade 'Name' 'Score' - Adds a new grade to a student with the matching name of the provided score.");
            Console.WriteLine();
            Console.WriteLine("RemoveGrade 'Name' 'Score' - Removes a grade to a student with the matching name and score.");
            Console.WriteLine();
            Console.WriteLine("Remove 'Name' - Removes the student with the provided name.");
            Console.WriteLine();
            Console.WriteLine("Statistics 'Name' - Gets statistics for the specified student.");
            Console.WriteLine();
            Console.WriteLine("Statistics All - Gets general statistics for the entire gradebook.");
            Console.WriteLine();
            Console.WriteLine("Close - closes the gradebook and takes you back to the starting command options.");
            Console.WriteLine();
            Console.WriteLine("Save - saves the gradebook to the hard drive for later use.");
        }
    }
}

using System;
using System.Linq;

using GradeBook.Enums;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GradeBook.GradeBooks
{
    public abstract class BaseGradeBook
    {
        public string Name { get; set; }
        public List<Student> Students { get; set; }
        public GradeBookType Type { get; set; }
        public bool IsWeighted { get; set; }

        public BaseGradeBook(string name, bool isWeighted)
        {
            Name = name;
            Students = new List<Student>();
            IsWeighted = isWeighted;
        }

        public void AddStudent(Student student)
        {
            if (string.IsNullOrEmpty(student.Name))
                throw new ArgumentException("A Name is required to add a student to a gradebook.");
            Students.Add(student);
        }

        public void RemoveStudent(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to remove a student from a gradebook.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            Students.Remove(student);
        }

        public void AddGrade(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to add a grade to a student.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            student.AddGrade(score);
        }

        public void RemoveGrade(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to remove a grade from a student.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            student.RemoveGrade(score);
        }

        public void ListStudents()
        {
            foreach (var student in Students)
            {
                Console.WriteLine("{0} : {1} : {2}", student.Name, student.Type, student.Enrollment);
            }
        }

        public static BaseGradeBook Load(string name)
        {
            if (!File.Exists(name + ".gdbk"))
            {
                Console.WriteLine("Gradebook could not be found.");
                return null;
            }

            using (var file = new FileStream(name + ".gdbk", FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(file))
                {
                    var json = reader.ReadToEnd();
                    return ConvertToGradeBook(json);
                }
            }
        }

        public void Save()
        {
            using (var file = new FileStream(Name + ".gdbk", FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(file))
                {
                    var json = JsonConvert.SerializeObject(this);
                    writer.Write(json);
                }
            }
        }

        public virtual double GetGPA(char letterGrade, StudentType studentType)
        {
            var gpa = 0;
            
            switch (letterGrade)
            {
                case 'A':
                    gpa = 4;
                    break;
                case 'B':
                    gpa = 3;
                    break;
                case 'C':
                    gpa = 2;
                    break;
                case 'D':
                    gpa = 1;
                    break;
            }
            
            if (IsWeighted && (studentType == StudentType.Honors || studentType == StudentType.DualEnrolled))
                gpa++;
            
            return gpa;
        }

        public virtual void CalculateStatistics()
        {
            var allStudentsPoints = 0d;
            var campusPoints = 0d;
            var statePoints = 0d;
            var nationalPoints = 0d;
            var internationalPoints = 0d;
            var standardPoints = 0d;
            var honorPoints = 0d;
            var dualEnrolledPoints = 0d;

            foreach (var student in Students)
            {
                student.LetterGrade = GetLetterGrade(student.AverageGrade);
                student.GPA = GetGPA(student.LetterGrade, student.Type);

                Console.WriteLine("{0} ({1}:{2}) GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
                allStudentsPoints += student.AverageGrade;

                switch (student.Enrollment)
                {
                    case EnrollmentType.Campus:
                        campusPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.State:
                        statePoints += student.AverageGrade;
                        break;
                    case EnrollmentType.National:
                        nationalPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.International:
                        internationalPoints += student.AverageGrade;
                        break;
                }

                switch (student.Type)
                {
                    case StudentType.Standard:
                        standardPoints += student.AverageGrade;
                        break;
                    case StudentType.Honors:
                        honorPoints += student.AverageGrade;
                        break;
                    case StudentType.DualEnrolled:
                        dualEnrolledPoints += student.AverageGrade;
                        break;
                }
            }

            //#todo refactor into it's own method with calculations performed here
            Console.WriteLine("Average Grade of all students is " + (allStudentsPoints / Students.Count));
            if (campusPoints != 0)
                Console.WriteLine("Average for only local students is " + (campusPoints / Students.Where(e => e.Enrollment == EnrollmentType.Campus).Count()));
            if (statePoints != 0)
                Console.WriteLine("Average for only state students (excluding local) is " + (statePoints / Students.Where(e => e.Enrollment == EnrollmentType.State).Count()));
            if (nationalPoints != 0)
                Console.WriteLine("Average for only national students (excluding state and local) is " + (nationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.National).Count()));
            if (internationalPoints != 0)
                Console.WriteLine("Average for only international students is " + (internationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.International).Count()));
            if (standardPoints != 0)
                Console.WriteLine("Average for students excluding honors and duel enrollment is " + (standardPoints / Students.Where(e => e.Type == StudentType.Standard).Count()));
            if (honorPoints != 0)
                Console.WriteLine("Average for only honors students is " + (honorPoints / Students.Where(e => e.Type == StudentType.Honors).Count()));
            if (dualEnrolledPoints != 0)
                Console.WriteLine("Average for only duel enrolled students is " + (dualEnrolledPoints / Students.Where(e => e.Type == StudentType.DualEnrolled).Count()));
        }

        public virtual void CalculateStudentStatistics(string name)
        {
            var student = Students.FirstOrDefault(e => e.Name == name);
            student.LetterGrade = GetLetterGrade(student.AverageGrade);
            student.GPA = GetGPA(student.LetterGrade, student.Type);

            Console.WriteLine("{0} ({1}:{2}) GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
            Console.WriteLine();
            Console.WriteLine("Grades:");
            foreach (var grade in student.Grades)
            {
                Console.WriteLine(grade);
            }
        }

        public virtual char GetLetterGrade(double averageGrade)
        {
            if (averageGrade >= 90)
                return 'A';
            else if (averageGrade >= 80)
                return 'B';
            else if (averageGrade >= 70)
                return 'C';
            else if (averageGrade >= 60)
                return 'D';
            else
                return 'F';
        }

        /// <summary>
        ///     Converts json to the appropriate grade book type.
        ///     Note: This method contains code that is not recommended practice.
        ///     This has been used as a compromise to avoid adding additional complexity to the learner.
        /// </summary>
        /// <returns>The to grade book.</returns>
        /// <param name="json">Json.</param>
        public static dynamic ConvertToGradeBook(string json)
        {
            // Get GradeBookType from the GradeBook.Enums namespace
            var gradebookEnum = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 from type in assembly.GetTypes()
                                 where type.FullName == "GradeBook.Enums.GradeBookType"
                                 select type).FirstOrDefault();

            var jobject = JsonConvert.DeserializeObject<JObject>(json);
            var gradeBookType = jobject.Property("Type")?.Value?.ToString();

            // Check if StandardGradeBook exists
            if ((from assembly in AppDomain.CurrentDomain.GetAssemblies()
                 from type in assembly.GetTypes()
                 where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                 select type).FirstOrDefault() == null)
                gradeBookType = "Base";
            else
            {
                if (string.IsNullOrEmpty(gradeBookType))
                    gradeBookType = "Standard";
                else
                    gradeBookType = Enum.GetName(gradebookEnum, int.Parse(gradeBookType));
            }

            // Get GradeBook from the GradeBook.GradeBooks namespace
            var gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks." + gradeBookType + "GradeBook"
                             select type).FirstOrDefault();


            //protection code
            if (gradebook == null)
                gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                             select type).FirstOrDefault();
            
            return JsonConvert.DeserializeObject(json, gradebook);
        }
    }
}

using GradeBook.Enums;
using System;
using System.Linq;

namespace GradeBook.GradeBooks
{
  public class RankedGradeBook : BaseGradeBook
  {
    public RankedGradeBook(string name, bool isWeighted) : base(name, isWeighted)
    {
      Type = GradeBookType.Ranked;
    }

    public override void CalculateStudentStatistics(string name)
    {
      if (Students.Count < 5)
      {
        System.Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
        return;
      }
      base.CalculateStudentStatistics(name);
    }

    public override void CalculateStatistics()
    {
      if (Students.Count < 5)
      {
        System.Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
        return;
      }
      base.CalculateStatistics();
    }

    public override char GetLetterGrade(double averageGrade)
    {
      if (Students.Count < 5)
      {
        throw new InvalidOperationException("Ranked grading requires at least 5 students");
      }

      // Number of students that should receive each letter grade
      var threshold = (int)Math.Ceiling(Students.Count * 0.2);

      //                Order students by       their avg grade   and take their avg grade val  to a list
      var grades = Students.OrderByDescending(e => e.AverageGrade).Select(e => e.AverageGrade).ToList();
      
      if (grades[threshold - 1] <= averageGrade)
        return 'A';
      else if (grades[(threshold * 2) - 1] <= averageGrade)
        return 'B';
      else if (grades[(threshold * 3) - 1] <= averageGrade)
        return 'C';
      else if (grades[(threshold * 4) - 1] <= averageGrade)
        return 'D';
      else
        return 'F';
    }
  }
}

using GradeBook.Enums;

namespace GradeBook.GradeBooks
{
  public class StandardGradeBook : BaseGradeBook
  {

    public StandardGradeBook(string name, bool isWeighted) : base(name, isWeighted)
    {
      Type = GradeBookType.Standard;
    }
  }
}

/*
	CSharp-GradeBookApplication
	
	Description: Project to add features to an existing C Sharp Grade Book Application.
	Website: http:\\www.pluralsight.com
	         https://github.com/pmash2/CSharp-GradeBookApplication
*/


using System;
using GradeBook.UserInterfaces;

namespace GradeBook
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("#=======================#");
            Console.WriteLine("# Welcome to GradeBook! #");
            Console.WriteLine("#=======================#");
            Console.WriteLine();

            StartingUserInterface.CommandLoop();
            
            Console.WriteLine("Thank you for using GradeBook!");
            Console.WriteLine("Have a nice day!");
            Console.Read();
        }
    }
}

namespace GradeBook.Enums
{
    public enum EnrollmentType
    {
        Campus,
        State,
        National,
        International
    }
}

namespace GradeBook.Enums
{
  public enum GradeBookType
  {
    Standard,
    Ranked,
    ESNU,
    OneToFour,
    SixPoint
  }
}

namespace GradeBook.Enums
{
    public enum StudentType
    {
        Standard,
        Honors,
        DualEnrolled
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

using GradeBook.Enums;

namespace GradeBook
{
    public class Student
    {
        public string Name { get; set; }
        public StudentType Type { get; set; }
        public EnrollmentType Enrollment { get; set; }
        public List<double> Grades { get; set; }
        [JsonIgnore]
        public double AverageGrade
        {
            get
            {
                return Grades.Average();
            }
        }
        [JsonIgnore]
        public char LetterGrade { get; set; }
        [JsonIgnore]
        public double GPA { get; set; }

        public Student(string name, StudentType studentType, EnrollmentType enrollment)
        {
            Name = name;
            Type = studentType;
            Enrollment = enrollment;
            Grades = new List<double>();
        }

        public void AddGrade(double grade)
        {
            if (grade < 0 || grade > 100)
                throw new ArgumentException("Grades must be between 0 and 100.");
            Grades.Add(grade);
        }

        public void RemoveGrade(double grade)
        {
            Grades.Remove(grade);
        }
    }
}

using GradeBook.GradeBooks;
using System;

namespace GradeBook.UserInterfaces
{
    public static class StartingUserInterface
    {
        public static bool Quit = false;
        public static void CommandLoop()
        {
            while (!Quit)
            {
                Console.WriteLine("What would you like to do?");
                var command = Console.ReadLine().ToLower();
                CommandRoute(command);
            }
        }

        public static void CommandRoute(string command)
        {
            if (command.StartsWith("create"))
                CreateCommand(command);
            else if (command.StartsWith("load"))
                LoadCommand(command);
            else if (command == "help")
                HelpCommand();
            else if (command == "quit")
                Quit = true;
            else
                Console.WriteLine("{0} was not recognized, please try again.", command);
        }

        public static void CreateCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Command not valid, Create requires a name, type of gradebook, if it's weighted (true / false).");
                return;
            }
            var name = parts[1];
            var type = parts[2].ToLower();
            var weighted = bool.Parse(parts[3]);
            BaseGradeBook gradeBook;

            if (type == "standard")
                gradeBook = new StandardGradeBook(name, weighted);
            else if (type == "ranked")
                gradeBook = new RankedGradeBook(name, weighted);
            else
            {
                System.Console.WriteLine("{0} is not a supported type of gradebook, please try again", type);
                return;
            }

            Console.WriteLine("Created gradebook {0}.", name);
            GradeBookUserInterface.CommandLoop(gradeBook);
        }

        public static void LoadCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Load requires a name.");
                return;
            }
            var name = parts[1];
            var gradeBook = BaseGradeBook.Load(name);

            if (gradeBook == null)
                return;

            GradeBookUserInterface.CommandLoop(gradeBook);
        }

        public static void HelpCommand()
        {
            Console.WriteLine("GradeBook accepts the following commands:");
            Console.WriteLine();
            Console.WriteLine("Create 'Name' 'Type' 'Weighted' - Creates a new gradebook where 'Name' is the name of the gradebook, 'Type' is what type of grading it should use, and 'Weighted' is whether or not grades should be weighted (true or false).");
            Console.WriteLine();
            Console.WriteLine("Load 'Name' - Loads the gradebook with the provided 'Name'.");
            Console.WriteLine();
            Console.WriteLine("Help - Displays all accepted commands.");
            Console.WriteLine();
            Console.WriteLine("Quit - Exits the application");
        }
    }
}


using GradeBook.Enums;
using GradeBook.GradeBooks;
using System;

namespace GradeBook.UserInterfaces
{
    public static class GradeBookUserInterface
    {
        public static BaseGradeBook GradeBook;
        public static bool Quit = false;
        public static void CommandLoop(BaseGradeBook gradeBook)
        {
            GradeBook = gradeBook;
            Quit = false;

            Console.WriteLine("#=======================#");
            Console.WriteLine(GradeBook.Name + " : " + GradeBook.GetType().Name);
            Console.WriteLine("#=======================#");
            Console.WriteLine(string.Empty);

            while(!Quit)
            {
                Console.WriteLine("What would you like to do?");
                var command = Console.ReadLine().ToLower();
                CommandRoute(command);
            }

            Console.WriteLine(GradeBook.Name + " has been closed.");
        }

        public static void CommandRoute(string command)
        {
            if (command == "save")
                SaveCommand();
            else if (command.StartsWith("addgrade"))
                AddGradeCommand(command);
            else if (command.StartsWith("removegrade"))
                RemoveGradeCommand(command);
            else if (command.StartsWith("add"))
                AddStudentCommand(command);
            else if (command.StartsWith("remove"))
                RemoveStudentCommand(command);
            else if (command == "list")
                ListCommand();
            else if (command == "statistics all")
                StatisticsCommand();
            else if (command.StartsWith("statistics"))
                StudentStatisticsCommand(command);
            else if (command == "help")
                HelpCommand();
            else if (command == "close")
                Quit = true;
            else
                Console.WriteLine("{0} was not recognized, please try again.", command);
        }

        public static void SaveCommand()
        {
            GradeBook.Save();
            Console.WriteLine("{0} has been saved.", GradeBook.Name);
        }
        
        public static void AddGradeCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Command not valid, AddGrade requires a name and score.");
                return;
            }
            var name = parts[1];
            var score = Double.Parse(parts[2]);
            GradeBook.AddGrade(name, score);
            Console.WriteLine("Added a score of {0} to {1}'s grades", score, name);
        }

        public static void RemoveGradeCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Command not valid, RemoveGrade requires a name and score.");
                return;
            }
            var name = parts[1];
            var score = Double.Parse(parts[2]);
            GradeBook.RemoveGrade(name, score);
            Console.WriteLine("Removed a score of {0} from {1}'s grades", score, name);
        }

        public static void AddStudentCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Command not valid, Add requires a name, student type, enrollment type.");
                return;
            }
            var name = parts[1];

            StudentType studentType;
            if (!Enum.TryParse(parts[2], true, out studentType))
            {
                Console.WriteLine("{0} is not a valid student type, try again.", parts[2]);
                return;
            }

            EnrollmentType enrollmentType;
            if (!Enum.TryParse(parts[3], true, out enrollmentType))
            {
                Console.WriteLine("{0} is not a volid enrollment type, try again.", parts[3]);
                return;
            }

            var student = new Student(name, studentType, enrollmentType);
            GradeBook.AddStudent(student);
            Console.WriteLine("Added {0} to the gradebook.", name);
        }
        
        public static void RemoveStudentCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Remove requires a name.");
                return;
            }
            var name = parts[1];
            GradeBook.RemoveStudent(name);
            Console.WriteLine("Removed {0} from the gradebook.", name);
        }

        public static void ListCommand()
        {
            GradeBook.ListStudents();
        }
        
        public static void StatisticsCommand()
        {
            GradeBook.CalculateStatistics();
        }

        public static void StudentStatisticsCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Requires Name or All.");
                return;
            }
            var name = parts[1];
            GradeBook.CalculateStudentStatistics(name);
        }

        public static void HelpCommand()
        {
            Console.WriteLine("While a gradebook is open you can use the following commands:");
            Console.WriteLine();
            Console.WriteLine("Add 'Name' 'Student Type' 'Enrollment Type' - Adds a new student to the gradebook with the provided name, type of student, and type of enrollment.");
            Console.WriteLine();
            Console.WriteLine("Accepted Student Types:");
            Console.WriteLine("Standard - Student not enrolled in Honors classes or Dual Enrolled.");
            Console.WriteLine("Honors - Students enrolled in Honors classes and not Dual Enrolled.");
            Console.WriteLine("DualEnrolled - Students who are Duel Enrolled.");
            Console.WriteLine();
            Console.WriteLine("Accepted Enrollement Types:");
            Console.WriteLine("Campus - Students who are in the same disctrict as the school.");
            Console.WriteLine("State - Students who's legal residence is outside the school's district, but is in the same state as the school.");
            Console.WriteLine("National - Students who's legal residence is not in the same state as the school, but is in the same country as the school.");
            Console.WriteLine("International - Students who's legal residence is not in the same country as the school.");
            Console.WriteLine();
            Console.WriteLine("List - Lists all students.");
            Console.WriteLine();
            Console.WriteLine("AddGrade 'Name' 'Score' - Adds a new grade to a student with the matching name of the provided score.");
            Console.WriteLine();
            Console.WriteLine("RemoveGrade 'Name' 'Score' - Removes a grade to a student with the matching name and score.");
            Console.WriteLine();
            Console.WriteLine("Remove 'Name' - Removes the student with the provided name.");
            Console.WriteLine();
            Console.WriteLine("Statistics 'Name' - Gets statistics for the specified student.");
            Console.WriteLine();
            Console.WriteLine("Statistics All - Gets general statistics for the entire gradebook.");
            Console.WriteLine();
            Console.WriteLine("Close - closes the gradebook and takes you back to the starting command options.");
            Console.WriteLine();
            Console.WriteLine("Save - saves the gradebook to the hard drive for later use.");
        }
    }
}

using System;
using System.Linq;

using GradeBook.Enums;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GradeBook.GradeBooks
{
    public abstract class BaseGradeBook
    {
        public string Name { get; set; }
        public List<Student> Students { get; set; }
        public GradeBookType Type { get; set; }
        public bool IsWeighted { get; set; }

        public BaseGradeBook(string name, bool isWeighted)
        {
            Name = name;
            Students = new List<Student>();
            IsWeighted = isWeighted;
        }

        public void AddStudent(Student student)
        {
            if (string.IsNullOrEmpty(student.Name))
                throw new ArgumentException("A Name is required to add a student to a gradebook.");
            Students.Add(student);
        }

        public void RemoveStudent(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to remove a student from a gradebook.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            Students.Remove(student);
        }

        public void AddGrade(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to add a grade to a student.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            student.AddGrade(score);
        }

        public void RemoveGrade(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to remove a grade from a student.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            student.RemoveGrade(score);
        }

        public void ListStudents()
        {
            foreach (var student in Students)
            {
                Console.WriteLine("{0} : {1} : {2}", student.Name, student.Type, student.Enrollment);
            }
        }

        public static BaseGradeBook Load(string name)
        {
            if (!File.Exists(name + ".gdbk"))
            {
                Console.WriteLine("Gradebook could not be found.");
                return null;
            }

            using (var file = new FileStream(name + ".gdbk", FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(file))
                {
                    var json = reader.ReadToEnd();
                    return ConvertToGradeBook(json);
                }
            }
        }

        public void Save()
        {
            using (var file = new FileStream(Name + ".gdbk", FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(file))
                {
                    var json = JsonConvert.SerializeObject(this);
                    writer.Write(json);
                }
            }
        }

        public virtual double GetGPA(char letterGrade, StudentType studentType)
        {
            var gpa = 0;
            
            switch (letterGrade)
            {
                case 'A':
                    gpa = 4;
                    break;
                case 'B':
                    gpa = 3;
                    break;
                case 'C':
                    gpa = 2;
                    break;
                case 'D':
                    gpa = 1;
                    break;
            }
            
            if (IsWeighted && (studentType == StudentType.Honors || studentType == StudentType.DualEnrolled))
                gpa++;
            
            return gpa;
        }

        public virtual void CalculateStatistics()
        {
            var allStudentsPoints = 0d;
            var campusPoints = 0d;
            var statePoints = 0d;
            var nationalPoints = 0d;
            var internationalPoints = 0d;
            var standardPoints = 0d;
            var honorPoints = 0d;
            var dualEnrolledPoints = 0d;

            foreach (var student in Students)
            {
                student.LetterGrade = GetLetterGrade(student.AverageGrade);
                student.GPA = GetGPA(student.LetterGrade, student.Type);

                Console.WriteLine("{0} ({1}:{2}) GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
                allStudentsPoints += student.AverageGrade;

                switch (student.Enrollment)
                {
                    case EnrollmentType.Campus:
                        campusPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.State:
                        statePoints += student.AverageGrade;
                        break;
                    case EnrollmentType.National:
                        nationalPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.International:
                        internationalPoints += student.AverageGrade;
                        break;
                }

                switch (student.Type)
                {
                    case StudentType.Standard:
                        standardPoints += student.AverageGrade;
                        break;
                    case StudentType.Honors:
                        honorPoints += student.AverageGrade;
                        break;
                    case StudentType.DualEnrolled:
                        dualEnrolledPoints += student.AverageGrade;
                        break;
                }
            }

            //#todo refactor into it's own method with calculations performed here
            Console.WriteLine("Average Grade of all students is " + (allStudentsPoints / Students.Count));
            if (campusPoints != 0)
                Console.WriteLine("Average for only local students is " + (campusPoints / Students.Where(e => e.Enrollment == EnrollmentType.Campus).Count()));
            if (statePoints != 0)
                Console.WriteLine("Average for only state students (excluding local) is " + (statePoints / Students.Where(e => e.Enrollment == EnrollmentType.State).Count()));
            if (nationalPoints != 0)
                Console.WriteLine("Average for only national students (excluding state and local) is " + (nationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.National).Count()));
            if (internationalPoints != 0)
                Console.WriteLine("Average for only international students is " + (internationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.International).Count()));
            if (standardPoints != 0)
                Console.WriteLine("Average for students excluding honors and duel enrollment is " + (standardPoints / Students.Where(e => e.Type == StudentType.Standard).Count()));
            if (honorPoints != 0)
                Console.WriteLine("Average for only honors students is " + (honorPoints / Students.Where(e => e.Type == StudentType.Honors).Count()));
            if (dualEnrolledPoints != 0)
                Console.WriteLine("Average for only duel enrolled students is " + (dualEnrolledPoints / Students.Where(e => e.Type == StudentType.DualEnrolled).Count()));
        }

        public virtual void CalculateStudentStatistics(string name)
        {
            var student = Students.FirstOrDefault(e => e.Name == name);
            student.LetterGrade = GetLetterGrade(student.AverageGrade);
            student.GPA = GetGPA(student.LetterGrade, student.Type);

            Console.WriteLine("{0} ({1}:{2}) GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
            Console.WriteLine();
            Console.WriteLine("Grades:");
            foreach (var grade in student.Grades)
            {
                Console.WriteLine(grade);
            }
        }

        public virtual char GetLetterGrade(double averageGrade)
        {
            if (averageGrade >= 90)
                return 'A';
            else if (averageGrade >= 80)
                return 'B';
            else if (averageGrade >= 70)
                return 'C';
            else if (averageGrade >= 60)
                return 'D';
            else
                return 'F';
        }

        /// <summary>
        ///     Converts json to the appropriate grade book type.
        ///     Note: This method contains code that is not recommended practice.
        ///     This has been used as a compromise to avoid adding additional complexity to the learner.
        /// </summary>
        /// <returns>The to grade book.</returns>
        /// <param name="json">Json.</param>
        public static dynamic ConvertToGradeBook(string json)
        {
            // Get GradeBookType from the GradeBook.Enums namespace
            var gradebookEnum = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 from type in assembly.GetTypes()
                                 where type.FullName == "GradeBook.Enums.GradeBookType"
                                 select type).FirstOrDefault();

            var jobject = JsonConvert.DeserializeObject<JObject>(json);
            var gradeBookType = jobject.Property("Type")?.Value?.ToString();

            // Check if StandardGradeBook exists
            if ((from assembly in AppDomain.CurrentDomain.GetAssemblies()
                 from type in assembly.GetTypes()
                 where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                 select type).FirstOrDefault() == null)
                gradeBookType = "Base";
            else
            {
                if (string.IsNullOrEmpty(gradeBookType))
                    gradeBookType = "Standard";
                else
                    gradeBookType = Enum.GetName(gradebookEnum, int.Parse(gradeBookType));
            }

            // Get GradeBook from the GradeBook.GradeBooks namespace
            var gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks." + gradeBookType + "GradeBook"
                             select type).FirstOrDefault();


            //protection code
            if (gradebook == null)
                gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                             select type).FirstOrDefault();
            
            return JsonConvert.DeserializeObject(json, gradebook);
        }
    }
}

using GradeBook.Enums;
using System;
using System.Linq;

namespace GradeBook.GradeBooks
{
  public class RankedGradeBook : BaseGradeBook
  {
    public RankedGradeBook(string name, bool isWeighted) : base(name, isWeighted)
    {
      Type = GradeBookType.Ranked;
    }

    public override void CalculateStudentStatistics(string name)
    {
      if (Students.Count < 5)
      {
        System.Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
        return;
      }
      base.CalculateStudentStatistics(name);
    }

    public override void CalculateStatistics()
    {
      if (Students.Count < 5)
      {
        System.Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
        return;
      }
      base.CalculateStatistics();
    }

    public override char GetLetterGrade(double averageGrade)
    {
      if (Students.Count < 5)
      {
        throw new InvalidOperationException("Ranked grading requires at least 5 students");
      }

      // Number of students that should receive each letter grade
      var threshold = (int)Math.Ceiling(Students.Count * 0.2);

      //                Order students by       their avg grade   and take their avg grade val  to a list
      var grades = Students.OrderByDescending(e => e.AverageGrade).Select(e => e.AverageGrade).ToList();
      
      if (grades[threshold - 1] <= averageGrade)
        return 'A';
      else if (grades[(threshold * 2) - 1] <= averageGrade)
        return 'B';
      else if (grades[(threshold * 3) - 1] <= averageGrade)
        return 'C';
      else if (grades[(threshold * 4) - 1] <= averageGrade)
        return 'D';
      else
        return 'F';
    }
  }
}

using GradeBook.Enums;

namespace GradeBook.GradeBooks
{
  public class StandardGradeBook : BaseGradeBook
  {

    public StandardGradeBook(string name, bool isWeighted) : base(name, isWeighted)
    {
      Type = GradeBookType.Standard;
    }
  }
}

/*
	CSharp-GradeBookApplication
	
	Description: Project to add features to an existing C Sharp Grade Book Application.
	Website: http:\\www.pluralsight.com
	         https://github.com/pmash2/CSharp-GradeBookApplication
*/


using System;
using GradeBook.UserInterfaces;

namespace GradeBook
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("#=======================#");
            Console.WriteLine("# Welcome to GradeBook! #");
            Console.WriteLine("#=======================#");
            Console.WriteLine();

            StartingUserInterface.CommandLoop();
            
            Console.WriteLine("Thank you for using GradeBook!");
            Console.WriteLine("Have a nice day!");
            Console.Read();
        }
    }
}

namespace GradeBook.Enums
{
    public enum EnrollmentType
    {
        Campus,
        State,
        National,
        International
    }
}

namespace GradeBook.Enums
{
  public enum GradeBookType
  {
    Standard,
    Ranked,
    ESNU,
    OneToFour,
    SixPoint
  }
}

namespace GradeBook.Enums
{
    public enum StudentType
    {
        Standard,
        Honors,
        DualEnrolled
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

using GradeBook.Enums;

namespace GradeBook
{
    public class Student
    {
        public string Name { get; set; }
        public StudentType Type { get; set; }
        public EnrollmentType Enrollment { get; set; }
        public List<double> Grades { get; set; }
        [JsonIgnore]
        public double AverageGrade
        {
            get
            {
                return Grades.Average();
            }
        }
        [JsonIgnore]
        public char LetterGrade { get; set; }
        [JsonIgnore]
        public double GPA { get; set; }

        public Student(string name, StudentType studentType, EnrollmentType enrollment)
        {
            Name = name;
            Type = studentType;
            Enrollment = enrollment;
            Grades = new List<double>();
        }

        public void AddGrade(double grade)
        {
            if (grade < 0 || grade > 100)
                throw new ArgumentException("Grades must be between 0 and 100.");
            Grades.Add(grade);
        }

        public void RemoveGrade(double grade)
        {
            Grades.Remove(grade);
        }
    }
}

using GradeBook.GradeBooks;
using System;

namespace GradeBook.UserInterfaces
{
    public static class StartingUserInterface
    {
        public static bool Quit = false;
        public static void CommandLoop()
        {
            while (!Quit)
            {
                Console.WriteLine("What would you like to do?");
                var command = Console.ReadLine().ToLower();
                CommandRoute(command);
            }
        }

        public static void CommandRoute(string command)
        {
            if (command.StartsWith("create"))
                CreateCommand(command);
            else if (command.StartsWith("load"))
                LoadCommand(command);
            else if (command == "help")
                HelpCommand();
            else if (command == "quit")
                Quit = true;
            else
                Console.WriteLine("{0} was not recognized, please try again.", command);
        }

        public static void CreateCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Command not valid, Create requires a name, type of gradebook, if it's weighted (true / false).");
                return;
            }
            var name = parts[1];
            var type = parts[2].ToLower();
            var weighted = bool.Parse(parts[3]);
            BaseGradeBook gradeBook;

            if (type == "standard")
                gradeBook = new StandardGradeBook(name, weighted);
            else if (type == "ranked")
                gradeBook = new RankedGradeBook(name, weighted);
            else
            {
                System.Console.WriteLine("{0} is not a supported type of gradebook, please try again", type);
                return;
            }

            Console.WriteLine("Created gradebook {0}.", name);
            GradeBookUserInterface.CommandLoop(gradeBook);
        }

        public static void LoadCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Load requires a name.");
                return;
            }
            var name = parts[1];
            var gradeBook = BaseGradeBook.Load(name);

            if (gradeBook == null)
                return;

            GradeBookUserInterface.CommandLoop(gradeBook);
        }

        public static void HelpCommand()
        {
            Console.WriteLine("GradeBook accepts the following commands:");
            Console.WriteLine();
            Console.WriteLine("Create 'Name' 'Type' 'Weighted' - Creates a new gradebook where 'Name' is the name of the gradebook, 'Type' is what type of grading it should use, and 'Weighted' is whether or not grades should be weighted (true or false).");
            Console.WriteLine();
            Console.WriteLine("Load 'Name' - Loads the gradebook with the provided 'Name'.");
            Console.WriteLine();
            Console.WriteLine("Help - Displays all accepted commands.");
            Console.WriteLine();
            Console.WriteLine("Quit - Exits the application");
        }
    }
}


using GradeBook.Enums;
using GradeBook.GradeBooks;
using System;

namespace GradeBook.UserInterfaces
{
    public static class GradeBookUserInterface
    {
        public static BaseGradeBook GradeBook;
        public static bool Quit = false;
        public static void CommandLoop(BaseGradeBook gradeBook)
        {
            GradeBook = gradeBook;
            Quit = false;

            Console.WriteLine("#=======================#");
            Console.WriteLine(GradeBook.Name + " : " + GradeBook.GetType().Name);
            Console.WriteLine("#=======================#");
            Console.WriteLine(string.Empty);

            while(!Quit)
            {
                Console.WriteLine("What would you like to do?");
                var command = Console.ReadLine().ToLower();
                CommandRoute(command);
            }

            Console.WriteLine(GradeBook.Name + " has been closed.");
        }

        public static void CommandRoute(string command)
        {
            if (command == "save")
                SaveCommand();
            else if (command.StartsWith("addgrade"))
                AddGradeCommand(command);
            else if (command.StartsWith("removegrade"))
                RemoveGradeCommand(command);
            else if (command.StartsWith("add"))
                AddStudentCommand(command);
            else if (command.StartsWith("remove"))
                RemoveStudentCommand(command);
            else if (command == "list")
                ListCommand();
            else if (command == "statistics all")
                StatisticsCommand();
            else if (command.StartsWith("statistics"))
                StudentStatisticsCommand(command);
            else if (command == "help")
                HelpCommand();
            else if (command == "close")
                Quit = true;
            else
                Console.WriteLine("{0} was not recognized, please try again.", command);
        }

        public static void SaveCommand()
        {
            GradeBook.Save();
            Console.WriteLine("{0} has been saved.", GradeBook.Name);
        }
        
        public static void AddGradeCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Command not valid, AddGrade requires a name and score.");
                return;
            }
            var name = parts[1];
            var score = Double.Parse(parts[2]);
            GradeBook.AddGrade(name, score);
            Console.WriteLine("Added a score of {0} to {1}'s grades", score, name);
        }

        public static void RemoveGradeCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Command not valid, RemoveGrade requires a name and score.");
                return;
            }
            var name = parts[1];
            var score = Double.Parse(parts[2]);
            GradeBook.RemoveGrade(name, score);
            Console.WriteLine("Removed a score of {0} from {1}'s grades", score, name);
        }

        public static void AddStudentCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Command not valid, Add requires a name, student type, enrollment type.");
                return;
            }
            var name = parts[1];

            StudentType studentType;
            if (!Enum.TryParse(parts[2], true, out studentType))
            {
                Console.WriteLine("{0} is not a valid student type, try again.", parts[2]);
                return;
            }

            EnrollmentType enrollmentType;
            if (!Enum.TryParse(parts[3], true, out enrollmentType))
            {
                Console.WriteLine("{0} is not a volid enrollment type, try again.", parts[3]);
                return;
            }

            var student = new Student(name, studentType, enrollmentType);
            GradeBook.AddStudent(student);
            Console.WriteLine("Added {0} to the gradebook.", name);
        }
        
        public static void RemoveStudentCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Remove requires a name.");
                return;
            }
            var name = parts[1];
            GradeBook.RemoveStudent(name);
            Console.WriteLine("Removed {0} from the gradebook.", name);
        }

        public static void ListCommand()
        {
            GradeBook.ListStudents();
        }
        
        public static void StatisticsCommand()
        {
            GradeBook.CalculateStatistics();
        }

        public static void StudentStatisticsCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Requires Name or All.");
                return;
            }
            var name = parts[1];
            GradeBook.CalculateStudentStatistics(name);
        }

        public static void HelpCommand()
        {
            Console.WriteLine("While a gradebook is open you can use the following commands:");
            Console.WriteLine();
            Console.WriteLine("Add 'Name' 'Student Type' 'Enrollment Type' - Adds a new student to the gradebook with the provided name, type of student, and type of enrollment.");
            Console.WriteLine();
            Console.WriteLine("Accepted Student Types:");
            Console.WriteLine("Standard - Student not enrolled in Honors classes or Dual Enrolled.");
            Console.WriteLine("Honors - Students enrolled in Honors classes and not Dual Enrolled.");
            Console.WriteLine("DualEnrolled - Students who are Duel Enrolled.");
            Console.WriteLine();
            Console.WriteLine("Accepted Enrollement Types:");
            Console.WriteLine("Campus - Students who are in the same disctrict as the school.");
            Console.WriteLine("State - Students who's legal residence is outside the school's district, but is in the same state as the school.");
            Console.WriteLine("National - Students who's legal residence is not in the same state as the school, but is in the same country as the school.");
            Console.WriteLine("International - Students who's legal residence is not in the same country as the school.");
            Console.WriteLine();
            Console.WriteLine("List - Lists all students.");
            Console.WriteLine();
            Console.WriteLine("AddGrade 'Name' 'Score' - Adds a new grade to a student with the matching name of the provided score.");
            Console.WriteLine();
            Console.WriteLine("RemoveGrade 'Name' 'Score' - Removes a grade to a student with the matching name and score.");
            Console.WriteLine();
            Console.WriteLine("Remove 'Name' - Removes the student with the provided name.");
            Console.WriteLine();
            Console.WriteLine("Statistics 'Name' - Gets statistics for the specified student.");
            Console.WriteLine();
            Console.WriteLine("Statistics All - Gets general statistics for the entire gradebook.");
            Console.WriteLine();
            Console.WriteLine("Close - closes the gradebook and takes you back to the starting command options.");
            Console.WriteLine();
            Console.WriteLine("Save - saves the gradebook to the hard drive for later use.");
        }
    }
}

using System;
using System.Linq;

using GradeBook.Enums;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GradeBook.GradeBooks
{
    public abstract class BaseGradeBook
    {
        public string Name { get; set; }
        public List<Student> Students { get; set; }
        public GradeBookType Type { get; set; }
        public bool IsWeighted { get; set; }

        public BaseGradeBook(string name, bool isWeighted)
        {
            Name = name;
            Students = new List<Student>();
            IsWeighted = isWeighted;
        }

        public void AddStudent(Student student)
        {
            if (string.IsNullOrEmpty(student.Name))
                throw new ArgumentException("A Name is required to add a student to a gradebook.");
            Students.Add(student);
        }

        public void RemoveStudent(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to remove a student from a gradebook.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            Students.Remove(student);
        }

        public void AddGrade(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to add a grade to a student.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            student.AddGrade(score);
        }

        public void RemoveGrade(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to remove a grade from a student.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            student.RemoveGrade(score);
        }

        public void ListStudents()
        {
            foreach (var student in Students)
            {
                Console.WriteLine("{0} : {1} : {2}", student.Name, student.Type, student.Enrollment);
            }
        }

        public static BaseGradeBook Load(string name)
        {
            if (!File.Exists(name + ".gdbk"))
            {
                Console.WriteLine("Gradebook could not be found.");
                return null;
            }

            using (var file = new FileStream(name + ".gdbk", FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(file))
                {
                    var json = reader.ReadToEnd();
                    return ConvertToGradeBook(json);
                }
            }
        }

        public void Save()
        {
            using (var file = new FileStream(Name + ".gdbk", FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(file))
                {
                    var json = JsonConvert.SerializeObject(this);
                    writer.Write(json);
                }
            }
        }

        public virtual double GetGPA(char letterGrade, StudentType studentType)
        {
            var gpa = 0;
            
            switch (letterGrade)
            {
                case 'A':
                    gpa = 4;
                    break;
                case 'B':
                    gpa = 3;
                    break;
                case 'C':
                    gpa = 2;
                    break;
                case 'D':
                    gpa = 1;
                    break;
            }
            
            if (IsWeighted && (studentType == StudentType.Honors || studentType == StudentType.DualEnrolled))
                gpa++;
            
            return gpa;
        }

        public virtual void CalculateStatistics()
        {
            var allStudentsPoints = 0d;
            var campusPoints = 0d;
            var statePoints = 0d;
            var nationalPoints = 0d;
            var internationalPoints = 0d;
            var standardPoints = 0d;
            var honorPoints = 0d;
            var dualEnrolledPoints = 0d;

            foreach (var student in Students)
            {
                student.LetterGrade = GetLetterGrade(student.AverageGrade);
                student.GPA = GetGPA(student.LetterGrade, student.Type);

                Console.WriteLine("{0} ({1}:{2}) GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
                allStudentsPoints += student.AverageGrade;

                switch (student.Enrollment)
                {
                    case EnrollmentType.Campus:
                        campusPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.State:
                        statePoints += student.AverageGrade;
                        break;
                    case EnrollmentType.National:
                        nationalPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.International:
                        internationalPoints += student.AverageGrade;
                        break;
                }

                switch (student.Type)
                {
                    case StudentType.Standard:
                        standardPoints += student.AverageGrade;
                        break;
                    case StudentType.Honors:
                        honorPoints += student.AverageGrade;
                        break;
                    case StudentType.DualEnrolled:
                        dualEnrolledPoints += student.AverageGrade;
                        break;
                }
            }

            //#todo refactor into it's own method with calculations performed here
            Console.WriteLine("Average Grade of all students is " + (allStudentsPoints / Students.Count));
            if (campusPoints != 0)
                Console.WriteLine("Average for only local students is " + (campusPoints / Students.Where(e => e.Enrollment == EnrollmentType.Campus).Count()));
            if (statePoints != 0)
                Console.WriteLine("Average for only state students (excluding local) is " + (statePoints / Students.Where(e => e.Enrollment == EnrollmentType.State).Count()));
            if (nationalPoints != 0)
                Console.WriteLine("Average for only national students (excluding state and local) is " + (nationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.National).Count()));
            if (internationalPoints != 0)
                Console.WriteLine("Average for only international students is " + (internationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.International).Count()));
            if (standardPoints != 0)
                Console.WriteLine("Average for students excluding honors and duel enrollment is " + (standardPoints / Students.Where(e => e.Type == StudentType.Standard).Count()));
            if (honorPoints != 0)
                Console.WriteLine("Average for only honors students is " + (honorPoints / Students.Where(e => e.Type == StudentType.Honors).Count()));
            if (dualEnrolledPoints != 0)
                Console.WriteLine("Average for only duel enrolled students is " + (dualEnrolledPoints / Students.Where(e => e.Type == StudentType.DualEnrolled).Count()));
        }

        public virtual void CalculateStudentStatistics(string name)
        {
            var student = Students.FirstOrDefault(e => e.Name == name);
            student.LetterGrade = GetLetterGrade(student.AverageGrade);
            student.GPA = GetGPA(student.LetterGrade, student.Type);

            Console.WriteLine("{0} ({1}:{2}) GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
            Console.WriteLine();
            Console.WriteLine("Grades:");
            foreach (var grade in student.Grades)
            {
                Console.WriteLine(grade);
            }
        }

        public virtual char GetLetterGrade(double averageGrade)
        {
            if (averageGrade >= 90)
                return 'A';
            else if (averageGrade >= 80)
                return 'B';
            else if (averageGrade >= 70)
                return 'C';
            else if (averageGrade >= 60)
                return 'D';
            else
                return 'F';
        }

        /// <summary>
        ///     Converts json to the appropriate grade book type.
        ///     Note: This method contains code that is not recommended practice.
        ///     This has been used as a compromise to avoid adding additional complexity to the learner.
        /// </summary>
        /// <returns>The to grade book.</returns>
        /// <param name="json">Json.</param>
        public static dynamic ConvertToGradeBook(string json)
        {
            // Get GradeBookType from the GradeBook.Enums namespace
            var gradebookEnum = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 from type in assembly.GetTypes()
                                 where type.FullName == "GradeBook.Enums.GradeBookType"
                                 select type).FirstOrDefault();

            var jobject = JsonConvert.DeserializeObject<JObject>(json);
            var gradeBookType = jobject.Property("Type")?.Value?.ToString();

            // Check if StandardGradeBook exists
            if ((from assembly in AppDomain.CurrentDomain.GetAssemblies()
                 from type in assembly.GetTypes()
                 where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                 select type).FirstOrDefault() == null)
                gradeBookType = "Base";
            else
            {
                if (string.IsNullOrEmpty(gradeBookType))
                    gradeBookType = "Standard";
                else
                    gradeBookType = Enum.GetName(gradebookEnum, int.Parse(gradeBookType));
            }

            // Get GradeBook from the GradeBook.GradeBooks namespace
            var gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks." + gradeBookType + "GradeBook"
                             select type).FirstOrDefault();


            //protection code
            if (gradebook == null)
                gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                             select type).FirstOrDefault();
            
            return JsonConvert.DeserializeObject(json, gradebook);
        }
    }
}

using GradeBook.Enums;
using System;
using System.Linq;

namespace GradeBook.GradeBooks
{
  public class RankedGradeBook : BaseGradeBook
  {
    public RankedGradeBook(string name, bool isWeighted) : base(name, isWeighted)
    {
      Type = GradeBookType.Ranked;
    }

    public override void CalculateStudentStatistics(string name)
    {
      if (Students.Count < 5)
      {
        System.Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
        return;
      }
      base.CalculateStudentStatistics(name);
    }

    public override void CalculateStatistics()
    {
      if (Students.Count < 5)
      {
        System.Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
        return;
      }
      base.CalculateStatistics();
    }

    public override char GetLetterGrade(double averageGrade)
    {
      if (Students.Count < 5)
      {
        throw new InvalidOperationException("Ranked grading requires at least 5 students");
      }

      // Number of students that should receive each letter grade
      var threshold = (int)Math.Ceiling(Students.Count * 0.2);

      //                Order students by       their avg grade   and take their avg grade val  to a list
      var grades = Students.OrderByDescending(e => e.AverageGrade).Select(e => e.AverageGrade).ToList();
      
      if (grades[threshold - 1] <= averageGrade)
        return 'A';
      else if (grades[(threshold * 2) - 1] <= averageGrade)
        return 'B';
      else if (grades[(threshold * 3) - 1] <= averageGrade)
        return 'C';
      else if (grades[(threshold * 4) - 1] <= averageGrade)
        return 'D';
      else
        return 'F';
    }
  }
}

using GradeBook.Enums;

namespace GradeBook.GradeBooks
{
  public class StandardGradeBook : BaseGradeBook
  {

    public StandardGradeBook(string name, bool isWeighted) : base(name, isWeighted)
    {
      Type = GradeBookType.Standard;
    }
  }
}

/*
	CSharp-GradeBookApplication
	
	Description: Project to add features to an existing C Sharp Grade Book Application.
	Website: http:\\www.pluralsight.com
	         https://github.com/pmash2/CSharp-GradeBookApplication
*/


using System;
using GradeBook.UserInterfaces;

namespace GradeBook
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("#=======================#");
            Console.WriteLine("# Welcome to GradeBook! #");
            Console.WriteLine("#=======================#");
            Console.WriteLine();

            StartingUserInterface.CommandLoop();
            
            Console.WriteLine("Thank you for using GradeBook!");
            Console.WriteLine("Have a nice day!");
            Console.Read();
        }
    }
}

namespace GradeBook.Enums
{
    public enum EnrollmentType
    {
        Campus,
        State,
        National,
        International
    }
}

namespace GradeBook.Enums
{
  public enum GradeBookType
  {
    Standard,
    Ranked,
    ESNU,
    OneToFour,
    SixPoint
  }
}

namespace GradeBook.Enums
{
    public enum StudentType
    {
        Standard,
        Honors,
        DualEnrolled
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

using GradeBook.Enums;

namespace GradeBook
{
    public class Student
    {
        public string Name { get; set; }
        public StudentType Type { get; set; }
        public EnrollmentType Enrollment { get; set; }
        public List<double> Grades { get; set; }
        [JsonIgnore]
        public double AverageGrade
        {
            get
            {
                return Grades.Average();
            }
        }
        [JsonIgnore]
        public char LetterGrade { get; set; }
        [JsonIgnore]
        public double GPA { get; set; }

        public Student(string name, StudentType studentType, EnrollmentType enrollment)
        {
            Name = name;
            Type = studentType;
            Enrollment = enrollment;
            Grades = new List<double>();
        }

        public void AddGrade(double grade)
        {
            if (grade < 0 || grade > 100)
                throw new ArgumentException("Grades must be between 0 and 100.");
            Grades.Add(grade);
        }

        public void RemoveGrade(double grade)
        {
            Grades.Remove(grade);
        }
    }
}

using GradeBook.GradeBooks;
using System;

namespace GradeBook.UserInterfaces
{
    public static class StartingUserInterface
    {
        public static bool Quit = false;
        public static void CommandLoop()
        {
            while (!Quit)
            {
                Console.WriteLine("What would you like to do?");
                var command = Console.ReadLine().ToLower();
                CommandRoute(command);
            }
        }

        public static void CommandRoute(string command)
        {
            if (command.StartsWith("create"))
                CreateCommand(command);
            else if (command.StartsWith("load"))
                LoadCommand(command);
            else if (command == "help")
                HelpCommand();
            else if (command == "quit")
                Quit = true;
            else
                Console.WriteLine("{0} was not recognized, please try again.", command);
        }

        public static void CreateCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Command not valid, Create requires a name, type of gradebook, if it's weighted (true / false).");
                return;
            }
            var name = parts[1];
            var type = parts[2].ToLower();
            var weighted = bool.Parse(parts[3]);
            BaseGradeBook gradeBook;

            if (type == "standard")
                gradeBook = new StandardGradeBook(name, weighted);
            else if (type == "ranked")
                gradeBook = new RankedGradeBook(name, weighted);
            else
            {
                System.Console.WriteLine("{0} is not a supported type of gradebook, please try again", type);
                return;
            }

            Console.WriteLine("Created gradebook {0}.", name);
            GradeBookUserInterface.CommandLoop(gradeBook);
        }

        public static void LoadCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Load requires a name.");
                return;
            }
            var name = parts[1];
            var gradeBook = BaseGradeBook.Load(name);

            if (gradeBook == null)
                return;

            GradeBookUserInterface.CommandLoop(gradeBook);
        }

        public static void HelpCommand()
        {
            Console.WriteLine("GradeBook accepts the following commands:");
            Console.WriteLine();
            Console.WriteLine("Create 'Name' 'Type' 'Weighted' - Creates a new gradebook where 'Name' is the name of the gradebook, 'Type' is what type of grading it should use, and 'Weighted' is whether or not grades should be weighted (true or false).");
            Console.WriteLine();
            Console.WriteLine("Load 'Name' - Loads the gradebook with the provided 'Name'.");
            Console.WriteLine();
            Console.WriteLine("Help - Displays all accepted commands.");
            Console.WriteLine();
            Console.WriteLine("Quit - Exits the application");
        }
    }
}


using GradeBook.Enums;
using GradeBook.GradeBooks;
using System;

namespace GradeBook.UserInterfaces
{
    public static class GradeBookUserInterface
    {
        public static BaseGradeBook GradeBook;
        public static bool Quit = false;
        public static void CommandLoop(BaseGradeBook gradeBook)
        {
            GradeBook = gradeBook;
            Quit = false;

            Console.WriteLine("#=======================#");
            Console.WriteLine(GradeBook.Name + " : " + GradeBook.GetType().Name);
            Console.WriteLine("#=======================#");
            Console.WriteLine(string.Empty);

            while(!Quit)
            {
                Console.WriteLine("What would you like to do?");
                var command = Console.ReadLine().ToLower();
                CommandRoute(command);
            }

            Console.WriteLine(GradeBook.Name + " has been closed.");
        }

        public static void CommandRoute(string command)
        {
            if (command == "save")
                SaveCommand();
            else if (command.StartsWith("addgrade"))
                AddGradeCommand(command);
            else if (command.StartsWith("removegrade"))
                RemoveGradeCommand(command);
            else if (command.StartsWith("add"))
                AddStudentCommand(command);
            else if (command.StartsWith("remove"))
                RemoveStudentCommand(command);
            else if (command == "list")
                ListCommand();
            else if (command == "statistics all")
                StatisticsCommand();
            else if (command.StartsWith("statistics"))
                StudentStatisticsCommand(command);
            else if (command == "help")
                HelpCommand();
            else if (command == "close")
                Quit = true;
            else
                Console.WriteLine("{0} was not recognized, please try again.", command);
        }

        public static void SaveCommand()
        {
            GradeBook.Save();
            Console.WriteLine("{0} has been saved.", GradeBook.Name);
        }
        
        public static void AddGradeCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Command not valid, AddGrade requires a name and score.");
                return;
            }
            var name = parts[1];
            var score = Double.Parse(parts[2]);
            GradeBook.AddGrade(name, score);
            Console.WriteLine("Added a score of {0} to {1}'s grades", score, name);
        }

        public static void RemoveGradeCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Command not valid, RemoveGrade requires a name and score.");
                return;
            }
            var name = parts[1];
            var score = Double.Parse(parts[2]);
            GradeBook.RemoveGrade(name, score);
            Console.WriteLine("Removed a score of {0} from {1}'s grades", score, name);
        }

        public static void AddStudentCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Command not valid, Add requires a name, student type, enrollment type.");
                return;
            }
            var name = parts[1];

            StudentType studentType;
            if (!Enum.TryParse(parts[2], true, out studentType))
            {
                Console.WriteLine("{0} is not a valid student type, try again.", parts[2]);
                return;
            }

            EnrollmentType enrollmentType;
            if (!Enum.TryParse(parts[3], true, out enrollmentType))
            {
                Console.WriteLine("{0} is not a volid enrollment type, try again.", parts[3]);
                return;
            }

            var student = new Student(name, studentType, enrollmentType);
            GradeBook.AddStudent(student);
            Console.WriteLine("Added {0} to the gradebook.", name);
        }
        
        public static void RemoveStudentCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Remove requires a name.");
                return;
            }
            var name = parts[1];
            GradeBook.RemoveStudent(name);
            Console.WriteLine("Removed {0} from the gradebook.", name);
        }

        public static void ListCommand()
        {
            GradeBook.ListStudents();
        }
        
        public static void StatisticsCommand()
        {
            GradeBook.CalculateStatistics();
        }

        public static void StudentStatisticsCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Requires Name or All.");
                return;
            }
            var name = parts[1];
            GradeBook.CalculateStudentStatistics(name);
        }

        public static void HelpCommand()
        {
            Console.WriteLine("While a gradebook is open you can use the following commands:");
            Console.WriteLine();
            Console.WriteLine("Add 'Name' 'Student Type' 'Enrollment Type' - Adds a new student to the gradebook with the provided name, type of student, and type of enrollment.");
            Console.WriteLine();
            Console.WriteLine("Accepted Student Types:");
            Console.WriteLine("Standard - Student not enrolled in Honors classes or Dual Enrolled.");
            Console.WriteLine("Honors - Students enrolled in Honors classes and not Dual Enrolled.");
            Console.WriteLine("DualEnrolled - Students who are Duel Enrolled.");
            Console.WriteLine();
            Console.WriteLine("Accepted Enrollement Types:");
            Console.WriteLine("Campus - Students who are in the same disctrict as the school.");
            Console.WriteLine("State - Students who's legal residence is outside the school's district, but is in the same state as the school.");
            Console.WriteLine("National - Students who's legal residence is not in the same state as the school, but is in the same country as the school.");
            Console.WriteLine("International - Students who's legal residence is not in the same country as the school.");
            Console.WriteLine();
            Console.WriteLine("List - Lists all students.");
            Console.WriteLine();
            Console.WriteLine("AddGrade 'Name' 'Score' - Adds a new grade to a student with the matching name of the provided score.");
            Console.WriteLine();
            Console.WriteLine("RemoveGrade 'Name' 'Score' - Removes a grade to a student with the matching name and score.");
            Console.WriteLine();
            Console.WriteLine("Remove 'Name' - Removes the student with the provided name.");
            Console.WriteLine();
            Console.WriteLine("Statistics 'Name' - Gets statistics for the specified student.");
            Console.WriteLine();
            Console.WriteLine("Statistics All - Gets general statistics for the entire gradebook.");
            Console.WriteLine();
            Console.WriteLine("Close - closes the gradebook and takes you back to the starting command options.");
            Console.WriteLine();
            Console.WriteLine("Save - saves the gradebook to the hard drive for later use.");
        }
    }
}

using System;
using System.Linq;

using GradeBook.Enums;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GradeBook.GradeBooks
{
    public abstract class BaseGradeBook
    {
        public string Name { get; set; }
        public List<Student> Students { get; set; }
        public GradeBookType Type { get; set; }
        public bool IsWeighted { get; set; }

        public BaseGradeBook(string name, bool isWeighted)
        {
            Name = name;
            Students = new List<Student>();
            IsWeighted = isWeighted;
        }

        public void AddStudent(Student student)
        {
            if (string.IsNullOrEmpty(student.Name))
                throw new ArgumentException("A Name is required to add a student to a gradebook.");
            Students.Add(student);
        }

        public void RemoveStudent(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to remove a student from a gradebook.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            Students.Remove(student);
        }

        public void AddGrade(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to add a grade to a student.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            student.AddGrade(score);
        }

        public void RemoveGrade(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to remove a grade from a student.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            student.RemoveGrade(score);
        }

        public void ListStudents()
        {
            foreach (var student in Students)
            {
                Console.WriteLine("{0} : {1} : {2}", student.Name, student.Type, student.Enrollment);
            }
        }

        public static BaseGradeBook Load(string name)
        {
            if (!File.Exists(name + ".gdbk"))
            {
                Console.WriteLine("Gradebook could not be found.");
                return null;
            }

            using (var file = new FileStream(name + ".gdbk", FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(file))
                {
                    var json = reader.ReadToEnd();
                    return ConvertToGradeBook(json);
                }
            }
        }

        public void Save()
        {
            using (var file = new FileStream(Name + ".gdbk", FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(file))
                {
                    var json = JsonConvert.SerializeObject(this);
                    writer.Write(json);
                }
            }
        }

        public virtual double GetGPA(char letterGrade, StudentType studentType)
        {
            var gpa = 0;
            
            switch (letterGrade)
            {
                case 'A':
                    gpa = 4;
                    break;
                case 'B':
                    gpa = 3;
                    break;
                case 'C':
                    gpa = 2;
                    break;
                case 'D':
                    gpa = 1;
                    break;
            }
            
            if (IsWeighted && (studentType == StudentType.Honors || studentType == StudentType.DualEnrolled))
                gpa++;
            
            return gpa;
        }

        public virtual void CalculateStatistics()
        {
            var allStudentsPoints = 0d;
            var campusPoints = 0d;
            var statePoints = 0d;
            var nationalPoints = 0d;
            var internationalPoints = 0d;
            var standardPoints = 0d;
            var honorPoints = 0d;
            var dualEnrolledPoints = 0d;

            foreach (var student in Students)
            {
                student.LetterGrade = GetLetterGrade(student.AverageGrade);
                student.GPA = GetGPA(student.LetterGrade, student.Type);

                Console.WriteLine("{0} ({1}:{2}) GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
                allStudentsPoints += student.AverageGrade;

                switch (student.Enrollment)
                {
                    case EnrollmentType.Campus:
                        campusPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.State:
                        statePoints += student.AverageGrade;
                        break;
                    case EnrollmentType.National:
                        nationalPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.International:
                        internationalPoints += student.AverageGrade;
                        break;
                }

                switch (student.Type)
                {
                    case StudentType.Standard:
                        standardPoints += student.AverageGrade;
                        break;
                    case StudentType.Honors:
                        honorPoints += student.AverageGrade;
                        break;
                    case StudentType.DualEnrolled:
                        dualEnrolledPoints += student.AverageGrade;
                        break;
                }
            }

            //#todo refactor into it's own method with calculations performed here
            Console.WriteLine("Average Grade of all students is " + (allStudentsPoints / Students.Count));
            if (campusPoints != 0)
                Console.WriteLine("Average for only local students is " + (campusPoints / Students.Where(e => e.Enrollment == EnrollmentType.Campus).Count()));
            if (statePoints != 0)
                Console.WriteLine("Average for only state students (excluding local) is " + (statePoints / Students.Where(e => e.Enrollment == EnrollmentType.State).Count()));
            if (nationalPoints != 0)
                Console.WriteLine("Average for only national students (excluding state and local) is " + (nationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.National).Count()));
            if (internationalPoints != 0)
                Console.WriteLine("Average for only international students is " + (internationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.International).Count()));
            if (standardPoints != 0)
                Console.WriteLine("Average for students excluding honors and duel enrollment is " + (standardPoints / Students.Where(e => e.Type == StudentType.Standard).Count()));
            if (honorPoints != 0)
                Console.WriteLine("Average for only honors students is " + (honorPoints / Students.Where(e => e.Type == StudentType.Honors).Count()));
            if (dualEnrolledPoints != 0)
                Console.WriteLine("Average for only duel enrolled students is " + (dualEnrolledPoints / Students.Where(e => e.Type == StudentType.DualEnrolled).Count()));
        }

        public virtual void CalculateStudentStatistics(string name)
        {
            var student = Students.FirstOrDefault(e => e.Name == name);
            student.LetterGrade = GetLetterGrade(student.AverageGrade);
            student.GPA = GetGPA(student.LetterGrade, student.Type);

            Console.WriteLine("{0} ({1}:{2}) GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
            Console.WriteLine();
            Console.WriteLine("Grades:");
            foreach (var grade in student.Grades)
            {
                Console.WriteLine(grade);
            }
        }

        public virtual char GetLetterGrade(double averageGrade)
        {
            if (averageGrade >= 90)
                return 'A';
            else if (averageGrade >= 80)
                return 'B';
            else if (averageGrade >= 70)
                return 'C';
            else if (averageGrade >= 60)
                return 'D';
            else
                return 'F';
        }

        /// <summary>
        ///     Converts json to the appropriate grade book type.
        ///     Note: This method contains code that is not recommended practice.
        ///     This has been used as a compromise to avoid adding additional complexity to the learner.
        /// </summary>
        /// <returns>The to grade book.</returns>
        /// <param name="json">Json.</param>
        public static dynamic ConvertToGradeBook(string json)
        {
            // Get GradeBookType from the GradeBook.Enums namespace
            var gradebookEnum = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 from type in assembly.GetTypes()
                                 where type.FullName == "GradeBook.Enums.GradeBookType"
                                 select type).FirstOrDefault();

            var jobject = JsonConvert.DeserializeObject<JObject>(json);
            var gradeBookType = jobject.Property("Type")?.Value?.ToString();

            // Check if StandardGradeBook exists
            if ((from assembly in AppDomain.CurrentDomain.GetAssemblies()
                 from type in assembly.GetTypes()
                 where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                 select type).FirstOrDefault() == null)
                gradeBookType = "Base";
            else
            {
                if (string.IsNullOrEmpty(gradeBookType))
                    gradeBookType = "Standard";
                else
                    gradeBookType = Enum.GetName(gradebookEnum, int.Parse(gradeBookType));
            }

            // Get GradeBook from the GradeBook.GradeBooks namespace
            var gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks." + gradeBookType + "GradeBook"
                             select type).FirstOrDefault();


            //protection code
            if (gradebook == null)
                gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                             select type).FirstOrDefault();
            
            return JsonConvert.DeserializeObject(json, gradebook);
        }
    }
}

using GradeBook.Enums;
using System;
using System.Linq;

namespace GradeBook.GradeBooks
{
  public class RankedGradeBook : BaseGradeBook
  {
    public RankedGradeBook(string name, bool isWeighted) : base(name, isWeighted)
    {
      Type = GradeBookType.Ranked;
    }

    public override void CalculateStudentStatistics(string name)
    {
      if (Students.Count < 5)
      {
        System.Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
        return;
      }
      base.CalculateStudentStatistics(name);
    }

    public override void CalculateStatistics()
    {
      if (Students.Count < 5)
      {
        System.Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
        return;
      }
      base.CalculateStatistics();
    }

    public override char GetLetterGrade(double averageGrade)
    {
      if (Students.Count < 5)
      {
        throw new InvalidOperationException("Ranked grading requires at least 5 students");
      }

      // Number of students that should receive each letter grade
      var threshold = (int)Math.Ceiling(Students.Count * 0.2);

      //                Order students by       their avg grade   and take their avg grade val  to a list
      var grades = Students.OrderByDescending(e => e.AverageGrade).Select(e => e.AverageGrade).ToList();
      
      if (grades[threshold - 1] <= averageGrade)
        return 'A';
      else if (grades[(threshold * 2) - 1] <= averageGrade)
        return 'B';
      else if (grades[(threshold * 3) - 1] <= averageGrade)
        return 'C';
      else if (grades[(threshold * 4) - 1] <= averageGrade)
        return 'D';
      else
        return 'F';
    }
  }
}

using GradeBook.Enums;

namespace GradeBook.GradeBooks
{
  public class StandardGradeBook : BaseGradeBook
  {

    public StandardGradeBook(string name, bool isWeighted) : base(name, isWeighted)
    {
      Type = GradeBookType.Standard;
    }
  }
}

/*
	CSharp-GradeBookApplication
	
	Description: Project to add features to an existing C Sharp Grade Book Application.
	Website: http:\\www.pluralsight.com
	         https://github.com/pmash2/CSharp-GradeBookApplication
*/


using System;
using GradeBook.UserInterfaces;

namespace GradeBook
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("#=======================#");
            Console.WriteLine("# Welcome to GradeBook! #");
            Console.WriteLine("#=======================#");
            Console.WriteLine();

            StartingUserInterface.CommandLoop();
            
            Console.WriteLine("Thank you for using GradeBook!");
            Console.WriteLine("Have a nice day!");
            Console.Read();
        }
    }
}

namespace GradeBook.Enums
{
    public enum EnrollmentType
    {
        Campus,
        State,
        National,
        International
    }
}

namespace GradeBook.Enums
{
  public enum GradeBookType
  {
    Standard,
    Ranked,
    ESNU,
    OneToFour,
    SixPoint
  }
}

namespace GradeBook.Enums
{
    public enum StudentType
    {
        Standard,
        Honors,
        DualEnrolled
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

using GradeBook.Enums;

namespace GradeBook
{
    public class Student
    {
        public string Name { get; set; }
        public StudentType Type { get; set; }
        public EnrollmentType Enrollment { get; set; }
        public List<double> Grades { get; set; }
        [JsonIgnore]
        public double AverageGrade
        {
            get
            {
                return Grades.Average();
            }
        }
        [JsonIgnore]
        public char LetterGrade { get; set; }
        [JsonIgnore]
        public double GPA { get; set; }

        public Student(string name, StudentType studentType, EnrollmentType enrollment)
        {
            Name = name;
            Type = studentType;
            Enrollment = enrollment;
            Grades = new List<double>();
        }

        public void AddGrade(double grade)
        {
            if (grade < 0 || grade > 100)
                throw new ArgumentException("Grades must be between 0 and 100.");
            Grades.Add(grade);
        }

        public void RemoveGrade(double grade)
        {
            Grades.Remove(grade);
        }
    }
}

using GradeBook.GradeBooks;
using System;

namespace GradeBook.UserInterfaces
{
    public static class StartingUserInterface
    {
        public static bool Quit = false;
        public static void CommandLoop()
        {
            while (!Quit)
            {
                Console.WriteLine("What would you like to do?");
                var command = Console.ReadLine().ToLower();
                CommandRoute(command);
            }
        }

        public static void CommandRoute(string command)
        {
            if (command.StartsWith("create"))
                CreateCommand(command);
            else if (command.StartsWith("load"))
                LoadCommand(command);
            else if (command == "help")
                HelpCommand();
            else if (command == "quit")
                Quit = true;
            else
                Console.WriteLine("{0} was not recognized, please try again.", command);
        }

        public static void CreateCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Command not valid, Create requires a name, type of gradebook, if it's weighted (true / false).");
                return;
            }
            var name = parts[1];
            var type = parts[2].ToLower();
            var weighted = bool.Parse(parts[3]);
            BaseGradeBook gradeBook;

            if (type == "standard")
                gradeBook = new StandardGradeBook(name, weighted);
            else if (type == "ranked")
                gradeBook = new RankedGradeBook(name, weighted);
            else
            {
                System.Console.WriteLine("{0} is not a supported type of gradebook, please try again", type);
                return;
            }

            Console.WriteLine("Created gradebook {0}.", name);
            GradeBookUserInterface.CommandLoop(gradeBook);
        }

        public static void LoadCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Load requires a name.");
                return;
            }
            var name = parts[1];
            var gradeBook = BaseGradeBook.Load(name);

            if (gradeBook == null)
                return;

            GradeBookUserInterface.CommandLoop(gradeBook);
        }

        public static void HelpCommand()
        {
            Console.WriteLine("GradeBook accepts the following commands:");
            Console.WriteLine();
            Console.WriteLine("Create 'Name' 'Type' 'Weighted' - Creates a new gradebook where 'Name' is the name of the gradebook, 'Type' is what type of grading it should use, and 'Weighted' is whether or not grades should be weighted (true or false).");
            Console.WriteLine();
            Console.WriteLine("Load 'Name' - Loads the gradebook with the provided 'Name'.");
            Console.WriteLine();
            Console.WriteLine("Help - Displays all accepted commands.");
            Console.WriteLine();
            Console.WriteLine("Quit - Exits the application");
        }
    }
}


using GradeBook.Enums;
using GradeBook.GradeBooks;
using System;

namespace GradeBook.UserInterfaces
{
    public static class GradeBookUserInterface
    {
        public static BaseGradeBook GradeBook;
        public static bool Quit = false;
        public static void CommandLoop(BaseGradeBook gradeBook)
        {
            GradeBook = gradeBook;
            Quit = false;

            Console.WriteLine("#=======================#");
            Console.WriteLine(GradeBook.Name + " : " + GradeBook.GetType().Name);
            Console.WriteLine("#=======================#");
            Console.WriteLine(string.Empty);

            while(!Quit)
            {
                Console.WriteLine("What would you like to do?");
                var command = Console.ReadLine().ToLower();
                CommandRoute(command);
            }

            Console.WriteLine(GradeBook.Name + " has been closed.");
        }

        public static void CommandRoute(string command)
        {
            if (command == "save")
                SaveCommand();
            else if (command.StartsWith("addgrade"))
                AddGradeCommand(command);
            else if (command.StartsWith("removegrade"))
                RemoveGradeCommand(command);
            else if (command.StartsWith("add"))
                AddStudentCommand(command);
            else if (command.StartsWith("remove"))
                RemoveStudentCommand(command);
            else if (command == "list")
                ListCommand();
            else if (command == "statistics all")
                StatisticsCommand();
            else if (command.StartsWith("statistics"))
                StudentStatisticsCommand(command);
            else if (command == "help")
                HelpCommand();
            else if (command == "close")
                Quit = true;
            else
                Console.WriteLine("{0} was not recognized, please try again.", command);
        }

        public static void SaveCommand()
        {
            GradeBook.Save();
            Console.WriteLine("{0} has been saved.", GradeBook.Name);
        }
        
        public static void AddGradeCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Command not valid, AddGrade requires a name and score.");
                return;
            }
            var name = parts[1];
            var score = Double.Parse(parts[2]);
            GradeBook.AddGrade(name, score);
            Console.WriteLine("Added a score of {0} to {1}'s grades", score, name);
        }

        public static void RemoveGradeCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Command not valid, RemoveGrade requires a name and score.");
                return;
            }
            var name = parts[1];
            var score = Double.Parse(parts[2]);
            GradeBook.RemoveGrade(name, score);
            Console.WriteLine("Removed a score of {0} from {1}'s grades", score, name);
        }

        public static void AddStudentCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Command not valid, Add requires a name, student type, enrollment type.");
                return;
            }
            var name = parts[1];

            StudentType studentType;
            if (!Enum.TryParse(parts[2], true, out studentType))
            {
                Console.WriteLine("{0} is not a valid student type, try again.", parts[2]);
                return;
            }

            EnrollmentType enrollmentType;
            if (!Enum.TryParse(parts[3], true, out enrollmentType))
            {
                Console.WriteLine("{0} is not a volid enrollment type, try again.", parts[3]);
                return;
            }

            var student = new Student(name, studentType, enrollmentType);
            GradeBook.AddStudent(student);
            Console.WriteLine("Added {0} to the gradebook.", name);
        }
        
        public static void RemoveStudentCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Remove requires a name.");
                return;
            }
            var name = parts[1];
            GradeBook.RemoveStudent(name);
            Console.WriteLine("Removed {0} from the gradebook.", name);
        }

        public static void ListCommand()
        {
            GradeBook.ListStudents();
        }
        
        public static void StatisticsCommand()
        {
            GradeBook.CalculateStatistics();
        }

        public static void StudentStatisticsCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Requires Name or All.");
                return;
            }
            var name = parts[1];
            GradeBook.CalculateStudentStatistics(name);
        }

        public static void HelpCommand()
        {
            Console.WriteLine("While a gradebook is open you can use the following commands:");
            Console.WriteLine();
            Console.WriteLine("Add 'Name' 'Student Type' 'Enrollment Type' - Adds a new student to the gradebook with the provided name, type of student, and type of enrollment.");
            Console.WriteLine();
            Console.WriteLine("Accepted Student Types:");
            Console.WriteLine("Standard - Student not enrolled in Honors classes or Dual Enrolled.");
            Console.WriteLine("Honors - Students enrolled in Honors classes and not Dual Enrolled.");
            Console.WriteLine("DualEnrolled - Students who are Duel Enrolled.");
            Console.WriteLine();
            Console.WriteLine("Accepted Enrollement Types:");
            Console.WriteLine("Campus - Students who are in the same disctrict as the school.");
            Console.WriteLine("State - Students who's legal residence is outside the school's district, but is in the same state as the school.");
            Console.WriteLine("National - Students who's legal residence is not in the same state as the school, but is in the same country as the school.");
            Console.WriteLine("International - Students who's legal residence is not in the same country as the school.");
            Console.WriteLine();
            Console.WriteLine("List - Lists all students.");
            Console.WriteLine();
            Console.WriteLine("AddGrade 'Name' 'Score' - Adds a new grade to a student with the matching name of the provided score.");
            Console.WriteLine();
            Console.WriteLine("RemoveGrade 'Name' 'Score' - Removes a grade to a student with the matching name and score.");
            Console.WriteLine();
            Console.WriteLine("Remove 'Name' - Removes the student with the provided name.");
            Console.WriteLine();
            Console.WriteLine("Statistics 'Name' - Gets statistics for the specified student.");
            Console.WriteLine();
            Console.WriteLine("Statistics All - Gets general statistics for the entire gradebook.");
            Console.WriteLine();
            Console.WriteLine("Close - closes the gradebook and takes you back to the starting command options.");
            Console.WriteLine();
            Console.WriteLine("Save - saves the gradebook to the hard drive for later use.");
        }
    }
}

using System;
using System.Linq;

using GradeBook.Enums;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GradeBook.GradeBooks
{
    public abstract class BaseGradeBook
    {
        public string Name { get; set; }
        public List<Student> Students { get; set; }
        public GradeBookType Type { get; set; }
        public bool IsWeighted { get; set; }

        public BaseGradeBook(string name, bool isWeighted)
        {
            Name = name;
            Students = new List<Student>();
            IsWeighted = isWeighted;
        }

        public void AddStudent(Student student)
        {
            if (string.IsNullOrEmpty(student.Name))
                throw new ArgumentException("A Name is required to add a student to a gradebook.");
            Students.Add(student);
        }

        public void RemoveStudent(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to remove a student from a gradebook.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            Students.Remove(student);
        }

        public void AddGrade(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to add a grade to a student.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            student.AddGrade(score);
        }

        public void RemoveGrade(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to remove a grade from a student.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            student.RemoveGrade(score);
        }

        public void ListStudents()
        {
            foreach (var student in Students)
            {
                Console.WriteLine("{0} : {1} : {2}", student.Name, student.Type, student.Enrollment);
            }
        }

        public static BaseGradeBook Load(string name)
        {
            if (!File.Exists(name + ".gdbk"))
            {
                Console.WriteLine("Gradebook could not be found.");
                return null;
            }

            using (var file = new FileStream(name + ".gdbk", FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(file))
                {
                    var json = reader.ReadToEnd();
                    return ConvertToGradeBook(json);
                }
            }
        }

        public void Save()
        {
            using (var file = new FileStream(Name + ".gdbk", FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(file))
                {
                    var json = JsonConvert.SerializeObject(this);
                    writer.Write(json);
                }
            }
        }

        public virtual double GetGPA(char letterGrade, StudentType studentType)
        {
            var gpa = 0;
            
            switch (letterGrade)
            {
                case 'A':
                    gpa = 4;
                    break;
                case 'B':
                    gpa = 3;
                    break;
                case 'C':
                    gpa = 2;
                    break;
                case 'D':
                    gpa = 1;
                    break;
            }
            
            if (IsWeighted && (studentType == StudentType.Honors || studentType == StudentType.DualEnrolled))
                gpa++;
            
            return gpa;
        }

        public virtual void CalculateStatistics()
        {
            var allStudentsPoints = 0d;
            var campusPoints = 0d;
            var statePoints = 0d;
            var nationalPoints = 0d;
            var internationalPoints = 0d;
            var standardPoints = 0d;
            var honorPoints = 0d;
            var dualEnrolledPoints = 0d;

            foreach (var student in Students)
            {
                student.LetterGrade = GetLetterGrade(student.AverageGrade);
                student.GPA = GetGPA(student.LetterGrade, student.Type);

                Console.WriteLine("{0} ({1}:{2}) GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
                allStudentsPoints += student.AverageGrade;

                switch (student.Enrollment)
                {
                    case EnrollmentType.Campus:
                        campusPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.State:
                        statePoints += student.AverageGrade;
                        break;
                    case EnrollmentType.National:
                        nationalPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.International:
                        internationalPoints += student.AverageGrade;
                        break;
                }

                switch (student.Type)
                {
                    case StudentType.Standard:
                        standardPoints += student.AverageGrade;
                        break;
                    case StudentType.Honors:
                        honorPoints += student.AverageGrade;
                        break;
                    case StudentType.DualEnrolled:
                        dualEnrolledPoints += student.AverageGrade;
                        break;
                }
            }

            //#todo refactor into it's own method with calculations performed here
            Console.WriteLine("Average Grade of all students is " + (allStudentsPoints / Students.Count));
            if (campusPoints != 0)
                Console.WriteLine("Average for only local students is " + (campusPoints / Students.Where(e => e.Enrollment == EnrollmentType.Campus).Count()));
            if (statePoints != 0)
                Console.WriteLine("Average for only state students (excluding local) is " + (statePoints / Students.Where(e => e.Enrollment == EnrollmentType.State).Count()));
            if (nationalPoints != 0)
                Console.WriteLine("Average for only national students (excluding state and local) is " + (nationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.National).Count()));
            if (internationalPoints != 0)
                Console.WriteLine("Average for only international students is " + (internationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.International).Count()));
            if (standardPoints != 0)
                Console.WriteLine("Average for students excluding honors and duel enrollment is " + (standardPoints / Students.Where(e => e.Type == StudentType.Standard).Count()));
            if (honorPoints != 0)
                Console.WriteLine("Average for only honors students is " + (honorPoints / Students.Where(e => e.Type == StudentType.Honors).Count()));
            if (dualEnrolledPoints != 0)
                Console.WriteLine("Average for only duel enrolled students is " + (dualEnrolledPoints / Students.Where(e => e.Type == StudentType.DualEnrolled).Count()));
        }

        public virtual void CalculateStudentStatistics(string name)
        {
            var student = Students.FirstOrDefault(e => e.Name == name);
            student.LetterGrade = GetLetterGrade(student.AverageGrade);
            student.GPA = GetGPA(student.LetterGrade, student.Type);

            Console.WriteLine("{0} ({1}:{2}) GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
            Console.WriteLine();
            Console.WriteLine("Grades:");
            foreach (var grade in student.Grades)
            {
                Console.WriteLine(grade);
            }
        }

        public virtual char GetLetterGrade(double averageGrade)
        {
            if (averageGrade >= 90)
                return 'A';
            else if (averageGrade >= 80)
                return 'B';
            else if (averageGrade >= 70)
                return 'C';
            else if (averageGrade >= 60)
                return 'D';
            else
                return 'F';
        }

        /// <summary>
        ///     Converts json to the appropriate grade book type.
        ///     Note: This method contains code that is not recommended practice.
        ///     This has been used as a compromise to avoid adding additional complexity to the learner.
        /// </summary>
        /// <returns>The to grade book.</returns>
        /// <param name="json">Json.</param>
        public static dynamic ConvertToGradeBook(string json)
        {
            // Get GradeBookType from the GradeBook.Enums namespace
            var gradebookEnum = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 from type in assembly.GetTypes()
                                 where type.FullName == "GradeBook.Enums.GradeBookType"
                                 select type).FirstOrDefault();

            var jobject = JsonConvert.DeserializeObject<JObject>(json);
            var gradeBookType = jobject.Property("Type")?.Value?.ToString();

            // Check if StandardGradeBook exists
            if ((from assembly in AppDomain.CurrentDomain.GetAssemblies()
                 from type in assembly.GetTypes()
                 where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                 select type).FirstOrDefault() == null)
                gradeBookType = "Base";
            else
            {
                if (string.IsNullOrEmpty(gradeBookType))
                    gradeBookType = "Standard";
                else
                    gradeBookType = Enum.GetName(gradebookEnum, int.Parse(gradeBookType));
            }

            // Get GradeBook from the GradeBook.GradeBooks namespace
            var gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks." + gradeBookType + "GradeBook"
                             select type).FirstOrDefault();


            //protection code
            if (gradebook == null)
                gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                             select type).FirstOrDefault();
            
            return JsonConvert.DeserializeObject(json, gradebook);
        }
    }
}

using GradeBook.Enums;
using System;
using System.Linq;

namespace GradeBook.GradeBooks
{
  public class RankedGradeBook : BaseGradeBook
  {
    public RankedGradeBook(string name, bool isWeighted) : base(name, isWeighted)
    {
      Type = GradeBookType.Ranked;
    }

    public override void CalculateStudentStatistics(string name)
    {
      if (Students.Count < 5)
      {
        System.Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
        return;
      }
      base.CalculateStudentStatistics(name);
    }

    public override void CalculateStatistics()
    {
      if (Students.Count < 5)
      {
        System.Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
        return;
      }
      base.CalculateStatistics();
    }

    public override char GetLetterGrade(double averageGrade)
    {
      if (Students.Count < 5)
      {
        throw new InvalidOperationException("Ranked grading requires at least 5 students");
      }

      // Number of students that should receive each letter grade
      var threshold = (int)Math.Ceiling(Students.Count * 0.2);

      //                Order students by       their avg grade   and take their avg grade val  to a list
      var grades = Students.OrderByDescending(e => e.AverageGrade).Select(e => e.AverageGrade).ToList();
      
      if (grades[threshold - 1] <= averageGrade)
        return 'A';
      else if (grades[(threshold * 2) - 1] <= averageGrade)
        return 'B';
      else if (grades[(threshold * 3) - 1] <= averageGrade)
        return 'C';
      else if (grades[(threshold * 4) - 1] <= averageGrade)
        return 'D';
      else
        return 'F';
    }
  }
}

using GradeBook.Enums;

namespace GradeBook.GradeBooks
{
  public class StandardGradeBook : BaseGradeBook
  {

    public StandardGradeBook(string name, bool isWeighted) : base(name, isWeighted)
    {
      Type = GradeBookType.Standard;
    }
  }
}

/*
	CSharp-GradeBookApplication
	
	Description: Project to add features to an existing C Sharp Grade Book Application.
	Website: http:\\www.pluralsight.com
	         https://github.com/pmash2/CSharp-GradeBookApplication
*/


using System;
using GradeBook.UserInterfaces;

namespace GradeBook
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("#=======================#");
            Console.WriteLine("# Welcome to GradeBook! #");
            Console.WriteLine("#=======================#");
            Console.WriteLine();

            StartingUserInterface.CommandLoop();
            
            Console.WriteLine("Thank you for using GradeBook!");
            Console.WriteLine("Have a nice day!");
            Console.Read();
        }
    }
}

namespace GradeBook.Enums
{
    public enum EnrollmentType
    {
        Campus,
        State,
        National,
        International
    }
}

namespace GradeBook.Enums
{
  public enum GradeBookType
  {
    Standard,
    Ranked,
    ESNU,
    OneToFour,
    SixPoint
  }
}

namespace GradeBook.Enums
{
    public enum StudentType
    {
        Standard,
        Honors,
        DualEnrolled
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

using GradeBook.Enums;

namespace GradeBook
{
    public class Student
    {
        public string Name { get; set; }
        public StudentType Type { get; set; }
        public EnrollmentType Enrollment { get; set; }
        public List<double> Grades { get; set; }
        [JsonIgnore]
        public double AverageGrade
        {
            get
            {
                return Grades.Average();
            }
        }
        [JsonIgnore]
        public char LetterGrade { get; set; }
        [JsonIgnore]
        public double GPA { get; set; }

        public Student(string name, StudentType studentType, EnrollmentType enrollment)
        {
            Name = name;
            Type = studentType;
            Enrollment = enrollment;
            Grades = new List<double>();
        }

        public void AddGrade(double grade)
        {
            if (grade < 0 || grade > 100)
                throw new ArgumentException("Grades must be between 0 and 100.");
            Grades.Add(grade);
        }

        public void RemoveGrade(double grade)
        {
            Grades.Remove(grade);
        }
    }
}

using GradeBook.GradeBooks;
using System;

namespace GradeBook.UserInterfaces
{
    public static class StartingUserInterface
    {
        public static bool Quit = false;
        public static void CommandLoop()
        {
            while (!Quit)
            {
                Console.WriteLine("What would you like to do?");
                var command = Console.ReadLine().ToLower();
                CommandRoute(command);
            }
        }

        public static void CommandRoute(string command)
        {
            if (command.StartsWith("create"))
                CreateCommand(command);
            else if (command.StartsWith("load"))
                LoadCommand(command);
            else if (command == "help")
                HelpCommand();
            else if (command == "quit")
                Quit = true;
            else
                Console.WriteLine("{0} was not recognized, please try again.", command);
        }

        public static void CreateCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Command not valid, Create requires a name, type of gradebook, if it's weighted (true / false).");
                return;
            }
            var name = parts[1];
            var type = parts[2].ToLower();
            var weighted = bool.Parse(parts[3]);
            BaseGradeBook gradeBook;

            if (type == "standard")
                gradeBook = new StandardGradeBook(name, weighted);
            else if (type == "ranked")
                gradeBook = new RankedGradeBook(name, weighted);
            else
            {
                System.Console.WriteLine("{0} is not a supported type of gradebook, please try again", type);
                return;
            }

            Console.WriteLine("Created gradebook {0}.", name);
            GradeBookUserInterface.CommandLoop(gradeBook);
        }

        public static void LoadCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Load requires a name.");
                return;
            }
            var name = parts[1];
            var gradeBook = BaseGradeBook.Load(name);

            if (gradeBook == null)
                return;

            GradeBookUserInterface.CommandLoop(gradeBook);
        }

        public static void HelpCommand()
        {
            Console.WriteLine("GradeBook accepts the following commands:");
            Console.WriteLine();
            Console.WriteLine("Create 'Name' 'Type' 'Weighted' - Creates a new gradebook where 'Name' is the name of the gradebook, 'Type' is what type of grading it should use, and 'Weighted' is whether or not grades should be weighted (true or false).");
            Console.WriteLine();
            Console.WriteLine("Load 'Name' - Loads the gradebook with the provided 'Name'.");
            Console.WriteLine();
            Console.WriteLine("Help - Displays all accepted commands.");
            Console.WriteLine();
            Console.WriteLine("Quit - Exits the application");
        }
    }
}


using GradeBook.Enums;
using GradeBook.GradeBooks;
using System;

namespace GradeBook.UserInterfaces
{
    public static class GradeBookUserInterface
    {
        public static BaseGradeBook GradeBook;
        public static bool Quit = false;
        public static void CommandLoop(BaseGradeBook gradeBook)
        {
            GradeBook = gradeBook;
            Quit = false;

            Console.WriteLine("#=======================#");
            Console.WriteLine(GradeBook.Name + " : " + GradeBook.GetType().Name);
            Console.WriteLine("#=======================#");
            Console.WriteLine(string.Empty);

            while(!Quit)
            {
                Console.WriteLine("What would you like to do?");
                var command = Console.ReadLine().ToLower();
                CommandRoute(command);
            }

            Console.WriteLine(GradeBook.Name + " has been closed.");
        }

        public static void CommandRoute(string command)
        {
            if (command == "save")
                SaveCommand();
            else if (command.StartsWith("addgrade"))
                AddGradeCommand(command);
            else if (command.StartsWith("removegrade"))
                RemoveGradeCommand(command);
            else if (command.StartsWith("add"))
                AddStudentCommand(command);
            else if (command.StartsWith("remove"))
                RemoveStudentCommand(command);
            else if (command == "list")
                ListCommand();
            else if (command == "statistics all")
                StatisticsCommand();
            else if (command.StartsWith("statistics"))
                StudentStatisticsCommand(command);
            else if (command == "help")
                HelpCommand();
            else if (command == "close")
                Quit = true;
            else
                Console.WriteLine("{0} was not recognized, please try again.", command);
        }

        public static void SaveCommand()
        {
            GradeBook.Save();
            Console.WriteLine("{0} has been saved.", GradeBook.Name);
        }
        
        public static void AddGradeCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Command not valid, AddGrade requires a name and score.");
                return;
            }
            var name = parts[1];
            var score = Double.Parse(parts[2]);
            GradeBook.AddGrade(name, score);
            Console.WriteLine("Added a score of {0} to {1}'s grades", score, name);
        }

        public static void RemoveGradeCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Command not valid, RemoveGrade requires a name and score.");
                return;
            }
            var name = parts[1];
            var score = Double.Parse(parts[2]);
            GradeBook.RemoveGrade(name, score);
            Console.WriteLine("Removed a score of {0} from {1}'s grades", score, name);
        }

        public static void AddStudentCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Command not valid, Add requires a name, student type, enrollment type.");
                return;
            }
            var name = parts[1];

            StudentType studentType;
            if (!Enum.TryParse(parts[2], true, out studentType))
            {
                Console.WriteLine("{0} is not a valid student type, try again.", parts[2]);
                return;
            }

            EnrollmentType enrollmentType;
            if (!Enum.TryParse(parts[3], true, out enrollmentType))
            {
                Console.WriteLine("{0} is not a volid enrollment type, try again.", parts[3]);
                return;
            }

            var student = new Student(name, studentType, enrollmentType);
            GradeBook.AddStudent(student);
            Console.WriteLine("Added {0} to the gradebook.", name);
        }
        
        public static void RemoveStudentCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Remove requires a name.");
                return;
            }
            var name = parts[1];
            GradeBook.RemoveStudent(name);
            Console.WriteLine("Removed {0} from the gradebook.", name);
        }

        public static void ListCommand()
        {
            GradeBook.ListStudents();
        }
        
        public static void StatisticsCommand()
        {
            GradeBook.CalculateStatistics();
        }

        public static void StudentStatisticsCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Requires Name or All.");
                return;
            }
            var name = parts[1];
            GradeBook.CalculateStudentStatistics(name);
        }

        public static void HelpCommand()
        {
            Console.WriteLine("While a gradebook is open you can use the following commands:");
            Console.WriteLine();
            Console.WriteLine("Add 'Name' 'Student Type' 'Enrollment Type' - Adds a new student to the gradebook with the provided name, type of student, and type of enrollment.");
            Console.WriteLine();
            Console.WriteLine("Accepted Student Types:");
            Console.WriteLine("Standard - Student not enrolled in Honors classes or Dual Enrolled.");
            Console.WriteLine("Honors - Students enrolled in Honors classes and not Dual Enrolled.");
            Console.WriteLine("DualEnrolled - Students who are Duel Enrolled.");
            Console.WriteLine();
            Console.WriteLine("Accepted Enrollement Types:");
            Console.WriteLine("Campus - Students who are in the same disctrict as the school.");
            Console.WriteLine("State - Students who's legal residence is outside the school's district, but is in the same state as the school.");
            Console.WriteLine("National - Students who's legal residence is not in the same state as the school, but is in the same country as the school.");
            Console.WriteLine("International - Students who's legal residence is not in the same country as the school.");
            Console.WriteLine();
            Console.WriteLine("List - Lists all students.");
            Console.WriteLine();
            Console.WriteLine("AddGrade 'Name' 'Score' - Adds a new grade to a student with the matching name of the provided score.");
            Console.WriteLine();
            Console.WriteLine("RemoveGrade 'Name' 'Score' - Removes a grade to a student with the matching name and score.");
            Console.WriteLine();
            Console.WriteLine("Remove 'Name' - Removes the student with the provided name.");
            Console.WriteLine();
            Console.WriteLine("Statistics 'Name' - Gets statistics for the specified student.");
            Console.WriteLine();
            Console.WriteLine("Statistics All - Gets general statistics for the entire gradebook.");
            Console.WriteLine();
            Console.WriteLine("Close - closes the gradebook and takes you back to the starting command options.");
            Console.WriteLine();
            Console.WriteLine("Save - saves the gradebook to the hard drive for later use.");
        }
    }
}

using System;
using System.Linq;

using GradeBook.Enums;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GradeBook.GradeBooks
{
    public abstract class BaseGradeBook
    {
        public string Name { get; set; }
        public List<Student> Students { get; set; }
        public GradeBookType Type { get; set; }
        public bool IsWeighted { get; set; }

        public BaseGradeBook(string name, bool isWeighted)
        {
            Name = name;
            Students = new List<Student>();
            IsWeighted = isWeighted;
        }

        public void AddStudent(Student student)
        {
            if (string.IsNullOrEmpty(student.Name))
                throw new ArgumentException("A Name is required to add a student to a gradebook.");
            Students.Add(student);
        }

        public void RemoveStudent(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to remove a student from a gradebook.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            Students.Remove(student);
        }

        public void AddGrade(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to add a grade to a student.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            student.AddGrade(score);
        }

        public void RemoveGrade(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to remove a grade from a student.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            student.RemoveGrade(score);
        }

        public void ListStudents()
        {
            foreach (var student in Students)
            {
                Console.WriteLine("{0} : {1} : {2}", student.Name, student.Type, student.Enrollment);
            }
        }

        public static BaseGradeBook Load(string name)
        {
            if (!File.Exists(name + ".gdbk"))
            {
                Console.WriteLine("Gradebook could not be found.");
                return null;
            }

            using (var file = new FileStream(name + ".gdbk", FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(file))
                {
                    var json = reader.ReadToEnd();
                    return ConvertToGradeBook(json);
                }
            }
        }

        public void Save()
        {
            using (var file = new FileStream(Name + ".gdbk", FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(file))
                {
                    var json = JsonConvert.SerializeObject(this);
                    writer.Write(json);
                }
            }
        }

        public virtual double GetGPA(char letterGrade, StudentType studentType)
        {
            var gpa = 0;
            
            switch (letterGrade)
            {
                case 'A':
                    gpa = 4;
                    break;
                case 'B':
                    gpa = 3;
                    break;
                case 'C':
                    gpa = 2;
                    break;
                case 'D':
                    gpa = 1;
                    break;
            }
            
            if (IsWeighted && (studentType == StudentType.Honors || studentType == StudentType.DualEnrolled))
                gpa++;
            
            return gpa;
        }

        public virtual void CalculateStatistics()
        {
            var allStudentsPoints = 0d;
            var campusPoints = 0d;
            var statePoints = 0d;
            var nationalPoints = 0d;
            var internationalPoints = 0d;
            var standardPoints = 0d;
            var honorPoints = 0d;
            var dualEnrolledPoints = 0d;

            foreach (var student in Students)
            {
                student.LetterGrade = GetLetterGrade(student.AverageGrade);
                student.GPA = GetGPA(student.LetterGrade, student.Type);

                Console.WriteLine("{0} ({1}:{2}) GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
                allStudentsPoints += student.AverageGrade;

                switch (student.Enrollment)
                {
                    case EnrollmentType.Campus:
                        campusPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.State:
                        statePoints += student.AverageGrade;
                        break;
                    case EnrollmentType.National:
                        nationalPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.International:
                        internationalPoints += student.AverageGrade;
                        break;
                }

                switch (student.Type)
                {
                    case StudentType.Standard:
                        standardPoints += student.AverageGrade;
                        break;
                    case StudentType.Honors:
                        honorPoints += student.AverageGrade;
                        break;
                    case StudentType.DualEnrolled:
                        dualEnrolledPoints += student.AverageGrade;
                        break;
                }
            }

            //#todo refactor into it's own method with calculations performed here
            Console.WriteLine("Average Grade of all students is " + (allStudentsPoints / Students.Count));
            if (campusPoints != 0)
                Console.WriteLine("Average for only local students is " + (campusPoints / Students.Where(e => e.Enrollment == EnrollmentType.Campus).Count()));
            if (statePoints != 0)
                Console.WriteLine("Average for only state students (excluding local) is " + (statePoints / Students.Where(e => e.Enrollment == EnrollmentType.State).Count()));
            if (nationalPoints != 0)
                Console.WriteLine("Average for only national students (excluding state and local) is " + (nationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.National).Count()));
            if (internationalPoints != 0)
                Console.WriteLine("Average for only international students is " + (internationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.International).Count()));
            if (standardPoints != 0)
                Console.WriteLine("Average for students excluding honors and duel enrollment is " + (standardPoints / Students.Where(e => e.Type == StudentType.Standard).Count()));
            if (honorPoints != 0)
                Console.WriteLine("Average for only honors students is " + (honorPoints / Students.Where(e => e.Type == StudentType.Honors).Count()));
            if (dualEnrolledPoints != 0)
                Console.WriteLine("Average for only duel enrolled students is " + (dualEnrolledPoints / Students.Where(e => e.Type == StudentType.DualEnrolled).Count()));
        }

        public virtual void CalculateStudentStatistics(string name)
        {
            var student = Students.FirstOrDefault(e => e.Name == name);
            student.LetterGrade = GetLetterGrade(student.AverageGrade);
            student.GPA = GetGPA(student.LetterGrade, student.Type);

            Console.WriteLine("{0} ({1}:{2}) GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
            Console.WriteLine();
            Console.WriteLine("Grades:");
            foreach (var grade in student.Grades)
            {
                Console.WriteLine(grade);
            }
        }

        public virtual char GetLetterGrade(double averageGrade)
        {
            if (averageGrade >= 90)
                return 'A';
            else if (averageGrade >= 80)
                return 'B';
            else if (averageGrade >= 70)
                return 'C';
            else if (averageGrade >= 60)
                return 'D';
            else
                return 'F';
        }

        /// <summary>
        ///     Converts json to the appropriate grade book type.
        ///     Note: This method contains code that is not recommended practice.
        ///     This has been used as a compromise to avoid adding additional complexity to the learner.
        /// </summary>
        /// <returns>The to grade book.</returns>
        /// <param name="json">Json.</param>
        public static dynamic ConvertToGradeBook(string json)
        {
            // Get GradeBookType from the GradeBook.Enums namespace
            var gradebookEnum = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 from type in assembly.GetTypes()
                                 where type.FullName == "GradeBook.Enums.GradeBookType"
                                 select type).FirstOrDefault();

            var jobject = JsonConvert.DeserializeObject<JObject>(json);
            var gradeBookType = jobject.Property("Type")?.Value?.ToString();

            // Check if StandardGradeBook exists
            if ((from assembly in AppDomain.CurrentDomain.GetAssemblies()
                 from type in assembly.GetTypes()
                 where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                 select type).FirstOrDefault() == null)
                gradeBookType = "Base";
            else
            {
                if (string.IsNullOrEmpty(gradeBookType))
                    gradeBookType = "Standard";
                else
                    gradeBookType = Enum.GetName(gradebookEnum, int.Parse(gradeBookType));
            }

            // Get GradeBook from the GradeBook.GradeBooks namespace
            var gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks." + gradeBookType + "GradeBook"
                             select type).FirstOrDefault();


            //protection code
            if (gradebook == null)
                gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                             select type).FirstOrDefault();
            
            return JsonConvert.DeserializeObject(json, gradebook);
        }
    }
}

using GradeBook.Enums;
using System;
using System.Linq;

namespace GradeBook.GradeBooks
{
  public class RankedGradeBook : BaseGradeBook
  {
    public RankedGradeBook(string name, bool isWeighted) : base(name, isWeighted)
    {
      Type = GradeBookType.Ranked;
    }

    public override void CalculateStudentStatistics(string name)
    {
      if (Students.Count < 5)
      {
        System.Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
        return;
      }
      base.CalculateStudentStatistics(name);
    }

    public override void CalculateStatistics()
    {
      if (Students.Count < 5)
      {
        System.Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
        return;
      }
      base.CalculateStatistics();
    }

    public override char GetLetterGrade(double averageGrade)
    {
      if (Students.Count < 5)
      {
        throw new InvalidOperationException("Ranked grading requires at least 5 students");
      }

      // Number of students that should receive each letter grade
      var threshold = (int)Math.Ceiling(Students.Count * 0.2);

      //                Order students by       their avg grade   and take their avg grade val  to a list
      var grades = Students.OrderByDescending(e => e.AverageGrade).Select(e => e.AverageGrade).ToList();
      
      if (grades[threshold - 1] <= averageGrade)
        return 'A';
      else if (grades[(threshold * 2) - 1] <= averageGrade)
        return 'B';
      else if (grades[(threshold * 3) - 1] <= averageGrade)
        return 'C';
      else if (grades[(threshold * 4) - 1] <= averageGrade)
        return 'D';
      else
        return 'F';
    }
  }
}

using GradeBook.Enums;

namespace GradeBook.GradeBooks
{
  public class StandardGradeBook : BaseGradeBook
  {

    public StandardGradeBook(string name, bool isWeighted) : base(name, isWeighted)
    {
      Type = GradeBookType.Standard;
    }
  }
}

/*
	CSharp-GradeBookApplication
	
	Description: Project to add features to an existing C Sharp Grade Book Application.
	Website: http:\\www.pluralsight.com
	         https://github.com/pmash2/CSharp-GradeBookApplication
*/


using System;
using GradeBook.UserInterfaces;

namespace GradeBook
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("#=======================#");
            Console.WriteLine("# Welcome to GradeBook! #");
            Console.WriteLine("#=======================#");
            Console.WriteLine();

            StartingUserInterface.CommandLoop();
            
            Console.WriteLine("Thank you for using GradeBook!");
            Console.WriteLine("Have a nice day!");
            Console.Read();
        }
    }
}

namespace GradeBook.Enums
{
    public enum EnrollmentType
    {
        Campus,
        State,
        National,
        International
    }
}

namespace GradeBook.Enums
{
  public enum GradeBookType
  {
    Standard,
    Ranked,
    ESNU,
    OneToFour,
    SixPoint
  }
}

namespace GradeBook.Enums
{
    public enum StudentType
    {
        Standard,
        Honors,
        DualEnrolled
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

using GradeBook.Enums;

namespace GradeBook
{
    public class Student
    {
        public string Name { get; set; }
        public StudentType Type { get; set; }
        public EnrollmentType Enrollment { get; set; }
        public List<double> Grades { get; set; }
        [JsonIgnore]
        public double AverageGrade
        {
            get
            {
                return Grades.Average();
            }
        }
        [JsonIgnore]
        public char LetterGrade { get; set; }
        [JsonIgnore]
        public double GPA { get; set; }

        public Student(string name, StudentType studentType, EnrollmentType enrollment)
        {
            Name = name;
            Type = studentType;
            Enrollment = enrollment;
            Grades = new List<double>();
        }

        public void AddGrade(double grade)
        {
            if (grade < 0 || grade > 100)
                throw new ArgumentException("Grades must be between 0 and 100.");
            Grades.Add(grade);
        }

        public void RemoveGrade(double grade)
        {
            Grades.Remove(grade);
        }
    }
}

using GradeBook.GradeBooks;
using System;

namespace GradeBook.UserInterfaces
{
    public static class StartingUserInterface
    {
        public static bool Quit = false;
        public static void CommandLoop()
        {
            while (!Quit)
            {
                Console.WriteLine("What would you like to do?");
                var command = Console.ReadLine().ToLower();
                CommandRoute(command);
            }
        }

        public static void CommandRoute(string command)
        {
            if (command.StartsWith("create"))
                CreateCommand(command);
            else if (command.StartsWith("load"))
                LoadCommand(command);
            else if (command == "help")
                HelpCommand();
            else if (command == "quit")
                Quit = true;
            else
                Console.WriteLine("{0} was not recognized, please try again.", command);
        }

        public static void CreateCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Command not valid, Create requires a name, type of gradebook, if it's weighted (true / false).");
                return;
            }
            var name = parts[1];
            var type = parts[2].ToLower();
            var weighted = bool.Parse(parts[3]);
            BaseGradeBook gradeBook;

            if (type == "standard")
                gradeBook = new StandardGradeBook(name, weighted);
            else if (type == "ranked")
                gradeBook = new RankedGradeBook(name, weighted);
            else
            {
                System.Console.WriteLine("{0} is not a supported type of gradebook, please try again", type);
                return;
            }

            Console.WriteLine("Created gradebook {0}.", name);
            GradeBookUserInterface.CommandLoop(gradeBook);
        }

        public static void LoadCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Load requires a name.");
                return;
            }
            var name = parts[1];
            var gradeBook = BaseGradeBook.Load(name);

            if (gradeBook == null)
                return;

            GradeBookUserInterface.CommandLoop(gradeBook);
        }

        public static void HelpCommand()
        {
            Console.WriteLine("GradeBook accepts the following commands:");
            Console.WriteLine();
            Console.WriteLine("Create 'Name' 'Type' 'Weighted' - Creates a new gradebook where 'Name' is the name of the gradebook, 'Type' is what type of grading it should use, and 'Weighted' is whether or not grades should be weighted (true or false).");
            Console.WriteLine();
            Console.WriteLine("Load 'Name' - Loads the gradebook with the provided 'Name'.");
            Console.WriteLine();
            Console.WriteLine("Help - Displays all accepted commands.");
            Console.WriteLine();
            Console.WriteLine("Quit - Exits the application");
        }
    }
}


using GradeBook.Enums;
using GradeBook.GradeBooks;
using System;

namespace GradeBook.UserInterfaces
{
    public static class GradeBookUserInterface
    {
        public static BaseGradeBook GradeBook;
        public static bool Quit = false;
        public static void CommandLoop(BaseGradeBook gradeBook)
        {
            GradeBook = gradeBook;
            Quit = false;

            Console.WriteLine("#=======================#");
            Console.WriteLine(GradeBook.Name + " : " + GradeBook.GetType().Name);
            Console.WriteLine("#=======================#");
            Console.WriteLine(string.Empty);

            while(!Quit)
            {
                Console.WriteLine("What would you like to do?");
                var command = Console.ReadLine().ToLower();
                CommandRoute(command);
            }

            Console.WriteLine(GradeBook.Name + " has been closed.");
        }

        public static void CommandRoute(string command)
        {
            if (command == "save")
                SaveCommand();
            else if (command.StartsWith("addgrade"))
                AddGradeCommand(command);
            else if (command.StartsWith("removegrade"))
                RemoveGradeCommand(command);
            else if (command.StartsWith("add"))
                AddStudentCommand(command);
            else if (command.StartsWith("remove"))
                RemoveStudentCommand(command);
            else if (command == "list")
                ListCommand();
            else if (command == "statistics all")
                StatisticsCommand();
            else if (command.StartsWith("statistics"))
                StudentStatisticsCommand(command);
            else if (command == "help")
                HelpCommand();
            else if (command == "close")
                Quit = true;
            else
                Console.WriteLine("{0} was not recognized, please try again.", command);
        }

        public static void SaveCommand()
        {
            GradeBook.Save();
            Console.WriteLine("{0} has been saved.", GradeBook.Name);
        }
        
        public static void AddGradeCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Command not valid, AddGrade requires a name and score.");
                return;
            }
            var name = parts[1];
            var score = Double.Parse(parts[2]);
            GradeBook.AddGrade(name, score);
            Console.WriteLine("Added a score of {0} to {1}'s grades", score, name);
        }

        public static void RemoveGradeCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Command not valid, RemoveGrade requires a name and score.");
                return;
            }
            var name = parts[1];
            var score = Double.Parse(parts[2]);
            GradeBook.RemoveGrade(name, score);
            Console.WriteLine("Removed a score of {0} from {1}'s grades", score, name);
        }

        public static void AddStudentCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Command not valid, Add requires a name, student type, enrollment type.");
                return;
            }
            var name = parts[1];

            StudentType studentType;
            if (!Enum.TryParse(parts[2], true, out studentType))
            {
                Console.WriteLine("{0} is not a valid student type, try again.", parts[2]);
                return;
            }

            EnrollmentType enrollmentType;
            if (!Enum.TryParse(parts[3], true, out enrollmentType))
            {
                Console.WriteLine("{0} is not a volid enrollment type, try again.", parts[3]);
                return;
            }

            var student = new Student(name, studentType, enrollmentType);
            GradeBook.AddStudent(student);
            Console.WriteLine("Added {0} to the gradebook.", name);
        }
        
        public static void RemoveStudentCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Remove requires a name.");
                return;
            }
            var name = parts[1];
            GradeBook.RemoveStudent(name);
            Console.WriteLine("Removed {0} from the gradebook.", name);
        }

        public static void ListCommand()
        {
            GradeBook.ListStudents();
        }
        
        public static void StatisticsCommand()
        {
            GradeBook.CalculateStatistics();
        }

        public static void StudentStatisticsCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Requires Name or All.");
                return;
            }
            var name = parts[1];
            GradeBook.CalculateStudentStatistics(name);
        }

        public static void HelpCommand()
        {
            Console.WriteLine("While a gradebook is open you can use the following commands:");
            Console.WriteLine();
            Console.WriteLine("Add 'Name' 'Student Type' 'Enrollment Type' - Adds a new student to the gradebook with the provided name, type of student, and type of enrollment.");
            Console.WriteLine();
            Console.WriteLine("Accepted Student Types:");
            Console.WriteLine("Standard - Student not enrolled in Honors classes or Dual Enrolled.");
            Console.WriteLine("Honors - Students enrolled in Honors classes and not Dual Enrolled.");
            Console.WriteLine("DualEnrolled - Students who are Duel Enrolled.");
            Console.WriteLine();
            Console.WriteLine("Accepted Enrollement Types:");
            Console.WriteLine("Campus - Students who are in the same disctrict as the school.");
            Console.WriteLine("State - Students who's legal residence is outside the school's district, but is in the same state as the school.");
            Console.WriteLine("National - Students who's legal residence is not in the same state as the school, but is in the same country as the school.");
            Console.WriteLine("International - Students who's legal residence is not in the same country as the school.");
            Console.WriteLine();
            Console.WriteLine("List - Lists all students.");
            Console.WriteLine();
            Console.WriteLine("AddGrade 'Name' 'Score' - Adds a new grade to a student with the matching name of the provided score.");
            Console.WriteLine();
            Console.WriteLine("RemoveGrade 'Name' 'Score' - Removes a grade to a student with the matching name and score.");
            Console.WriteLine();
            Console.WriteLine("Remove 'Name' - Removes the student with the provided name.");
            Console.WriteLine();
            Console.WriteLine("Statistics 'Name' - Gets statistics for the specified student.");
            Console.WriteLine();
            Console.WriteLine("Statistics All - Gets general statistics for the entire gradebook.");
            Console.WriteLine();
            Console.WriteLine("Close - closes the gradebook and takes you back to the starting command options.");
            Console.WriteLine();
            Console.WriteLine("Save - saves the gradebook to the hard drive for later use.");
        }
    }
}

using System;
using System.Linq;

using GradeBook.Enums;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GradeBook.GradeBooks
{
    public abstract class BaseGradeBook
    {
        public string Name { get; set; }
        public List<Student> Students { get; set; }
        public GradeBookType Type { get; set; }
        public bool IsWeighted { get; set; }

        public BaseGradeBook(string name, bool isWeighted)
        {
            Name = name;
            Students = new List<Student>();
            IsWeighted = isWeighted;
        }

        public void AddStudent(Student student)
        {
            if (string.IsNullOrEmpty(student.Name))
                throw new ArgumentException("A Name is required to add a student to a gradebook.");
            Students.Add(student);
        }

        public void RemoveStudent(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to remove a student from a gradebook.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            Students.Remove(student);
        }

        public void AddGrade(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to add a grade to a student.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            student.AddGrade(score);
        }

        public void RemoveGrade(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to remove a grade from a student.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            student.RemoveGrade(score);
        }

        public void ListStudents()
        {
            foreach (var student in Students)
            {
                Console.WriteLine("{0} : {1} : {2}", student.Name, student.Type, student.Enrollment);
            }
        }

        public static BaseGradeBook Load(string name)
        {
            if (!File.Exists(name + ".gdbk"))
            {
                Console.WriteLine("Gradebook could not be found.");
                return null;
            }

            using (var file = new FileStream(name + ".gdbk", FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(file))
                {
                    var json = reader.ReadToEnd();
                    return ConvertToGradeBook(json);
                }
            }
        }

        public void Save()
        {
            using (var file = new FileStream(Name + ".gdbk", FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(file))
                {
                    var json = JsonConvert.SerializeObject(this);
                    writer.Write(json);
                }
            }
        }

        public virtual double GetGPA(char letterGrade, StudentType studentType)
        {
            var gpa = 0;
            
            switch (letterGrade)
            {
                case 'A':
                    gpa = 4;
                    break;
                case 'B':
                    gpa = 3;
                    break;
                case 'C':
                    gpa = 2;
                    break;
                case 'D':
                    gpa = 1;
                    break;
            }
            
            if (IsWeighted && (studentType == StudentType.Honors || studentType == StudentType.DualEnrolled))
                gpa++;
            
            return gpa;
        }

        public virtual void CalculateStatistics()
        {
            var allStudentsPoints = 0d;
            var campusPoints = 0d;
            var statePoints = 0d;
            var nationalPoints = 0d;
            var internationalPoints = 0d;
            var standardPoints = 0d;
            var honorPoints = 0d;
            var dualEnrolledPoints = 0d;

            foreach (var student in Students)
            {
                student.LetterGrade = GetLetterGrade(student.AverageGrade);
                student.GPA = GetGPA(student.LetterGrade, student.Type);

                Console.WriteLine("{0} ({1}:{2}) GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
                allStudentsPoints += student.AverageGrade;

                switch (student.Enrollment)
                {
                    case EnrollmentType.Campus:
                        campusPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.State:
                        statePoints += student.AverageGrade;
                        break;
                    case EnrollmentType.National:
                        nationalPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.International:
                        internationalPoints += student.AverageGrade;
                        break;
                }

                switch (student.Type)
                {
                    case StudentType.Standard:
                        standardPoints += student.AverageGrade;
                        break;
                    case StudentType.Honors:
                        honorPoints += student.AverageGrade;
                        break;
                    case StudentType.DualEnrolled:
                        dualEnrolledPoints += student.AverageGrade;
                        break;
                }
            }

            //#todo refactor into it's own method with calculations performed here
            Console.WriteLine("Average Grade of all students is " + (allStudentsPoints / Students.Count));
            if (campusPoints != 0)
                Console.WriteLine("Average for only local students is " + (campusPoints / Students.Where(e => e.Enrollment == EnrollmentType.Campus).Count()));
            if (statePoints != 0)
                Console.WriteLine("Average for only state students (excluding local) is " + (statePoints / Students.Where(e => e.Enrollment == EnrollmentType.State).Count()));
            if (nationalPoints != 0)
                Console.WriteLine("Average for only national students (excluding state and local) is " + (nationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.National).Count()));
            if (internationalPoints != 0)
                Console.WriteLine("Average for only international students is " + (internationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.International).Count()));
            if (standardPoints != 0)
                Console.WriteLine("Average for students excluding honors and duel enrollment is " + (standardPoints / Students.Where(e => e.Type == StudentType.Standard).Count()));
            if (honorPoints != 0)
                Console.WriteLine("Average for only honors students is " + (honorPoints / Students.Where(e => e.Type == StudentType.Honors).Count()));
            if (dualEnrolledPoints != 0)
                Console.WriteLine("Average for only duel enrolled students is " + (dualEnrolledPoints / Students.Where(e => e.Type == StudentType.DualEnrolled).Count()));
        }

        public virtual void CalculateStudentStatistics(string name)
        {
            var student = Students.FirstOrDefault(e => e.Name == name);
            student.LetterGrade = GetLetterGrade(student.AverageGrade);
            student.GPA = GetGPA(student.LetterGrade, student.Type);

            Console.WriteLine("{0} ({1}:{2}) GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
            Console.WriteLine();
            Console.WriteLine("Grades:");
            foreach (var grade in student.Grades)
            {
                Console.WriteLine(grade);
            }
        }

        public virtual char GetLetterGrade(double averageGrade)
        {
            if (averageGrade >= 90)
                return 'A';
            else if (averageGrade >= 80)
                return 'B';
            else if (averageGrade >= 70)
                return 'C';
            else if (averageGrade >= 60)
                return 'D';
            else
                return 'F';
        }

        /// <summary>
        ///     Converts json to the appropriate grade book type.
        ///     Note: This method contains code that is not recommended practice.
        ///     This has been used as a compromise to avoid adding additional complexity to the learner.
        /// </summary>
        /// <returns>The to grade book.</returns>
        /// <param name="json">Json.</param>
        public static dynamic ConvertToGradeBook(string json)
        {
            // Get GradeBookType from the GradeBook.Enums namespace
            var gradebookEnum = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 from type in assembly.GetTypes()
                                 where type.FullName == "GradeBook.Enums.GradeBookType"
                                 select type).FirstOrDefault();

            var jobject = JsonConvert.DeserializeObject<JObject>(json);
            var gradeBookType = jobject.Property("Type")?.Value?.ToString();

            // Check if StandardGradeBook exists
            if ((from assembly in AppDomain.CurrentDomain.GetAssemblies()
                 from type in assembly.GetTypes()
                 where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                 select type).FirstOrDefault() == null)
                gradeBookType = "Base";
            else
            {
                if (string.IsNullOrEmpty(gradeBookType))
                    gradeBookType = "Standard";
                else
                    gradeBookType = Enum.GetName(gradebookEnum, int.Parse(gradeBookType));
            }

            // Get GradeBook from the GradeBook.GradeBooks namespace
            var gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks." + gradeBookType + "GradeBook"
                             select type).FirstOrDefault();


            //protection code
            if (gradebook == null)
                gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                             select type).FirstOrDefault();
            
            return JsonConvert.DeserializeObject(json, gradebook);
        }
    }
}

using GradeBook.Enums;
using System;
using System.Linq;

namespace GradeBook.GradeBooks
{
  public class RankedGradeBook : BaseGradeBook
  {
    public RankedGradeBook(string name, bool isWeighted) : base(name, isWeighted)
    {
      Type = GradeBookType.Ranked;
    }

    public override void CalculateStudentStatistics(string name)
    {
      if (Students.Count < 5)
      {
        System.Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
        return;
      }
      base.CalculateStudentStatistics(name);
    }

    public override void CalculateStatistics()
    {
      if (Students.Count < 5)
      {
        System.Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
        return;
      }
      base.CalculateStatistics();
    }

    public override char GetLetterGrade(double averageGrade)
    {
      if (Students.Count < 5)
      {
        throw new InvalidOperationException("Ranked grading requires at least 5 students");
      }

      // Number of students that should receive each letter grade
      var threshold = (int)Math.Ceiling(Students.Count * 0.2);

      //                Order students by       their avg grade   and take their avg grade val  to a list
      var grades = Students.OrderByDescending(e => e.AverageGrade).Select(e => e.AverageGrade).ToList();
      
      if (grades[threshold - 1] <= averageGrade)
        return 'A';
      else if (grades[(threshold * 2) - 1] <= averageGrade)
        return 'B';
      else if (grades[(threshold * 3) - 1] <= averageGrade)
        return 'C';
      else if (grades[(threshold * 4) - 1] <= averageGrade)
        return 'D';
      else
        return 'F';
    }
  }
}

using GradeBook.Enums;

namespace GradeBook.GradeBooks
{
  public class StandardGradeBook : BaseGradeBook
  {

    public StandardGradeBook(string name, bool isWeighted) : base(name, isWeighted)
    {
      Type = GradeBookType.Standard;
    }
  }
}

/*
	CSharp-GradeBookApplication
	
	Description: Project to add features to an existing C Sharp Grade Book Application.
	Website: http:\\www.pluralsight.com
	         https://github.com/pmash2/CSharp-GradeBookApplication
*/


using System;
using GradeBook.UserInterfaces;

namespace GradeBook
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("#=======================#");
            Console.WriteLine("# Welcome to GradeBook! #");
            Console.WriteLine("#=======================#");
            Console.WriteLine();

            StartingUserInterface.CommandLoop();
            
            Console.WriteLine("Thank you for using GradeBook!");
            Console.WriteLine("Have a nice day!");
            Console.Read();
        }
    }
}

namespace GradeBook.Enums
{
    public enum EnrollmentType
    {
        Campus,
        State,
        National,
        International
    }
}

namespace GradeBook.Enums
{
  public enum GradeBookType
  {
    Standard,
    Ranked,
    ESNU,
    OneToFour,
    SixPoint
  }
}

namespace GradeBook.Enums
{
    public enum StudentType
    {
        Standard,
        Honors,
        DualEnrolled
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

using GradeBook.Enums;

namespace GradeBook
{
    public class Student
    {
        public string Name { get; set; }
        public StudentType Type { get; set; }
        public EnrollmentType Enrollment { get; set; }
        public List<double> Grades { get; set; }
        [JsonIgnore]
        public double AverageGrade
        {
            get
            {
                return Grades.Average();
            }
        }
        [JsonIgnore]
        public char LetterGrade { get; set; }
        [JsonIgnore]
        public double GPA { get; set; }

        public Student(string name, StudentType studentType, EnrollmentType enrollment)
        {
            Name = name;
            Type = studentType;
            Enrollment = enrollment;
            Grades = new List<double>();
        }

        public void AddGrade(double grade)
        {
            if (grade < 0 || grade > 100)
                throw new ArgumentException("Grades must be between 0 and 100.");
            Grades.Add(grade);
        }

        public void RemoveGrade(double grade)
        {
            Grades.Remove(grade);
        }
    }
}

using GradeBook.GradeBooks;
using System;

namespace GradeBook.UserInterfaces
{
    public static class StartingUserInterface
    {
        public static bool Quit = false;
        public static void CommandLoop()
        {
            while (!Quit)
            {
                Console.WriteLine("What would you like to do?");
                var command = Console.ReadLine().ToLower();
                CommandRoute(command);
            }
        }

        public static void CommandRoute(string command)
        {
            if (command.StartsWith("create"))
                CreateCommand(command);
            else if (command.StartsWith("load"))
                LoadCommand(command);
            else if (command == "help")
                HelpCommand();
            else if (command == "quit")
                Quit = true;
            else
                Console.WriteLine("{0} was not recognized, please try again.", command);
        }

        public static void CreateCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Command not valid, Create requires a name, type of gradebook, if it's weighted (true / false).");
                return;
            }
            var name = parts[1];
            var type = parts[2].ToLower();
            var weighted = bool.Parse(parts[3]);
            BaseGradeBook gradeBook;

            if (type == "standard")
                gradeBook = new StandardGradeBook(name, weighted);
            else if (type == "ranked")
                gradeBook = new RankedGradeBook(name, weighted);
            else
            {
                System.Console.WriteLine("{0} is not a supported type of gradebook, please try again", type);
                return;
            }

            Console.WriteLine("Created gradebook {0}.", name);
            GradeBookUserInterface.CommandLoop(gradeBook);
        }

        public static void LoadCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Load requires a name.");
                return;
            }
            var name = parts[1];
            var gradeBook = BaseGradeBook.Load(name);

            if (gradeBook == null)
                return;

            GradeBookUserInterface.CommandLoop(gradeBook);
        }

        public static void HelpCommand()
        {
            Console.WriteLine("GradeBook accepts the following commands:");
            Console.WriteLine();
            Console.WriteLine("Create 'Name' 'Type' 'Weighted' - Creates a new gradebook where 'Name' is the name of the gradebook, 'Type' is what type of grading it should use, and 'Weighted' is whether or not grades should be weighted (true or false).");
            Console.WriteLine();
            Console.WriteLine("Load 'Name' - Loads the gradebook with the provided 'Name'.");
            Console.WriteLine();
            Console.WriteLine("Help - Displays all accepted commands.");
            Console.WriteLine();
            Console.WriteLine("Quit - Exits the application");
        }
    }
}


using GradeBook.Enums;
using GradeBook.GradeBooks;
using System;

namespace GradeBook.UserInterfaces
{
    public static class GradeBookUserInterface
    {
        public static BaseGradeBook GradeBook;
        public static bool Quit = false;
        public static void CommandLoop(BaseGradeBook gradeBook)
        {
            GradeBook = gradeBook;
            Quit = false;

            Console.WriteLine("#=======================#");
            Console.WriteLine(GradeBook.Name + " : " + GradeBook.GetType().Name);
            Console.WriteLine("#=======================#");
            Console.WriteLine(string.Empty);

            while(!Quit)
            {
                Console.WriteLine("What would you like to do?");
                var command = Console.ReadLine().ToLower();
                CommandRoute(command);
            }

            Console.WriteLine(GradeBook.Name + " has been closed.");
        }

        public static void CommandRoute(string command)
        {
            if (command == "save")
                SaveCommand();
            else if (command.StartsWith("addgrade"))
                AddGradeCommand(command);
            else if (command.StartsWith("removegrade"))
                RemoveGradeCommand(command);
            else if (command.StartsWith("add"))
                AddStudentCommand(command);
            else if (command.StartsWith("remove"))
                RemoveStudentCommand(command);
            else if (command == "list")
                ListCommand();
            else if (command == "statistics all")
                StatisticsCommand();
            else if (command.StartsWith("statistics"))
                StudentStatisticsCommand(command);
            else if (command == "help")
                HelpCommand();
            else if (command == "close")
                Quit = true;
            else
                Console.WriteLine("{0} was not recognized, please try again.", command);
        }

        public static void SaveCommand()
        {
            GradeBook.Save();
            Console.WriteLine("{0} has been saved.", GradeBook.Name);
        }
        
        public static void AddGradeCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Command not valid, AddGrade requires a name and score.");
                return;
            }
            var name = parts[1];
            var score = Double.Parse(parts[2]);
            GradeBook.AddGrade(name, score);
            Console.WriteLine("Added a score of {0} to {1}'s grades", score, name);
        }

        public static void RemoveGradeCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 3)
            {
                Console.WriteLine("Command not valid, RemoveGrade requires a name and score.");
                return;
            }
            var name = parts[1];
            var score = Double.Parse(parts[2]);
            GradeBook.RemoveGrade(name, score);
            Console.WriteLine("Removed a score of {0} from {1}'s grades", score, name);
        }

        public static void AddStudentCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 4)
            {
                Console.WriteLine("Command not valid, Add requires a name, student type, enrollment type.");
                return;
            }
            var name = parts[1];

            StudentType studentType;
            if (!Enum.TryParse(parts[2], true, out studentType))
            {
                Console.WriteLine("{0} is not a valid student type, try again.", parts[2]);
                return;
            }

            EnrollmentType enrollmentType;
            if (!Enum.TryParse(parts[3], true, out enrollmentType))
            {
                Console.WriteLine("{0} is not a volid enrollment type, try again.", parts[3]);
                return;
            }

            var student = new Student(name, studentType, enrollmentType);
            GradeBook.AddStudent(student);
            Console.WriteLine("Added {0} to the gradebook.", name);
        }
        
        public static void RemoveStudentCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Remove requires a name.");
                return;
            }
            var name = parts[1];
            GradeBook.RemoveStudent(name);
            Console.WriteLine("Removed {0} from the gradebook.", name);
        }

        public static void ListCommand()
        {
            GradeBook.ListStudents();
        }
        
        public static void StatisticsCommand()
        {
            GradeBook.CalculateStatistics();
        }

        public static void StudentStatisticsCommand(string command)
        {
            var parts = command.Split(' ');
            if (parts.Length != 2)
            {
                Console.WriteLine("Command not valid, Requires Name or All.");
                return;
            }
            var name = parts[1];
            GradeBook.CalculateStudentStatistics(name);
        }

        public static void HelpCommand()
        {
            Console.WriteLine("While a gradebook is open you can use the following commands:");
            Console.WriteLine();
            Console.WriteLine("Add 'Name' 'Student Type' 'Enrollment Type' - Adds a new student to the gradebook with the provided name, type of student, and type of enrollment.");
            Console.WriteLine();
            Console.WriteLine("Accepted Student Types:");
            Console.WriteLine("Standard - Student not enrolled in Honors classes or Dual Enrolled.");
            Console.WriteLine("Honors - Students enrolled in Honors classes and not Dual Enrolled.");
            Console.WriteLine("DualEnrolled - Students who are Duel Enrolled.");
            Console.WriteLine();
            Console.WriteLine("Accepted Enrollement Types:");
            Console.WriteLine("Campus - Students who are in the same disctrict as the school.");
            Console.WriteLine("State - Students who's legal residence is outside the school's district, but is in the same state as the school.");
            Console.WriteLine("National - Students who's legal residence is not in the same state as the school, but is in the same country as the school.");
            Console.WriteLine("International - Students who's legal residence is not in the same country as the school.");
            Console.WriteLine();
            Console.WriteLine("List - Lists all students.");
            Console.WriteLine();
            Console.WriteLine("AddGrade 'Name' 'Score' - Adds a new grade to a student with the matching name of the provided score.");
            Console.WriteLine();
            Console.WriteLine("RemoveGrade 'Name' 'Score' - Removes a grade to a student with the matching name and score.");
            Console.WriteLine();
            Console.WriteLine("Remove 'Name' - Removes the student with the provided name.");
            Console.WriteLine();
            Console.WriteLine("Statistics 'Name' - Gets statistics for the specified student.");
            Console.WriteLine();
            Console.WriteLine("Statistics All - Gets general statistics for the entire gradebook.");
            Console.WriteLine();
            Console.WriteLine("Close - closes the gradebook and takes you back to the starting command options.");
            Console.WriteLine();
            Console.WriteLine("Save - saves the gradebook to the hard drive for later use.");
        }
    }
}

using System;
using System.Linq;

using GradeBook.Enums;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GradeBook.GradeBooks
{
    public abstract class BaseGradeBook
    {
        public string Name { get; set; }
        public List<Student> Students { get; set; }
        public GradeBookType Type { get; set; }
        public bool IsWeighted { get; set; }

        public BaseGradeBook(string name, bool isWeighted)
        {
            Name = name;
            Students = new List<Student>();
            IsWeighted = isWeighted;
        }

        public void AddStudent(Student student)
        {
            if (string.IsNullOrEmpty(student.Name))
                throw new ArgumentException("A Name is required to add a student to a gradebook.");
            Students.Add(student);
        }

        public void RemoveStudent(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to remove a student from a gradebook.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            Students.Remove(student);
        }

        public void AddGrade(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to add a grade to a student.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            student.AddGrade(score);
        }

        public void RemoveGrade(string name, double score)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("A Name is required to remove a grade from a student.");
            var student = Students.FirstOrDefault(e => e.Name == name);
            if (student == null)
            {
                Console.WriteLine("student {0} was not found, try again.", name);
                return;
            }
            student.RemoveGrade(score);
        }

        public void ListStudents()
        {
            foreach (var student in Students)
            {
                Console.WriteLine("{0} : {1} : {2}", student.Name, student.Type, student.Enrollment);
            }
        }

        public static BaseGradeBook Load(string name)
        {
            if (!File.Exists(name + ".gdbk"))
            {
                Console.WriteLine("Gradebook could not be found.");
                return null;
            }

            using (var file = new FileStream(name + ".gdbk", FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(file))
                {
                    var json = reader.ReadToEnd();
                    return ConvertToGradeBook(json);
                }
            }
        }

        public void Save()
        {
            using (var file = new FileStream(Name + ".gdbk", FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(file))
                {
                    var json = JsonConvert.SerializeObject(this);
                    writer.Write(json);
                }
            }
        }

        public virtual double GetGPA(char letterGrade, StudentType studentType)
        {
            var gpa = 0;
            
            switch (letterGrade)
            {
                case 'A':
                    gpa = 4;
                    break;
                case 'B':
                    gpa = 3;
                    break;
                case 'C':
                    gpa = 2;
                    break;
                case 'D':
                    gpa = 1;
                    break;
            }
            
            if (IsWeighted && (studentType == StudentType.Honors || studentType == StudentType.DualEnrolled))
                gpa++;
            
            return gpa;
        }

        public virtual void CalculateStatistics()
        {
            var allStudentsPoints = 0d;
            var campusPoints = 0d;
            var statePoints = 0d;
            var nationalPoints = 0d;
            var internationalPoints = 0d;
            var standardPoints = 0d;
            var honorPoints = 0d;
            var dualEnrolledPoints = 0d;

            foreach (var student in Students)
            {
                student.LetterGrade = GetLetterGrade(student.AverageGrade);
                student.GPA = GetGPA(student.LetterGrade, student.Type);

                Console.WriteLine("{0} ({1}:{2}) GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
                allStudentsPoints += student.AverageGrade;

                switch (student.Enrollment)
                {
                    case EnrollmentType.Campus:
                        campusPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.State:
                        statePoints += student.AverageGrade;
                        break;
                    case EnrollmentType.National:
                        nationalPoints += student.AverageGrade;
                        break;
                    case EnrollmentType.International:
                        internationalPoints += student.AverageGrade;
                        break;
                }

                switch (student.Type)
                {
                    case StudentType.Standard:
                        standardPoints += student.AverageGrade;
                        break;
                    case StudentType.Honors:
                        honorPoints += student.AverageGrade;
                        break;
                    case StudentType.DualEnrolled:
                        dualEnrolledPoints += student.AverageGrade;
                        break;
                }
            }

            //#todo refactor into it's own method with calculations performed here
            Console.WriteLine("Average Grade of all students is " + (allStudentsPoints / Students.Count));
            if (campusPoints != 0)
                Console.WriteLine("Average for only local students is " + (campusPoints / Students.Where(e => e.Enrollment == EnrollmentType.Campus).Count()));
            if (statePoints != 0)
                Console.WriteLine("Average for only state students (excluding local) is " + (statePoints / Students.Where(e => e.Enrollment == EnrollmentType.State).Count()));
            if (nationalPoints != 0)
                Console.WriteLine("Average for only national students (excluding state and local) is " + (nationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.National).Count()));
            if (internationalPoints != 0)
                Console.WriteLine("Average for only international students is " + (internationalPoints / Students.Where(e => e.Enrollment == EnrollmentType.International).Count()));
            if (standardPoints != 0)
                Console.WriteLine("Average for students excluding honors and duel enrollment is " + (standardPoints / Students.Where(e => e.Type == StudentType.Standard).Count()));
            if (honorPoints != 0)
                Console.WriteLine("Average for only honors students is " + (honorPoints / Students.Where(e => e.Type == StudentType.Honors).Count()));
            if (dualEnrolledPoints != 0)
                Console.WriteLine("Average for only duel enrolled students is " + (dualEnrolledPoints / Students.Where(e => e.Type == StudentType.DualEnrolled).Count()));
        }

        public virtual void CalculateStudentStatistics(string name)
        {
            var student = Students.FirstOrDefault(e => e.Name == name);
            student.LetterGrade = GetLetterGrade(student.AverageGrade);
            student.GPA = GetGPA(student.LetterGrade, student.Type);

            Console.WriteLine("{0} ({1}:{2}) GPA: {3}.", student.Name, student.LetterGrade, student.AverageGrade, student.GPA);
            Console.WriteLine();
            Console.WriteLine("Grades:");
            foreach (var grade in student.Grades)
            {
                Console.WriteLine(grade);
            }
        }

        public virtual char GetLetterGrade(double averageGrade)
        {
            if (averageGrade >= 90)
                return 'A';
            else if (averageGrade >= 80)
                return 'B';
            else if (averageGrade >= 70)
                return 'C';
            else if (averageGrade >= 60)
                return 'D';
            else
                return 'F';
        }

        /// <summary>
        ///     Converts json to the appropriate grade book type.
        ///     Note: This method contains code that is not recommended practice.
        ///     This has been used as a compromise to avoid adding additional complexity to the learner.
        /// </summary>
        /// <returns>The to grade book.</returns>
        /// <param name="json">Json.</param>
        public static dynamic ConvertToGradeBook(string json)
        {
            // Get GradeBookType from the GradeBook.Enums namespace
            var gradebookEnum = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 from type in assembly.GetTypes()
                                 where type.FullName == "GradeBook.Enums.GradeBookType"
                                 select type).FirstOrDefault();

            var jobject = JsonConvert.DeserializeObject<JObject>(json);
            var gradeBookType = jobject.Property("Type")?.Value?.ToString();

            // Check if StandardGradeBook exists
            if ((from assembly in AppDomain.CurrentDomain.GetAssemblies()
                 from type in assembly.GetTypes()
                 where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                 select type).FirstOrDefault() == null)
                gradeBookType = "Base";
            else
            {
                if (string.IsNullOrEmpty(gradeBookType))
                    gradeBookType = "Standard";
                else
                    gradeBookType = Enum.GetName(gradebookEnum, int.Parse(gradeBookType));
            }

            // Get GradeBook from the GradeBook.GradeBooks namespace
            var gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks." + gradeBookType + "GradeBook"
                             select type).FirstOrDefault();


            //protection code
            if (gradebook == null)
                gradebook = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.FullName == "GradeBook.GradeBooks.StandardGradeBook"
                             select type).FirstOrDefault();
            
            return JsonConvert.DeserializeObject(json, gradebook);
        }
    }
}

using GradeBook.Enums;
using System;
using System.Linq;

namespace GradeBook.GradeBooks
{
  public class RankedGradeBook : BaseGradeBook
  {
    public RankedGradeBook(string name, bool isWeighted) : base(name, isWeighted)
    {
      Type = GradeBookType.Ranked;
    }

    public override void CalculateStudentStatistics(string name)
    {
      if (Students.Count < 5)
      {
        System.Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
        return;
      }
      base.CalculateStudentStatistics(name);
    }

    public override void CalculateStatistics()
    {
      if (Students.Count < 5)
      {
        System.Console.WriteLine("Ranked grading requires at least 5 students with grades in order to properly calculate a student's overall grade.");
        return;
      }
      base.CalculateStatistics();
    }

    public override char GetLetterGrade(double averageGrade)
    {
      if (Students.Count < 5)
      {
        throw new InvalidOperationException("Ranked grading requires at least 5 students");
      }

      // Number of students that should receive each letter grade
      var threshold = (int)Math.Ceiling(Students.Count * 0.2);

      //                Order students by       their avg grade   and take their avg grade val  to a list
      var grades = Students.OrderByDescending(e => e.AverageGrade).Select(e => e.AverageGrade).ToList();
      
      if (grades[threshold - 1] <= averageGrade)
        return 'A';
      else if (grades[(threshold * 2) - 1] <= averageGrade)
        return 'B';
      else if (grades[(threshold * 3) - 1] <= averageGrade)
        return 'C';
      else if (grades[(threshold * 4) - 1] <= averageGrade)
        return 'D';
      else
        return 'F';
    }
  }
}

using GradeBook.Enums;

namespace GradeBook.GradeBooks
{
  public class StandardGradeBook : BaseGradeBook
  {

    public StandardGradeBook(string name, bool isWeighted) : base(name, isWeighted)
    {
      Type = GradeBookType.Standard;
    }
  }
}