using System.Text.Json.Serialization;

namespace WeeklyCalendar.Models;

[JsonPolymorphic]
[JsonDerivedType(typeof(Course), "course")]
[JsonDerivedType(typeof(Assignment), "assignment")]
[JsonDerivedType(typeof(Other), "other")]

public abstract class Obligation {
    //constant declaration
    public const int MaxTitleLength = 50;
    public const int MaxDescriptionLength = 200;
    
    //variables used in Obligation
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public Days Days { get; set; } = Days.None;
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public int? RepeatEveryWeeks { get; set; }
    public int StartWeek { get; set; }
    public bool RemovedThisWeek { get; set; }

    //Does the Obligation have a date and time?
    public bool IsScheduled() {
        if (Days != Days.None && StartTime != null && EndTime != null) {
            return true;
        } else {
            return false;
        }

    }

    //Does the Obligation not repeat?
    public bool IsOneTime() {
        if(RepeatEveryWeeks == 0 || RepeatEveryWeeks == null) {
            return true;
        } else {
            return false;
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
        var errors  = new List<string>();
        if(string.IsNullOrWhiteSpace(Title)) {
            errors.Add("Title is required.");
        }
        if(Title.Length > MaxTitleLength) {
            errors.Add("Title must be 50 characters or shorter.");
        }
        if(Description.Length > MaxDescriptionLength) {
            errors.Add("Description must be 200 characters or shorter.");
        }
        if(StartTime == null ^ EndTime == null) {
            errors.Add("Enter both a start and end time, or leave both empty.");
        }

        if(Days != Days.None ^ (StartTime  != null && EndTime != null)) {
            errors.Add("Enter both a Day and Time, or leave both empty");
        }
        if((StartTime != null && EndTime != null) && EndTime <= StartTime) {
            errors.Add("The end time must be after the start time.");
        }
        if(RepeatEveryWeeks < 1) {
            errors.Add("Repeat frequency must be at least 1 week.");
        }

        return errors;
    }

    //What type of Obligation is it?
    public abstract string GetTypeName();
}