# Teams Status Pub

[![Continuous integration](https://github.com/tetsuo13/TeamsStatusPub/actions/workflows/ci.yml/badge.svg)](https://github.com/tetsuo13/TeamsStatusPub/actions/workflows/ci.yml) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT) [![release](https://img.shields.io/github/release/tetsuo13/TeamsStatusPub.svg)](https://github.com/tetsuo13/TeamsStatusPub/releases)

<img src="src/TeamsStatusPub/Icons/logo.png" width="100" alt="Logo" align="right" />

Serve a RESTful interface from a computer running [Microsoft Teams](https://www.microsoft.com/en-us/microsoft-teams/group-chat-software/) to act as a sensor for [Home Assistant](https://www.home-assistant.io/). This sensor can then be used, for example, to toggle power to a smart lightbulb when you're in a meeting.

Teams Status Pub runs on the system tray to publish your Teams status via a web server that listens for incoming requests. When a request comes in, it will look at the Teams log file to determine your status as either "available" or "busy" and serve a JSON payload with the result. You're considered busy in Teams when on a call, regardless of video or mic status.

## Motivation and Alternatives

There are other projects available for integrating Teams with Home Assistant however their usage is largely dependent on how Teams and/or your computer is configured within the organization.

### Microsoft Graph

Your status is available via the [presence](https://learn.microsoft.com/en-us/graph/api/presence-get) API in [Microsoft Graph](https://learn.microsoft.com/en-us/graph/overview) if your organization has granted consent. If so then [isaacrlevin/PresenceLight](https://github.com/isaacrlevin/PresenceLight) may be a better alternative in this scenario. Note that hardware support for bulbs is limited.

### Directly Update Home Assistant

If the computer that Teams is installed on can reach Home Assistant on the network, then [EBOOZE/TeamsStatus](https://github.com/EBOOZ/TeamsStatus) may be worth a try. After creating the status sensors in Home Assistant and providing TeamStatus with a long-lived access token, it will determine your availability from Teams locally and reach out to Home Assistant to set the values of the sensors accordingly.

### Teams Status Pub

If you've made it this far, the above alternatives are lacking something or you're in an organization that doesn't provide consent to read presense from the Graph API or the computer Teams is installed on cannot reach Home Assistant (due to network segregation or a restricted VLAN).

These were the primary motivating factors behind this project.

## Getting Started

- Your computer must be running Windows 10 or later.
- .NET 8 runtime.
- The desktop version of Teams must be installed, either Teams or Teams Classic.

Teasted against the following minimum versions and is known to work on the latest available version:

- Microsoft Teams 24033.811.2738.2546
- Microsoft Teams classic for Windows versions 1.5.00.21463

> [!NOTE]
> Microsoft Teams Classic is considered deprecated and will eventually be removed.

### Teams

No additional changes are required, the default settings already log sufficient data.

Note that there is sometimes a delay between the status shown in Teams and the logs used by Teams Status Pub. For Teams Classic this delay was rarely longer than a few seconds but in Teams it can be up to a few minutes. Resetting or otherwise changing the status in Teams doesn't seem to cause the log to be updated any faster.

Teams Status Pub will reflect the statuses in Teams that are deemed "not available" -- **Busy** and **Do not disturb**. This is slightly different than how availability was deemed with Teams Classic as it was strictly based on whether or not you were engaged in a call. This can be apparent after ending a meeting call early: in Teams you remain in **Busy** status until the end of the meeting so therefor Teams Status Pub reports as unavailable but in Teams Classic Teams Status Pub reports as available despite the status.

### Teams Status Pub

Download the installer from the [Releases](https://github.com/tetsuo13/TeamsStatusPub/releases) page. By default it will install to your `C:\Users\username\AppData\Local\Programs\TeamsStatusPub` folder. As part of the installation there will be an `appsettings.json` file used to configure some basic runtime settings.

The default listen address is http://192.168.1.1:17493/ but the IP address will most likely need to be changed. The following settings are available in `appsettings.json`:

| Setting | Description |
| ------- | ----------- |
| `Runtime:AvailabilityHandler` | Can be either "MicrosoftTeams" (default) or "MicrosoftTeamsClassic". |
| `Runtime:ListenAddress` | The IP address to listen on. |
| `Runtime:ListenPort` | The port to listen on. Should be greater than 1024. |

In order for Home Assistant to successfully query the computer, you will likely need to add an inbound rule to allow this application through the firewall. Open **Windows Defender Firewall with Advanced Security** and create a new rule using the following custom values for the **New Inbound Rule Wizard**, customize all others as needed:

- Rule type
  - Custom
- Program
  - This program path: C:\Users\username\AppData\Local\Programs\TeamsStatusPub\TeamsStatusPub.exe
- Protocol and Ports
  - Protocol type: TCP
  - Local port: Specific ports 17493 (**Note:** Reference the `Runtime:ListenPort` value used in the `appsettings.json` file.)
- Profile
  - Domain, Private, Public
- Name
  - Name: TeamsStatusPub

### Home Assistant

Teams Status Pub will output a single object with a boolean value that will indicate whether the user is busy in Teams or not:

```json
{"busy":false}
```

Set up a [RESTful binary sensor](https://www.home-assistant.io/integrations/binary_sensor.rest/) targeting the IP and port set in `appsettings.json`:

```yaml
binary_sensor:
  - platform: rest
    resource: http://LISTEN_ADDRESS:LISTEN_PORT/
    name: Microsoft Teams on Call
    value_template: "{{ value_json.busy }}"
```

With that binary sensor in place there are all sorts of [automations](https://www.home-assistant.io/docs/automation/) that can be created. One example is to toggle a light:

```yaml
automation:
  - alias: Toggle Light When Busy in Microsoft Teams
    trigger:
      platform: state
      entity_id: binary_sensor.microsoft_teams_on_call
    action:
      - service: light.turn_{{ trigger.to_state.state }}
        target:
          entity_id: light.red_light_above_office_door
```

## Troubleshooting

To enable logging, change the `Serilog:MinimumLevel:Default` value in `appsettings.json` to "Information" (use "Debug" to increase the verbosity level). The default value that's there is set in order to disable logging. An application restart will be required. By changing the value, a log file will be written in the same directory as the application.

## Development

This was written as a WinForms application using the Model View Presenter (MVP) design pattern.

There has been some consideration taken to make the code modular enough to handle integrating with other conferencing tools other than Microsoft Teams. In the future it could be possible to add additional availability handlers and use configuration to determine which handler(s) to use. A detailed breakdown of handlers and how a user is considered busy can be found in the [README](src/TeamsStatusPub.Core/Services/AvailabilityHandlers/README.md) file in the availability handlers directory.

## License

See [LICENSE](LICENSE)
