using UnityEngine;

namespace Accel{
public class GameTime : MonoBehaviour{

    public float startingHour = 4;
    public int secondsPerDay = 500;
    public float dayStart = 6f, dayEnd = 18f, hoursPerDay = 24;
    // informative
    public string currentHour, status;

    public void DayBreak () => hour = dayStart;

    public void NightFall () => hour = dayEnd;

    public float hour{
        get => ((Time.time + timeOffset) % secondsPerDay)
                                        / secondsPerDay * hoursPerDay;
        set => startingHour -= (hour - value);
    }

    public bool isDayTime => hour > dayStart && hour < dayEnd;

    void Update(){
        currentHour = $"{hour:0.00}";
        status = isDayTime ? "Day" : "Night";
    }

    float timeOffset => HourToSeconds(startingHour);

    float HourToSeconds(float h) => h * secondsPerHour;

    float secondsPerHour => secondsPerDay / (float)hoursPerDay;

}}
