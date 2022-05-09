using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AroFloApi
{
    public interface IProjectService
    {
        /// <summary>
        /// Gets all <see cref="Project"/>s.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads
        /// to receive notice of cancellation.</param>
        /// <returns>List&lt;Project&gt;.</returns>
        Task<List<Project>> GetProjectsAsync(CancellationToken cancellationToken = default);

        /// <summary>Gets all <see cref="Project"/>s with status 'open'</summary>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads
        /// to receive notice of cancellation.</param>
        /// <returns><see cref="List{T}"/> where T is <see cref="Project"/></returns>
        Task<List<Project>> GetOpenProjectsAsync(CancellationToken cancellationToken = default);

        /// <summary>Gets all <see cref="Project"/>s with status 'closed'</summary>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads
        /// to receive notice of cancellation.</param>
        /// <returns><see cref="List{T}"/> where T is <see cref="Project"/></returns>
        Task<List<Project>> GetClosedProjectsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all <see cref="Project"/>s where the searchText is contained in the name or client name.
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads
        /// to receive notice of cancellation.</param>
        /// <returns>List&lt;Project&gt;.</returns>
        Task<List<Project>> GetProjectsAsync(string searchText, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the <see cref="Project"/> that matches the number provided.
        /// </summary>
        /// <param name="number">The number of the project.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;Project&gt;.</returns>
        Task<Project> GetProjectAsync(int number, CancellationToken cancellationToken = default);
    }
}
