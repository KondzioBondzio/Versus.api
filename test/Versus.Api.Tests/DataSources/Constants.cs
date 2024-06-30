namespace Versus.Api.Tests.DataSources;

public static class Constants
{
    public const string UserPassword = "Qwerty1!";
    public const string UserLoginTemplate = "user{0}@versus.local";

    public static string GetTemplatedLogin(int arg) => string.Format(UserLoginTemplate, arg);
}