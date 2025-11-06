using ProductManager.Domain.Dtos;
using ProductManager.Domain.Entities;

namespace ProductManager.Domain.Extensions;

public static class CategoryExtension
{
    public static CategoryDto ToDto(this Category category)
    {
        return new CategoryDto(
            category.Identifier,
            category.Id,
            category.Name);
    }
}