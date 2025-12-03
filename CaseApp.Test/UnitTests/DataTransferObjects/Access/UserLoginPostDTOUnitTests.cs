using CaseApp.DataTransferObjects.Access;
using System.ComponentModel.DataAnnotations;

namespace CaseApp.Test.UnitTests.DataTransferObjects.Access;

public class UserLoginPostDTOUnitTests
{
    [Fact]
    public void UserName_ShouldBeRequiredAndValidEmail()
    {
        var userLoginPostDTO = new UserLoginPostDTO
        {
            UserName = "",
            PasswordHash = "somePassword"
        };

        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(userLoginPostDTO);

        var isValid = Validator.TryValidateObject(userLoginPostDTO, context, validationResults, true);

        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.ErrorMessage.Contains("precisa ser informado"));
    }

    [Fact]
    public void PasswordHash_ShouldBeRequired()
    {
        var userLoginPostDTO = new UserLoginPostDTO
        {
            UserName = "user@test.com",
            PasswordHash = ""
        };

        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(userLoginPostDTO);

        var isValid = Validator.TryValidateObject(userLoginPostDTO, context, validationResults, true);

        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.ErrorMessage.Contains("precisa ser informado"));
    }

    [Fact]
    public void ValidDTO_ShouldPassValidation()
    {
        var userLoginPostDTO = new UserLoginPostDTO
        {
            UserName = "user@test.com",
            PasswordHash = "securePassword"
        };

        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(userLoginPostDTO);

        var isValid = Validator.TryValidateObject(userLoginPostDTO, context, validationResults, true);

        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

}