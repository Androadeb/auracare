using Alexapps.SkinCare.Dtos.DiagnosticSessionTest.Queries;
using FluentValidation;

namespace Alexapps.SkinCare.Validators
{
    public class GetNearbyLabsForTestQueryValidator : AbstractValidator<GetNearbyLabsForTestQuery>
    {
        public GetNearbyLabsForTestQueryValidator()
        {
          
            RuleFor(x => x.DiagnosticSessionTestId)
                .NotEmpty()
                .WithMessage("DiagnosticSessionTestId is required.");

         
            RuleFor(x => x.CustomerLat)
                .InclusiveBetween(-90, 90)
                .WithMessage("Invalid Latitude.");

       
            RuleFor(x => x.CustomerLong)
                .InclusiveBetween(-180, 180)
                .WithMessage("Invalid Longitude.");

         
        }
    }
}