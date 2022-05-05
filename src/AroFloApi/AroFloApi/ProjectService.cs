using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AroFloApi
{
    public class ProjectService : IProjectService
    {
        public async Task<List<Project>> GetProjectsAsync(CancellationToken cancellationToken = default)
        {
            var aroFloController = new AroFloController();
            var projects = await aroFloController.GetAroFloObjectsAsync<ProjectZoneResult, Project>(cancellationToken);
            return projects;
        }

        public async Task<List<Project>> GetOpenProjectsAsync(CancellationToken cancellationToken = default)
        {
            var projects = await GetProjectsAsync(cancellationToken);
            var list = projects.Where(project => project.Status == "open").ToList();
            return list;
        }

        public async Task<List<Project>> GetClosedProjectsAsync(CancellationToken cancellationToken = default)
        {
            var projects = await GetProjectsAsync(cancellationToken);
            var list = projects.Where(project => project.Status == "closed").ToList();
            return list;
        }

        public async Task<List<Project>> GetProjectsAsync(string searchText, CancellationToken cancellationToken = default)
        {
            var projects = await GetProjectsAsync(cancellationToken);
            var list = projects.Where(project => project.ProjectName.Contains(searchText) ||
                                                 project.Client.Name.Contains(searchText)).ToList();
            return list;
        }

        public async Task<Project> GetProjectAsync(int number, CancellationToken cancellationToken = default)
        {
            var projects = await GetProjectsAsync(cancellationToken);
            var project = projects.FirstOrDefault(p => p.ProjectNumber.Equals(number));

            var properties = typeof(Project).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                Console.WriteLine($"{property.Name}: {property.GetValue(project)}");
            }

            return project;
        }
    }
}
