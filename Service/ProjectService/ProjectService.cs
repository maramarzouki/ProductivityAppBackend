using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Repository.ProjectRepository;
using Repository.UserRepository;

namespace Service.ProjectService
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;
        public ProjectService(IProjectRepository projectRepository, IUserRepository userRepository)
        {
            _projectRepository = projectRepository;
            _userRepository = userRepository;
        }

        public async Task<string> CreateProject(ProjectModel project)
        {
            var user = _userRepository.GetUserById(project.UserId);
            if (user == null)
            {
                throw new Exception("User not found!");
            }
            var newProject = new ProjectModel
            {
                Id = project.Id,
                Description = project.Description,
                Title = project.Title,
                Deadline = project.Deadline,
                IsCompleted = project.IsCompleted,
                UserId = project.UserId,
            };
            await _projectRepository.CreatePrject(newProject);
            return "Project created successfully!";
        }

        public async Task<ProjectModel> GetProjectById(int projectId)
        {
            var project = await _projectRepository.GetProjectById(projectId);
            if(project == null)
            {
                throw new KeyNotFoundException($"Project with ID {projectId} not found.");
            }
            return project;
        }

        public async Task<List<ProjectModel>> GetUserProjects(int userId)
        {
            return await _projectRepository.GetProjectsByUserId(userId);
        }

        public async Task<string> CompleteProject(bool isCompleted, int projectd)
        {
            await _projectRepository.CompleteProject(isCompleted, projectd);
            return "Projct completed!";
        }

        public async Task<string> DeleteProject(int projectId)
        {
            bool deleted = await _projectRepository.DeleteProject(projectId);
            if (!deleted)
            {
                return "Project not found!";
            }
            return "Project deleted!";
        }

        public async Task<string> UpdateProject(ProjectModel project)
        {
            var result = await _projectRepository.UpdateProject(project);
            if (!result)
            {
                throw new Exception("Error updating the project!");
            }
            return "Project updated!";
        }
    }
}
