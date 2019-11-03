using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ClosedXML.Excel;
using ExcelReader.Enums;
using ExcelReader.Extensions;
using ExcelReader.Models;
using MongoDB.Driver;

namespace ExcelReader
{
    class Program
    {
        static void Main(string[] args)
        {
            var states = new LinkedList<State>();
            var cities = new LinkedList<City>();
            var hasItemsLoaded = false;

            const string dbUrl =
                "mongodb+srv://redplane:Redplane1@cluster0-pu4id.mongodb.net/test?retryWrites=true&w=majority";

            const string dbName = "sodakoq";

            var applicationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var excelFile = Path.Combine(applicationPath, "data", "Malaysia City and states.xlsx");
            using (var fileStream = File.Open(excelFile, FileMode.Open))
            using (var xlWorkbook = new XLWorkbook(fileStream))
            {
                var activeWorksheet = xlWorkbook.Worksheet("Sheet1");
                foreach (var column in activeWorksheet.Columns())
                {
                    var isFirstCell = true;
                    foreach (var cell in column.Cells(true))
                    {
                        var cellValue = cell?.Value.ToString();
                        if (string.IsNullOrEmpty(cellValue))
                            continue;

                        // Id of state.
                        var stateId = Guid.NewGuid();

                        // First cell will be the sate name.
                        if (isFirstCell)
                        {
                            var state = new State(stateId);
                            state.Name = cellValue;
                            state.Availability = MasterItemAvailabilities.Available;
                            state.CreatedTime = DateTime.UtcNow.ToUnixTime();
                            state.DeliveryFee = 10;

                            states.AddLast(state);
                            
                            // Cell is marked not to be the first.
                            isFirstCell = false;

                            continue;
                        }

                        var city = new City(Guid.NewGuid());
                        city.StateId = stateId;
                        city.Name = cellValue;
                        city.Availability = MasterItemAvailabilities.Available;
                        city.CreatedTime = DateTime.UtcNow.ToUnixTime();
                        cities.AddLast(city);
                    }
                }
            }

            if (states.Count > 0 && cities.Count > 0)
            {
                var dbDriver = new MongoUrl(dbUrl);
                var dbClient = new MongoClient(dbDriver);
                var db = dbClient.GetDatabase(dbName);
                var statesCollection = db.GetCollection<State>("states");
                var citiesCollection = db.GetCollection<City>("cities");

                using (var session = dbClient.StartSession())
                {
                    try
                    {
                        // Start session.
                        session.StartTransaction();

                        statesCollection.DeleteMany(session, FilterDefinition<State>.Empty);
                        foreach (var state in states)
                            statesCollection.InsertOne(session, state);

                        citiesCollection.DeleteMany(session, FilterDefinition<City>.Empty);
                        foreach (var city in cities)
                            citiesCollection.InsertOne(session, city);

                        session.CommitTransaction();
                        Console.WriteLine("Completed");
                    }
                    catch (Exception exception)
                    {
                        session.AbortTransaction();
                        Console.WriteLine(exception.Message);
                    }
                }
            }

            Console.ReadLine();
        }
    }
}
