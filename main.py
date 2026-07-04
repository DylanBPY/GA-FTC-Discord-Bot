import discord
from discord.ext import commands
from dotenv import load_dotenv
import os

import api

intents = discord.Intents.default()
intents.message_content = True 

bot = commands.Bot(command_prefix="!", intents=intents)

@bot.event
async def on_ready():
    print(f"Logged in as {bot.user} (ID: {bot.user.id})")

@bot.hybrid_command(name="ping", description="Check the bot's latency")
async def ping(ctx: commands.Context):
    await ctx.send(f"Pong! (in {round(bot.latency * 1000)}ms)")

@bot.hybrid_command(name="join", description="Add yourself to a team")
async def join(ctx: commands.Context, team_number: str):
    role = discord.utils.get(ctx.guild.roles, name=team_number)

    if not role:
        await ctx.send(f"Team role doesn't exist")
        return

    if role in ctx.author.roles:
        await ctx.send(f"You are already on team {team_number}!")
        return
        
    try:
        await ctx.author.add_roles(role)
        await ctx.send(f"You have been added to team {team_number}!")
    except discord.Forbidden:
        await ctx.send("I don't have perms :(")

@bot.hybrid_command(name="leave", description="Remove yourself from a team")
async def leave(ctx: commands.Context, team_number: str):
    role = discord.utils.get(ctx.guild.roles, name=str(team_number))

    if not role:
        await ctx.send(f"Team role doesn't exist")
        return

    if role not in ctx.author.roles:
        await ctx.send(f"You are not on team {team_number}!")
        return

    try:
        await ctx.author.remove_roles(role)
        await ctx.send(f"You have been removed from team {team_number}!")
    except discord.Forbidden:
        await ctx.send("I don't have perms :(")

@bot.hybrid_command(name="team", description="Show information about a team")
async def info(ctx: commands.Context, team_number: str):
    team_info = api.get_team_info(team_number)

    if not team_info:
        await ctx.send(f"Could not retrieve information for team {team_number}.")
        return

    embed = discord.Embed(
        description=team_info.get("nameShort", "Team name not available."),
        title=f"Team {team_number}",
        color=discord.Color.random()
    )
    
    embed.add_field(name="Location", value=team_info.get("displayLocation", "Location not available."), inline=False)
    embed.add_field(name="Website", value=team_info.get("website", "Website not available."), inline=False)
    embed.add_field(name="Rookie Year", value=team_info.get("rookieYear", "Rookie year not available."), inline=False)
    embed.add_field(name="Sponsors", value=team_info.get("nameFull", "Sponsors not available."), inline=False)
    
    embed.set_footer(text="Requested by " + str(ctx.author.name))
    embed.timestamp = ctx.message.created_at

    await ctx.send(embed=embed)

@bot.hybrid_command(name="sync", description="Sync slash commands", hidden=True)
@commands.is_owner()
async def sync(ctx: commands.Context):
    try:
        synced = await bot.tree.sync()
        await ctx.send(f"Successfully synced {len(synced)} command(s) globally.")
    except Exception as e:
        await ctx.send(f"Failed to sync commands: {e}")

if __name__ == "__main__":
    load_dotenv(".env") # Load environment variables from .env file
    api.USERNAME = os.getenv('FTC_API_USERNAME')
    api.TOKEN = os.getenv('FTC_API_TOKEN')
    bot.run(os.getenv("BOT_TOKEN"))