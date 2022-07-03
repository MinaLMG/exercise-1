using Spectre.Console;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
//using Newtonsoft.Json;
public static class Program
{
    public static Dictionary<Guid, string> categoriesNamesMap = new Dictionary<Guid, string>();

    public class Category
    {
        public string name { get; set; }
        public Guid id { get; set; }
        public Category()
        {

        }
        public Category(string x)
        {
            this.name = x;
            this.id = Guid.NewGuid(); ;
        }
        public string display()
        {
            return "this category has a name : " + this.name;
        }
    }
    public class Recipe
    {
        public string Title { get; set; }
        public string Ingredients { get; set; }
        public string Instructions { get; set; }
        public List<Guid> Categories { get; set; }
        //Category[] Categories{ get;set; }
        public Recipe()
        {

        }
        public Recipe(string title, string ingredients, string instructions, List<Guid> categories)
        {
            this.Title = title;
            this.Ingredients = ingredients;
            this.Instructions = instructions;
            this.Categories = categories;
        }
        public string display()
        {
            string toDisplay = "this receipe is called :" + this.Title + ", to do it we need: " + this.Ingredients + ", the instructions are: " + this.Instructions;
            for (int i = 0; i < this.Categories.Count; i++)
            {
                if (i == 0)
                {
                    toDisplay += ", for categories it is considered as: ";
                }
                if (i != 0) { toDisplay += "       "; }
                toDisplay += categoriesNamesMap[this.Categories[i]];
            }
            toDisplay += "\n\n";
            return toDisplay;
        }

    }
    public static string Select(string[] choices, string title = "")
    {
        var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
        .Title(title)
        .PageSize(10)
        //.MoreChoicesText("[grey](Move up and down to reveal more fruits)[/]")
        .AddChoices(choices));
        return choice;
    }


    public static void writeInFolder(string text, string path)
    {

        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.WriteLine(text);
        }
    }
    public static string listCategories(List<Category> categories)
    {
        string categoriesString = "";
        for (int i = 0; i < categories.Count; i++)
        {
            categoriesString += "at index ";
            categoriesString += i;
            categoriesString += " ";
            categoriesString += categories[i].display();
            categoriesString += "\n\n";
        }
        return categoriesString;
    }
    public static string listrecipes(List<Recipe> receipes)
    {
        string receipesString = "";
        for (int i = 0; i < receipes.Count; i++)
        {
            receipesString += "at index ";
            receipesString += i;
            receipesString += " ";
            receipesString += receipes[i].display();
            receipesString += "\n\n";
        }
        return receipesString;
    }
    public static void Main(string[] args)
    {
        AnsiConsole.Write(new FigletText("Mena Lateaf").Centered().Color(Color.Grey));
        string? mainMenuChoice = null;
        string? categoryChoice = null;
        string? recipeChoice = null;
        string? backChoice = null;

        //Category c1 = new Category("sweet");
        //Category c2 = new Category("pizza");
        //Category c3 = new Category("pasta");
        //List<Category> categories = new List<Category>() { c1, c2, c3 };

        string mainPath = Environment.CurrentDirectory;
        string categoriesLoc = $@"{mainPath}\..\..\..\..\categories.json";
        string categoriesString = File.ReadAllText(categoriesLoc);
        var categories = JsonSerializer.Deserialize<List<Category>>(categoriesString);
        Dictionary<string, Guid?> categoriesMap = new Dictionary<string, Guid?>();
        for (int i = 0; i < categories.Count; i++)
        {
            categoriesMap[categories[i].name] = categories[i].id;
            categoriesNamesMap[categories[i].id] = categories[i].name;
        }


        string recipesLoc = $@"{mainPath}\..\..\..\..\recipes.json";
        string recipesString = File.ReadAllText(recipesLoc);
        var recipes = JsonSerializer.Deserialize<List<Recipe>>(recipesString);



        //Recipe r1 = new Recipe("sweet", "sweet", "sweet", new List<Guid> { });
        //Recipe r2 = new Recipe("sweet", "sweet", "sweet", new List<Guid> { });
        //Recipe r3 = new Recipe("sweet", "sweet", "sweet", new List<Guid> { });
        //List<Recipe> recipes = new List<Recipe>() { r1, r2, r3 };
        //writeInFolder(JsonSerializer.Serialize(recipes, new JsonSerializerOptions { WriteIndented = true }), recipesLoc);

        //jsonconvert.deserializeobject<list<category>>(json)

        //List<Category> categories = new();
        //categories[0] = c1;
        //categories[1] = c2;
        //categories[2] = c3;
        var options = new JsonSerializerOptions { WriteIndented = true };


        bool coninue = true;
        while (coninue)
        {

            switch (mainMenuChoice)
            {
                case null:
                    mainMenuChoice = Select(new[] { "Recipes", "Categories", "Close program" }, "which struct would you like to deal with ?");
                    break;
                case "Recipes":
                    switch (recipeChoice)
                    {
                        case null:
                            recipeChoice = Select(new[] { "List", "Add", "Edit", "Bact to main menu" }, "what would you like to do with recipes?");
                            break;
                        case "List":
                            //AnsiConsole.Write(new FigletText("Listing Recipe").Centered().Color(Color.Grey));
                            switch (backChoice)
                            {
                                case null:
                                    //AnsiConsole.Markup("[white]{0}[/]", Markup.Escape(File.ReadAllText(recipesLoc)));
                                    string recipesStringToEdit = listrecipes(recipes);
                                    AnsiConsole.Markup("[white]{0}[/]", Markup.Escape(recipesStringToEdit));
                                    backChoice = Select(new[] { "Back" });
                                    break;
                                case "Back":
                                    backChoice = null;
                                    recipeChoice = null;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "Add":
                            switch (backChoice)
                            {
                                case null:
                                    var title = AnsiConsole.Ask<string>("What's the recipe title?");
                                    var ingredients = AnsiConsole.Ask<string>("What's the recipe ingredients?");
                                    var instructions = AnsiConsole.Ask<string>("What's the recipe instructions?");
                                    ////string[] CategoriesNames = new string[] { };
                                    //for(int i = 0; i < categoriesString.Length;i++)
                                    //{
                                    //    CategoriesNames[i] = Categories[i].name;
                                    //}
                                    var categoryNames = categories.Select(x => x.name).ToArray();
                                    var chosenCategories = AnsiConsole.Prompt(
                                         new MultiSelectionPrompt<string>()
                                             .Title("What are your [green]favorite fruits[/]?")
                                             .NotRequired() // Not required to have a favorite fruit
                                             .PageSize(10)
                                             //.MoreChoicesText("[grey](Move up and down to reveal more fruits)[/]")
                                             .InstructionsText(
                                                 "[grey](Press [blue]<space>[/] to toggle a fruit, " +
                                                 "[green]<enter>[/] to accept)[/]")
                                             .AddChoices(categoryNames));

                                    List<Guid> chosenCategoriesFinal = new List<Guid> { };

                                    for (int i = 0; i < chosenCategories.Count; i++)
                                    {
                                        chosenCategoriesFinal.Add(categoriesMap[chosenCategories[i]].Value);
                                    }
                                    Recipe to_add = new Recipe(title, ingredients, instructions, chosenCategoriesFinal);
                                    recipes.Add(to_add);
                                    writeInFolder(JsonSerializer.Serialize(recipes, options), recipesLoc);
                                    backChoice = "Back";
                                    break;
                                case "Back":
                                    backChoice = null;
                                    recipeChoice = null;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "Edit":
                            switch (backChoice)
                            {
                                case null:
                                    string recipesStringToEdit = listrecipes(recipes);
                                    AnsiConsole.Markup("[white]{0}[/]", Markup.Escape(recipesStringToEdit));
                                    var index = -1;
                                    while (index < 0 || index >= recipes.Count)
                                    {
                                        index = int.Parse(AnsiConsole.Ask<string>("choose an index to edit"));
                                    }
                                    var title = AnsiConsole.Ask<string>("What's the recipe new title?");
                                    var ingredients = AnsiConsole.Ask<string>("What's the recipe new ingredients?");
                                    var instructions = AnsiConsole.Ask<string>("What's the recipe new instructions?");
                                    var categoryNames = categories.Select(x => x.name).ToArray();
                                    var chosenCategories = AnsiConsole.Prompt(
                                        new MultiSelectionPrompt<string>()
                                            .Title("What are your [green]favorite fruits[/]?")
                                            .NotRequired() // Not required to have a favorite fruit
                                            .PageSize(10)
                                            //.MoreChoicesText("[grey](Move up and down to reveal more fruits)[/]")
                                            .InstructionsText(
                                                "[grey](Press [blue]<space>[/] to toggle a fruit, " +
                                                "[green]<enter>[/] to accept)[/]")
                                            .AddChoices(categoryNames));

                                    // Write the selected fruits to the terminal
                                    List<Guid> chosenCategoriesFinal = new List<Guid> { };

                                    for (int i = 0; i < chosenCategories.Count; i++)
                                    {
                                        chosenCategoriesFinal.Add(categoriesMap[chosenCategories[i]].Value);
                                    }
                                    Recipe to_edit = new Recipe(title, ingredients, instructions, chosenCategoriesFinal);
                                    recipes[index] = to_edit;
                                    writeInFolder(JsonSerializer.Serialize(recipes, options), recipesLoc);
                                    backChoice = "Back";
                                    break;
                                case "Back":
                                    backChoice = null;
                                    recipeChoice = null;
                                    break;
                                default:
                                    break;
                            }
                            break;

                        case "Bact to main menu":
                            mainMenuChoice = null;
                            recipeChoice = null;
                            break;
                        default:
                            break;

                    }
                    break;
                case "Categories":

                    switch (categoryChoice)
                    {
                        case null:
                            categoryChoice = Select(new[] { "List", "Add", "Edit", "Bact to main menu" }, "what would you like to do with categories?");
                            break;
                        case "List":
                            //AnsiConsole.Write(new FigletText("Listing Recipe").Centered().Color(Color.Grey));
                            switch (backChoice)
                            {
                                case null:
                                    AnsiConsole.Markup("[white]{0}[/]", Markup.Escape(File.ReadAllText(categoriesLoc)));
                                    backChoice = Select(new[] { "Back" });
                                    break;
                                case "Back":
                                    backChoice = null;
                                    categoryChoice = null;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "Add":
                            switch (backChoice)
                            {
                                case null:
                                    var name = AnsiConsole.Ask<string>("What's the category name?").ToLower().Trim();
                                    try
                                    {
                                        while (categoriesMap[name] != null)
                                        {
                                            name = AnsiConsole.Ask<string>("this category name already exists, Enter neew one please. ").ToLower().Trim();

                                        }
                                    }
                                    catch (Exception e) { }
                                    Category to_add = new Category(name);
                                    categories.Add(to_add);
                                    categoriesMap[name] = to_add.id;
                                    categoriesNamesMap[to_add.id] = to_add.name;
                                    writeInFolder(JsonSerializer.Serialize(categories, options), categoriesLoc);
                                    backChoice = "Back";
                                    break;
                                case "Back":
                                    backChoice = null;
                                    categoryChoice = null;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "Edit":
                            switch (backChoice)
                            {
                                case null:
                                    string categoriesStringToEdit = listCategories(categories);
                                    AnsiConsole.Markup("[white]{0}[/]", Markup.Escape(categoriesStringToEdit));
                                    var index = -1;
                                    while (index < 0 || index >= categories.Count)
                                    {
                                        index = int.Parse(AnsiConsole.Ask<string>("choose an index to edit"));
                                    }
                                    categoriesMap[categories[index].name] = null;
                                    var name = AnsiConsole.Ask<string>("What's the category new name?").ToLower().Trim();
                                    try
                                    {
                                        while (categoriesMap[name] != null)
                                        {
                                            name = AnsiConsole.Ask<string>("this category name already exists, Enter neew one please. ").ToLower().Trim();
                                        }
                                    }
                                    catch (Exception e) { }
                                    //Category to_edit = new Category(name);
                                    categories[index].name = name;
                                    categoriesMap[categories[index].name] = categories[index].id;
                                    categoriesNamesMap[categories[index].id] = categories[index].name;

                                    writeInFolder(JsonSerializer.Serialize(categories, options), categoriesLoc);
                                    backChoice = "Back";
                                    break;
                                case "Back":
                                    backChoice = null;
                                    categoryChoice = null;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "Bact to main menu":
                            mainMenuChoice = null;
                            categoryChoice = null;
                            break;
                        default:
                            break;

                    }
                    break;
                case "Close program":
                    coninue = false;
                    break;
                default:

                    break;
            }
        }


    }
}