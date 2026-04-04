using JobApplicantLibrary;
using JobApplicantLibrary.Models;
using JobApplicantLibrary.Services;
using Moq;
using FluentAssertions;

namespace JobApplicationLibrary.UnitTest
{
    public class ApplicationEvaluateUnitTest
    {
        [Test]
        public void Application_WithUnderAge_TransferredToAutoRejected()
        {
            //Arrange
            var evaluator = new ApplicationEvaluator(null);
            var applicant = new Applicant()
            {
                Age = 17
            };
            var form = new JobApplication
            {
                Applicant = applicant
            };
            //Action
            var result = evaluator.Evaluate(form);

            //Assert
            //Assert.AreEqual(ApplicationResult.AutoRejected, result);
            result.Should().Be(ApplicationResult.AutoRejected);
        }

        [Test]
        public void Application_WithNoTechStack_TransferredToAutoRejected()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(true);

            //mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Throws<Exception>();  // Exception da dönülebiliyor.
            //mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Throws(new Exception());

            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(true);


            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("ISTANBUL");

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication
            {
                Applicant = new Applicant() { Age = 25, IdentityNumber = "123" },
                TechStackList = new List<string>() { "Java", "Azure", "SQL" },
                YearsOfExperience = 5
            };
            //Action
            var result = evaluator.Evaluate(form);

            //Assert
            //Assert.AreEqual(ApplicationResult.AutoRejected, result);
            result.Should().Be(ApplicationResult.AutoRejected);
        }

        [Test]
        public void Application_WithTechStackOver75P_TransferredToAutoAccepted()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(true);
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("ISTANBUL");

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication
            {
                Applicant = new Applicant() { Age = 25, IdentityNumber = "123" },
                TechStackList = new List<string>() { "C#", "RabbitMQ", "Microservices", "Visual Studio" },
                YearsOfExperience = 15
            };
            //Action
            var result = evaluator.Evaluate(form);

            //Assert
            //Assert.AreEqual(ApplicationResult.AutoAccepted, result);
            result.Should().Be(ApplicationResult.AutoAccepted);
        }

        [Test]
        public void Application_WithInvalidIdentityNumber_TransferredToHR()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>(MockBehavior.Loose);
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(false);
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("ISTANBUL");

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication
            {
                Applicant = new Applicant() { Age = 25 }
            };
            //Action
            var result = evaluator.Evaluate(form);

            //Assert
            //Assert.AreEqual(ApplicationResult.TransferredToHR, result);
            result.Should().Be(ApplicationResult.TransferredToHR);
        }

        [Test]
        public void Application_WithOfficeLocation_TransferredToCTO()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator
                .Setup(i => i.CountryDataProvider.CountryData.Country).Returns("Spain");
            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication
            {
                Applicant = new Applicant() { Age = 25 }
            };
            //Action
            var result = evaluator.Evaluate(form);

            //Assert
            //Assert.AreEqual(ApplicationResult.TransferredToCTO, result);
            result.Should().Be(ApplicationResult.TransferredToCTO);
        }

        [Test]
        public void Application_WithOver50_ValidationModeToDetailed()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.SetupAllProperties();
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("Spain");
            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication
            {
                Applicant = new Applicant() { Age = 51 }
            };
            //Action
            var result = evaluator.Evaluate(form);

            //Assert
            //Assert.AreEqual(ValidationMode.Detailed, mockValidator.Object.ValidationMode);
            mockValidator.Object.ValidationMode.Should().Be(ValidationMode.Detailed);
        }

        [Test]
        public void Application_WithNullApplicant_ThrowArgumentNullException()
        {
            //Arrange
            var evaluator = new ApplicationEvaluator(null);
            var form = new JobApplication();
            //Action
            Action resultAction = () => evaluator.Evaluate(form);
            //Assert
            resultAction.Should().Throw<ArgumentNullException>();
        }
        
        [Test]
        public void Application_WithDefaultValue_IsValidCalled()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("ISTANBUL");

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication
            {
                Applicant = new Applicant() { Age = 25, IdentityNumber="1234" }
            };
            //Action
            var result = evaluator.Evaluate(form);

            //Assert
            //mockValidator.Verify(i => i.IsValid("123"),"IsValidMethod should be called with 123");
            mockValidator.Verify(i => i.IsValid(It.IsAny<string>()));
            //mockValidator.Verify(i => i.IsValid(It.IsAny<string>()),Times.Exactly(1)); // 1 Kere çağrılması gerekiyor
            //mockValidator.Verify(i => i.IsValid(It.IsAny<string>()), Times.Once()); // 1 Kere çağrılması gerekiyor

        }

        [Test]
        public void Application_WithYoungAge_IsValidNeverCalled()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("ISTANBUL");

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication
            {
                Applicant = new Applicant() { Age = 17 }
            };
            //Action
            var result = evaluator.Evaluate(form);

            //Assert
            mockValidator.Verify(i => i.IsValid(It.IsAny<string>()),Times.Never());

        }
    }
}