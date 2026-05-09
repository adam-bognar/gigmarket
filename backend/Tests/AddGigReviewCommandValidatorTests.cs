using FluentValidation.TestHelper;
using GigMarket.Application.Features.Reviews.Commands.AddGigReview;

namespace Tests;

public class AddGigReviewCommandValidatorTests
{
    private readonly AddGigReviewCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_GigId_Is_Empty()
    {
        var command = new AddGigReviewCommand(CreateValidRequest() with { GigId = Guid.Empty });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Request.GigId);
    }

    [Fact]
    public void Should_Have_Error_When_Rating_Is_Below_1()
    {
        var command = new AddGigReviewCommand(CreateValidRequest() with { Rating = 0 });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Request.Rating);
    }

    [Fact]
    public void Should_Have_Error_When_Rating_Is_Above_5()
    {
        var command = new AddGigReviewCommand(CreateValidRequest() with { Rating = 6 });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Request.Rating);
    }

    [Fact]
    public void Should_Have_Error_When_Description_Is_Empty()
    {
        var command = new AddGigReviewCommand(CreateValidRequest() with { Description = "" });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Request.Description);
    }

    [Fact]
    public void Should_Have_Error_When_Description_Is_Too_Short()
    {
        var command = new AddGigReviewCommand(CreateValidRequest() with { Description = new string('a', 9) });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Request.Description);
    }

    [Fact]
    public void Should_Have_Error_When_Description_Is_Too_Long()
    {
        var command = new AddGigReviewCommand(CreateValidRequest() with { Description = new string('a', 2001) });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Request.Description);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void Should_Not_Have_Error_When_Rating_Is_Between_1_And_5(int rating)
    {
        var command = new AddGigReviewCommand(CreateValidRequest() with { Rating = rating });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Request.Rating);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        var result = _validator.TestValidate(new AddGigReviewCommand(CreateValidRequest()));

        result.ShouldNotHaveAnyValidationErrors();
    }


    private static AddGigReviewRequest CreateValidRequest() =>
        new(
            Guid.NewGuid(),
            5,
            "This is a great gig, highly recommended!");
}