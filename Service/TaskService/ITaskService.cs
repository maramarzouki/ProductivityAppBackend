using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Service.TaskService
{
    public interface ITaskService
    {
        Task<string> CreateTask(TaskModel task);
        Task<TaskModel> GetTaskById(int taskId);
        Task<List<TaskModel>> GetUserTasksByDate(int userId, DateTime selectedDate);
        Task<List<TaskModel>> GetUserTasks(int userId);
        Task<List<TaskModel>> GetTasksByProjectId(int projectId);
        Task<string> CheckTask(bool isChecked, int taskId);
        Task<string> DeleteTask(int taskId);
        Task<string> UpdateTask(TaskModel task);
    }
}
