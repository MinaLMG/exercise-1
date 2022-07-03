using Spectre.Console;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
//using Newtonsoft.Json;
public static class Program
{

    public class Category
    {
        public string Name { get; set; }
        public Guid ID { get; set; }

        public Category()
        {

        }
        public Category(string x)
        {
            this.Name = x;
            this.ID = Guid.NewGuid(); ;
        }
        public string Display()
        {
            return "this category has a name : " + this.Name;
        }
    }
    public class Recipe
    {
        public string Title { get; set; }
        public string Ingredients { get; set; }
        public string Instructions { get; set; }
        public List<Guid> Categories { get; set; }

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
        public string Display(Dictionary<Guid, string> categoriesNamesMap)
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
        .AddChoices(choices));
        return choice;
    }


    public static void WriteInFolder(string text, string path)
    {

        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.WriteLine(text);
        }
    }
    public static string ListCategories(List<Category> categories)
    {
        string categoriesString = "";
        for (int i = 0; i < categories.Count; i++)
        {
            categoriesString += "at index ";
            categoriesString += i;
            categoriesString += " ";
            categoriesString += categories[i].Display();
            categoriesString += "\n\n";
        }
        return categoriesString;
    }
    public static string ListRecipes(List<Recipe> receipes, Dictionary<Guid, string> categoriesNamesMap)
    {
        string receipesString = "";
        for (int i = 0; i < receipes.Count; i++)
        {
            receipesString += "at index ";
            receipesString += i;
            receipesString += " ";
            receipesString += receipes[i].Display(categoriesNamesMap);
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
        string mainPath = Environment.CurrentDirectory;
        string categoriesLoc = $@"{mainPath}\..\..\..\..\categories.json";
        string categoriesString = File.ReadAllText(categoriesLoc);
        var categories = JsonSerializer.Deserialize<List<Category>>(categoriesString);
        Dictionary<string, Guid?> categoriesMap = new Dictionary<string, Guid?>();
        Dictionary<Guid, string> categoriesNamesMap = new Dictionary<Guid, string>();

        for (int i = 0; i < categories.Count; i++)
        {
            categoriesMap[categories[i].Name] = categories[i].ID;
            categoriesNamesMap[categories[i].ID] = categories[i].Name;
        }
        string recipesLoc = $@"{mainPath}\..\..\..\..\recipes.json";
        string recipesString = File.ReadAllText(recipesLoc);
        var recipes = JsonSerializer.Deserialize<List<Recipe>>(recipesString);
        var options = new JsonSerializerOptions { WriteIndented = true };
        bool continueCode = true;
        while (continueCode)
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
                            switch (backChoice)
                            {
                                case null:
                                    string recipesStringToEdit = ListRecipes(recipes,categoriesNamesMap);
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
                                    var categoryNames = categories.Select(x => x.Name).ToArray();
                                    var chosenCategories = AnsiConsole.Prompt(
                                         new MultiSelectionPrompt<string>()
                                             .Title("What are your [green]favorite fruits[/]?")
                                             .NotRequired()
                                             .PageSize(10)
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
                                    WriteInFolder(JsonSerializer.Serialize(recipes, options), recipesLoc);
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
                                    string recipesStringToEdit = ListRecipes(recipes,categoriesNamesMap);
                                    AnsiConsole.Markup("[white]{0}[/]", Markup.Escape(recipesStringToEdit));
                                    var index = -1;
                                    while (index < 0 || index >= recipes.Count)
                                    {
                                        index = int.Parse(AnsiConsole.Ask<string>("choose an index to edit"));
                                    }
                                    var title = AnsiConsole.Ask<string>("What's the recipe new title?");
                                    var ingredients = AnsiConsole.Ask<string>("What's the recipe new ingredients?");
                                    var instructions = AnsiConsole.Ask<string>("What's the recipe new instructions?");
                                    var categoryNames = categories.Select(x => x.Name).ToArray();
                                    var chosenCategories = AnsiConsole.Prompt(
                                        new MultiSelectionPrompt<string>()
                                            .Title("What are your [green]favorite fruits[/]?")
                                            .NotRequired()
                                            .PageSize(10)
                                            .InstructionsText(
                                                "[grey](Press [blue]<space>[/] to toggle a fruit, " +
                                                "[green]<enter>[/] to accept)[/]")
                                            .AddChoices(categoryNames));

                                    List<Guid> chosenCategoriesFinal = new List<Guid> { };
                                    for (int i = 0; i < chosenCategories.Count; i++)
                                    {
                                        chosenCategoriesFinal.Add(categoriesMap[chosenCategories[i]].Value);
                                    }
                                    Recipe to_edit = new Recipe(title, ingredients, instructions, chosenCategoriesFinal);
                                    recipes[index] = to_edit;
                                    WriteInFolder(JsonSerializer.Serialize(recipes, options), recipesLoc);
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
                                    categoriesMap[name] = to_add.ID;
                                    categoriesNamesMap[to_add.ID] = to_add.Name;
                                    WriteInFolder(JsonSerializer.Serialize(categories, options), categoriesLoc);
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
                                    string categoriesStringToEdit = ListCategories(categories);
                                    AnsiConsole.Markup("[white]{0}[/]", Markup.Escape(categoriesStringToEdit));
                                    var index = -1;
                                    while (index < 0 || index >= categories.Count)
                                    {
                                        index = int.Parse(AnsiConsole.Ask<string>("choose an index to edit"));
                                    }
                                    categoriesMap[categories[index].Name] = null;
                                    var name = AnsiConsole.Ask<string>("What's the category new name?").ToLower().Trim();
                                    try
                                    {
                                        while (categoriesMap[name] != null)
                                        {
                                            name = AnsiConsole.Ask<string>("this category name already exists, Enter neew one please. ").ToLower().Trim();
                                        }
                                    }
                                    catch (Exception e) { }
                                    categories[index].Name = name;
                                    categoriesMap[categories[index].Name] = categories[index].ID;
                                    categoriesNamesMap[categories[index].ID] = categories[index].Name;
                                    WriteInFolder(JsonSerializer.Serialize(categories, options), categoriesLoc);
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
                    continueCode = false;
                    break;
                default:
                    break;
            }
        }
    }
}