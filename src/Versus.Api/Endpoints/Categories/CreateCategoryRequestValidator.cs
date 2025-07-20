using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;
using Versus.Shared.Categories;

namespace Versus.Api.Endpoints.Categories;

public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    private readonly VersusDbContext _dbContext;

    public CreateCategoryRequestValidator(VersusDbContext dbContext)
    {
        _dbContext = dbContext;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(Resources.Common.NotEmpty)
            .MaximumLength(50).WithMessage(string.Format(Resources.Common.MaxLength, 50))
            .MustAsync(UniqueName).WithMessage(Resources.Categories.NameTaken);

        RuleFor(x => x.Description)
            .MaximumLength(250).WithMessage(string.Format(Resources.Common.MaxLength, 250));
    }

    private async Task<bool> UniqueName(string name, CancellationToken cancellationToken)
    {
        return !await _dbContext.Categories
            .Where(x => x.Name.ToLower() == name.ToLower())
            .AnyAsync(cancellationToken);
    }
}