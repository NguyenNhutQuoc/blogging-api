namespace BloggingSystem.Shared.DTOs;

/// <summary>
    /// Data transfer object for Permission
    /// </summary>
    public class PermissionDto
    {
        /// <summary>
        /// Permission ID
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// Permission name
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Permission slug
        /// </summary>
        public string Slug { get; set; }
        
        /// <summary>
        /// Permission description
        /// </summary>
        public string Description { get; set; }
    }