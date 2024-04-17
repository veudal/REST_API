using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace REST_API.Controllers
{


    [ApiController]
    [Route("[controller]")]
    public class TaskListController : ControllerBase
    {
        private static List<TaskItem> TaskList = new()
        {
            new TaskItem("Organize documents", 0, DateTime.Now.AddMinutes(-5), true, DateOnly.FromDateTime(DateTime.Now.AddDays(2))),
            new TaskItem("Purchase subscription", 1, DateTime.Now.AddMinutes(-2), true, DateOnly.FromDateTime(DateTime.Now.AddDays(5))),
            new TaskItem("Upload photos", 2, DateTime.Now, false, null),
        };

        private readonly ILogger<TaskListController> _logger;

        public TaskListController(ILogger<TaskListController> logger)
        {
            _logger = logger;
        }

        [HttpGet("Tasks")]
        public IActionResult Tasks(bool? isCompleted)
        {
            if (isCompleted is not null)
            {
                List<TaskItem> tasks = new List<TaskItem>();
                foreach (var task in TaskList)
                {
                    if (task.Completed == isCompleted)
                        tasks.Add(task);
                }
                return Ok(tasks);
            }
            return Ok(TaskList);

        }


        [HttpGet("TaskByID")]
        public IActionResult TaskByID(int ID)
        {
            if (ID < 0)
                return BadRequest("ID not valid.");

            int task = TaskList.FindIndex(t => t.ID == ID);
            if (task == -1)
                return BadRequest("No task with this ID.");

            return Ok(TaskList[task]);
        }

        [HttpGet("TaskByKeyword")]
        public IActionResult TaskByKeyword(string keyword)
        {
            if (keyword == string.Empty)
                return BadRequest("Keyword cannot be empty.");

            var tasks = TaskList.FindAll(task => task.Content.ToLower().Contains(keyword.ToLower()));
            if (tasks.Count == 0)
                return NotFound("Keyword could not be found.");

            return Ok(tasks);
        }


        [HttpPost("Create")]
        public IActionResult Create([FromBody] TaskItem task)
        {
            if (string.IsNullOrEmpty(task.Content))
                return BadRequest("Not enough information was provided.");

            task.CreationDate = DateTime.UtcNow;
            task.ID = TaskList[TaskList.Count - 1].ID + 1;
            TaskList.Add(task);
            return Ok();
        }

        [HttpPut("Update")]
        public IActionResult Update(int ID, string? content, bool? completed, bool? flagged, DateOnly? dueDate)
        {

            if (ID < 0)
                return BadRequest("ID not valid.");

            int taskToUpdate = TaskList.FindIndex(t => t.ID == ID);
            if (taskToUpdate == -1)
                return BadRequest("No task with this ID.");

            if (content is not null)
                TaskList[taskToUpdate].Content = content;
            if (completed is not null)
                TaskList[taskToUpdate].Completed = (bool)completed;
            if (flagged is not null)
                TaskList[taskToUpdate].Flagged = (bool)flagged;
            if (dueDate is not null)
                TaskList[taskToUpdate].DueDate = (DateOnly)dueDate;

            return Ok();
        }

        [HttpPut("EditContent")]
        public IActionResult EditContent(int ID, string newContent)
        {
            if (ID < 0)
                return BadRequest("ID not valid.");

            int taskToUpdate = TaskList.FindIndex(t => t.ID == ID);
            if (taskToUpdate == -1)
                return BadRequest("No task with this ID.");

            TaskList[taskToUpdate].Content = newContent;
            return Ok();
        }

        [HttpPut("ToggleCompletion")]
        public IActionResult ToggleCompletion(int ID)
        {
            if (ID < 0)
                return BadRequest("ID not valid.");

            int taskToUpdate = TaskList.FindIndex(t => t.ID == ID);
            if (taskToUpdate == -1)
                return BadRequest("No task with this ID.");

            TaskList[taskToUpdate].Completed = !TaskList[taskToUpdate].Completed;
            return Ok();
        }

        [HttpPut("ToggleFlag")]
        public IActionResult ToggleFlag(int ID)
        {
            if (ID < 0)
                return BadRequest("ID not valid.");

            int taskToUpdate = TaskList.FindIndex(t => t.ID == ID);
            if (taskToUpdate == -1)
                return BadRequest("No task with this ID.");

            TaskList[taskToUpdate].Flagged = !TaskList[taskToUpdate].Flagged;
            return Ok();
        }

        [HttpDelete("Delete")]
        public IActionResult Delete(int ID)
        {
            if (ID < 0)
                return BadRequest("ID not valid.");

            TaskItem taskToRemove = TaskList.FirstOrDefault(task => task.ID == ID);
            if (TaskList.Remove(taskToRemove) == false)
                return BadRequest("No task with this ID.");

            return Ok();
        }

        [HttpDelete("BatchDelete")]
        public IActionResult BatchDelete(int startID, int endID)
        {
            if (startID > endID)
                return BadRequest("Start ID cannot be smaller than end ID.");

            if (startID < 0 || endID < 0)
                return BadRequest("ID not valid.");

            int counter = 0;
            for (int i = startID; i <= endID; i++)
            {
                TaskItem taskToRemove = TaskList.FirstOrDefault(task => task.ID == i);
                if (TaskList.Remove(taskToRemove))
                    counter++;
            }
            return Ok(counter + " tasks deleted.");
        }
    }
}