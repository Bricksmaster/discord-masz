using System.ComponentModel.DataAnnotations;
using masz.Enums;

namespace masz.Dtos.GuildConfig
{
    public class GuildConfigForPutDto
    {
        [Required(ErrorMessage = "ModRoles field is required")]
        public ulong[] ModRoles { get; set; }
        [Required(ErrorMessage = "AdminRoles field is required")]
        public ulong[] AdminRoles { get; set; }
        [Required(ErrorMessage = "MutedRoles field is required")]
        public ulong[] MutedRoles { get; set; }
        public bool ModNotificationDM { get; set; }
        [Url(ErrorMessage = "Webhook needs to be a valid url")]
        [RegularExpression(@"^https://discord(app)?\.com/.*$", ErrorMessage = "please specify a url that starts with 'https://discordapp.com/'.")]
        public string ModPublicNotificationWebhook { get; set; }
        [Url(ErrorMessage = "Webhook needs to be a valid url")]
        [RegularExpression(@"^https://discord(app)?\.com/.*$", ErrorMessage = "please specify a url that starts with 'https://discordapp.com/'.")]
        public string ModInternalNotificationWebhook { get; set; }
        [Required(ErrorMessage = "ExecuteWhoisOnJoin field is required")]
        public bool ExecuteWhoisOnJoin { get; set; }
        [Required(ErrorMessage = "StrictModPermissionCheck field is required")]
        public bool StrictModPermissionCheck { get; set; }
        [Required(ErrorMessage = "PublishModeratorInfo field is required")]
        public bool PublishModeratorInfo { get; set; }
        public Language PreferredLanguage { get; set; } = Language.en;
    }
}