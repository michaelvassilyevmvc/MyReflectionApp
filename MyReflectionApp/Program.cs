using MyReflectionApp;
using ReflectionMagic;
using System.Reflection;
using static MyReflectionApp.IoCExampleClasses;

Console.Title = "Изучение рефлексии";

var person = new Person("Michael");
var privateField = person.GetType().GetField("_aPrivateField", BindingFlags.Instance | BindingFlags.NonPublic);
privateField?.SetValue(person, "New private field value");

person.AsDynamic()._aPrivateField = "Update value via reflection magic";

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

static void NetworkMonitorExample()
{
    NetworkMonitor.BootstrapFromConfiguration();
    Console.WriteLine("Что то пошло не так ...");

    NetworkMonitor.Warn();
}

static void CodeFromFourthModule()
{
    var myList = new List<Person>();
    Console.WriteLine(myList.GetType());

    var myDictionary = new Dictionary<string, int>();
    Console.WriteLine(myDictionary.GetType());

    var dictionaryType = myDictionary.GetType();
    foreach (var genericTypeArgument in dictionaryType.GenericTypeArguments)
    {
        Console.WriteLine(genericTypeArgument);
    }

    foreach (var genericArgument in dictionaryType.GetGenericArguments())
    {
        Console.WriteLine(genericArgument);
    }

    var openDictinaryType = typeof(Dictionary<,>);
    foreach (var genericTypeArgument in openDictinaryType.GenericTypeArguments)
    {
        Console.WriteLine(genericTypeArgument);
    }

    foreach (var genericArgument in openDictinaryType.GetGenericArguments())
    {
        Console.WriteLine(genericArgument);
    }

    var createdInstance = Activator.CreateInstance(typeof(List<Person>));
    Console.WriteLine(createdInstance?.GetType());

    //var openResultType = typeof(Result<>);
    //var closedResultType = openResultType.MakeGenericType(typeof(Person));
    //var createdResult = Activator.CreateInstance(closedResultType);
    //Console.WriteLine(createdResult.GetType());

    var openResultType = Type.GetType("MyReflectionApp.Result`1");
    var closedResultType = openResultType.MakeGenericType(Type.GetType("MyReflectionApp.Person"));
    var createdResult = Activator.CreateInstance(closedResultType);


    var methodInfo = closedResultType.GetMethod("AlterAndReturnValue");
    Console.WriteLine(methodInfo);

    var genericMethodInfo = methodInfo.MakeGenericMethod(typeof(Employee));
    genericMethodInfo.Invoke(createdResult, new object[] { new Employee() });
}

static void IoCContainerExample()
{
    var iocContainer = new IoCContainer();
    iocContainer.Register<IWaterService, TapWaterService>();
    var waterService = iocContainer.Resolve<IWaterService>();

    //iocContainer.Register<IBeanService<Catimor>, ArabicaBeanService<Catimor>>();

    //iocContainer.Register<ICoffeeService, CoffeeService>();
    //var coffeeService = iocContainer.Resolve<ICoffeeService>();

    iocContainer.Register(typeof(IBeanService<>), typeof(ArabicaBeanService<>));

    iocContainer.Register<ICoffeeService, CoffeeService>();
    var coffeeService = iocContainer.Resolve<ICoffeeService>();
}