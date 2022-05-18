using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AroFloApi;
using AroFloApi.Models;
using NUnit.Framework;

namespace _3DS_CivilSurveySuiteAroFloTests
{
    [TestFixture]
    public class ProjectTests
    {
        [SetUp]
        public void AroFloConfiguration()
        {
            AroFloApi.AroFloConfiguration.SECRET_KEY = "RHIzTUFiUlJhSUpPenNQaFA2WHBzcGMzYXJlM1RxMCtDVW5uNkRKdnhITzI1S0krNW4vM0NZdk45SnlnNFFTaG1wcnB0WXBlRGMzNlFYeDEwVE9Wbmc9PQ==";
            AroFloApi.AroFloConfiguration.U_ENCODE = "PjZPQjtBSEM7RihdOjI6JDJMKlwgJiohQ0AxTVw4Klg9Jzk6NDUpWiwK";
            AroFloApi.AroFloConfiguration.P_ENCODE = "cTdod3FkODFlNnI0TGVk";
            AroFloApi.AroFloConfiguration.ORG_ENCODE = "JSc6TyBQLFAgCg==";
        }

        [Test]
        public async Task GetAllProjects()
        {
            var projects = new ProjectController();
            var p = await projects.GetProjectsAsync();

            Assert.IsTrue(p.Count > 1);
        }

        [Test]
        public async Task GetAllProjects_Cancel_After_1_Seconds()
        {
            var projects = new ProjectController();
            var cancelToken = new CancellationTokenSource();
            cancelToken.CancelAfter(TimeSpan.FromMilliseconds(1));
            var p = await projects.GetProjectsAsync(cancelToken.Token);
            Assert.IsTrue(p.Count == 0);
        }

        [Test]
        public async Task GetOpenProjects()
        {
            var projects = new ProjectController();
            var p = await projects.GetOpenProjectsAsync();

            foreach (Project project in p.Where(project => project.Status != "open"))
            {
                Assert.Fail("a project status was not open.");
            }
        }

        [Test]
        public async Task GetClosedProjects()
        {
            var projects = new ProjectController();
            var p = await projects.GetClosedProjectsAsync();

            foreach (Project project in p.Where(project => project.Status != "closed"))
            {
                Assert.Fail("a project status was not closed.");
            }
        }

        [Test]
        public async Task GetProjectByNumberTest()
        {
            var projects = new ProjectController();
            var p = await projects.GetProjectAsync(834);

            Assert.AreEqual(834, p.ProjectNumber);
        }
    }
}
