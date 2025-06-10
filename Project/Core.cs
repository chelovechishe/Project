using ProjectData;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

public delegate void SpaceObjectDel(SpaceObject spaceObject);
public delegate void AstronomerDel(Astronomer astronomer);
public delegate void DiscoveryDel(Discover discovery);

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
        private const string DataFile = "SpaceCatalog_data.json";

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
        
        public void DeleteSpaceObject(string name)
        {
            var spaceObj = SpaceObjects.FirstOrDefault(o => o.Name == name);
            if (spaceObj == null)
            {
                Log($"Ошибка: объект '{name}' не найден");
            }
            else if (Discovers.Any(d => d.SpaceObj.Name == name))
            {
                Log($"Ошибка: нельзя удалить объект '{name}' - есть связанные открытия");
            }
            else
            {
                SpaceObjects.Remove(spaceObj);
                Log($"Объект '{name}' успешно удален");
            }
        }
        public void AddAstronomer(Astronomer astronomer)
        {
            if (astronomer.Fio.Equals(new FIO()))
                throw new InvalidDataException("FIO cannot be empty");

            Astronomers.Add(astronomer);
            Log($"Astronomer added: {astronomer.FullName} ({astronomer.Age})");
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
            Log($"Space Object discovered: {SpaceObj.Name} by {Astronomer.FullName}");
        }

        public SpaceObject[] SearchByKeyword(string keyword)
        {
            return SpaceObjects.Where(o =>
            o.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            o.ObjType.ToString().Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .ToArray();
        }

        public T[] Filter<T>(T[] objects, Predicate<T> condition)
        {
            List<T> result = new List<T>();

            foreach (var SpaceObject in objects)
            {
                if (condition(SpaceObject))
                {
                    result.Add(SpaceObject);
                }
            }

            return result.ToArray();
        }

        public Dictionary<Astronomer, SpaceObject[]> GetDiscoverByAstronomer(Discover[] discovers, SpaceObject[] SpaceObjects)
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
                for (int j = 1+i; j < sortedSpaceObject.Length - 1; j++)
                {
                    if (string.Compare(sortedSpaceObject[i].Name, sortedSpaceObject[j].Name)>j)
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
        public void LoadData(string FileName)
        {
            if (FileName == "") FileName = DataFile;
            else{
                if (FileName.Length > 5)
                {
                    if (FileName.Substring(FileName.Length - 5) != ".json")
                        FileName += ".json";
                }
                else FileName += ".json";
            }
            if (!File.Exists(FileName)) return;

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true
                };
                string json = File.ReadAllText(FileName, Encoding.UTF8);
                //byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
                var data = JsonSerializer.Deserialize<SpaceCatalogData>(json,options);

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
        public class SpaceCatalogData
        {
            public List<SpaceObject> SpaceObjects { get; set; }
            public List<Astronomer> Astronomers { get; set; }
            public List<Discover> Discovers { get; set; }
            [JsonConstructor]
            public SpaceCatalogData(List<SpaceObject> spaceObjects, List<Astronomer> astronomers, List<Discover> discovers)
            {
                SpaceObjects = spaceObjects ?? new List<SpaceObject>();
                Astronomers = astronomers ?? new List<Astronomer>();
                Discovers = discovers ?? new List<Discover>();
            }
        }

        public void SaveData(string FileName)
        {
            if (FileName == "") FileName = DataFile;
            var data = new SpaceCatalogData(SpaceObjects,Astronomers,Discovers);

            try
            {
                string json = JsonSerializer.Serialize(data);
                if (FileName.Length > 5)
                {
                    if (FileName.Substring(FileName.Length - 5) != ".json")
                    {
                        File.WriteAllText(FileName + ".json", json,Encoding.UTF8);
                    }
                    else File.WriteAllText(FileName, json,Encoding.UTF8);
                }
                else File.WriteAllText(FileName + ".json", json,Encoding.UTF8);
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
