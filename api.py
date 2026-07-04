import requests

# Loaded from .env file in main.py
USERNAME =""
TOKEN = ""

def get_team_info(team_number: str) -> dict:
    response = requests.get(
        url="https://ftc-api.firstinspires.org/v2.0/2025/teams/",
        params={"teamNumber": team_number},
        auth=(USERNAME, TOKEN)
    )
    print(USERNAME, TOKEN)

    if response.status_code == 200:
        return response.json()["teams"][0]
    else:
        print(f"API error in get_team_info: {response.status_code} - {response.text}")
        return None