using Newtonsoft.Json;

namespace Agile_Pract6
{
    public static class TaskRepository
    {
        private static readonly string filePath = "tasks.json";

        public static List<Task> LoadTasks()
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<List<Task>>(json) ?? new List<Task>();
            }
            return new List<Task>();
        }

        public static void SaveTasks(List<Task> tasks)
        {
            var json = JsonConvert.SerializeObject(tasks, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        // Melnyk Viacheslav
        public static Task AddTask()
        {
            Console.Clear();
            Console.WriteLine("Enter Task Title:");
            string title = Console.ReadLine();

            Console.Clear();
            Console.WriteLine("Enter Task Description (optional, press Enter to skip):");
            string description = Console.ReadLine();

            Console.Clear();
            Console.WriteLine("Enter Flag (e.g., Important, Unimportant):");
            string flag = Console.ReadLine();

            Console.Clear();
            Console.WriteLine("Enter Label Color:");
            string labelColor = Console.ReadLine();

            Console.Clear();
            Console.WriteLine("Enter Task Color:");
            string taskColor = Console.ReadLine();

            DateTime creationDate = DateTime.Now;
            DateTime? endDate = null;

            Console.Clear();
            Console.WriteLine("Do you want to set an end date? (y/n)");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                Console.WriteLine("Enter the number of days from today to set the end date:");
                if (int.TryParse(Console.ReadLine(), out int daysToAdd))
                {
                    endDate = creationDate.AddDays(daysToAdd);
                }
                else
                {
                    Console.WriteLine("Invalid input for days. End date will not be set.");
                }
            }

            Console.Clear();
            Console.WriteLine("Summary of the new task:");
            Console.WriteLine($"Title: {title}\nDescription: {description}\nFlag: {flag}\nLabel Color: {labelColor}\nTask Color: {taskColor}");
            Console.WriteLine($"Creation Date: {creationDate.ToShortDateString()}");
            Console.WriteLine($"End Date: {(endDate.HasValue ? endDate.Value.ToShortDateString() : "Not set")}");
            Console.WriteLine("Confirm add task? (y/n)");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                var newTask = new Task
                {
                    Id = TaskRepository.LoadTasks().Count + 1,
                    Title = title,
                    CreationDate = creationDate,
                    EndDate = endDate,
                    Description = description,
                    Flag = flag,
                    LabelColor = labelColor,
                    TaskColor = taskColor
                };
                return newTask;
            }
            return null;
        }


        // Andrusenko Oksana
        public static void EditTask()
        {
            List<Task> tasks = TaskRepository.LoadTasks();

            Console.Clear();
            Console.WriteLine("Enter the Task ID you want to edit:");
            if (int.TryParse(Console.ReadLine(), out int taskId))
            {
                var task = tasks.FirstOrDefault(t => t.Id == taskId);
                if (task != null)
                {
                    bool hasChanges = false;
                    int choice;

                    do
                    {
                        Console.Clear();
                        Console.WriteLine($"Editing Task ID: {task.Id}");
                        Console.WriteLine($"1. Title: {task.Title}");
                        Console.WriteLine($"2. Description: {task.Description ?? "Not set"}");
                        Console.WriteLine($"3. Flag: {task.Flag}");
                        Console.WriteLine($"4. Label Color: {task.LabelColor}");
                        Console.WriteLine($"5. Task Color: {task.TaskColor}");
                        Console.WriteLine($"6. Creation Date: {task.CreationDate.ToShortDateString()}");
                        Console.WriteLine($"7. End Date: {(task.EndDate.HasValue ? task.EndDate.Value.ToShortDateString() : "Not set")}");
                        Console.WriteLine("0. Exit");
                        Console.WriteLine("Select a field to edit (0-7):");

                        if (int.TryParse(Console.ReadLine(), out choice))
                        {
                            switch (choice)
                            {
                                case 1:
                                    Console.Clear();
                                    Console.WriteLine("Enter new Title:");
                                    task.Title = Console.ReadLine();
                                    hasChanges = true;
                                    break;
                                case 2:
                                    Console.Clear();
                                    Console.WriteLine("Enter new Description (optional, press Enter to skip):");
                                    task.Description = Console.ReadLine();
                                    hasChanges = true;
                                    break;
                                case 3:
                                    Console.Clear();
                                    Console.WriteLine("Enter new Flag:");
                                    task.Flag = Console.ReadLine();
                                    hasChanges = true;
                                    break;
                                case 4:
                                    Console.Clear();
                                    Console.WriteLine("Enter new Label Color:");
                                    task.LabelColor = Console.ReadLine();
                                    hasChanges = true;
                                    break;
                                case 5:
                                    Console.Clear();
                                    Console.WriteLine("Enter new Task Color:");
                                    task.TaskColor = Console.ReadLine();
                                    hasChanges = true;
                                    break;
                                case 7:
                                    Console.Clear();
                                    Console.WriteLine("Enter the number of days from the creation date to set the end date:");
                                    if (int.TryParse(Console.ReadLine(), out int daysToAdd))
                                    {
                                        task.EndDate = task.CreationDate.AddDays(daysToAdd);
                                        Console.WriteLine($"New End Date: {task.EndDate.Value.ToShortDateString()}");
                                        hasChanges = true;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Invalid input for days. End date not changed.");
                                    }
                                    break;
                                case 0:
                                    if (hasChanges)
                                    {
                                        Console.Clear();
                                        Console.WriteLine("Do you want to save changes? (y/n)");
                                        if (Console.ReadLine()?.ToLower() == "y")
                                        {
                                            TaskRepository.SaveTasks(tasks);
                                            Console.WriteLine("Changes saved.");
                                        }
                                        else
                                        {
                                            Console.WriteLine("Changes discarded.");
                                        }
                                    }
                                    break;
                                default:
                                    Console.WriteLine("Invalid choice. Please select a valid option.");
                                    break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid input. Please enter a number between 0 and 7.");
                        }

                    } while (choice != 0);
                }
                else
                {
                    Console.WriteLine("Task not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input for Task ID.");
            }
        }



    }
}

