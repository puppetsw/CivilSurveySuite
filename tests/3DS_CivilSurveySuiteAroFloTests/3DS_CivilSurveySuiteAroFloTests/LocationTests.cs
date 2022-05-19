using System.Threading.Tasks;
using AroFloApi;
using NUnit.Framework;

namespace _3DS_CivilSurveySuiteAroFloTests
{
    [TestFixture]
    public class LocationTests
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
        public async Task GetLocationTest()
        {
            var location = await LocationController.GetLocationAsync("JSYqKyBSXFggCg==").ConfigureAwait(false);
            Assert.AreEqual(-34.7293199477, location.Latitude);
            Assert.AreEqual(138.580861384, location.Longitude);
            Assert.AreEqual("5110", location.PostCode);
            Assert.AreEqual("Burton", location.Suburb);
            Assert.AreEqual("SA", location.State);
            Assert.AreEqual("North HUB - Waterloo Corner Road", location.LocationName);
            Assert.IsFalse(location.IsArchived);
        }
    }
}

// <location>
//   <locationid>JSYqKyBSXFggCg==</locationid>
//   <gpslat>-34.7293199477</gpslat>
//   <postcode>5110</postcode>
//   <sitecontact></sitecontact>
//   <state>SA</state>
//   <suburb>Burton</suburb>
//   <siteemail></siteemail>
//   <customfields></customfields>
//   <linkedto>
//   <linkedtoname>Boral Asphalt SA</linkedtoname>
//   <linkedtoid>JSc6Uy1QTFwgCg==</linkedtoid>
//   <linkedtotype>client</linkedtotype>
//   </linkedto>
//   <locationname>North HUB - Waterloo Corner Road</locationname>
//   <notes></notes>
//   <country>AUSTRALIA</country>
//   <gpslong>138.580861384</gpslong>
//   <address></address>
//   <documentsandphotos></documentsandphotos>
//   <archived>FALSE</archived>
//   <sitephone></sitephone>
// </location>
