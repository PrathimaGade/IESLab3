using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using FTPAp.Models;
using FTPAp.Models.Utilities;
using static FTPAp.Models.Constants;
using FTP = FTPAp.Models.Utilities.FTP;
using Student = FTPAp.Models.Student;


namespace FTPAp
{

    class Program
    {
        private static object entries;

        static void Main(string[] args)
        {

            Student myrecord = new Student { StudentId = "200430242", FirstName = "BalaPrathima", LastName = "Gade" };
            List<Student> students = new List<Student>();
            List<string> directories = FTP.GetDirectory(Constants.FTP.BaseUrl);



            foreach (var directory in directories)
            {
                Student student = new Student() { AbsoluteUrl = Constants.FTP.BaseUrl };
                student.FromDirectory(directory);
                Console.WriteLine(directory);
                Console.WriteLine(student);
                string infoFilePath = student.FullPathUrl + "/" + Constants.Locations.InfoFile;
                string imageFilePath = student.FullPathUrl + "/" + Constants.Locations.ImageFile;


                bool fileExists = FTP.FileExists(infoFilePath);
                if (FTP.FileExists(student.InfoCSVPath))
                {
                    var csvBytes = FTP.DownloadFileBytes(student.InfoCSVPath);

                    string csvFileData = Encoding.ASCII.GetString(csvBytes, 0, csvBytes.Length);

                    string[] data = csvFileData.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
                    if (data.Length != 2)
                    {
                        Console.WriteLine("Error in your Age");
                    }
                    else
                    {
                        student.FromCSV(data[1]);
                        Console.WriteLine("Age:" + student.Age);
                    }


                }

              
                if (student.StudentId == "200430242")
                {
                    student.MyRecord = true;
                }
                if (fileExists == true)
                {
                    string csvPath = $@"/Users/shine/Desktop/Student/{directory}.csv";


                    //FTP.DownloadFile(infoFilePath, csvPath);
                    Console.WriteLine("Found info file:");
                }
                
                else
                {
                    Console.WriteLine("Could not find info file:");
                }

                Console.WriteLine("\t" + infoFilePath);



                bool imageFileExists = FTP.FileExists(imageFilePath);

                if (imageFileExists == true)
                {

                    Console.WriteLine("Found image file:");
                }
                else
                {
                    Console.WriteLine("Could not find image file:");
                }

                Console.WriteLine("\t" + imageFilePath);

                students.Add(student);
            }

          
            int count = students.Count();
            Console.WriteLine("Number of students:" + count);

            string inputStartsWith = "P";
            string inputContains = "ra";
            int countStartsWith = 0;
            int countContains = 0;
            Console.WriteLine("list of lastnames that starts with 'P' ");
            foreach (var student in students)
            {
                string name = student.LastName;
                if (name.StartsWith(inputStartsWith))
                {
                    Console.WriteLine(  "\n"+ student);
                    countStartsWith++;
                }
            }
            Console.WriteLine("list of lastnames that contains 'ra':");
            foreach (var student in students)
            {
                string name = student.LastName;
            if (name.Contains(inputContains))
            {
                Console.WriteLine( "\n" + student);
                countContains++;
            }
           }
            Console.WriteLine("Number of people having LastName that starts with 'P':" + countStartsWith);
            Console.WriteLine("Number of people having LastName that contains 'ra':" + countContains);

            Student me = students.SingleOrDefault(x => x.StudentId == myrecord.StudentId);
            Student meUsingFind = students.Find(x => x.StudentId == myrecord.StudentId);
            

            var avgage = students.Average(x => x.Age);
            var minage = students.Min(x => x.Age);
            var maxage = students.Max(x => x.Age);

            Console.WriteLine("Average Age:" + avgage);
            Console.WriteLine("Highest Age:" + maxage);
            Console.WriteLine("Lowest Age:" + minage);

            string studentsCSVPath = $"{Constants.Locations.DataFolder}//students.csv";
            //Establish a file stream to collect data from the response
            using (StreamWriter fs = new StreamWriter(studentsCSVPath))
            {
                foreach (var student in students)
                {
                    fs.WriteLine(student.ToCSV());
                }
            }

            string studentsjsonPath = $"{Constants.Locations.DataFolder}//students.json";
            //Establish a file stream to collect data from the response
            using (StreamWriter fs = new StreamWriter(studentsjsonPath))
            {
                foreach (var student in students)
                {
                    string Student = Newtonsoft.Json.JsonConvert.SerializeObject(student);
                    fs.WriteLine(Student.ToString());
                }
            }
            string localUploadFilePathjson = $"{Constants.Locations.DataFolder}//students.json";
            string localUploadFilePathxml = $"{Constants.Locations.DataFolder}//students.xml";
            string localUploadFilePathcsv = $"{Constants.Locations.DataFolder}//students.csv";


            FTP.UploadFile(localUploadFilePathjson, Constants.FTP.BaseUrl + "/200430242 BalaPrathima Gade/students.json");

           
            FTP.UploadFile(localUploadFilePathcsv, Constants.FTP.BaseUrl + "/200430242 BalaPrathima Gade/students.csv");

            string studentsxmlPath = $"{Constants.Locations.DataFolder}//students.xml";
            //Establish a file stream to collect data from the response
            using (StreamWriter fs = new StreamWriter(studentsxmlPath))
            {
                foreach (var student in students)
                {
                    XmlSerializer x = new XmlSerializer(students.GetType());
                    x.Serialize(fs, students);
                    Console.WriteLine();
                }
            }


            FTP.UploadFile(localUploadFilePathxml, Constants.FTP.BaseUrl + " / 200430242 BalaPrathima Gade/students.xml");

            return;
        }
    }
}
