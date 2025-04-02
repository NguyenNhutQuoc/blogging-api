namespace BloggingSystem.Shared.Constants;

public class AuthConstants
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Editor = "Editor";
        public const string Author = "Author";
        public const string User = "User";
    }
    
    public static class Policies
    {
        public const string RequireAdmin = "RequireAdmin";
        public const string RequireEditor = "RequireEditor";
        public const string ManageUsers = "ManageUsers";
        public const string ManagePosts = "ManagePosts";
    }
    
    public static class Permissions
    {
        public const string CreatePost = "Permissions.Posts.Create";
        public const string EditPost = "Permissions.Posts.Edit";
        public const string DeletePost = "Permissions.Posts.Delete";
        public const string ViewUsers = "Permissions.Users.View";
        public const string EditUsers = "Permissions.Users.Edit";
        public const string DeleteUsers = "Permissions.Users.Delete";
        public const string ManageRoles = "Permissions.Roles.Manage";
        public const string ViewRoles = "Permissions.Roles.View";
        public const string EditRoles = "Permissions.Roles.Edit";
        public const string DeleteRoles = "Permissions.Roles.Delete";
        public const string CreateComments = "Permissions.Comments.Create";
        public const string EditComments = "Permissions.Comments.Edit";
        public const string DeleteComments = "Permissions.Comments.Delete";
        public const string ApproveComments = "Permissions.Comments.Approve";
        public const string EditOtherUsersComments = "Permissions.Comments.EditOthers";
        public const string DeleteOtherUsersComments = "Permissions.Comments.DeleteOthers";
        public const string ManageLikes = "Permissions.Likes.Manage";
        public const string ManageNotifications = "Permissions.Notifications.Manage";
        public const string ManageSettings = "Permissions.Settings.Manage";
        public const string ManageTags = "Permissions.Tags.Manage";
        public const string ManageCategories = "Permissions.Categories.Manage";
        public const string ManageSocialMediaAccounts = "Permissions.SocialMediaAccounts.Manage";
        
        public const string ViewPosts = "Permissions.Posts.View";
        public const string PublishPost = "Permissions.Posts.Publish";
        public const string UnpublishPost = "Permissions.Posts.Unpublish";
        public const string EditOtherUsersPosts = "Permissions.Posts.EditOthers"; // Sửa bài viết của người khác
        public const string DeleteOtherUsersPosts = "Permissions.Posts.DeleteOthers"; // Xóa bài viết của người khác
        
        public const string UploadMedia = "Permissions.Media.Upload";
        public const string DeleteMedia = "Permissions.Media.Delete";
        public const string ManageMedia = "Permissions.Media.Manage";
        
        public const string ViewAnalytics = "Permissions.Analytics.View";
        public const string ExportAnalytics = "Permissions.Analytics.Export";
        public const string ManageNewsletters = "Permissions.Newsletters.Manage";
        public const string CreateNewsletters = "Permissions.Newsletters.Create";
        public const string SendNewsletters = "Permissions.Newsletters.Send";
        public const string ViewSubscribers = "Permissions.Newsletters.ViewSubscribers";
        
        public const string ManageApiTokens = "Permissions.ApiTokens.Manage";
        public const string CreateApiTokens = "Permissions.ApiTokens.Create";
        public const string RevokeApiTokens = "Permissions.ApiTokens.Revoke";
        
        public const string ManageSeries = "Permissions.Series.Manage";
        public const string CreateSeries = "Permissions.Series.Create";
        public const string EditSeries = "Permissions.Series.Edit";
        public const string DeleteSeries = "Permissions.Series.Delete";
        
        public const string ManageSeo = "Permissions.Seo.Manage";
    }
}