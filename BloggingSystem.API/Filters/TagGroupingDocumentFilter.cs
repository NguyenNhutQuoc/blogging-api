using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BloggingSystem.API.Filters // Thay thế YourNamespace bằng namespace của dự án
{
    public class TagGroupingDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            // Danh sách các controller thuộc nhóm User Management
            var userManagementControllers = new[] { "Auth", "Permission", "Role", "Users" };
            
            foreach (var path in swaggerDoc.Paths)
            {
                foreach (var operation in path.Value.Operations)
                {
                    // Kiểm tra nếu operation thuộc về một trong các controller cần nhóm
                    if (operation.Value.Tags.Any(tag => userManagementControllers.Contains(tag.Name)))
                    {
                        // Xóa tag cũ
                        operation.Value.Tags.Clear();
                        // Thêm tag mới
                        operation.Value.Tags.Add(new OpenApiTag { Name = "User Management" });
                    }
                }
            }
        }
    }
}