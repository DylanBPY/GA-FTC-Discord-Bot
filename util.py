import discord
from discord.ext import commands

DEFAULT_ROLE_COLOR: discord.Color = discord.Color.blue()
CACHE_FILE = "cache.json"
LEAGUE_ID_KEY = {
    "AL": "Albany-Commodore",
    "ATL": "Atlanta-Marist",
    "COL": "Columbus-Muscogee",
    "DUG": "Douglasville",
    "EP": "Etowah-Paulding",
    "LEJ": "Lakeview-East Jackson",
    "MAC": "Macon-FPDS",
    "MW": "Marietta-Wheeler"
}

async def is_valid_team_number(ctx: commands.Context, team_number: str) -> bool:
    if not team_number.isdigit() or len(team_number) > 5:
        await ctx.send("Invalid team number.")
        return False
    return True

async def is_valid_league_id(ctx: commands.Context, league_id: str) -> bool:
    if league_id.upper() not in LEAGUE_ID_KEY:
        embed = Embed(
            title="Invalid league ID",
            description="See valid league IDs below",
            color=discord.Color.red()
        )
        embed.add_field(
            name="Valid league IDs",
            value="\n".join([f"**{key}** - {value}" for key, value in LEAGUE_ID_KEY.items()]),
            inline=False
        )

        await embed.send(ctx)
        return False
    return True

async def get_role(ctx: commands.Context, role_name: str, verbose: bool = True) -> discord.Role:
    role: discord.Role = discord.utils.get(ctx.guild.roles, name=role_name)
    if not role and verbose:
        await ctx.send(f"The {role_name} role doesn't exist!")
        return None
    return role

def check_user_has_role(ctx: commands.Context, role: discord.Role) -> bool:
    return role in ctx.author.roles

async def add_role_to_user(ctx: commands.Context, role: discord.Role, verbose: bool = True):
    try:
        await ctx.author.add_roles(role)
        if verbose:
            await ctx.send(f"You have been added to {role.name}!")
    except discord.Forbidden:
        await ctx.send("I don't have permission to add roles.")

async def remove_role_from_user(ctx: commands.Context, role: discord.Role, verbose: bool = True):
    try:
        await ctx.author.remove_roles(role)
        if verbose:
            await ctx.send(f"You have been removed from {role.name}!")
    except discord.Forbidden:
        await ctx.send("I don't have permission to remove roles.")

class Embed:
    embed: discord.Embed

    def __init__(self, title: str, description: str = "", color: discord.Color = DEFAULT_ROLE_COLOR):
        self.embed = discord.Embed(title=title, description=description, color=color)

    def add_field(self, name: str, value: str, inline: bool = False):
        self.embed.add_field(name=name, value=value, inline=inline)
    
    async def send(self, ctx: commands.Context):
        self.embed.set_footer(text="Requested by " + str(ctx.author.name))
        self.embed.timestamp = ctx.message.created_at
        await ctx.send(embed=self.embed)


class Team:
    number: str
    name: str
    sponsors: str
    school: str
    city: str
    state: str
    country: str
    website: str
    rookie_year: int
    location: str
    league: str

    def __init__(self, data: dict):
        self.number = str(data.get("teamNumber"))
        self.name = data.get("nameShort")
        self.sponsors = data.get("nameFull")
        self.school = data.get("schoolName")
        self.city = data.get("city")
        self.state = data.get("stateProv")
        self.country = data.get("country")
        self.website = data.get("website")
        self.rookie_year = data.get("rookieYear")
        self.location = data.get("displayLocation")
        self.league = data.get("league")

    def get_role(self, ctx: commands.Context) -> discord.Role:
        return discord.utils.get(ctx.guild.roles, name=self.number)

class Cache:
    teams: list[Team]
    timestamp: float

    def __init__(self, data: dict):
        self.teams = [Team(team) for team in data.get("teams", [])]
        self.timestamp = data.get("timestamp")