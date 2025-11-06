using FastEndpoints;
using FluentValidation;

namespace ProductManager.Application.Endpoints.Product.Create.Validators;

public class ProductCreateValidator : Validator<ProductCreateRequest>
{
    public ProductCreateValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
        
        RuleFor(x => x.UnitCost)
            .GreaterThan(0);
        
        RuleFor(x => x.IdCategory)
            .GreaterThan(0);
    }
}