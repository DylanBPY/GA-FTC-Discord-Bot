import discord
from discord.ext import commands
from dotenv import load_dotenv
import os

intents = discord.Intents.default()
intents.message_content = True 

bot = commands.Bot(command_prefix="!", intents=intents)

@bot.event
async def on_ready():
    print(f"Logged in as {bot.user} (ID: {bot.user.id})")

@bot.hybrid_command(name="ping", description="Check the bot's latency")
async def ping(ctx: commands.Context):
    await ctx.send(f"Pong! (in {round(bot.latency * 1000)}ms)")

@bot.command(name="sync", description="Sync slash commands", hidden=True)
@commands.is_owner()
async def sync(ctx: commands.Context):
    try:
        synced = await bot.tree.sync()
        await ctx.send(f"Successfully synced {len(synced)} command(s) globally.")
    except Exception as e:
        await ctx.send(f"Failed to sync commands: {e}")

if __name__ == "__main__":
    load_dotenv(".env") # Load environment variables from .env file
    bot.run(os.getenv("BOT_TOKEN"))