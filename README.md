<img src="src/TeamsStatusPub/logo.png" width="100" alt="Logo" />

# Teams Status Pub

[![Continuous integration](https://github.com/tetsuo13/TeamsStatusPub/actions/workflows/ci.yaml/badge.svg)](https://github.com/tetsuo13/TeamsStatusPub/actions/workflows/ci.yaml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Serve a RESTful interface from a computer running [Microsoft Teams](https://www.microsoft.com/en-us/microsoft-teams/group-chat-software/) to act as a sensor for [Home Assistant](https://www.home-assistant.io/). This sensor can then be used, for example, to toggle power to a smart lightbulb when you're in a meeting.

Teams Status Pub runs on the system tray to publish your Teams status via a web server that listens for incoming requests. When a request comes in, it will look at the Teams log file to determine your status as either "available" or "busy" and serve a JSON payload with the result. You're considered busy in Teams when on a call, regardless of video or mic status.

## Motiviation and Alternatives

There are other projects available for integrating Teams with Home Assistant however their usage is largly dependent on how Teams and/or your computer is configured within the organization.

### Microsoft Graph

Your status is available via the [presence](https://docs.microsoft.com/en-us/graph/api/presence-get) API in [Microsoft Graph](https://docs.microsoft.com/en-us/graph/overview) if your organization has granted consent. If so then [PresenceLight](https://github.com/isaacrlevin/PresenceLight) may be a better alternative in this scenario. Note that hardware support for bulbs is limited.

### Directly Update Home Assistant

If the computer that Teams is installed on can reach Home Assistant on the network, then [EBOOZE/TeamsStatus](https://github.com/EBOOZ/TeamsStatus) may be worth a try. After creating the status sensors in Home Assistant and providing TeamStatus with a long-lived access token, it will determine your availability from Teams locally and reach out to Home Assistant to set the values of the sensors accordingly.

### Teams Status Pub

If you've made it this far, the above alternatives are lacking something or you're in an organization that doesn't provide consent to read presense from the Graph API or the computer Teams is installed on cannot reach Home Assistant (due to network segregation or a restricted VLAN). These were the primary motivating factors behind this project.

## Getting Started

- Your computer must be running Windows 10 or later.
- .NET 6 runtime.
- The desktop version of Teams must be installed.

### Teams Status Pub

Download the latest release and unzip it to a dedicated folder. As part of the release there will be an *appsettings.json* file used to configure some basic runtime settings.

The default listen address is http://192.168.1.1:17493/ but this will most likely need to be changed. The following settings are available in *appsettings.json*:

| Setting | Description |
| ------- | ----------- |
| `Runtime:ListenAddress` | The IP address to listen on.  |
| `Runtime:ListenPort` | The port to listen on. Should be greater than 1024. |

In order for Home Assistant to successfully query the computer, you will likely need to add an inbound rule to allow this application through the firewall.

### Home Assistant

Teams Status Pub will output a single object with a boolean value that will indicate whether the user is busy in Teams or not:

```json
{"busy":false}
```

Set up a [RESTful binary sensor](https://www.home-assistant.io/integrations/binary_sensor.rest/) targetting the IP and port set in *appsettings.json*:

```yaml
binary_sensor:
  - platform: rest
    resource: http://LISTEN_ADDRESS:LISTEN_PORT/
    name: "Microsoft Teams on Call"
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
      - service: >
          light.turn_{{ trigger.to_state.state }}
        target:
          entity_id: light.red_light_above_office_door
```

## Troubleshooting

To enable logging, change the `Serilog:MinimumLevel:Default` value in *appsettings.json* to "Information" (use "Debug" to increase the verbosity level). The default value that's there is set in order to disable logging. An application restart will be required. By changing the value, a log file will be written in the same directory as the application.

## License

See [LICENSE](LICENSE)
