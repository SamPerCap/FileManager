using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace FileManager
{
    public class JsonWritter
    {
        static void Main(string[] args)
        {
            while (true)
            {
                //Diagnostics
                Stopwatch sw = new Stopwatch();

                Console.WriteLine("Remember to have a folder called 'DBFiles' \n\n");
                Console.WriteLine("Welcome. Please, choose what to do (integer):" +
                    "\n1.- Json Writter." +
                    "\n2.- XML Writter.");

                GatherInformation(Convert.ToInt32(Console.ReadLine()), sw);
            }
        }
        static void GatherInformation(int election, Stopwatch sw)
        {
            //Gather information about computer through console
            Console.WriteLine("Welcome to JSON creator. Please, insert the data for the computer: " +
                "\nBrand:");
            string brand = Console.ReadLine();
            Console.WriteLine("Colour:");
            string colour = Console.ReadLine();
            Console.WriteLine("Price (integer):");
            int price = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Year of release (integer):");
            int year = Convert.ToInt32(Console.ReadLine());

            //Create new entity from the data inserted
            Computer computer = new Computer();
            computer.Brand = brand;
            computer.Colour = colour;
            computer.Price = price;
            computer.ReleasedYear = year;

            string FileName = brand.ToUpper() + "_" + year;

            switch (election)
            {
                case 1:
                    //Start diagnostic
                    sw.Start();
                    string JSONresult = JsonConvert.SerializeObject(computer);
                    WriteJson(JSONresult, FileName);
                    //Stopdiagnostic
                    sw.Stop();
                    break;
                case 2:
                    //Start diagnostic
                    sw.Start();
                    WriteXML(computer, FileName);
                    //Stopdiagnostic
                    sw.Stop();
                    break;
            }

            Console.WriteLine("Elapsed={0}", sw.Elapsed);
        }
        static void WriteJson(string JSONresult, string FileName)
        {
            string path = @"C:\Users\Samuel\Desktop\DBFiles\" + FileName + ".json";

            if (File.Exists(path))
                File.Delete(path);

            using (var sw = new StreamWriter(path, true))
            {
                sw.WriteLine(JSONresult.ToString());
                sw.Close();
            }
        }

        static void WriteXML(Computer computer, string FileName)
        {
            string path = @"C:\Users\Samuel\Desktop\DBFiles\" + FileName + ".XML";
            if (File.Exists(path))
                File.Delete(path);

            using (XmlWriter writer = XmlWriter.Create(path))
            {
                writer.WriteStartElement("Computer");
                writer.WriteElementString("Brand", computer.Brand);
                writer.WriteElementString("Colour", computer.Colour);
                writer.WriteElementString("Price", computer.Price.ToString());
                writer.WriteElementString("Year", computer.ReleasedYear.ToString());
            }
        }
    }
}
