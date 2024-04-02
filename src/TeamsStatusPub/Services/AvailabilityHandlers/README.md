# Availability Handlers

Handlers can be added by creating a class that implements the [`IAvailabilityHandler`](IAvailabilityHandler.cs) interface. How a handler decides if the user is busy is up to the implementation of the `IsAvailable` method however a return value of `true` means that the user isn't busy.

The rest of this file is the implementation details of each handler.

## Microsoft Teams

Microsoft Teams maintains a log file for certain operations. This log file is kept relatively small around 2.01MB whereby a new one will be created and used as the "current" log file.

The log file is written under the `C:\Users\user\AppData\Local\Packages\MSTeams_%id%\LocalCache\Microsoft\MSTeams\Logs` where `%id%` is a 13-character string containing numbers and lowercase letters.

The log file uses the pattern `MSTeams_yyyy-MM-dd_HH-mm-ss.%counter%.log` where `%counter%` is a two-digit zero-based counter that usually starts at `00` but not always. It will be incremented if more than one log file is written on that day.

When the status is changed in Teams, 2-3 INFO level lines are written related to updating the account's status in the cloud.

Example status change to "busy":

```
2024-04-01T16:11:01.694594-04:00 0x00007b24 <INFO> native_modules::UserDataCrossCloudModule: BroadcastGlobalState: New Global State Event: UserDataGlobalState total number of users: 1 { user id :118c7aa5-4e0a-4276-9cd5-68e8e9ea9ede, availability: Busy, unread notification count: 0 }
```

The relevant parts of the line broken down:

```
[1] native_modules::UserDataCrossCloudModule: BroadcastGlobalState:
[2] New Global State Event: UserDataGlobalState
[3] total number of users: 1
[4] { user id :118c7aa5-4e0a-4276-9cd5-68e8e9ea9ede, availability: Busy, unread notification count: 0 }
```

1. This module is responsible for several cloud-related activities. While **CloudStateChanged** could also have been used, it was decided to only focus on **BroadcastGlobalState** as it appears to be more wide reaching.
2. This line indicates that a new state event was initiated.
3. Indicates how many accounts are affected by the new state event.
4. Data on the account and related event. This is not JSON data. The relevant portion is "availability".

## Microsoft Teams Classic

The desktop version of Microsoft Teams Classic (running on Windows) writes to a log file in `C:\Users\user\AppData\Roaming\Microsoft\Teams\logs.txt` (this is under the user's APPDATA directory). It's unclear when the file is truncated.

When a call is initiated, a line that contains the string `eventData: s::;m::1;a::1` will be written. When that call ends a line will be written that contains `eventData: s::;m::1;a::3`. In between of those two lines there may be thousands of other status lines however those two lines represent the start and end of the call.

During a call, if the screen is shared then a line that contains `eventData: s::;m::1;a::0` will be written. If the screen share is ended while the call continues, `eventData: s::;m::1;a::1` will be written to indicate that.
