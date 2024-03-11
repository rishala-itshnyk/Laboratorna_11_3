using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Laboratorna_11_3
{
    public enum Specialization
    {
        ComputerScience,
        Informatics,
        MathematicsEconomics,
        PhysicsInformatics,
        LaborTraining
    }

    public struct Marks
    {
        public int Physics;
        public int Mathematics;
        public int Informatics;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct AdditionalMarks
    {
        [FieldOffset(0)] public int Programming;

        [FieldOffset(4)] public int NumericalMethods;

        [FieldOffset(8)] public int Pedagogy;
    }

    public struct StudentLevel
    {
        public int StudentNumber;
        public string LastName;
        public int Course;
        public Specialization Specialization;
        public Marks SubjectMarks;
        public AdditionalMarks AdditionalMarks;
    }

    public class Program
    {
        static void Main()
        {
            Console.Write("Введіть кількість студентів: ");
            int numberOfStudents = int.Parse(Console.ReadLine());

            Console.Write("Введіть ім'я файлу для збереження даних: ");
            string fileName = Console.ReadLine();

            SaveToFile(numberOfStudents, fileName, 1);

            Console.WriteLine("\nТаблиця:");
            StudentLevel[] loadedData = LoadFromFile(fileName);
            Print(loadedData);

            ComputeAverageMarks(loadedData);
            CountStudentsWithHighPhysicsMarks(loadedData);

            Console.WriteLine("\nСортування:");
            Sort(fileName);
            Print(loadedData);

            Console.WriteLine("\nІндексне впорядкування та вивід даних:");
            IndexSort(fileName);
            Print(loadedData);

            Console.WriteLine("\nБінарний пошук:");

            Console.Write("Введіть прізвище для пошуку: ");
            string searchLastName = Console.ReadLine();

            Console.Write(
                "Введіть спеціалізацію для пошуку (0-Комп'ютерні науки, 1-Інформатика, 2-Математика та Економіка, 3-Фізика та Інформатика, 4-Трудове навчання): ");
            Specialization searchSpecialization =
                (Specialization)Enum.Parse(typeof(Specialization), Console.ReadLine());

            Console.Write("Введіть оцінку з третього предмету для пошуку: ");
            int searchGrade = int.Parse(Console.ReadLine());

            int foundIndex = BinarySearch(fileName, searchLastName, searchSpecialization, searchGrade);

            if (foundIndex != -1)
            {
                Console.WriteLine($"Знайдено студента в позиції {foundIndex + 1}");
            }
            else
            {
                Console.WriteLine("Шуканого студента не знайдено");
            }
        }

        public static void SaveToFile(int numberOfStudents, string fileName, int menuMode)
        {
            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                {
                    for (int i = 0; i < numberOfStudents; i++)
                    {
                        if (menuMode == 1)
                        {
                            Console.WriteLine($"\nВкажіть дані студента {i + 1}:");
                            StudentLevel student;

                            student.AdditionalMarks.Programming = 0;
                            student.AdditionalMarks.NumericalMethods = 0;
                            student.AdditionalMarks.Pedagogy = 0;

                            student.StudentNumber = i + 1;

                            Console.Write("Прізвище Студента: ");
                            student.LastName = Console.ReadLine();

                            Console.Write("Курс: ");
                            student.Course = int.Parse(Console.ReadLine());

                            Console.Write(
                                "Спеціалізація (0-Комп'ютерні науки, 1-Інформатика, 2-Математика та Економіка, 3-Фізика та Інформатика, 4-Трудове навчання): ");
                            student.Specialization = (Specialization)Enum.Parse(typeof(Specialization),
                                Console.ReadLine());

                            Console.Write("Оцінка з Фізика: ");
                            student.SubjectMarks.Physics = int.Parse(Console.ReadLine());

                            Console.Write("Оцінка з Математики: ");
                            student.SubjectMarks.Mathematics = int.Parse(Console.ReadLine());

                            switch (student.Specialization)
                            {
                                case Specialization.ComputerScience:
                                    Console.Write("Оцінка з Програмування: ");
                                    student.AdditionalMarks.Programming = int.Parse(Console.ReadLine());
                                    break;
                                case Specialization.Informatics:
                                    Console.Write("Оцінка з Чисельних Методів: ");
                                    student.AdditionalMarks.NumericalMethods = int.Parse(Console.ReadLine());
                                    break;
                                default:
                                    Console.Write("Оцінка з Педагогіки: ");
                                    student.AdditionalMarks.Pedagogy = int.Parse(Console.ReadLine());
                                    break;
                            }

                            writer.Write(student.StudentNumber);
                            writer.Write(student.LastName.PadRight(50, '\0').ToCharArray());
                            writer.Write(student.Course);
                            writer.Write((int)student.Specialization);
                            writer.Write(student.SubjectMarks.Physics);
                            writer.Write(student.SubjectMarks.Mathematics);

                            switch (student.Specialization)
                            {
                                case Specialization.ComputerScience:
                                    writer.Write(student.AdditionalMarks.Programming);
                                    break;
                                case Specialization.Informatics:
                                    writer.Write(student.AdditionalMarks.NumericalMethods);
                                    break;
                                default:
                                    writer.Write(student.AdditionalMarks.Pedagogy);
                                    break;
                            }
                        }
                    }
                }

                Console.WriteLine($"Дані збережено у файл {fileName}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при збереженні файлу: {ex.Message}");
            }
        }


        public static StudentLevel[] LoadFromFile(string fileName)
        {
            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open)))
                {
                    List<StudentLevel> loadedData = new List<StudentLevel>();

                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        StudentLevel student = default;

                        student.StudentNumber = reader.ReadInt32();
                        student.LastName = new string(reader.ReadChars(50)).TrimEnd('\0');
                        student.Course = reader.ReadInt32();
                        student.Specialization = (Specialization)reader.ReadInt32();
                        student.SubjectMarks.Physics = reader.ReadInt32();
                        student.SubjectMarks.Mathematics = reader.ReadInt32();

                        switch (student.Specialization)
                        {
                            case Specialization.ComputerScience:
                                student.AdditionalMarks.Programming = reader.ReadInt32();
                                break;
                            case Specialization.Informatics:
                                student.AdditionalMarks.NumericalMethods = reader.ReadInt32();
                                break;
                            default:
                                student.AdditionalMarks.Pedagogy = reader.ReadInt32();
                                break;
                        }

                        loadedData.Add(student);
                    }

                    Console.WriteLine($"Дані зчитано з файлу {fileName}.");
                    return loadedData.ToArray();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при зчитуванні файлу: {ex.Message}");
                return Array.Empty<StudentLevel>();
            }
        }


        public static void Print(StudentLevel[] students, int[] indexes = null)
        {
            Console.WriteLine(
                "| № | Прізвище | Курс | Спеціалізація | Фізика | Математика | Програмування | Чисельні Методи | Педагогіка |");
            Console.WriteLine(
                "========================================================================================================");

            if (indexes == null)
            {
                for (int i = 0; i < students.Length; i++)
                {
                    Console.Write(
                        $"| {students[i].StudentNumber,2} | {students[i].LastName,-10} | {students[i].Course,4} | {students[i].Specialization,-14} | {students[i].SubjectMarks.Physics,6} | {students[i].SubjectMarks.Mathematics,9} |");

                    switch (students[i].Specialization)
                    {
                        case Specialization.ComputerScience:
                            Console.Write($" {students[i].AdditionalMarks.Programming,12} |");
                            break;
                        case Specialization.Informatics:
                            Console.Write($" {students[i].AdditionalMarks.NumericalMethods,15} |");
                            break;
                        default:
                            Console.Write($" {students[i].AdditionalMarks.Pedagogy,10} |");
                            break;
                    }

                    Console.WriteLine();
                }
            }
            else
            {
                foreach (var index in indexes)
                {
                    Console.Write(
                        $"| {students[index].StudentNumber,2} | {students[index].LastName,-10} | {students[index].Course,4} | {students[index].Specialization,-14} | {students[index].SubjectMarks.Physics,6} | {students[index].SubjectMarks.Mathematics,9} |");

                    switch (students[index].Specialization)
                    {
                        case Specialization.ComputerScience:
                            Console.Write($" {students[index].AdditionalMarks.Programming,12} |");
                            break;
                        case Specialization.Informatics:
                            Console.Write($" {students[index].AdditionalMarks.NumericalMethods,15} |");
                            break;
                        default:
                            Console.Write($" {students[index].AdditionalMarks.Pedagogy,10} |");
                            break;
                    }

                    Console.WriteLine();
                }
            }
        }


        public static void ComputeAverageMarks(StudentLevel[] students)
        {
            foreach (var student in students)
            {
                double average = (student.SubjectMarks.Physics + student.SubjectMarks.Mathematics +
                                  GetAdditionalMark(student)) / 3.0;
                Console.WriteLine($"Середній бал студента {student.LastName}: {average:F2}");
            }
        }

        public static void CountStudentsWithHighPhysicsMarks(StudentLevel[] students)
        {
            int count = students.Count(student => student.SubjectMarks.Physics > 4);
            Console.WriteLine($"\nКількість студентів з високими оцінками з фізики: {count}");
        }

        public static void Sort(string fileName)
        {
            List<StudentLevel> students = LoadFromFile(fileName).ToList();
            int numberOfStudents = students.Count;

            for (int i = 0; i < numberOfStudents - 1; i++)
            for (int j = 0; j < numberOfStudents - i - 1; j++)
            {
                if (students[j].Course > students[j + 1].Course ||
                    (students[j].Course == students[j + 1].Course &&
                     GetSubjectMark(students[j]) > GetSubjectMark(students[j + 1])) ||
                    (students[j].Course == students[j + 1].Course &&
                     GetSubjectMark(students[j]) == GetSubjectMark(students[j + 1]) &&
                     students[j].LastName.CompareTo(students[j + 1].LastName) < 0))
                {
                    var temp = students[j];
                    students[j] = students[j + 1];
                    students[j + 1] = temp;
                }
            }

            SaveToFile(numberOfStudents, fileName, 0);
        }

        public static void IndexSort(string fileName)
        {
            List<StudentLevel> students = LoadFromFile(fileName).ToList();
            int[] indexes = Enumerable.Range(0, students.Count).ToArray();

            for (int i = 1; i < students.Count; i++)
            {
                int value = indexes[i];
                int j = i - 1;

                while (j >= 0 && (students[indexes[j]].Course > students[value].Course ||
                                  (students[indexes[j]].Course == students[value].Course &&
                                   GetAdditionalMark(students[indexes[j]]) > GetAdditionalMark(students[value])) ||
                                  (students[indexes[j]].Course == students[value].Course &&
                                   GetAdditionalMark(students[indexes[j]]) == GetAdditionalMark(students[value]) &&
                                   students[indexes[j]].LastName.CompareTo(students[value].LastName) > 0)))
                {
                    indexes[j + 1] = indexes[j];
                    j--;
                }

                indexes[j + 1] = value;
            }

            SaveToFile(students.Count, fileName, 0);
        }

        public static int BinarySearch(string fileName, string lastName, Specialization specialization, int grade)
        {
            List<StudentLevel> students = LoadFromFile(fileName).ToList();
            int left = 0;
            int right = students.Count - 1;

            while (left <= right)
            {
                int middle = (left + right) / 2;

                if (string.Compare(students[middle].LastName, lastName, StringComparison.Ordinal) == 0 &&
                    students[middle].Specialization == specialization &&
                    students[middle].SubjectMarks.Physics == grade)
                {
                    return middle;
                }

                if (string.Compare(students[middle].LastName, lastName, StringComparison.Ordinal) < 0 ||
                    students[middle].Specialization < specialization ||
                    (students[middle].Specialization == specialization && students[middle].SubjectMarks.Physics < grade))
                {
                    left = middle + 1;
                }
                else
                {
                    right = middle - 1;
                }
            }

            return -1;
        }
        

        private static int GetSubjectMark(StudentLevel student)
        {
            switch (student.Specialization)
            {
                case Specialization.ComputerScience:
                    return student.SubjectMarks.Physics;
                case Specialization.Informatics:
                    return student.SubjectMarks.Mathematics;
                case Specialization.MathematicsEconomics:
                    return student.SubjectMarks.Mathematics;
                case Specialization.PhysicsInformatics:
                    return student.SubjectMarks.Informatics;
                default:
                    return 0;
            }
        }

        private static int GetAdditionalMark(StudentLevel student)
        {
            switch (student.Specialization)
            {
                case Specialization.ComputerScience:
                    return student.AdditionalMarks.Programming;
                case Specialization.Informatics:
                    return student.AdditionalMarks.NumericalMethods;
                default:
                    return student.AdditionalMarks.Pedagogy;
            }
        }
    }
}