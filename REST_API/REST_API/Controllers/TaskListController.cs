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


        // Retrieves a list of tasks, can optionally be filtered by completion status
        [HttpGet("Tasks")]
        public List<TaskItem> Tasks(bool? isCompleted)
        {
            _logger.LogInformation("Processing request for method: 'Tasks'");

            if (isCompleted is not null)
            {
                List<TaskItem> tasks = new List<TaskItem>();
                foreach (var task in TaskList)
                {
                    if (task.Completed == isCompleted)
                        tasks.Add(task);
                }
                return tasks;
            }
            return TaskList;

        }

        // Retrieves a task by its unique ID.
        [HttpGet("TaskByID")]
        public TaskItem TaskByID(int ID)
        {
            _logger.LogInformation("Processing request for method: 'TaskByID', ID: {ID}", ID);


            if (ID < 0)
                return null; //"ID not valid.";

            int task = TaskList.FindIndex(t => t.ID == ID);
            if (task == -1)
                return null; //"No task with this ID.";

            return TaskList[task];
        }

        // Searches tasks based on a keyword in their content.
        [HttpGet("TaskByKeyword")]
        public List<TaskItem> TaskByKeyword(string keyword)
        {
            _logger.LogInformation("Processing request for method: 'TaskByKeyword', Keyword: {keyword}", keyword);


            if (keyword == string.Empty)
                return new List<TaskItem>(); //"Keyword cannot be empty.";

            var tasks = TaskList.FindAll(task => task.Content.ToLower().Contains(keyword.ToLower()));
            if (tasks.Count == 0)
                return new List<TaskItem>(); //"Keyword could not be found.";

            return tasks;
        }

        // Creates a new task from the provided information.
        [HttpPost("Create")]
        public string Create([FromBody] TaskItem task)
        {
            _logger.LogInformation("Processing request for method: 'Create'");

            if (string.IsNullOrEmpty(task.Content))
                return "Not enough information was provided.";

            task.CreationDate = DateTime.UtcNow;
            task.ID = TaskList[TaskList.Count - 1].ID + 1;
            TaskList.Add(task);
            return "Success.";
        }

        // Updates an existing tasks information like: content, completion status, flagged status or due date.
        [HttpPut("Update")]
        public string Update(int ID, string? content, bool? completed, bool? flagged, DateOnly? dueDate)
        {
            _logger.LogInformation("Processing request for method: 'Update', ID: {ID}", ID);

            if (ID < 0)
                return "ID not valid.";

            int taskToUpdate = TaskList.FindIndex(t => t.ID == ID);
            if (taskToUpdate == -1)
                return "No task with this ID.";

            if (content is not null)
                TaskList[taskToUpdate].Content = content;
            if (completed is not null)
                TaskList[taskToUpdate].Completed = (bool)completed;
            if (flagged is not null)
                TaskList[taskToUpdate].Flagged = (bool)flagged;
            if (dueDate is not null)
                TaskList[taskToUpdate].DueDate = (DateOnly)dueDate;

            return "Success.";
        }

        // Updates the content of an existing task.
        [HttpPut("EditContent")]
        public string EditContent(int ID, string newContent)
        {
            _logger.LogInformation("Processing request for method: 'EditContent', ID: {ID}", ID);

            if (ID < 0)
                return "ID not valid.";

            int taskToUpdate = TaskList.FindIndex(t => t.ID == ID);
            if (taskToUpdate == -1)
                return "No task with this ID.";

            TaskList[taskToUpdate].Content = newContent;
            return "Success.";
        }

        // Toggles the completion status of a task by its ID.
        [HttpPut("ToggleCompletion")]
        public string ToggleCompletion(int ID)
        {
            _logger.LogInformation("Processing request for method: 'ToogleCompletion', ID: {ID}", ID);

            if (ID < 0)
                return "ID not valid.";

            int taskToUpdate = TaskList.FindIndex(t => t.ID == ID);
            if (taskToUpdate == -1)
                return "No task with this ID.";

            TaskList[taskToUpdate].Completed = !TaskList[taskToUpdate].Completed;
            return "Success.";
        }

        // Toggles the flagged status of a task by its ID.
        [HttpPut("ToggleFlag")]
        public string ToggleFlag(int ID)
        {
            _logger.LogInformation("Processing request for method: 'ToggleFlag', ID: {ID}", ID);

            if (ID < 0)
                return "ID not valid.";

            int taskToUpdate = TaskList.FindIndex(t => t.ID == ID);
            if (taskToUpdate == -1)
                return "No task with this ID.";

            TaskList[taskToUpdate].Flagged = !TaskList[taskToUpdate].Flagged;
            return "Success.";
        }

        // Deletes a task by its ID.
        [HttpDelete("Delete")]
        public string Delete(int ID)
        {
            _logger.LogInformation("Processing request for method: 'Delete', ID: {ID}", ID);

            if (ID < 0)
                return "ID not valid.";

            TaskItem taskToRemove = TaskList.FirstOrDefault(task => task.ID == ID);
            if (TaskList.Remove(taskToRemove) == false)
                return "No task with this ID.";

            return "Success.";
        }

        // Deletes multiple tasks within a specified ID range.
        [HttpDelete("BatchDelete")]
        public string BatchDelete(int startID, int endID)
        {
            _logger.LogInformation("Processing request for method: 'BatchDelete', startID: {StartID}, endID: {EndID}", startID, endID);

            if (startID > endID)
                return "Start ID cannot be smaller than end ID.";

            if (startID < 0 || endID < 0)
                return "ID not valid.";

            int counter = 0;
            for (int i = startID; i <= endID; i++)
            {
                TaskItem taskToRemove = TaskList.FirstOrDefault(task => task.ID == i);
                if (TaskList.Remove(taskToRemove))
                    counter++;
            }
            return "Success. " + counter + " tasks deleted.";
        }
    }
}