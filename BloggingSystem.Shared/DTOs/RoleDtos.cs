namespace BloggingSystem.Shared.DTOs;

public class RoleDto
{
    /// <summary>
    /// Role ID
    /// </summary>
    public long Id { get; set; }
        
    /// <summary>
    /// Role name
    /// </summary>
    public string Name { get; set; }
        
    /// <summary>
    /// Role slug
    /// </summary>
    public string Slug { get; set; }
        
    /// <summary>
    /// Role description
    /// </summary>
    public string Description { get; set; }
}
    
public class CreateRoleDto
{
    public string Name { get; set; }
    public string Slug { get; set; }
    public string Description { get; set; }
}
    
public class UpdateRoleDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string Description { get; set; }
}
    
public class RoleSummaryDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string Description { get; set; }
}