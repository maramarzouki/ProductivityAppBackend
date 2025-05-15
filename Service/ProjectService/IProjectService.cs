using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Service.ProjectService
{
    public interface IProjectService
    {
        Task<string> CreateProject(ProjectModel project);
        Task<ProjectModel> GetProjectById(int projectId);
        Task<List<ProjectModel>> GetUserProjects(int userId);
        Task<string> CompleteProject(bool isCompleted, int projectd);
        Task<string> DeleteProject(int projectId);
        Task<String> UpdateProject(ProjectModel project);
    }
}
