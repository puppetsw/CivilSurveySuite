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
    public static class ProjectController
    {
        /// <summary>
        /// Gets all projects from AroFlo.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to notify that operations should be canceled.</param>
        /// <returns>A <see cref="List{T}"/> of <see cref="Project"/> containing all projects.</returns>
        public static async Task<List<Project>> GetProjectsAsync(CancellationToken cancellationToken = default)
        {
            var aroFloController = new AroFloController();
            var projects = await aroFloController.GetAroFloObjectsAsync<ProjectZoneResponse, Project>(cancellationToken);
            return projects;
        }

        // TODO: In future hopefully we can use more filters as they enable them with the API.
        // var controller = new AroFloController();
        // return await controller.GetAroFloObjectsAsync<ProjectZoneResult, Project>(Fields.Status, "open", cancellationToken);
        public static async Task<List<Project>> GetOpenProjectsAsync(CancellationToken cancellationToken = default)
        {
            var projects = await GetProjectsAsync(cancellationToken);
            var list = projects.Where(project => project.Status == "open").ToList();
            return list;
        }

        public static async Task<List<Project>> GetClosedProjectsAsync(CancellationToken cancellationToken = default)
        {
            var projects = await GetProjectsAsync(cancellationToken);
            var list = projects.Where(project => project.Status == "closed").ToList();
            return list;
        }

        /// <summary>
        /// Gets a list of <see cref="Project"/>(s) by their name. Name must match exactly.
        /// </summary>
        /// <param name="searchText">The project name text to 'search' for.</param>
        /// <param name="cancellationToken">Cancellation token to notify that operations should be canceled.</param>
        /// <returns>A <see cref="List{T}"/> of <see cref="Project"/>.</returns>
        public static async Task<List<Project>> GetProjectsByProjectNameAsync(string searchText, CancellationToken cancellationToken = default)
        {
            // var projects = await GetProjectsAsync(cancellationToken);
            // var list = projects.Where(project => project.ProjectName.Contains(searchText) ||
            //                                      project.Client.Name.Contains(searchText)).ToList();
            // return list;
            var aroFloController = new AroFloController();
            var list = await aroFloController.GetAroFloObjectsAsync<ProjectZoneResponse, Project>(Field.ProjectName, searchText, cancellationToken);
            return list;
        }

        public static async Task<Project> GetProjectAsync(int number, CancellationToken cancellationToken = default)
        {
            var controller = new AroFloController();
            var project = await controller.GetAroFloObject<ProjectZoneResponse, Project>(Field.ProjectNumber, number.ToString(), cancellationToken);

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
