using System.Linq;
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
            var p = await projects.GetProjects();

            Assert.IsTrue(p.Count > 1);
        }

        [Test]
        public async Task GetOpenProjects()
        {
            var projects = new ProjectService();
            var p = await projects.GetOpenProjects();

            foreach (Project project in p.Where(project => project.Status != "open"))
            {
                Assert.Fail("a project status was not open.");
            }
        }

        [Test]
        public async Task GetClosedProjects()
        {
            var projects = new ProjectService();
            var p = await projects.GetClosedProjects();

            foreach (Project project in p.Where(project => project.Status != "closed"))
            {
                Assert.Fail("a project status was not closed.");
            }
        }


    }
}
