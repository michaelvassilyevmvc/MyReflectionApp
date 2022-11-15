using MyReflectionApp;
using System.Reflection;

Console.Title = "Изучение рефлексии";

NetworkMonitor.BootstrapFromConfiguration();
Console.WriteLine("Что то пошло не так ...");

NetworkMonitor.Warn();


Console.ReadLine();

static string GetTypeFromConfiguration()
{
    return "MyReflectionApp.Person";
}

static void CodeFromSecondModule()
{

    //string name = "Михаил";
    //var stringType = typeof(string);
    //Console.WriteLine(stringType);


    var currentAssembly = Assembly.GetExecutingAssembly();
    var typesFromCurrentAssembly = currentAssembly.GetTypes();

    //foreach (var type in typesFromCurrentAssembly)
    //{
    //    Console.WriteLine(type.Name);
    //}

    var oneTypeFromCurrentAssembly = currentAssembly.GetType("MyReflectionApp.Person");

    foreach (var constructor in oneTypeFromCurrentAssembly.GetConstructors())
    {
        Console.WriteLine(constructor);
    }

    foreach (var method in oneTypeFromCurrentAssembly.GetMethods(BindingFlags.Public | BindingFlags.NonPublic))
    {
        Console.WriteLine($"method: {method} - IsPublic: {method.IsPublic}");
    }

    foreach (var field in oneTypeFromCurrentAssembly.GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
    {
        Console.WriteLine(field);
    }


    //var externalAssembly = Assembly.Load("System.Text.Json");
    //var typesFromExternalAssembly = externalAssembly.GetTypes();
    //var oneTypeFromExternalAssembly = externalAssembly.GetType("System.Text.Json.JsonProperty");

    //var modulesFromExternalAssembly = externalAssembly.GetModules();
    //var oneModuleFromExternalAssembly = externalAssembly.GetModule("System.Text.Json.dll");

    //var typesFromModuleExternalAssembly = oneModuleFromExternalAssembly?.GetTypes();
    //var oneTypeFromModuleFromExternalAssembly = oneModuleFromExternalAssembly?.GetType("System.Text.Json.JsonProperty");


}

static void CodeFromThirdModule()
{
    var personType = typeof(Person);
    var personConstructors = personType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

    foreach (var personConsturctor in personConstructors)
    {
        Console.WriteLine(personConsturctor);
    }

    var privatePersonConstructor = personType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
        null,
        new Type[] { typeof(string), typeof(int) }, null);

    var person1 = personConstructors[0].Invoke(null);
    var person2 = personConstructors[1].Invoke(new object[] { "Kevin" });
    var person3 = personConstructors[2].Invoke(new object[] { "Kevin", 40 });

    var person4 = Activator.CreateInstance("MyReflectionApp", "MyReflectionApp.Person").Unwrap();
    var person5 = Activator.CreateInstance("MyReflectionApp", "MyReflectionApp.Person", true, BindingFlags.Instance | BindingFlags.Public, null,
        new object[] { "Kevin" }, null, null);

    var personTypeFromString = Type.GetType("MyReflectionApp.Person");
    var person6 = Activator.CreateInstance(personTypeFromString, new object[] { "Kevin" });

    var person7 = Activator.CreateInstance("MyReflectionApp", "MyReflectionApp.Person", true, BindingFlags.Instance | BindingFlags.NonPublic, null,
        new object[] { "Kevin", 40 }, null, null);

    var assembly = Assembly.GetExecutingAssembly();
    var person8 = assembly.CreateInstance("MyReflectionApp.Person");

    var actualTypeFromConfiguration = Type.GetType(GetTypeFromConfiguration());
    var iTalkInstance = Activator.CreateInstance(actualTypeFromConfiguration) as ITalk;
    iTalkInstance.Talk("Hello world");

    dynamic dynamicITalkInstance = Activator.CreateInstance(actualTypeFromConfiguration);
    dynamicITalkInstance.Talk("Hello world");


    var personForManipulation = Activator.CreateInstance("MyReflectionApp", "MyReflectionApp.Person", true, BindingFlags.Instance | BindingFlags.NonPublic,
        null, new object[] { "Kevin", 40 }, null, null)?.Unwrap();

    var nameProperty = personForManipulation?.GetType().GetProperty("Name");
    nameProperty?.SetValue(personForManipulation, "Sven");

    var ageField = personForManipulation?.GetType().GetField("age");
    ageField?.SetValue(personForManipulation, 36);

    var privateField = personForManipulation?.GetType().GetField("_aPrivateField", BindingFlags.Instance | BindingFlags.NonPublic);
    privateField?.SetValue(personForManipulation, "updated private field value");






    Console.WriteLine(personForManipulation);
}