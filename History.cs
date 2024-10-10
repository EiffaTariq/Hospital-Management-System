using HospitalDAL;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace HospitalDAL
{
    public class DeletionHistoryRecord
    {
        public int PatientId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Disease { get; set; }
        public DateTime DeletionDate { get; set; }
        public string Timestamp { get; set; } // New property for additional metadata
    }

    public class DeletionHistoryManager
    {
        private string patientHistoryFilePath = @"D:DeletedPatients.json";  // New file for deleted patients
        private string doctorHistoryFilePath = @"D:DeletedDoctors.json";    // New file for deleted doctors
        private string appointmentHistoryFilePath = @"D:DeletedAppointments.json"; // New file for deleted appointments


        public Patient GetPatientById(int patientId)
        {
            string connString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=Hospital;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

            Patient patient = null;

            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                sqlConnection.Open();
                string query = "SELECT PatientId, Name, Email, Disease FROM Patient WHERE PatientId = @PatientId";

                using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                {
                    cmd.Parameters.AddWithValue("@PatientId", patientId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) // Check if patient exists
                        {
                            patient = new Patient
                            {
                                PatientId = reader.GetInt32(reader.GetOrdinal("PatientId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Disease = reader.IsDBNull(reader.GetOrdinal("Disease")) ? null : reader.GetString(reader.GetOrdinal("Disease"))
                            };
                        }
                    }
                }
            }

            return patient; // Return retrieved patient or null if not found
        }

        public void SavePatientData(int patientId)
        {
            Patient patient = GetPatientById(patientId); // Retrieve patient data by ID
            string filePath = @"D:\Web\DeletedPatients";
            if (patient != null)
            {
                List<Patient> patients = File.Exists(filePath)
                    ? JsonSerializer.Deserialize<List<Patient>>(File.ReadAllText(filePath)) ?? new List<Patient>()
                    : new List<Patient>();
                var existingPatient = patients.Find(p => p.PatientId == patient.PatientId);
                if (existingPatient != null)
                {
                    existingPatient.Name = patient.Name;
                    existingPatient.Email = patient.Email;
                    existingPatient.Disease = patient.Disease;
                    Console.WriteLine($"Updated patient data for ID: {patient.PatientId}");
                }
                else
                {
                    patients.Add(patient);
                    Console.WriteLine($"Added new patient data for ID: {patient.PatientId}");
                }

                string updatedJson = JsonSerializer.Serialize(patients, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, updatedJson);
            }
            else
            {
                Console.WriteLine($"No patient found with ID: {patientId}");
            }
        }



        public Doctor GetDoctorById(int doctorId)
        {
            Doctor doctor = null;
            string connString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=Hospital;Integrated Security=True;Connect Timeout=30;";

            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                sqlConnection.Open();
                string query = "SELECT DoctorId, DoctorName, DoctorSpecialization FROM Doctor WHERE DoctorId = @DoctorId";
                using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                {
                    cmd.Parameters.AddWithValue("@DoctorId", doctorId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            doctor = new Doctor
                            {
                                DoctorId = (int)reader["DoctorId"],
                                DoctorName = (string)reader["DoctorName"],
                                DoctorSpecialization = reader.IsDBNull(reader.GetOrdinal("DoctorSpecialization")) ? null : (string)reader["DoctorSpecialization"]
                            };
                        }
                    }
                }
            }
            return doctor;
        }


        public void SaveDoctorData(int doctorId)
        {
            Doctor doctor = GetDoctorById(doctorId);
            string filePath = @"D:\Web\DeletedDoctors.json";

            if (doctor != null)
            {
                List<Doctor> doctors = File.Exists(filePath)
                    ? JsonSerializer.Deserialize<List<Doctor>>(File.ReadAllText(filePath)) ?? new List<Doctor>()
                    : new List<Doctor>();
                var existingDoctor = doctors.Find(d => d.DoctorId == doctor.DoctorId);

                //if (existingDoctor != null)
                //{
                //    existingDoctor.DoctorName = doctor.DoctorName;
                //    existingDoctor.DoctorSpecialization = doctor.DoctorSpecialization;
                //    Console.WriteLine($"Updated doctor data for ID: {doctor.DoctorId}");
                //}
                //else
                //{
                //    doctors.Add(doctor);
                //    Console.WriteLine($"Added new doctor data for ID: {doctor.DoctorId}");
                //}

                string updatedJson = JsonSerializer.Serialize(doctors, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, updatedJson);
            }
            else
            {
                Console.WriteLine($"No doctor found with ID: {doctorId}");
            }
        }


        public Appointment GetAppointmentById(int appointmentId)
        {
            Appointment app = null;
            string connString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=Hospital;Integrated Security=True;Connect Timeout=30;";

            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                sqlConnection.Open();
                string query = "SELECT DoctorId, DoctorName, DoctorSpecialization FROM Doctor WHERE DoctorId = @DoctorId";
                using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                {
                    cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);


                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            app = new Appointment
                            {
                                //    DoctorId = (int)reader["DoctorId"],
                                //    DoctorName = (string)reader["DoctorName"],
                                //    DoctorSpecialization = reader.IsDBNull(reader.GetOrdinal("DoctorSpecialization")) ? null : (string)reader["DoctorSpecialization"]
                                AppointmentId = appointmentId,
                                PatientId = 1, // Replace with actual patient ID
                                DoctorId = 2, // Replace with actual doctor ID
                                AppointmentDate = DateTime.Now // Replace with actual appointment date

                            };
                        }
                    }
                }
            }
            return app;
            //return new Appointment 
            //{
            //    AppointmentId = appointmentId,
            //    PatientId = 1, // Replace with actual patient ID
            //    DoctorId = 2, // Replace with actual doctor ID
            //    AppointmentDate = DateTime.Now // Replace with actual appointment date
            //};
        }
        public void SaveAppointmentData(int appointmentId)
        {
            Appointment appointment = GetAppointmentById(appointmentId);
            string filePath = @"D:\Web\Appointments.json";

            if (appointment != null)
            {
                List<Appointment> appointments = File.Exists(filePath)
                    ? JsonSerializer.Deserialize<List<Appointment>>(File.ReadAllText(filePath)) ?? new List<Appointment>()
                    : new List<Appointment>();

                var existingAppointment = appointments.Find(a => a.AppointmentId == appointment.AppointmentId);

                if (existingAppointment != null)
                {
                    existingAppointment.PatientId = appointment.PatientId;
                    existingAppointment.DoctorId = appointment.DoctorId;
                    existingAppointment.AppointmentDate = appointment.AppointmentDate;
                    Console.WriteLine($"Updated appointment data for ID: {appointment.AppointmentId}");
                }
                else
                {
                    appointments.Add(appointment);
                    Console.WriteLine($"Added new appointment data for ID: {appointment.AppointmentId}");
                }

                string updatedJson = JsonSerializer.Serialize(appointments, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, updatedJson);
            }
            else
            {
                Console.WriteLine($"No appointment found with ID: {appointmentId}");
            }
        }


        public void SavePatientDeletionRecord(Patient patient)
        {
            SaveDeletionRecord(patient, patientHistoryFilePath);
        }

        public void SaveDoctorDeletionRecord(Doctor doctor)
        {
            SaveDeletionRecord(doctor, doctorHistoryFilePath);
        }

        public void SaveAppointmentDeletionRecord(Appointment appointment)
        {
            SaveDeletionRecord(appointment, appointmentHistoryFilePath);
        }

        private void SaveDeletionRecord<T>(T record, string filePath)
        {
            try
            {
                var deletionRecord = new DeletionHistoryRecord
                {
                    // Populate based on type
                    PatientId = record is Patient p ? p.PatientId : (record is Doctor d ? d.DoctorId : 0),
                    Name = record is Patient pp ? pp.Name : (record is Doctor dd ? dd.DoctorName : string.Empty),
                    Email = record is Patient pp1 ? pp1.Email : string.Empty,
                    Disease = record is Patient pp2 ? pp2.Disease : string.Empty,
                    DeletionDate = DateTime.Now,
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") // Add formatted timestamp
                };

                List<DeletionHistoryRecord> historyRecords = GetHistoryRecords(filePath);
                historyRecords.Add(deletionRecord);
                SaveHistoryToFile(historyRecords, filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while saving deletion record: {ex.Message}");
            }
        }

        private List<DeletionHistoryRecord> GetHistoryRecords(string filePath)
        {
            if (!File.Exists(filePath))
                return new List<DeletionHistoryRecord>();

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<DeletionHistoryRecord>>(json) ?? new List<DeletionHistoryRecord>();
        }

        private void SaveHistoryToFile(List<DeletionHistoryRecord> historyRecords, string filePath)
        {
            string json = JsonSerializer.Serialize(historyRecords);
            File.WriteAllText(filePath, json);
        }

        public void DisplayDeletionHistory(string type)
        {
            string filePath = type switch
            {
                "patient" => patientHistoryFilePath,
                "doctor" => doctorHistoryFilePath,
                "appointment" => appointmentHistoryFilePath,
                _ => throw new ArgumentException("Invalid type specified.")
            };

            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"No {type} deletion history found.");
                    return;
                }

                string json = File.ReadAllText(filePath);
                var historyRecords = JsonSerializer.Deserialize<List<DeletionHistoryRecord>>(json);

                if (historyRecords == null || historyRecords.Count == 0)
                {
                    Console.WriteLine($"No {type} deletion records available.");
                    return;
                }

                Console.WriteLine($"{type} Deletion History:");
                foreach (var record in historyRecords)
                {
                    Console.WriteLine($"ID: {record.PatientId}, Name: {record.Name}, Email: {record.Email}, Disease: {record.Disease}, " +
                                      $"Deleted On: {record.DeletionDate}, Timestamp: {record.Timestamp}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading {type} deletion history: {ex.Message}");
            }
        }
    }
}
public class History<T>
{
    private string filePath;


    public History(string fileName)
    {
        filePath = $"{fileName}.txt";
    }

    public void SaveToHistory(T record)
    {
        StreamWriter streamWriter = new StreamWriter(filePath, true);
        string json = JsonSerializer.Serialize(record);
        streamWriter.WriteLine(json);
        streamWriter.Close();
    }

    public void DisplayHistory()
    {
        StreamReader streamReader = new StreamReader(filePath);

        Console.WriteLine($"Here is the list of the deleted records..");

        bool findFlag = false;
        try
        {
            while (streamReader != null)
            {
                findFlag = true;
                string json = streamReader.ReadLine();
                Console.WriteLine(JsonSerializer.Deserialize<T>(json));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error...");
        }
        finally
        {
            if (!findFlag)
            {
                Console.WriteLine("No record Found...");
            }
        }
    }
}