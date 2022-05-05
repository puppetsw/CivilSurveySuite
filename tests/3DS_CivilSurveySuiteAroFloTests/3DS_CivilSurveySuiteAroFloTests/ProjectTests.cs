using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AroFloApi;
using NUnit.Framework;

namespace _3DS_CivilSurveySuiteAroFloTests
{
    [TestFixture]
    public class ProjectTests
    {
        [Test]
        public async Task GetAllProjects()
        {
            var projects = new ProjectService();
            var p = await projects.GetProjectsAsync();

            Assert.IsTrue(p.Count > 1);
        }

        [Test]
        public async Task GetAllProjects_Cancel_After_1_Seconds()
        {
            var projects = new ProjectService();
            var cancelToken = new CancellationTokenSource();
            cancelToken.CancelAfter(TimeSpan.FromSeconds(1));
            var p = await projects.GetProjectsAsync(cancelToken.Token);
            Assert.IsTrue(p.Count == 0);
        }

        [Test]
        public async Task GetOpenProjects()
        {
            var projects = new ProjectService();
            var p = await projects.GetOpenProjectsAsync();

            foreach (Project project in p.Where(project => project.Status != "open"))
            {
                Assert.Fail("a project status was not open.");
            }
        }

        [Test]
        public async Task GetClosedProjects()
        {
            var projects = new ProjectService();
            var p = await projects.GetClosedProjectsAsync();

            foreach (Project project in p.Where(project => project.Status != "closed"))
            {
                Assert.Fail("a project status was not closed.");
            }
        }

        [Test]
        public async Task GetProjectByNumberTest()
        {
            var projects = new ProjectService();
            var p = await projects.GetProjectAsync(834);

            Assert.AreEqual(834, p.ProjectNumber);
        }


    }
}
