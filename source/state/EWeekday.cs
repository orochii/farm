using System;

[Flags]
public enum EWeekday {
    MONDAY = 1 << 1, 
    TUESDAY = 1 << 2, 
    WEDNESDAY = 1 << 3,
    THURSDAY = 1 << 4,
    FRIDAY = 1 << 5,
    SATURDAY = 1 << 6,
    SUNDAY = 1 << 7
}
