using ProjectCore;
using ProjectData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;
using System.Transactions;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

namespace ProjectUI
{
    public class ConsoleInterface
    {
        private SpaceCatalog spaceCatalog = new SpaceCatalog();
        private const string DataFile = "SpaceCatalog_data.json";

        public ConsoleInterface()
        {
            spaceCatalog.LoadData();

            spaceCatalog.OnLogMessage += message => Console.WriteLine($"[LOG] {DateTime.Now}: {message}");
            spaceCatalog.NewDiscovered += (SpaceObject, Astronomer) =>
                Console.WriteLine($"Notification: {Astronomer.FullName} borrowed '{SpaceObject.Name}'");

            spaceCatalog.LoadData();
            AppDomain.CurrentDomain.ProcessExit += (s, e) => spaceCatalog.SaveData();
        }

        public void Run()
        {
            Console.WriteLine("Space Objects Catalog System");
            while (true)
            {
                Console.WriteLine("\nMenu:");
                Console.WriteLine("1. Add Space Object");
                Console.WriteLine("2. Add Astronomer");
                Console.WriteLine("3. Discover Object");
                Console.WriteLine("4. Search Object");
                Console.WriteLine("5. List All Objects");
                Console.WriteLine("6. List All Objects Types");
                Console.WriteLine("7. Exit");

                Console.Write("Select option: ");
                var input = Console.ReadLine();

                try
                {
                    switch (input)
                    {
                        case "1":
                            AddSpaceObject();
                            break;
                        case "2":
                            AddAstronomer();
                            break;
                        case "3":
                            UnDiscover();
                            break;
                        case "4":
                            SearchObjects();
                            break;
                        case "5":
                            ListObjects();
                            break;
                        case "6":
                            spaceCatalog.ListAllTypes();
                            break;
                        case "7":
                            spaceCatalog.SaveData();
                            return;
                        default:
                            Console.WriteLine("Invalid option");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                finally
                {
                    spaceCatalog.SaveData();
                }
            }
        }

        private void AddSpaceObject()
        {
            Console.Write("Enter Space Object Name: ");
            var name = Console.ReadLine();

            Console.Write("Enter Object Mass: ");
            var mass = double.Parse(Console.ReadLine());

            Console.Write("Enter Object Type Number: ");
            var objType = int.Parse(Console.ReadLine());

            Console.Write("Enter Object Distance from Earth: ");
            var distance = double.Parse(Console.ReadLine());

            var spaceObj = new SpaceObject(name,objType,mass,distance);

            spaceCatalog.AddSpaceObject(spaceObj);
            Console.WriteLine("Object added successfully");
        }

        private void AddAstronomer()
        {
            Console.Write("Enter reader first name: ");
            var name = Console.ReadLine();

            Console.Write("Enter reader sur name: ");
            var surName = Console.ReadLine();

            Console.Write("Enter reader Patronymic: ");
            var patronymic = Console.ReadLine();

            Console.Write("Enter age: ");
            var age = int.Parse(Console.ReadLine());

            var astronomer = new Astronomer(name,surName,patronymic,age);

            spaceCatalog.AddAstronomer(astronomer);
            Console.WriteLine("Reader added successfully");
        }

        private void UnDiscover()
        {
            Console.Write("Enter object name: ");
            var name = Console.ReadLine();

            Console.Write("Enter astronomer firstname: ");
            var firstname = Console.ReadLine();

            Console.Write("Enter astronomer surname: ");
            var surname = Console.ReadLine();

            Console.Write("Enter astronomer patronymic: ");
            var patronymic = Console.ReadLine();

            spaceCatalog.UnDiscover(name, new FIO(firstname, surname, patronymic));
            Console.WriteLine("Object discowered successfully");
        }

        private void SearchObjects()
        {
            Console.Write("Enter search keyword: ");
            var keyword = Console.ReadLine();

            var results = spaceCatalog.SearchByKeyword(keyword);

            Console.WriteLine("\nSearch results:");
            foreach (var spaceObj in results)
            {
                Console.WriteLine(spaceCatalog.ToString());
            }
        }

        private void ListObjects()
        {
            var objects = spaceCatalog.GetObjecstSortedByName(spaceCatalog.getSpaceObj().ToArray());

            Console.WriteLine("\nAll Objects sorted by name:");
            foreach (var obj in objects)
            {
                Console.WriteLine(obj.ToString());
            }
        }
    }
}
