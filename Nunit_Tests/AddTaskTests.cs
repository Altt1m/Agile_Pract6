using System;
using System.Collections.Generic;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Nunit_Tests
{
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public string Flag { get; set; } // "важливе", "неважливе" тощо
        public string LabelColor { get; set; }
        public string TaskColor { get; set; }
    }

    public class AddTaskTests
    {
        private Mock<IConsole> mockConsole;
        private TasksRepository taskRepo;

        [SetUp]
        public void Setup()
        {
            mockConsole = new Mock<IConsole>();
            taskRepo = new TasksRepository(mockConsole.Object);
        }

        [Test]
        public void AddTask_AllFieldsFilled_CreatesTaskWithAllFields()
        {
            // Arrange
            mockConsole.SetupSequence(c => c.ReadLine())
                .Returns("Test Title")
                .Returns("Test Description")
                .Returns("Important")
                .Returns("Red")
                .Returns("Blue")
                .Returns("y") // Set end date
                .Returns("3") // Days to add for end date
                .Returns("y"); // Confirm adding task

            // Act
            var task = taskRepo.AddTask();

            // Assert
            Assert.NotNull(task);
            Assert.AreEqual("Test Title", task.Title);
            Assert.AreEqual("Test Description", task.Description);
            Assert.AreEqual("Important", task.Flag);
            Assert.AreEqual("Red", task.LabelColor);
            Assert.AreEqual("Blue", task.TaskColor);
            Assert.NotNull(task.EndDate);
        }

        [Test]
        public void AddTask_OptionalFieldsSkipped_CreatesTaskWithDefaultValues()
        {
            // Arrange
            mockConsole.SetupSequence(c => c.ReadLine())
                .Returns("Test Title")
                .Returns("") // Skip Description
                .Returns("Important")
                .Returns("Red")
                .Returns("Blue")
                .Returns("n") // No end date
                .Returns("y"); // Confirm adding task

            // Act
            var task = taskRepo.AddTask();

            // Assert
            Assert.NotNull(task);
            Assert.AreEqual("Test Title", task.Title);
            Assert.IsNull(task.Description);
            Assert.AreEqual("Important", task.Flag);
            Assert.AreEqual("Red", task.LabelColor);
            Assert.AreEqual("Blue", task.TaskColor);
            Assert.IsNull(task.EndDate);
        }

        [Test]
        public void AddTask_InvalidEndDateInput_EndDateRemainsNull()
        {
            // Arrange
            mockConsole.SetupSequence(c => c.ReadLine())
                .Returns("Test Title")
                .Returns("Test Description")
                .Returns("Important")
                .Returns("Red")
                .Returns("Blue")
                .Returns("y") // Set end date
                .Returns("invalid") // Invalid input for days
                .Returns("y"); // Confirm adding task

            // Act
            var task = taskRepo.AddTask();

            // Assert
            Assert.NotNull(task);
            Assert.AreEqual("Test Title", task.Title);
            Assert.AreEqual("Test Description", task.Description);
            Assert.IsNull(task.EndDate); // End date should be null
        }

        [Test]
        public void AddTask_CancelAddingTask_ReturnsNull()
        {
            // Arrange
            mockConsole.SetupSequence(c => c.ReadLine())
                .Returns("Test Title")
                .Returns("Test Description")
                .Returns("Important")
                .Returns("Red")
                .Returns("Blue")
                .Returns("y") // Set end date
                .Returns("3") // Days for end date
                .Returns("n"); // Cancel adding task

            // Act
            var task = taskRepo.AddTask();

            // Assert
            Assert.IsNull(task); // Task should not be created
        }

        [Test]
        public void AddTask_AssignsUniqueIdBasedOnExistingTasks()
        {
            // Arrange
            var existingTasks = new List<Task>
            {
                new Task { Id = 1, Title = "Existing Task 1" },
                new Task { Id = 2, Title = "Existing Task 2" }
            };
            mockConsole.SetupSequence(c => c.ReadLine())
                .Returns("New Task Title")
                .Returns("New Task Description")
                .Returns("Important")
                .Returns("Red")
                .Returns("Blue")
                .Returns("n") // No end date
                .Returns("y"); // Confirm adding task

            // Mock LoadTasks to return existing tasks
            Mock<TasksRepository> mockRepo = new Mock<TasksRepository>(mockConsole.Object) { CallBase = true };
            mockRepo.Setup(repo => repo.LoadTasks()).Returns(existingTasks);

            // Act
            var task = mockRepo.Object.AddTask();

            // Assert
            Assert.NotNull(task);
            Assert.AreEqual(3, task.Id); // Id should be 3 (next after existing tasks)
        }
    }

    // Інтерфейс для Console, щоб можна було створити мок-об'єкти
    public interface IConsole
    {
        string ReadLine();
        void WriteLine(string message);
    }

    // Модифікований TaskRepository для тестування
    public class TasksRepository
    {
        private readonly IConsole console;
        private static readonly string filePath = "tasks.json";

        public TasksRepository(IConsole console)
        {
            this.console = console;
        }

        public virtual List<Task> LoadTasks()
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<List<Task>>(json) ?? new List<Task>();
            }
            return new List<Task>();
        }

        public virtual Task AddTask()
        {
            console.WriteLine("Enter Task Title:");
            string title = console.ReadLine();

            console.WriteLine("Enter Task Description (optional, press Enter to skip):");
            string description = console.ReadLine();

            console.WriteLine("Enter Flag (e.g., Important, Unimportant):");
            string flag = console.ReadLine();

            console.WriteLine("Enter Label Color:");
            string labelColor = console.ReadLine();

            console.WriteLine("Enter Task Color:");
            string taskColor = console.ReadLine();

            DateTime creationDate = DateTime.Now;
            DateTime? endDate = null;

            console.WriteLine("Do you want to set an end date? (y/n)");
            if (console.ReadLine()?.ToLower() == "y")
            {
                console.WriteLine("Enter the number of days from today to set the end date:");
                if (int.TryParse(console.ReadLine(), out int daysToAdd))
                {
                    endDate = creationDate.AddDays(daysToAdd);
                }
            }

            console.WriteLine("Confirm add task? (y/n)");
            if (console.ReadLine()?.ToLower() == "y")
            {
                var newTask = new Task
                {
                    Id = LoadTasks().Count + 1,
                    Title = title,
                    CreationDate = creationDate,
                    EndDate = endDate,
                    Description = string.IsNullOrEmpty(description) ? null : description,
                    Flag = flag,
                    LabelColor = labelColor,
                    TaskColor = taskColor
                };
                return newTask;
            }
            return null;
        }
    }
}