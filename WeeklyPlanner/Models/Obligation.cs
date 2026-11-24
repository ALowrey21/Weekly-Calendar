using System.Text.Json.Serialization;

namespace WeeklyCalendar.Models;

[JsonPolymorphic]
[JsonDerivedType(typeof(Course), "course")]
[JsonDerivedType(typeof(Assignment), "assignment")]
[JsonDerivedType(typeof(Other), "other")]
public abstract class Obligation
public abstract class Obligation {
    //varaible used in Oblifation
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public Days Days { get; set; } = Days.None;
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public int? RepeatEveryWeeks { get; set; }
    public int StartWeek { get; set; }
    public bool RemovedThisWeek { get; set; }

    //Does the Obligation have a date time?
    public bool IsScheduled() {
        if (Days != Days.None && StartTime != null && EndTime != null) {
            return true;
        } else {
            return false;
        }

    }

    //Does the Obligation repeat?
    public bool IsOneTime() {
        if(RepaetEveryWeeks == 0 || RepaetEveryWeeks == null) {
            return true;
        } else {
            false;
        }
    }

    //Does the Obligation happen on this day?
    public bool OccursOn(Days day) {
        if((Days & day) != Days.None) {
            return true;
        } else {
            return false;
        }
    }

    //Does the Obligation happen this week?
    public bool IsActiveInWeek(int week) {
        if(RemovedThisWeek) {
            return false;
        } else if (IsOneTime()) {
            return week == StartWeek;
        } else {
            return (week - StartWeek) % RepeatEveryWeeks.Value == 0;
        }
    }

    //Does this Obligation overlap with that Obligation?
    public bool OverlapsWith(Obligation other) {
        if(!IsScheduled() || !other.IsScheduled()) {
            return false;
        } else if (!OccursOn(other.Days)) {
            return false;
        } else {
            bool startsBeforeOtherEnds = StartTime.Value < other.EndTime.Value;
            bool otherStartsBeforeThisEnds = other.StartTime.Value < EndTime.Value;
            return startsBeforeOtherEnds && otherStartsBeforeThisEnds;
        }
    }

    //Is this Obligation safe to be added to the Planner?
    public virtual List<string> Validate() {

    }

    //
    public abstract string getTypeName();
}