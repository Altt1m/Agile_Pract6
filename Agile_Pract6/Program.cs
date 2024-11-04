namespace Agile_Pract6
{
    class Program
    {
        static void Main()
        {
            var tasks = TaskRepository.LoadTasks();
            bool cycle = true;

            while (cycle)
            {
                Console.WriteLine("1. Add Task");
                Console.WriteLine("2. Edit Task");
                Console.WriteLine("0. Quit");
                Console.WriteLine("Select an option:");
                int option = int.Parse(Console.ReadLine());

                switch (option)
                {
                    case 1:
                        var newTask = TaskRepository.AddTask();
                        if (newTask != null)
                        {
                            tasks.Add(newTask);
                            TaskRepository.SaveTasks(tasks);
                            Console.WriteLine("Task added successfully.");
                        }
                        break;
                    case 2:
                        TaskRepository.EditTask();
                        break;
                    case 0:
                        cycle = false;
                        Console.WriteLine("Goodbye :)");
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }


            }

        }
    }

}