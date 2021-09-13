using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using masz.Models;
using Microsoft.Extensions.Logging;

namespace masz.Repositories
{

    public class AutoModerationEventRepository : BaseRepository<AutoModerationEventRepository>
    {
        private AutoModerationEventRepository(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public static AutoModerationEventRepository CreateDefault(IServiceProvider serviceProvider) => new AutoModerationEventRepository(serviceProvider);

        public async Task<int> CountEvents()
        {
            return await _database.CountAllModerationEvents();
        }
        public async Task<int> CountEventsByGuild(ulong guildId)
        {
            return await _database.CountAllModerationEventsForGuild(guildId);
        }
        public async Task<AutoModerationEvent> RegisterEvent(AutoModerationEvent modEvent)
        {
            AutoModerationConfig modConfig = await AutoModerationConfigRepository.CreateDefault(_serviceProvider).GetConfigsByGuildAndType(modEvent.GuildId, modEvent.AutoModerationType);

            DiscordUser user = await _discordAPI.FetchUserInfo(modEvent.UserId, CacheBehavior.Default);
            if (user != null)
            {
                modEvent.Username = user.Username;
                modEvent.Discriminator = user.Discriminator;
            }

            modEvent.CreatedAt = DateTime.UtcNow;

            if (modConfig.AutoModerationAction == AutoModerationAction.CaseCreated || modConfig.AutoModerationAction == AutoModerationAction.CaseCreated)
            {
                ModCase modCase = new ModCase();
                modCase.Title = $"AutoModeration: {modEvent.AutoModerationType.ToString()}";
                modCase.Description = $"User triggered AutoModeration\nEvent: {modEvent.AutoModerationType.ToString()}\nAction: {modConfig.AutoModerationAction.ToString()}\nMessageId: {modEvent.MessageId}\nMessage content: {modEvent.MessageContent}";
                modCase.Labels = new List<string>() { "automoderation", modEvent.AutoModerationType.ToString() }.ToArray();
                modCase.CreationType = CaseCreationType.AutoModeration;
                modCase.PunishmentType = PunishmentType.None;
                modCase.PunishedUntil = null;
                if (modConfig.PunishmentType != null)
                {
                    modCase.PunishmentType = modConfig.PunishmentType.Value;
                }
                if (modConfig.PunishmentDurationMinutes != null)
                {
                    modCase.PunishedUntil = DateTime.UtcNow.AddMinutes(modConfig.PunishmentDurationMinutes.Value);
                }
                modCase.UserId = modEvent.UserId;
                modCase.GuildId = modEvent.GuildId;

                try
                {
                    modCase = await ModCaseRepository.CreateWithBotIdentity(_serviceProvider).CreateModCase(modCase, true, modConfig.SendPublicNotification, modConfig.SendDmNotification);

                    modEvent.AssociatedCaseId = modCase.Id;
                } catch (Exception e)
                {
                    _logger.LogError(e, $"Failed to create modcase for modevent {modEvent.GuildId}/{modEvent.UserId}/{modEvent.AutoModerationType}");
                }
            }

            await _database.SaveModerationEvent(modEvent);
            await _database.SaveChangesAsync();

            return modEvent;
        }
        public async Task DeleteEventsForGuild(ulong guildId)
        {
            await _database.DeleteAllModerationEventsForGuild(guildId);
            await _database.SaveChangesAsync();
        }
    }
}