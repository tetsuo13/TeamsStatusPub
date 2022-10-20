# Availability Handlers

Handlers can be added by creating a class that implements the [`IAvailabilityHandler`](IAvailabilityHandler.cs) interface. How a handler decides if the user is busy is up to the implementation of the `IsAvailable` method however a return value of `true` means that the user isn't busy.

The rest of this file is the implementation details of each handler.

## Microsoft Teams

The desktop version of Microsoft Teams (running on Windows) writes to a log file in `C:\Users\user\AppData\Roaming\Microsoft\Teams\logs.txt` (this is under the user's APPDATA directory). It's unclear when the file is truncated.

When a call is initiated, a line that contains the string `eventData: s::;m::1;a::1` will be written. When that call ends a line will be written that contains `eventData: s::;m::1;a::3`. In between of those two lines there may be thousands of other status lines however those two lines represent the start and end of the call.

During a call, if the screen is shared then a line that contains `eventData: s::;m::1;a::0` will be written. If the screen share is ended while the call continues, `eventData: s::;m::1;a::1` will be written to indicate that.
