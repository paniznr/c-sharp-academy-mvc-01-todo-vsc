using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using ToDo.Models;
using ToDo.Models.ViewModels;

namespace ToDo.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    // In order to sort the list by name, I added additional checks to the Index() function
    public IActionResult Index(string sortOrder = null) //sortOrder is activated by the "Sort by Name" button
    {
        // If sortOrder is null, check the session to see if it has been set previously
        if (string.IsNullOrEmpty(sortOrder))
        {
            sortOrder = HttpContext.Session.GetString("sortOrder");
        }
        else
        {
            // Store the sortOrder in the session for future requests
            HttpContext.Session.SetString("sortOrder", sortOrder);
        }
        var toDoListViewModel = string.IsNullOrEmpty(sortOrder) 
                                ? GetAllToDos() // Fetch unsorted list if no sortOrder is passed
                                : GetAllToDosSorted(); // Fetch sorted list if sortOrder is provided
        return View(toDoListViewModel);
    }


    [HttpGet]
    public JsonResult PopulateForm(int id)
    {
        var toDo = GetById(id);
        return Json(toDo);
    }

    //fetch data from DB to display To Do items
    internal ToDoViewModel GetAllToDos() 
    {
        List<ToDoItem> toDoList = new();
        using (SqliteConnection con = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = con.CreateCommand())
            {
                con.Open();
                tableCmd.CommandText = "SELECT * FROM ToDo";
                using (var reader = tableCmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read()) //while reader is reading data from db, when no more rows it will stop reading and exit
                        {
                            toDoList.Add( //adding to our list
                                new ToDoItem
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1)
                                });
                        }
                    } 
                    else 
                    {
                        return new ToDoViewModel //empty because no rows
                        {
                            ToDoList = toDoList
                        };
                    }

                }
            }
            return new ToDoViewModel
            {
                ToDoList = toDoList
            };

        }
        
    }

    //Similar to GetAllToDos(), but uses ORDER BY in query to sort by Name
    internal ToDoViewModel GetAllToDosSorted() 
    {
        List<ToDoItem> toDoList = new();
        using (SqliteConnection con = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = con.CreateCommand())
            {
                con.Open();
                tableCmd.CommandText = "SELECT * FROM ToDo ORDER BY Name";
                using (var reader = tableCmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read()) //while reader is reading data from db, when no more rows it will stop reading and exit
                        {
                            toDoList.Add( //adding to our list
                                new ToDoItem
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1)
                                });
                        }
                    } 
                    else 
                    {
                        return new ToDoViewModel //empty because no rows
                        {
                            ToDoList = toDoList
                        };
                    }

                }
            }
            return new ToDoViewModel
            {
                ToDoList = toDoList
            };

        }
        
    }
    internal ToDoItem GetById(int id)
    {
        ToDoItem toDo = new();
        using (SqliteConnection connection = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = connection.CreateCommand())
            {
                connection.Open();
                tableCmd.CommandText = $"SELECT * FROM toDo WHERE Id = '{id}'";

                using (var reader = tableCmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        toDo.Id = reader.GetInt32(0);
                        toDo.Name = reader.GetString(1);
                    } else {
                        return toDo;
                    }
                };
            }
        }
        return toDo;
    }
    public RedirectResult Insert(ToDoItem toDo) 
    {
        //With "using" because SQLite is an unmanaged resource that needs to be disposed of so we don't waste memory in background
        using (SqliteConnection con = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = con.CreateCommand())
            {
                con.Open();
                tableCmd.CommandText = $"INSERT INTO ToDo (name) VALUES ('{toDo.Name}')";
                try
                {
                    tableCmd.ExecuteNonQuery(); //because we don't need anything to return
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                }
            }
        }
        return Redirect("http://localhost:5146");
    }


    [HttpPost]
    public JsonResult Delete(int id)
    {
        using (SqliteConnection con = new SqliteConnection("Data Source = db.sqlite"))
        {
            using (var tableCmd = con.CreateCommand())
            {
                con.Open();
                tableCmd.CommandText = $"DELETE from ToDo WHERE Id = '{id}'";
                tableCmd.ExecuteNonQuery();
            }
        }
        return Json(new { });  //we dont need to return anything
    }

    public RedirectResult Update(ToDoItem toDo)
    {
        using (SqliteConnection con = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = con.CreateCommand())
            {
                con.Open();
                tableCmd.CommandText = $"UPDATE toDo SET name = '{toDo.Name}' WHERE Id = '{toDo.Id}'";
                try
                {
                    tableCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                }
            }
        }
        return Redirect("http://localhost:5146");

    }

}
