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
                if (fileExists == true)
                {
                    string csvPath = $@"/Users/shine/Desktop/Student/{directory}.csv";


                    //FTP.DownloadFile(infoFilePath, csvPath);
                    Console.WriteLine("Found info file:");
                }
                if (student.StudentId == "200430242")
                {
                    student.MyRecord = true;
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

            //Save to CSV
            string studentsCSVPath = $"{Constants.Locations.DataFolder}//students.csv";



            int count = students.Count();
            Console.WriteLine("Number of students:" + count);

            string inputStartsWith = "P";
            string inputContains = "ra";
            int countStartsWith = 0;
            int countContains = 0;
            foreach (var student in students)
            {
                string name = student.FirstName;
                if (name.StartsWith(inputStartsWith))
                {
                    Console.WriteLine("Last starts with " + inputStartsWith + " " + student);
                    countStartsWith++;
                }
            
            Console.WriteLine("Number of people having LastName that starts with 'P':" + countStartsWith);

            
            if (name.Contains(inputContains))
            {
                Console.WriteLine("Last starts with " + inputContains + " " + student);
                countContains++;
            }
           }
            Console.WriteLine("Number of people having LastName that contains 'ra':" + countContains);

            Student me = students.SingleOrDefault(x => x.StudentId == myrecord.StudentId);
            Student meUsingFind = students.Find(x => x.StudentId == myrecord.StudentId);
            

            var avgage = students.Average(x => x.Age);
            var minage = students.Min(x => x.Age);
            var maxage = students.Max(x => x.Age);

            Console.WriteLine("Average Age:" + avgage);
            Console.WriteLine("Highest Age:" + maxage);
            Console.WriteLine("Lowest Age:" + minage);

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

            string studentsxmlPath = $"{Constants.Locations.DataFolder}//students.xml";
            //Establish a file stream to collect data from the response
            using (StreamWriter fs = new StreamWriter(studentsxmlPath))
            {
                XmlSerializer x = new XmlSerializer(students.GetType());
                x.Serialize(fs, students);
                Console.WriteLine();
            }
            string localUploadFilePath = @"{Constants.Locations.DataFolder}//students.json";
 

            FTP.UploadFile(localUploadFilePath, Constants.FTP.BaseUrl + "/200430242 BalaPrathima Gade/students.json");


            //XmlSerializer serialiser = new XmlSerializer(typeof(List<Student>));

            //TextWriter Filestream = new StreamWriter(@"/Users/shine/Desktop/students.xml");

            //serialiser.Serialize(Filestream, students);

            //Filestream.Close();


            return;



        }
    }
}
