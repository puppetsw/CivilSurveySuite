using System.Xml.Serialization;

namespace AroFloApi.Enums
{
    public enum Status
    {
        // -99999 Authentication Failed - Signatures do not match	You have an error in the way you are generating you HMAC signature. Refer to the comments in the Pre-Req scripts of this PostMan Collection to ensure you are following all requried steps.
        //        Authentication Failed - AroFlo API setup is invalid	Most likely caused by not generating the auth credentials correctly by forgetting to save after generating the Secret Key. Ensure you copy this key BEFORE you save as it is only shown once
        //        Authentication Failed - Required headers are missing	You have not included all of the required headers in your request. Confirm that you have included the:Authentication, Authorization, Accept and afdatetimeutc headers.
        //        Authentication Failed - Payload has expired	Ensure that the computer you are sending on has their time correctly sync'd to a timeserver. We do allow a small window of difference between the datetime you include in your header and the datetime
        //        we receive the request, but if it is outside that window you will receive this error.
        [XmlEnum("-99999")]
        AuthenticationFailed = -99999,
        // 0	Login OK	Log in successful, proceed...
        [XmlEnum("0")]
        LoginOk = 0,
        // 1	Login Failed - Invalid Request String	Checks that uEncoded, pEncoded & orgEncoded all exist
        [XmlEnum("1")]
        LoginFailedInvalidRequestString = 1,
        // 2	Login Failed - Invalid Username or Password	Checks that uEncoded, pEncoded & orgEncoded contain values and are not empty
        [XmlEnum("2")]
        LoginFailedInvalidUsernamePassword = 2,
        // 3	Login Failed - Permission Denied	Incorrect Username, Password and/or Org. Or AroFloAPI access is not enabled
        [XmlEnum("3")]
        LoginFailedPermissionDenied = 3,
        // 4	Login Failed - Permission Denied	User logged in, but has no AroFloAPI Access enabled
        [XmlEnum("4")]
        LoginFailedPermissionDeniedNoApiAccess = 4,
        // 5	Invalid Request Method	The API Call was neither a GET or POST request
        [XmlEnum("5")]
        InvalidRequestMethod = 5,
        // 6	Exceeded Rate Limit - Requests per Minute	Exceeded the 60 requests per minute limit
        //      Exceeded Rate Limit - Requests per Second	Exceeded the 1 request a second limit
        [XmlEnum("6")]
        ExceededRateLimit = 6,
        // 7	Exceeded Rate Limit - Daily	Exceeded the 2,000 requests per day limit
        [XmlEnum("7")]
        ExceededRateLimitDaily = 7,
        // 8	Exceeded Size Limit	Exceeded the request size limit of 3.5Mb
        [XmlEnum("8")]
        ExceededSizeLimit = 8,
        // 20	Login Failed - Permission Denied	Legacy status code. Due to AroFloAPI Access not enabled via legacy login method
        [XmlEnum("20")]
        LoginFailedPermissionDeniedLegacy = 20,
        // 30	Login Failed - Permission Denied	AroFloAPI has been disabled globally for the entire system
        [XmlEnum("30")]
        LoginFailedPermissionDeniedApiDisabled = 30,
        // 429	Too Many Requests Per Second (max x3)	Slow down your request speed to conform to the limits
        [XmlEnum("429")]
        TooManyRequests = 429
    }
}
