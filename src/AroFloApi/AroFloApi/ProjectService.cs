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
        public async Task<List<Project>> GetProjects(CancellationToken cancellationToken = default)
        {
            var aroFloController = new AroFloController();
            var projects = await aroFloController.GetAroFloObjectsAsync<ProjectZoneResult, Project>(cancellationToken);
            return projects;
        }

        public async Task<List<Project>> GetOpenProjects(CancellationToken cancellationToken = default)
        {
            var projects = await GetProjects(cancellationToken);
            var list = projects.Where(project => project.Status == "open").ToList();
            return list;
        }

        public async Task<List<Project>> GetClosedProjects(CancellationToken cancellationToken = default)
        {
            var projects = await GetProjects(cancellationToken);
            var list = projects.Where(project => project.Status == "closed").ToList();
            return list;
        }

        public async Task<List<Project>> GetProjects(string searchText, CancellationToken cancellationToken = default)
        {
            var projects = await GetProjects(cancellationToken);
            var list = projects.Where(project => project.ProjectName.Contains(searchText) ||
                                                 project.Client.Name.Contains(searchText)).ToList();
            return list;
        }

        public async Task<Project> GetProject(string number, CancellationToken cancellationToken = default)
        {
            var projects = await GetProjects(cancellationToken);
            var project = projects.FirstOrDefault(p => p.ProjectNumber.Equals(number.ToUpperInvariant()));

            var properties = typeof(Project).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                Console.WriteLine($"{property.Name}: {property.GetValue(project)}");
            }

            return project;
        }
    }
}
