using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectData;
using System.Reflection.PortableExecutable;
using System.Transactions;
using Serilog;
using static System.Reflection.Metadata.BlobBuilder;
using System.Text.Json;

namespace ProjectCore
{
    public interface ISearchable
    {
        SpaceObject[] SearchByKeyword(string keyword);
    }

    public class SpaceCatalog : ISearchable, IDisposable
    {
        private List<SpaceObject> SpaceObjects = new List<SpaceObject>();
        private List<Astronomer> Astronomers = new List<Astronomer>();
        private List<Discover> Discovers = new List<Discover>();
        private const string DataFile = "library_data.json";

        public event Action<SpaceObject, Astronomer> NewDiscovered;
        public event Action<string> OnLogMessage;

        public List<SpaceObject> getSpaceObj()
        {
            return SpaceObjects;
        }
        public SpaceObject this[string Name]
        {
            get => SpaceObjects.FirstOrDefault(b => b.Name == Name);
        }

        public void AddSpaceObject(SpaceObject spaceObj)
        {
            if (string.IsNullOrWhiteSpace(spaceObj.Name))
                throw new InvalidDataException("Name cannot be empty");

            if (SpaceObjects.Any(b => b.Name == spaceObj.Name))
                throw new InvalidDataException("Space Object with this Name already exists");

            SpaceObjects.Add(spaceObj);
            Log($"Space object added: {spaceObj.Name} ({spaceObj.ObjType})");
        }

        public void AddAstronomer(Astronomer astronomer)
        {
            if (astronomer.Fio.Equals(new FIO()))
                throw new InvalidDataException("FIO cannot be empty");

            Astronomers.Add(astronomer);
            Log($"Reader added: {astronomer.FullName} ({astronomer.Age})");
        }

        public void UnDiscover(string Name, FIO Fio)
        {
            var SpaceObj = SpaceObjects.FirstOrDefault(o => o.Name == Name);
            var Astronomer = Astronomers.FirstOrDefault(a => a.Fio.Equals(Fio));

            if (SpaceObj is null || Astronomer == null || SpaceObj.Status != Status.Not_Discovered)
                throw new InvalidOperationException("Cannot discover object");

            SpaceObj.Status = Status.Discovered;
            Astronomer.DiscoveredObjects.Add(SpaceObj);
            var Discover = new Discover(DateTime.Now, SpaceObj, Astronomer);
            Discovers.Add(Discover);

            NewDiscovered?.Invoke(SpaceObj, Astronomer);
            Log($"Book borrowed: {SpaceObj.Name} by {Astronomer.FullName}");
        }

        public SpaceObject[] SearchByKeyword(string keyword)
        {
            return SpaceObjects.Where(o =>
            o.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            o.ObjType.ToString().Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .ToArray();
        }

        public SpaceObject[] Filter<T>(SpaceObject[] objects, Predicate<SpaceObject> condition)
        {
            List<SpaceObject> result = new List<SpaceObject>();

            foreach (var SpaceObject in SpaceObjects)
            {
                if (condition(SpaceObject))
                {
                    result.Add(SpaceObject);
                }
            }

            return result.ToArray();
        }

        public Dictionary<Astronomer, SpaceObject[]> GetTransactionsByAstronomer(Discover[] discovers, SpaceObject[] SpaceObjects)
        {
            var result = new Dictionary<Astronomer, SpaceObject[]>();

            foreach (var discover in discovers)
            {
                if (!result.ContainsKey(discover.Discoverer))
                {
                    var discoverObjects = discovers
                        .Where(d => d.Discoverer.Fio.Equals(discover.Discoverer.Fio))
                        .Select(d => SpaceObjects.FirstOrDefault(o => o.Name == d.SpaceObj.Name))
                        .Where(o => !o.Equals(default(SpaceObject)))
                        .ToArray();

                    result.Add(discover.Discoverer, discoverObjects);
                }
            }

            return result;
        }

        public SpaceObject[] GetObjecstSortedByName(SpaceObject[] SpaceObjects)
        {
            SpaceObject[] sortedSpaceObject = new SpaceObject[SpaceObjects.Length];
            Array.Copy(SpaceObjects, sortedSpaceObject, SpaceObjects.Length);

            for (int i = 0; i < sortedSpaceObject.Length - 1; i++)
            {
                for (int j = 0; j < sortedSpaceObject.Length - i - 1; j++)
                {
                    if (String.Compare(sortedSpaceObject[j].Name, sortedSpaceObject[j + 1].Name)>j)
                    {
                        SpaceObject temp = sortedSpaceObject[j];
                        sortedSpaceObject[j] = sortedSpaceObject[j + 1];
                        sortedSpaceObject[j + 1] = temp;
                    }
                }
            }

            return sortedSpaceObject;
        }
        private void Log(string message)
        {
            OnLogMessage?.Invoke(message);
        }
        public void ListAllTypes()
        {
            foreach(ObjectType objectType in Enum.GetValues(typeof(ObjectType)))
            {
                Console.WriteLine($"{(int)objectType}:{objectType}");
            }
        }
        public void LoadData()
        {
            if (!File.Exists(DataFile)) return;

            try
            {
                string json = File.ReadAllText(DataFile);
                var data = JsonSerializer.Deserialize<LibraryData>(json);

                SpaceObjects = data.SpaceObjects ?? new List<SpaceObject>();
                Astronomers = data.Astronomers ?? new List<Astronomer>();
                Discovers = data.Discovers ?? new List<Discover>();

                Console.WriteLine("Данные успешно загружены.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке: {ex.Message}");
                SpaceObjects = new List<SpaceObject>();
                Astronomers = new List<Astronomer>();
                Discovers = new List<Discover>();
            }
        }
        private class LibraryData
        {
            public List<SpaceObject> SpaceObjects { get; set; }
            public List<Astronomer> Astronomers { get; set; }
            public List<Discover> Discovers { get; set; }
        }

        public void SaveData()
        {
            var data = new
            {
                spaceObjects = SpaceObjects,
                astronomers = Astronomers,
                discovers = Discovers
            };

            try
            {
                string json = JsonSerializer.Serialize(data);
                File.WriteAllText(DataFile, json);
                Console.WriteLine("Данные успешно сохранены.");
                Console.WriteLine(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении: {ex.Message}");
            }
        }
        public void Dispose()
        {
            SpaceObjects.Clear();
            Astronomers.Clear();
            Discovers.Clear();
        }
    }
}
