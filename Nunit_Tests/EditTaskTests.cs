using Agile_Pract6;
using Newtonsoft.Json;

namespace Nunit_Tests
{
    [TestFixture]
    public class EditTaskTests
    {
        private List<Agile_Pract6.Task> _tasks;

        [SetUp]
        public void Setup()
        {
            _tasks = new List<Agile_Pract6.Task>
        {
            new Agile_Pract6.Task { Id = 1, Title = "Initial Task", Flag = "важливе" }
        };
            SaveTasksToFile(_tasks);
        }

        [Test]
        public void EditTask_UpdateTitle_TitleShouldChange()
        {
            // Arrange
            var taskId = 1;
            var newTitle = "Updated Task Title";

            // Act
            var task = _tasks.FirstOrDefault(t => t.Id == taskId);
            task.Title = newTitle;
            TaskRepository.SaveTasks(_tasks);

            // Assert
            var updatedTasks = TaskRepository.LoadTasks();
            Assert.AreEqual(newTitle, updatedTasks.First(t => t.Id == taskId).Title);
        }

        [Test]
        public void EditTask_UpdateFlag_FlagShouldChange()
        {
            // Arrange
            var taskId = 1;
            var newFlag = "неважливе";

            // Act
            var task = _tasks.FirstOrDefault(t => t.Id == taskId);
            task.Flag = newFlag;
            TaskRepository.SaveTasks(_tasks);

            // Assert
            var updatedTasks = TaskRepository.LoadTasks();
            Assert.AreEqual(newFlag, updatedTasks.First(t => t.Id == taskId).Flag);
        }

        [Test]
        public void EditTask_CancelChanges_ShouldNotSaveChanges()
        {
            // Arrange
            var taskId = 1;
            var originalTitle = _tasks.First(t => t.Id == taskId).Title;

            // Act
            var task = _tasks.FirstOrDefault(t => t.Id == taskId);
            task.Title = "Temporary Title"; // зміни, які не зберігаємо
                                            // Зберігаємо без збереження до файлу (імітація скасування)

            // Assert
            var loadedTasks = TaskRepository.LoadTasks();
            Assert.AreEqual(originalTitle, loadedTasks.First(t => t.Id == taskId).Title);
        }

        private void SaveTasksToFile(List<Agile_Pract6.Task> tasks)
        {
            var json = JsonConvert.SerializeObject(tasks, Formatting.Indented);
            File.WriteAllText("tasks.json", json);
        }
    }


}