using System.Text.Json.Serialization;

namespace ProjectData
{
    public struct Discover
    {
        public DateTime DiscoveryDate { get; set; }
        public SpaceObject SpaceObj { get; set; }
        public Astronomer Discoverer { get; set; }

        public Discover(DateTime discoveryDate, SpaceObject spaceObj, Astronomer astronomer)
        {
            DiscoveryDate = discoveryDate;
            SpaceObj = spaceObj;
            Discoverer = astronomer;
        }
    }
    public enum ObjectType {
        //Звезды(0,23)
        Аналог_Солнца,
        Звезда_Вольфа_Райе,
        Звезда_из_каталога_LP,
        Звезда_типа_Лямбды_Волопаса,
        Звезда_типа_T_Тельца,
        Звезда_Хербига,
        Коричневый_карлик,
        Магнетар,
        Аномальный_рентгеновский_пульсар,
        Источник_мягких_повторяющихся_гаммавсплесков,
        Нейтронная_звезда,
        Пульсар,
        Миллисекундный_пульсар,
        Радиопульсар,
        Рентгеновский_пульсар,
        Ртутно_марганцевая_звезда,
        Убегающая_звезда,
        Углеродная_звезда,
        Химически_пекулярная_звезда,
        Бариевая_звезда,
        Бедная_гелием_звезда,
        Эмиссионная_звезда,
        Be_звезда,
        Переменные_типа_Лямбды_Эридана,
        //планеты(24,41)
        Горячий_юпитер,
        Экзопланета,
        Миниземли,
        Нептун,
        Горячий_нептун,
        Холодный_нептун,
        Планета_земной_группы,
        Суперземли,
        Планета_гигант,
        Газовый_гигант,
        Ледяной_гигант,
        Планета_сирота,
        Пульсарная_планета,
        Мезопланета,
        Карликовая_планета,
        Контактно_двойная_малая_планета,
        Экзолуна,
        Малая_планета,
        //Астеройды(42,60)
        Астероид_класса_X,
        Астероид_класса_A,
        Астероид_класса_B,
        Астероид_класса_C,
        Астероид_класса_D,
        Астероид_класса_E,
        Астероид_класса_F,
        Астероид_класса_G,
        Астероид_класса_J,
        Астероид_класса_K,
        Астероид_класса_L,
        Астероид_класса_M,
        Астероид_класса_O,
        Астероид_класса_P,
        Астероид_класса_Q,
        Астероид_класса_R,
        Астероид_класса_S,
        Астероид_класса_T,
        Астероид_класса_V,
        //Другие объекты(61,78)
        Естественный_спутник,
        Квазиспутник,
        Классический_объект_пояса_Койпера,
        Комета,
        Куча_щебня,
        Ледяной_спутник,
        Малые_тела_Солнечной_системы,
        Межзвёздный_объект,
        Метеороид,
        Нерегулярный_спутник,
        Планетезималь,
        Регулярный_спутник,
        Спутник_спутника,
        Экзокомета,
        Черная_дыра,
        Галактика,
        Квазар,
        Туманность
    }
    public enum Status
    {
        Discovered,
        Not_Discovered
    }
    public abstract class AbsSpaceObject
    {
        public abstract string Name { get; set; }
        public abstract double Mass { get; set; }
        public abstract ObjectType ObjType { get; set; }
    }
    public class SpaceObject : AbsSpaceObject
    {
        public override string Name { get; set; }
        public override double Mass { get; set; }
        public override ObjectType ObjType { get; set; }
        public double DistanceFromEarth { get; set; }
        public Status Status { get; set; }

        [JsonConstructor]
        public SpaceObject(string name = "", int objType = 0, double mass = 0, double distFromYearth = 0,int status=1)
        {
            Name = name;
            if(Enum.TryParse(objType.ToString(), out ObjectType type)|| !(objType > 78) || !(objType< 0))
                ObjType = type;
            else
            {
                throw new ArgumentException($"Недопустимый объект: '{objType}'");
            }
            if (Status.TryParse(status.ToString(), out Status stat)|| status==1)
                Status = stat;
            else
            {
                throw new ArgumentException($"Недопустимый статус: '{objType}'");
            }
            Mass = mass;
            DistanceFromEarth = distFromYearth;
        }
        public override string ToString()
        {
            return $"{Name} ({ObjType}) - Масса: {Mass} кг, Расстояние: {DistanceFromEarth} св. лет, Статус: {Status}";
        }

        public static bool operator ==(SpaceObject left, SpaceObject right) 
        { 
            if(left.Equals(right) || (left.ObjType == right.ObjType && left.Mass == right.Mass)){
                return true;
            }
            else 
                return false;
        }
        public static bool operator !=(SpaceObject left, SpaceObject right) { return !left.Equals(right); }
    }
    public struct FIO
    {
        public string FirstName;
        public string SurName;
        public string Patronymic;

        public FIO(string firstName="",  string surName="", string patronymic="")
        {
            FirstName = firstName;
            SurName = surName;
            Patronymic = patronymic;
        }
    }
    public class Astronomer
    {
        public FIO Fio { get; set; }
        public int Age {  get; set; }
        public List<SpaceObject> DiscoveredObjects { get; set; } = new List<SpaceObject>();
        [JsonConstructor]
        public Astronomer(string FirstName = "",string SurName = "",string Patronymic = "", int age=0)
        {
            if (age < 0|| age>152)
            {
                throw new ArgumentOutOfRangeException("Неверный возраст");
            }

            Fio = new FIO(FirstName, SurName, Patronymic);
            Age = age;
        }
        public Astronomer(FIO newFio, int age)
        {
            if (age < 0|| age>152)
            {
                throw new ArgumentOutOfRangeException("Неверный возраст");
            }

            Fio = newFio;
            Age = age;
        }

        public string FullName
        {
            get => $"{Fio.SurName} {Fio.FirstName} {Fio.Patronymic}";
        }
    }
}
