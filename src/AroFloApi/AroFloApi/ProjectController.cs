using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AroFloApi.Enums;
using AroFloApi.Models;
using AroFloApi.Responses;

namespace AroFloApi
{
    public class ProjectController
    {
        public async Task<List<Project>> GetProjectsAsync(CancellationToken cancellationToken = default)
        {
            var aroFloController = new AroFloController();
            var projects = await aroFloController.GetAroFloObjectsAsync<ProjectZoneResponse, Project>(cancellationToken);
            return projects;
        }

        // TODO: In future hopefully we can use more filters as they enable them with the API.
        // var controller = new AroFloController();
        // return await controller.GetAroFloObjectsAsync<ProjectZoneResult, Project>(Fields.Status, "open", cancellationToken);
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
            var controller = new AroFloController();
            var project = await controller.GetAroFloObject<ProjectZoneResponse, Project>(Fields.ProjectNumber, number.ToString(), cancellationToken);

            if (project == null)
            {
                return null;
            }

            var properties = typeof(Project).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                Console.WriteLine($"{property.Name}: {property.GetValue(project)}");
            }

            return project;
        }
    }
}
