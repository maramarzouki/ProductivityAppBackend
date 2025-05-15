using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Repository.ProjectRepository
{
    public interface IProjectRepository
    {
        public Task CreatePrject(ProjectModel project);
        public Task<ProjectModel?> GetProjectById(int projectId);
        public Task<List<ProjectModel>> GetProjectsByUserId(int userId);
        public Task CompleteProject(bool isCompleted, int projectId);
        public Task<bool> DeleteProject(int projectId);
        public Task<bool> UpdateProject(ProjectModel project);
    }
}
