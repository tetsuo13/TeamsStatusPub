# Availability Handlers

Handlers can be added by creating a class that implements the [`IAvailabilityHandler`](IAvailabilityHandler.cs) interface. How a handler decides if the user is busy is up to the implementation of the `IsAvailable` method however a return value of `true` means that the user isn't busy.

The rest of this file is the implementation details of each handler.

## Microsoft Teams

Microsoft Teams maintains a log file for certain operations. This log file is kept relatively small around 2.01MB whereby a new one will be created and used as the "current" log file.

The log file is written under the `C:\Users\user\AppData\Local\Packages\MSTeams_%id%\LocalCache\Microsoft\MSTeams\Logs` where `%id%` is a 13-character string containing numbers and lowercase letters.

The log file uses the pattern `MSTeams_yyyy-MM-dd_HH-mm-ss.%counter%.log` where `%counter%` is a two-digit zero-based counter that usually starts at `00` but not always. It will be incremented if more than one log file is written on that day.

When the status is changed in Teams, 3 lines related to changing the badge are written to the log file. The badge name will indicate the availability.

Example status change to "busy":

```
<DBG>  TaskbarBadgeServiceLegacy:Work: SetBadge PreSetBadge verification: GlyphBadge{"busy"}, overlay: No items, status busy
```

Either the GlyphBadge or the last word on the line can be used as the indicator.

## Microsoft Teams Classic

The desktop version of Microsoft Teams Classic (running on Windows) writes to a log file in `C:\Users\user\AppData\Roaming\Microsoft\Teams\logs.txt` (this is under the user's APPDATA directory). It's unclear when the file is truncated.

When a call is initiated, a line that contains the string `eventData: s::;m::1;a::1` will be written. When that call ends a line will be written that contains `eventData: s::;m::1;a::3`. In between of those two lines there may be thousands of other status lines however those two lines represent the start and end of the call.

During a call, if the screen is shared then a line that contains `eventData: s::;m::1;a::0` will be written. If the screen share is ended while the call continues, `eventData: s::;m::1;a::1` will be written to indicate that.
