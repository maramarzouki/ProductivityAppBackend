using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Model.DTOs.UserDTOs;
using Repository.TaskRepository;
using Repository.UserRepository;

namespace Service.TaskService
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;
        public TaskService(ITaskRepository taskRepository, IUserRepository userRepository)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
        }
        public async Task<string> CreateTask(TaskModel task)
        {
            var user = _userRepository.GetUserById(task.UserId);
            if (user == null)
            {
                throw new Exception("User not found!");
            }
            var newTask = new TaskModel
            {
                Id = task.Id,
                Label = task.Label,
                Description = task.Description,
                StartTime = task.StartTime,
                EndTime = task.EndTime,
                Date = task.Date,
                Complexity = task.Complexity,
                Priority = task.Priority,
                TaskColor = task.TaskColor,
                Interval = task.Interval,
                Duration = task.Duration,
                DurationUnit = task.DurationUnit,
                IsChecked = task.IsChecked,
                IsRepetitive = task.IsRepetitive,
                RepetitionDates = task.RepetitionDates,
                Weekdays = task.Weekdays,
                UserId = task.UserId,
                ProjectId = task.ProjectId
            };
            await _taskRepository.CreateTask(newTask);
            return "Task created successfully!";
        }

        public async Task<string> DeleteTask(int taskId)
        {
            bool deleted = await _taskRepository.DeleteTask(taskId);
            if (!deleted)
            {
                return "Task not found!";
            }
            return "Task deleted!";
        }

        public async Task<TaskModel> GetTaskById(int taskId)
        {
            var task = await _taskRepository.GetTaskById(taskId);
            if (task == null)
            {
                throw new KeyNotFoundException($"Task with ID {taskId} not found.");
            }
            return task;
        }

        public async Task<string> CheckTask(bool isChecked, int taskId)
        {
            await _taskRepository.CheckTask(isChecked, taskId);
            return "Task checked!";
        }

        public async Task<List<TaskModel>> GetUserTasks(int userId)
        {
            var tasks = await _taskRepository.GetUserTasks(userId); ;
            return tasks;
        }

        public async Task<List<TaskModel>> GetUserTasksByDate(int userId, DateTime selectedDate)
        {
            var tasks = await _taskRepository.GetEachDayTasksByUserId(userId,selectedDate);;
            return tasks;
        }

        public async Task<List<TaskModel>> GetTasksByProjectId(int projectId)
        {
            var tasks = await _taskRepository.GetTasksByProjectId(projectId); ;
            return tasks;
        }

        public async Task<string> UpdateTask(TaskModel task)
        {
            var result = await _taskRepository.UpdateTask(task);
            if (!result)
            {
                throw new Exception("Error updating the task!");
            }
            return "Task updated!";
        }
    }
}
