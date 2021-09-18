import os
from datetime import datetime

from discord import Embed

from data import get_guildconfig
from helpers import console
from .infrastructure import registered_guild_and_admin_or_mod_only, record_usage, CommandDefinition


CHECK = "✅"
X_CHECK = "❌"


async def _features(ctx):
    await registered_guild_and_admin_or_mod_only(ctx)
    record_usage(ctx)
    if ctx.guild is None:
        await ctx.send("Only useable in a guild.")
        return
    try:
        cfg = await get_guildconfig(str(ctx.guild.id))
    except Exception as e:
        console.critical(f"Failed to get guildconfig: {e}")
        await ctx.send("Failed to fetch data from database.")
        return

    if cfg is None:
        if os.getenv('META_SERVICE_BASE_URL', ''):
            await ctx.send(f"MASZ is not registered on this guild.\nA siteadmin can register this guild at: {os.getenv('META_SERVICE_BASE_URL', '')}/guilds/new?guildid={ctx.guild.id}")
        else:
            await ctx.send(f"MASZ is not registered on this guild.")
        return

    internal_webhook_defined = bool(cfg["ModInternalNotificationWebhook"])
    muted_role_defined = cfg["MutedRoles"]
    muted_roles = []
    for role in cfg["MutedRoles"].split(","):
        if muted_role_defined:
            muted_roles.append(ctx.guild.get_role(int(role)))
        else:
            muted_roles = []
            break
    muted_role_managable = True
    for role in muted_roles:
        if role is None:
            muted_role_managable = False
            break
        if ctx.me.top_role <= role:
            muted_role_managable = False
            break

    permissions = ctx.me.guild_permissions
    kick = permissions.kick_members
    ban = permissions.ban_members
    mute = permissions.manage_roles
    manage_guild = permissions.manage_guild

    embed = Embed(title="Features")
    embed.timestamp = datetime.now()
    missing_permissions = f"\n- {CHECK if kick else X_CHECK} Kick permission {'not' if not kick else ''} granted" \
                          f"\n- {CHECK if ban else X_CHECK} Ban permission {'not' if not ban else ''} granted" \
                          f"\n- {CHECK if mute else X_CHECK} Manage role permission {'not' if not mute else ''} granted"

    if not muted_role_defined:
        missing_permissions += f"\n- {X_CHECK} Muted role not defined"
    else:
        if muted_roles:
            if muted_role_managable:
                missing_permissions += f"\n- {CHECK} Muted role defined"
            else:
                missing_permissions += f"\n- {X_CHECK} Muted role defined but too high in role hierarchy"
        else:
            missing_permissions += f"\n- {X_CHECK} Muted role defined but invalid"

    embed.add_field(
        name = f"{CHECK if kick and ban and mute and muted_roles and muted_role_managable else X_CHECK} Punishment execution",
        value = f"Let MASZ handle punishments (e.g. tempbans, mutes, etc.).{missing_permissions if not (kick and ban and mute and muted_roles and muted_role_managable) else ''}",
        inline = False
    )
    embed.add_field(
        name = f"{CHECK if ban else X_CHECK} Unban requests",
        value = "Allows banned members to see their cases and comment on it for unban requests.\n" +
                f"{'Grant this bot the ban permission to use this feature.' if not ban else ''}",
        inline = False
    )
    embed.add_field(
        name = f"{CHECK if internal_webhook_defined else X_CHECK} Report command",
        value = "Allows members to use the report command.\n" +
                f"{'Define a internal staff webhook to use this feature.' if not internal_webhook_defined else ''}",
        inline = False
    )
    embed.add_field(
        name = f"{CHECK if manage_guild else X_CHECK} Invite tracking",
        value = "Allows MASZ to track the invites new members are using.\n" +
                f"{'Grant this bot the manage guild permission to use this feature.' if not manage_guild else ''}",
        inline = False
    )

    if kick and ban and mute and muted_roles and muted_role_managable and internal_webhook_defined and manage_guild:
        embed.description = f"{CHECK} Your bot on this guild is configured correctly. All features of MASZ can be used."
        embed.color = 0x07eb0b
    else:
        embed.description = f"{X_CHECK} There are features of MASZ that you cannot use right now."
        embed.color = 0xf71b02

    await ctx.send(embed=embed)


features = CommandDefinition(
    func=_features,
    short_help="Checks if further configuration is needed to use MASZ features.",
    long_help=f"Checks if further configuration is needed to use MASZ features.",
)
