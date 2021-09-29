using AdvancedFilters.Domain.Filters.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;
using Tools;

namespace AdvancedFilters.Web.Binding
{
    public class AdvancedFilterModelBinder<TCriterion> : IModelBinder
        where TCriterion : AdvancedCriterion
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }
            var modelName = bindingContext.BinderModelName ?? "AdvancedFilter";

            var req = bindingContext.HttpContext.Request;
            if (req.Body == null)
            {
                bindingContext.ModelState.AddModelError(modelName, "invalid request body stream");
                return;
            }

            try
            {
                var serializer = BuildPolymorphicSerializer();
                var parsed = await serializer.DeserializeAsync<IAdvancedFilter>(req.Body);

                bindingContext.Result = ModelBindingResult.Success(parsed);
            }
            catch (Exception e)
            {
                bindingContext.ModelState.AddModelError(modelName, e.ToString());
                bindingContext.Result = ModelBindingResult.Failed();
            }
        }

        private IPolymorphicSerializer BuildPolymorphicSerializer()
        {
            return Serializer
                .WithPolymorphism<IAdvancedFilter, FilterElementTypes>(nameof(IAdvancedFilter.FilterElementType))
                    .AddMatch<TCriterion>(FilterElementTypes.Criterion)
                    .AddMatch<FilterCombination>(FilterElementTypes.LogicalOperator)
                .Build();
        }
    }
}
