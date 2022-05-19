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
        public void TestSetup()
        {
            AroFloConfiguration.SECRET_KEY = "RHIzTUFiUlJhSUpPenNQaFA2WHBzcGMzYXJlM1RxMCtDVW5uNkRKdnhITzI1S0krNW4vM0NZdk45SnlnNFFTaG1wcnB0WXBlRGMzNlFYeDEwVE9Wbmc9PQ==";
            AroFloConfiguration.U_ENCODE = "PjZPQjtBSEM7RihdOjI6JDJMKlwgJiohQ0AxTVw4Klg9Jzk6NDUpWiwK";
            AroFloConfiguration.P_ENCODE = "cTdod3FkODFlNnI0TGVk";
            AroFloConfiguration.ORG_ENCODE = "JSc6TyBQLFAgCg==";
        }

        [Test]
        public async Task GetAllProjects()
        {
            var p = await ProjectController.GetProjectsAsync();

            Assert.IsTrue(p.Count > 1);
        }

        [Test]
        public async Task GetAllProjects_Cancel_After_1_Seconds()
        {
            var cancelToken = new CancellationTokenSource();
            cancelToken.CancelAfter(TimeSpan.FromMilliseconds(1));
            var p = await ProjectController.GetProjectsAsync(cancelToken.Token);
            Assert.IsTrue(p.Count == 0);
        }

        [Test]
        public async Task GetOpenProjects()
        {
            var p = await ProjectController.GetOpenProjectsAsync();

            foreach (Project project in p.Where(project => project.Status != "open"))
            {
                Assert.Fail("a project status was not open.");
            }
        }

        [Test]
        public async Task GetClosedProjects()
        {
            var p = await ProjectController.GetClosedProjectsAsync();

            foreach (Project project in p.Where(project => project.Status != "closed"))
            {
                Assert.Fail("a project status was not closed.");
            }
        }

        [Test]
        public async Task GetProjectByNumberTest()
        {
            var p = await ProjectController.GetProjectAsync(834);

            Assert.AreEqual(834, p.ProjectNumber);
        }

        [Test]
        public async Task SearchProjectsTest()
        {
            var p = await ProjectController.GetProjectsByProjectNameAsync("PN911 Kurralta Park");

            Assert.IsTrue(p.Count > 0);
        }
    }
}
