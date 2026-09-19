using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace WebApplication1.Helpers
{
	public sealed class BearerSecuritySchemeTransformer
	: IOpenApiDocumentTransformer
	{
		public Task TransformAsync(
			OpenApiDocument document,
			OpenApiDocumentTransformerContext context,
			CancellationToken cancellationToken)
		{
			document.Components ??= new OpenApiComponents();

			document.Components.SecuritySchemes ??=
				new Dictionary<string, OpenApiSecurityScheme>();

			document.Components.SecuritySchemes["Bearer"] =
				new OpenApiSecurityScheme
				{
					Type = SecuritySchemeType.Http,
					Scheme = "bearer",
					In = ParameterLocation.Header,
					BearerFormat = "JWT"
				};

			foreach (var operation in document.Paths.Values
				 .SelectMany(path => path.Operations.Values))
			{
				operation.Security ??= [];

				operation.Security.Add(
					new OpenApiSecurityRequirement
					{
						[new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Id = "Bearer",
								Type = ReferenceType.SecurityScheme
							}
						}] = []
					});
			}

			return Task.CompletedTask;
		}
	}
}
