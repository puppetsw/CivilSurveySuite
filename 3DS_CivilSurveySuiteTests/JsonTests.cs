using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class JsonTests
    {
        [TestMethod]
        public void Test_Json_Writer()
        {

        }
    }

    public class DescriptionKey
    {
        #region Private Members

        private string key;
        private string layer;
        private string description;
        private bool draw2D;
        private bool draw3D;

        #endregion

        #region Properties

        public string Key { get => key; set { key = value; } }
        public string Layer { get => layer; set { layer = value;  } }
        public string Description { get => description; set { description = value; } }
        public bool Draw2D { get => draw2D; set { draw2D = value; } }
        public bool Draw3D { get => draw3D; set { draw3D = value; } }

        #endregion

    }
}
