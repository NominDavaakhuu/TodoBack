using System.Configuration;
using System.Net;
using System.Web;

public static class ApiClientFactory
{
    private const string CookieJarKey = "API_COOKIE_JAR";

    public static CookieContainer GetCookieJar()
    {
        var session = HttpContext.Current.Session;
        if (session[CookieJarKey] == null)
            session[CookieJarKey] = new CookieContainer();

        return (CookieContainer)session[CookieJarKey];
    }

    public static void ClearCookieJar()
    {
        HttpContext.Current.Session.Remove(CookieJarKey);
    }

    public static string BaseUrl =>
        ConfigurationManager.AppSettings["TodoApiBaseUrl"];
}