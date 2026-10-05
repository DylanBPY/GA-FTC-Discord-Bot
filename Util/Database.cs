using LiteDB;
using System.Text.Json.Serialization;

namespace Util;

public record TeamListResponse(
    [property: JsonPropertyName("teams")] Team[] Teams,
    [property: JsonPropertyName("teamCountTotal")] int TeamCountTotal,
    [property: JsonPropertyName("teamCountPage")] int TeamCountPage,
    [property: JsonPropertyName("pageCurrent")] int PageCurrent,
    [property: JsonPropertyName("pageTotal")] int PageTotal
);

public record Team(
    [property: JsonPropertyName("teamNumber")] int TeamNumber,
    [property: JsonPropertyName("displayTeamNumber")] string DisplayTeamNumber,
    [property: JsonPropertyName("teamId")] int TeamId,
    [property: JsonPropertyName("teamProfileId")] int TeamProfileId,
    [property: JsonPropertyName("nameFull")] string NameFull,
    [property: JsonPropertyName("nameShort")] string NameShort,
    [property: JsonPropertyName("schoolName")] string SchoolName,
    [property: JsonPropertyName("city")] string City,
    [property: JsonPropertyName("stateProv")] string StateProv,
    [property: JsonPropertyName("country")] string Country,
    [property: JsonPropertyName("website")] string Website,
    [property: JsonPropertyName("rookieYear")] int RookieYear,
    [property: JsonPropertyName("robotName")] string RobotName,
    [property: JsonPropertyName("districtCode")] string DistrictCode,
    [property: JsonPropertyName("homeCMP")] string HomeCmp,
    [property: JsonPropertyName("homeRegion")] string HomeRegion,
    [property: JsonPropertyName("displayLocation")] string DisplayLocation
);

public class Database : IDisposable
{
    private readonly LiteDatabase _db;
    private readonly ILiteCollection<Team> _teams;

    public Database()
    {
        _db = new LiteDatabase("cache.db");
        _teams = _db.GetCollection<Team>("teams");
    }

    public Team GetTeam(int number)
    {
        return _teams.FindOne(x => x.TeamNumber == number);
    }

    public void Dispose() => _db.Dispose();
}