using JobApplicantLibrary.Models;
using JobApplicantLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicantLibrary
{
    public class ApplicationEvaluator
    {
        private const int minimumAge = 18;
        private const int autoAcceptedYearOfExperience = 15;
        private List<string> techStackList = new() { "C#", "RabbitMQ", "Microservices", "Visual Studio" };
        private IIdentityValidator identityValidator;
        public ApplicationEvaluator(IIdentityValidator identityValidator)
        {
            this.identityValidator = identityValidator;
        }

        public ApplicationResult Evaluate(JobApplication form)
        {
            if (form.Applicant is null)
                throw new ArgumentNullException();

            if (form.Applicant.Age < minimumAge)
                return ApplicationResult.AutoRejected;
            identityValidator.ValidationMode = form.Applicant.Age > 50 ? ValidationMode.Detailed : ValidationMode.Quick;
            if (identityValidator.CountryDataProvider.CountryData.Country != "ISTANBUL")
                return ApplicationResult.TransferredToCTO;

            var validIdentity = identityValidator.IsValid(form.Applicant.IdentityNumber);
            if (!validIdentity)
                return ApplicationResult.TransferredToHR;

            var sr = GetTechStackSimilarityRate(form.TechStackList);

            if (sr < 25)
                return ApplicationResult.AutoRejected;
            if (sr > 75 && form.YearsOfExperience >= autoAcceptedYearOfExperience)
                return ApplicationResult.AutoAccepted;

            return ApplicationResult.AutoAccepted;
        }

        private int GetTechStackSimilarityRate(List<string> techStacks)
        {
            var matchedCount = techStacks
                                    .Where(i => techStackList.Contains(i, StringComparer.OrdinalIgnoreCase))
                                    .Count();
            return (int)((double)matchedCount / techStackList.Count * 100);
        }
    }

    public enum ApplicationResult
    {
        AutoRejected,
        AutoAccepted,
        TransferredToHR,
        TransferredToCTO,
        TransferredToLead
    }
}
