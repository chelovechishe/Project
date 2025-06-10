using ProjectCore;
using ProjectData;

namespace ProjectUI
{
    public class ConsoleInterface
    {
        public static SpaceCatalog spaceCatalog = new SpaceCatalog();
        private const string DataFile = "SpaceCatalog_data.json";

        public ConsoleInterface()
        {
            spaceCatalog.LoadData("");

            spaceCatalog.OnLogMessage += message => Console.WriteLine($"[LOG] {DateTime.Now}: {message}");
            spaceCatalog.NewDiscovered += (SpaceObject, Astronomer) =>
                Console.WriteLine($"Notification: {Astronomer.FullName} borrowed '{SpaceObject.Name}'");
        }

        public void Run()
        {
            Console.WriteLine("Space Objects Catalog System");
            while (true)
            {
                Console.WriteLine("\nMenu:");
                Console.WriteLine("1. Add Space Object");
                Console.WriteLine("2. Delete Space Object");
                Console.WriteLine("3. Add Astronomer");
                Console.WriteLine("4. Discover Object");
                Console.WriteLine("5. Search Object");
                Console.WriteLine("6. Get Discover ByA stronomer");
                Console.WriteLine("7. Filter Objects");
                Console.WriteLine("8. List All Objects");
                Console.WriteLine("9. List All Objects Types");
                Console.WriteLine("10. Load Data");
                Console.WriteLine("11. Save Data");
                Console.WriteLine("12. Exit");

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
                            DellSpaceObject();
                            break;
                        case "3":
                            AddAstronomer();
                            break;
                        case "4":
                            UnDiscover();
                            break;
                        case "5":
                            SearchObjects();
                            break;
                        case "6":
                            ListObjects();
                            break;
                        case "7":
                            FilterObjects();
                            break;
                        case "8":
                            ListObjects();
                            break;
                        case "9":
                            spaceCatalog.ListAllTypes();
                            break;
                        case "10":
                            Console.WriteLine("Enter Load FileName: ");
                            spaceCatalog.LoadData(Console.ReadLine());
                            break;
                        case "11":
                            Console.WriteLine("Enter Save FileName: ");
                            spaceCatalog.SaveData(Console.ReadLine());
                            break;
                        case "12":
                            spaceCatalog.SaveData("");
                            spaceCatalog.Dispose();
                            return;
                        case "TEST":
                            Test.TestDataCreate(); 
                            break;
                        default:
                            Console.WriteLine("Invalid option");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
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
        private void DellSpaceObject()
        {
            Console.Write("Enter Space Object Name: ");
            var name = Console.ReadLine();
            spaceCatalog.DeleteSpaceObject(name);
        }
        private void FilterObjects()
        {
            Console.WriteLine("\nFilter Space Objects:");
            Console.WriteLine("1. By Mass (more than)");
            Console.WriteLine("2. By Distance from Earth (less than)");
            Console.WriteLine("3. By Type");
            Console.Write("Select filter: ");

            var filterChoice = Console.ReadLine();
            var allObjects = spaceCatalog.getSpaceObj().ToArray();
            SpaceObject[] filteredObjects = Array.Empty<SpaceObject>();

            switch (filterChoice)
            {
                case "1": 
                    Console.Write("Enter minimum mass: ");
                    double minMass = double.Parse(Console.ReadLine());
                    filteredObjects = spaceCatalog.Filter(allObjects, o => o.Mass >= minMass);
                    break;

                case "2":
                    Console.Write("Enter maximum distance: ");
                    double maxDist = double.Parse(Console.ReadLine());
                    filteredObjects = spaceCatalog.Filter(allObjects, o => o.DistanceFromEarth <= maxDist);
                    break;

                case "3": 
                    Console.Write("Enter type number: ");
                    int typeNum = int.Parse(Console.ReadLine());
                    filteredObjects = spaceCatalog.Filter(allObjects, o => (int)o.ObjType == typeNum);
                    break;

                default:
                    Console.WriteLine("Invalid filter option");
                    return;
            }

            Console.WriteLine($"\nFound {filteredObjects.Length} objects:");
            foreach (var obj in filteredObjects)
            {
                Console.WriteLine(obj.ToString());
            }
        }
        private void AddAstronomer()
        {
            Console.Write("Enter astronomer first name: ");
            var name = Console.ReadLine();

            Console.Write("Enter astronomer sur name: ");
            var surName = Console.ReadLine();

            Console.Write("Enter astronomer Patronymic: ");
            var patronymic = Console.ReadLine();

            Console.Write("Enter age: ");
            var age = int.Parse(Console.ReadLine());

            var astronomer = new Astronomer(name,surName,patronymic,age);

            spaceCatalog.AddAstronomer(astronomer);
            Console.WriteLine("Astronomer added successfully");
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
                Console.WriteLine(spaceObj.ToString());
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
