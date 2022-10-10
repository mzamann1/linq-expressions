using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using LinqExpressions.Constants;
using LinqExpressions.Expressions;
using LinqExpressions.Extensions;
using LinqExpressions.Models;



var list = new List<User>()
{
    new User() { Age = 24, Name = "Zaman" },
    new User() { Age = 19, Name = "Faizan" }
}.AsQueryable();


//ExtensionPracticeClass<User>.RunStartsWith(list, "Name", "Z");
//ExtensionPracticeClass<User>.RunEndsWith(list, "Name", "i");
//ExtensionPracticeClass<User>.RunContains(list, "Name", "i");

//PrintExpression.PrintConsoleMessage("12313");
//Console.WriteLine(ArithmeticOperationExpression.ApplyArithmeticOperation(5, 6));


ExtensionPracticeClass<User>.RunOrderBy(list, "Age",OrderByType.Ascending);



public static class ExtensionPracticeClass<T> where T : class
{
    public static void RunStartsWith(IQueryable<T> inputList, string Name = "", string Value = "")
    {
        var response = inputList.WhereCustom(Name, Value, MethodType.StartsWith);
        PrintList(MethodBase.GetCurrentMethod()?.Name, response);
    }
    public static void RunEndsWith(IQueryable<T> inputList, string Name = "", string Value = "")
    {
        var response = inputList.WhereCustom(Name, Value, MethodType.EndsWith);
        PrintList(MethodBase.GetCurrentMethod()?.Name, response);
    }

    public static void RunContains(IQueryable<T> inputList, string Name = "", string Value = "")
    {
        var response = inputList.WhereCustom(Name, Value, MethodType.Contains);
        PrintList(MethodBase.GetCurrentMethod()?.Name, response);
    }

    public static void RunOrderBy(IQueryable<T> inputList, string Name = "", OrderByType orderBy = OrderByType.Ascending)
    {
        var response = inputList.OrderByCustom(Name, orderBy);
        PrintList(MethodBase.GetCurrentMethod()?.Name, response);
    }

    private static void PrintList<T>(string callerMethod, IQueryable<T> userList) where T : class
    {
        Console.WriteLine($"======================= Method Name => {callerMethod} Started Printing =====================\n");

        if (userList?.Count() > 0)
        {
            foreach (var user in userList)
            {
                Console.WriteLine(user.ToString());
            }
        }
        else
        {
            Console.WriteLine("No Record Found");
        }

        Console.WriteLine($"\n======================= Method Name => {callerMethod} Ended Printing  =====================\n\n\n");
    }




}

