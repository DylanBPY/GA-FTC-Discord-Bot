import os
import requests

import util

# Loaded from .env
USERNAME = ""
TOKEN = ""

ROOT = "https://ftc-api.firstinspires.org/v2.0/"
    
def init():
    global USERNAME, TOKEN
    USERNAME = os.getenv('FTC_API_USERNAME')
    TOKEN = os.getenv('FTC_API_TOKEN')

def get_teams() -> dict:
    response = requests.get(
        url=ROOT + util.SEASON + "/teams",
        params={"state": "GA"},
        auth=(USERNAME, TOKEN)
    )

    if response.status_code == 200:
        # Remove teams that are not from GA, USA
        teams: dict = response.json()
        for i, team in enumerate(teams["teams"]):
            if team["country"] != "USA":
                del teams["teams"][i]
        return teams
    else:
        print(f"API error in get_team_info: {response.status_code} - {response.text}")
        return None
    
def get_league_teams(league_id: str) -> list[int]:
    response = requests.get(
        url=ROOT + util.SEASON + "/leagues/members/USGA/" + league_id,
        auth=(USERNAME, TOKEN)
    )

    if response.status_code == 200:
        return response.json()["members"]
    else:
        print(f"API error in get_league_teams: {response.status_code} - {response.text}")
        return None