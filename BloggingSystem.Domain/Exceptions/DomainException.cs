namespace BloggingSystem.Domain.Exceptions
{
    /// <summary>
    /// Base exception for all domain exceptions
    /// </summary>
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message)
        {
        }
        
        public DomainException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
    
    /// <summary>
    /// Exception thrown when an entity is not found
    /// </summary>
    public class EntityNotFoundException : DomainException
    {
        public string EntityName { get; }
        public object EntityId { get; }
        
        public EntityNotFoundException(string entityName, object entityId) 
            : base($"Entity {entityName} with ID {entityId} was not found")
        {
            EntityName = entityName;
            EntityId = entityId;
        }
    }
    
    /// <summary>
    /// Exception thrown when a business rule is violated
    /// </summary>
    public class BusinessRuleViolationException : DomainException
    {
        public string RuleName { get; }
        
        public BusinessRuleViolationException(string ruleName, string message) 
            : base(message)
        {
            RuleName = ruleName;
        }
    }
}