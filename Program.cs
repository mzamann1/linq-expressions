// See https://aka.ms/new-console-template for more information

using System.Collections;
using System.Runtime.CompilerServices;
using LinqExpressions.Constants;
using LinqExpressions.Extensions;
using LinqExpressions.Models;



var list = new List<User>()
{
    new User() { Age = 24, Name = "Zaman" },
    new User() { Age = 19, Name = "Faizan" }
}.AsQueryable();

ExtensionPracticeClass.RunEndssWith(list, "Name", "n");


public static class ExtensionPracticeClass
{
    public static void RunStartsWith<T>(IQueryable<T> inputList, string Name = "", string Value = "") where T : class
    {
        var response = inputList.WhereCustom(Name, Value, MethodType.StartsWith);

        foreach (var resp in response)
        {
            Console.WriteLine(resp.ToString());
        }
    }
    public static void RunEndssWith<T>(IQueryable<T> inputList, string Name = "", string Value = "") where T : class
    {
        var response = inputList.WhereCustom(Name, Value, MethodType.EndsWith);

        foreach (var resp in response)
        {
            Console.WriteLine(resp.ToString());
        }
    }
}


