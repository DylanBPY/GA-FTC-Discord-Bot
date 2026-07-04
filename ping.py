import discord
from discord.ext import commands

intents = discord.Intents.default()
intents.message_content = True 

bot = commands.Bot(command_prefix="!", intents=intents)

@bot.hybrid_command(name="ping", description="Check the bot's latency!")
async def ping(ctx: commands.Context):
    await ctx.send(f"Pong! My latency is {round(bot.latency * 1000)}ms")