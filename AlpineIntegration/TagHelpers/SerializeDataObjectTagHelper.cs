using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AlpineIntegration.TagHelpers
{
    [HtmlTargetElement(Attributes = SerializeAttributeName)]
    public class SerializeDataObjectTagHelper : TagHelper
    {
        private const string SerializeAttributeName = "x-data.serialize";
        private const string DataAttributeName = "x-data";

        /// <summary>
        /// Initialize a new component with the following data object serialized to JSON using camel case naming policy and JsonStringEnumConverter.
        /// If "x-data" is also specified, the Tag Helper tries to combine both data objects using spread syntax.
        /// </summary>
        [HtmlAttributeName(SerializeAttributeName)]
        public object DataObject { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            options.Converters.Add(new JsonStringEnumConverter());

            var json = JsonSerializer.Serialize(DataObject, options);

            var currentValue = context.AllAttributes.FirstOrDefault(a => a.Name == DataAttributeName)
                ?.Value.ToString()
                .Trim(';', ' ');

            var newValue = string.IsNullOrWhiteSpace(currentValue)
                ? json
                : $"{{ ...{currentValue}, ...{json} }}";

            output.Attributes.SetAttribute(DataAttributeName, newValue);
        }
    }
}