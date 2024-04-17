using System.ComponentModel.DataAnnotations;

namespace REST_API
{
    public class TaskItem
    {
        public TaskItem(string content, int id, DateTime creationDate, bool flagged, DateOnly? dueDate) 
        {
            Content = content;
            ID = id;
            CreationDate = creationDate;
            Flagged = flagged;

            if(dueDate != null) 
                DueDate = (DateOnly)dueDate;
        }

        public TaskItem()
        {

        }

        public int ID { get; set; }

        public string Content { get; set; }

        public bool Completed { get; set; }

        public bool Flagged { get; set; }

        public DateTime CreationDate { get; set; }

        public DateOnly DueDate { get; set; }

    }
}