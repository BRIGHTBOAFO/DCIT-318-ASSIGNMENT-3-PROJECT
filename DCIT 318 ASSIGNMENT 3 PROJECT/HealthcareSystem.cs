using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class HealthSystemApp
{
    private Repository<Patient> _patientRepo;
    private Repository<Prescription> _prescriptionRepo;
    private Dictionary<int, List<Prescription>> _prescriptionMap;

    private string _patientFile;
    private string _prescriptionFile;

    public HealthSystemApp(string patientFile, string prescriptionFile)
    {
        _patientRepo = new Repository<Patient>();
        _prescriptionRepo = new Repository<Prescription>();
        _prescriptionMap = new Dictionary<int, List<Prescription>>();
        _patientFile = patientFile;
        _prescriptionFile = prescriptionFile;
    }

    public void SeedData()
    {
        _patientRepo.Add(new Patient(1, "John Doe", 30, "Male"));
        _patientRepo.Add(new Patient(2, "Jane Smith", 25, "Female"));
        _patientRepo.Add(new Patient(3, "Michael Brown", 40, "Male"));

        _prescriptionRepo.Add(new Prescription(1, 1, "Paracetamol", DateTime.Now.AddDays(-10)));
        _prescriptionRepo.Add(new Prescription(2, 1, "Amoxicillin", DateTime.Now.AddDays(-5)));
        _prescriptionRepo.Add(new Prescription(3, 2, "Ibuprofen", DateTime.Now.AddDays(-8)));
        _prescriptionRepo.Add(new Prescription(4, 3, "Vitamin C", DateTime.Now.AddDays(-3)));
        _prescriptionRepo.Add(new Prescription(5, 2, "Cough Syrup", DateTime.Now.AddDays(-1)));
    }

    public void SaveData()
    {
        File.WriteAllText(_patientFile, JsonSerializer.Serialize(_patientRepo.GetAll()));
        File.WriteAllText(_prescriptionFile, JsonSerializer.Serialize(_prescriptionRepo.GetAll()));
    }

    public void LoadData()
    {
        if (File.Exists(_patientFile))
        {
            var patients = JsonSerializer.Deserialize<List<Patient>>(File.ReadAllText(_patientFile));
            foreach (var p in patients) _patientRepo.Add(p);
        }

        if (File.Exists(_prescriptionFile))
        {
            var prescriptions = JsonSerializer.Deserialize<List<Prescription>>(File.ReadAllText(_prescriptionFile));
            foreach (var pr in prescriptions) _prescriptionRepo.Add(pr);
        }
    }

    public void BuildPrescriptionMap()
    {
        _prescriptionMap = _prescriptionRepo.GetAll()
            .GroupBy(p => p.PatientId)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(p => p.DateIssued).ToList());
    }

    public void RunMenu()
    {
        while (true)
        {
            Console.WriteLine("\n==== Health System Menu ====");
            Console.WriteLine("1. View All Patients");
            Console.WriteLine("2. View Prescriptions for Patient");
            Console.WriteLine("3. Add New Patient");
            Console.WriteLine("4. Add New Prescription");
            Console.WriteLine("5. Exit");
            Console.Write("Select: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1": PrintAllPatients(); break;
                case "2":
                    Console.Write("Enter Patient ID: ");
                    int pid = int.Parse(Console.ReadLine());
                    PrintPrescriptionsForPatient(pid);
                    break;
                case "3": AddNewPatient(); SaveData(); break;
                case "4": AddNewPrescription(); SaveData(); break;
                case "5":
                    SaveData();
                    Console.WriteLine("Data saved. Exiting...");
                    return;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
    }

    public void PrintAllPatients()
    {
        Console.WriteLine("\n--- Patients ---");
        foreach (var p in _patientRepo.GetAll())
        {
            Console.WriteLine($"{p.Id}: {p.Name} ({p.Age} yrs, {p.Gender})");
        }
    }

    public void PrintPrescriptionsForPatient(int id)
    {
        if (_prescriptionMap.ContainsKey(id))
        {
            Console.WriteLine($"\n--- Prescriptions for Patient {id} ---");
            foreach (var pr in _prescriptionMap[id])
            {
                Console.WriteLine($"{pr.Id}: {pr.MedicationName} - {pr.DateIssued:yyyy-MM-dd}");
            }
        }
        else
        {
            Console.WriteLine("No prescriptions found for this patient.");
        }
    }

    private void AddNewPatient()
    {
        Console.Write("Enter ID: "); int id = int.Parse(Console.ReadLine());
        Console.Write("Enter Name: "); string name = Console.ReadLine();
        Console.Write("Enter Age: "); int age = int.Parse(Console.ReadLine());
        Console.Write("Enter Gender: "); string gender = Console.ReadLine();
        _patientRepo.Add(new Patient(id, name, age, gender));
        Console.WriteLine("Patient added.");
    }

    private void AddNewPrescription()
    {
        Console.Write("Enter ID: "); int id = int.Parse(Console.ReadLine());
        Console.Write("Enter Patient ID: "); int pid = int.Parse(Console.ReadLine());
        Console.Write("Enter Medication Name: "); string med = Console.ReadLine();
        DateTime dateIssued = DateTime.Now;
        _prescriptionRepo.Add(new Prescription(id, pid, med, dateIssued));
        BuildPrescriptionMap();
        Console.WriteLine("Prescription added.");
    }
}
