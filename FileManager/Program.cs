using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace FileManager
{
    public class JsonWritter
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Remember to have a folder called 'DBFiles' \n\n");

            while (true)
            {
                //Diagnostics
                Stopwatch sw = new Stopwatch();
                Console.WriteLine("\nWelcome. Select services (integer):" +
                    "\n1.- Json/XML string validator." +
                    "\n2.- Json/XML Writter.");

                if (Console.ReadLine() == "1")
                {
                    Console.WriteLine("Please, choose what to do (integer):" +
                  "\n1.- Validate with Json Schema." +
                  "\n2.- Validate with XML Schema.");
                    ValidateInput(Console.ReadLine());
                }
                else if (Console.ReadLine() == "2")
                {
                    Console.WriteLine("Please, choose what to do (integer):" +
                  "\n1.- Json Writter." +
                  "\n2.- XML Writter.");

                    GatherInformation(Convert.ToInt32(Console.ReadLine()), sw);
                }
                else
                {
                    Console.WriteLine("\nPlease try again \n");
                }
            }
        }

        private static void ValidateInput(string option)
        {

            Console.WriteLine("Paste your string below:\n");
            string input = Console.Read().ToString();
            if (option == "1")
            {
                ValidateJson(input);
            }
            else if (option == "2")
            {
                ValidateXML(input);
            }
        }

        private static void ValidateJson(string input)
        {
            JSchema schema = JSchema.Parse(File.ReadAllText("../../../JsonFiles/ComputerSchema.json"));
            JObject jobject = JObject.Parse(input);

            IList<string> validationEvents = new List<string>();
            if (jobject.IsValid(schema, out validationEvents))
            {
                Console.WriteLine("\nYour json matches our schema: \n");
                Console.WriteLine(jobject);
            }
            else
            {
                Console.WriteLine("\nYour doesn't match our schema: \n");
                foreach (string validationEvent in validationEvents)
                {
                    Console.WriteLine(validationEvent);
                }
            }
        }

        private static void ValidateXML(string input)
        {

            XmlReader xmlReader = null;
            //XmlDocument xmlDoc = new XmlDocument();
            //xmlDoc.LoadXml(input);
            try
            {
                byte[] byteArray = Encoding.ASCII.GetBytes(input);
                MemoryStream stream = new MemoryStream(byteArray);
                stream.Position = 2;

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                settings.ValidationFlags |= System.Xml.Schema.XmlSchemaValidationFlags.ProcessSchemaLocation;
                settings.ValidationFlags |= System.Xml.Schema.XmlSchemaValidationFlags.ReportValidationWarnings;
                settings.ValidationEventHandler += new System.Xml.Schema.ValidationEventHandler(ValidationCallback);
                settings.Schemas.Add(null, XmlReader.Create("../../../XMLFiles/ComputerSchema.xml"));

                xmlReader = XmlReader.Create(stream, settings);

                while (xmlReader.Read()) { }
                Console.WriteLine("\nYour XML matches our schema! \n");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message + e.StackTrace);
            }
            finally
            {
                if (xmlReader != null)
                {
                    xmlReader.Close();
                }
            }
        }

        private static void ValidationCallback(object sender, System.Xml.Schema.ValidationEventArgs e)
        {
            Console.WriteLine("Your doesn't match our schema:");
            throw new Exception("Error: " + e.Message);
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
