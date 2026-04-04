using JobApplicantLibrary.Models;

namespace JobApplicantLibrary.Services
{
    public interface IIdentityValidator
    {
        bool IsValid(string identityNumber);

        bool CheckConnectionToRemoteServer();
        ICountryDataProvider CountryDataProvider { get; }

        public ValidationMode ValidationMode { get; set; }
    }
    public interface ICountryData
    {
        string Country { get; }
    }
    public interface ICountryDataProvider
    {
        ICountryData CountryData { get; }
    }
    public enum ValidationMode
    {
        Quick,
        Detailed,
    }
}
