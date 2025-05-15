using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Repository.TaskRepository
{
    public interface ITaskRepository
    {
        public Task CreateTask(TaskModel task);
        public Task<TaskModel?> GetTaskById(int taskId);
        public Task<List<TaskModel>> GetEachDayTasksByUserId(int userId, DateTime selectedDate);
        public Task<List<TaskModel>> GetUserTasks(int userId);
        public Task<List<TaskModel>> GetTasksByProjectId(int projectId);
        public Task CheckTask(bool isChecked, int taskId);
        public Task<bool> DeleteTask(int id);
        public Task<bool> UpdateTask(TaskModel task);
    }
}
