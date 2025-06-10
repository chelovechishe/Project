using ProjectCore;
using ProjectData;
using ProjectUI;
using System.Data;
using System.Xml.Linq;
public class Test
{    
    public static void TestDataCreate()
    {
        string[] names = { "Звезда", "Планета", "Что-то", "Камень", "Пыль","Star","Planet","Something" };
        Random random = new Random();
        int a = random.Next(0, 2);
        Console.WriteLine(a);
        SpaceCatalog spaceCatalog = ConsoleInterface.spaceCatalog;

        var spaceObj = new SpaceObject();
        for(int i = 0; i < 10; i++)
        {
            spaceObj = new SpaceObject($"{names[random.Next(0, 8)]}_{i}", random.Next(0, 79), random.Next(0, 10000), random.Next(0, 10000));
            spaceCatalog.AddSpaceObject(spaceObj); 
        }
    }
}
